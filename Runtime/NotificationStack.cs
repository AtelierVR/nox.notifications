using System;
using Cysharp.Threading.Tasks;
using Nox.CCK.Utils;
using UnityEngine;
using Logger = Nox.CCK.Utils.Logger;

namespace Nox.Notifications.Runtime {
	/// <summary>
	/// Manages the display of multiple notification panels in the container.
	/// Instantiates <c>notification.prefab</c> instances, stacks them with
	/// Z/Y offset for depth, and cleans up when notifications are cancelled.
	///
	/// The most recent notification appears in front (highest sort order / lowest Z).
	/// </summary>
	public class NotificationStack : MonoBehaviour {
		[Header("Stacking")]
		[SerializeField]
		[Tooltip("Z offset between stacked notifications (depth).")]
		private float m_ZOffset = 0.10f;

		[SerializeField]
		[Tooltip("Y offset between stacked notifications.")]
		private float m_YOffset = 0.10f;

		[SerializeField]
		[Tooltip("Maximum number of visible notifications.")]
		private int m_MaxVisible = 5;

		[SerializeField]
		[Tooltip("Scale reduction per position in the stack (0 = no scaling).")]
		[Range(0f, 0.2f)]
		private float m_ScaleStep = 0.05f;

		private NotificationDisplay[] _active = Array.Empty<NotificationDisplay>();

		// ── Unity lifecycle ────────────────────────────────────────────────

		public void Initialize() {
			Logger.Log("NotificationStack.Awake — subscribing to events.", tag: "Notifications");
			Main.Instance.OnNotificationPosted.AddListener(OnNotificationPosted);
			Main.Instance.OnNotificationCancelled.AddListener(OnNotificationCancelled);
		}

		private void OnDestroy() {
			Main.Instance.OnNotificationPosted.RemoveListener(OnNotificationPosted);
			Main.Instance.OnNotificationCancelled.RemoveListener(OnNotificationCancelled);
		}

		// ── Event handlers ─────────────────────────────────────────────────

		private void OnNotificationPosted(INotification notification)
			=> OnNotificationPostedAsync(notification).Forget();

		private async UniTask OnNotificationPostedAsync(INotification notification) {
			Logger.Log($"NotificationStack: received notification id={notification.Id}", tag: "Notifications");

			var prefab = await Client.CoreAPI.AssetAPI.GetAssetAsync<GameObject>("notification.prefab");
			if (!prefab) {
				Logger.LogWarning("NotificationStack: Prefab is null, cannot display.", tag: "Notifications");
				return;
			}

			// Remove excess old notifications if over the limit.
			while (_active.Length >= m_MaxVisible) {
				var oldest = _active[^1]; // last = oldest
				if (oldest && oldest.NotificationId > 0)
					Main.Instance?.Cancel(oldest.NotificationId);
				else if (oldest)
					oldest.gameObject.Destroy();
			}

			// Instantiate and set up.
			var display = await prefab.InstantiateAsync<NotificationDisplay>(transform);
			if (!display) {
				Logger.LogWarning("NotificationStack: Prefab has no NotificationDisplay component.", tag: "Notifications");
				return;
			}

            var rt = display.gameObject.GetComponent<RectTransform>();
            rt.localScale = Vector3.one;
            rt.localRotation = Quaternion.identity;
            rt.localPosition = Vector3.zero;

			display.SetNotification(notification);

			// Insert newest at index 0.
			Array.Resize(ref _active, _active.Length + 1);
			for (var i = _active.Length - 1; i > 0; i--)
				_active[i] = _active[i - 1];
			_active[0] = display;

			// Re-stack all.
			ApplyStacking();
		}

		private void OnNotificationCancelled(int id) {
			// Remove from active list and destroy.
			for (var i = 0; i < _active.Length; i++) {
				if (!_active[i])
					continue;
				if (_active[i].NotificationId != id)
					continue;
				_active[i].gameObject.Destroy();
				RemoveAt(i);
				break;
			}

			ApplyStacking();
		}

		// ── Stacking ───────────────────────────────────────────────────────

		private void ApplyStacking() {
			// Compact nulls.
			var count = 0;
			for (var i = 0; i < _active.Length; i++)
				if (_active[i])
					_active[count++] = _active[i];
			Array.Resize(ref _active, count);

			// Stack: index 0 = front (newest), last = back (oldest).
			for (var i = 0; i < _active.Length; i++) {
				if (!_active[i])
					continue;

				var rt = _active[i].GetComponent<RectTransform>();
				if (!rt)
					continue;

				// Z: further back = more negative (or higher depending on canvas)
				// We use localPosition Z to create depth in world-space canvas.
				var z = -i * m_ZOffset;
				var y = -i * m_YOffset;

				var pos = rt.localPosition;
				pos.z            = z;
				pos.y            = y;
				rt.localPosition = pos;

				// Sort order: newest = highest (renders on top).
				var canvas = _active[i].GetComponent<Canvas>();
				if (canvas)
					canvas.sortingOrder = _active.Length - 1 - i;

				// Scale: older = smaller.
				var scale = Mathf.Max(0.5f, 1f - i * m_ScaleStep);
				rt.localScale = Vector3.one * scale;
			}
		}

		private void RemoveAt(int index) {
			for (var i = index; i < _active.Length - 1; i++)
				_active[i] = _active[i + 1];
			Array.Resize(ref _active, _active.Length - 1);
		}
	}
}