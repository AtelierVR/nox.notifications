using Nox.CCK.Notifications;
using Nox.CCK.Settings;
using UnityEngine;

namespace Nox.Notifications.Runtime.Settings {
	/// <summary>
	/// Toggle: vibrate when a notification is posted.
	/// Path: notifications > vibration
	/// </summary>
	internal sealed class VibrationSetting : ToggleHandler {
		public override string[] GetPath()
			=> new[] { "notifications", "vibration" };

		protected override GameObject GetPrefab()
			=> Main.CoreAPI.AssetAPI.GetAsset<GameObject>("settings:prefabs/toggle.prefab");

		public VibrationSetting() {
			SetValue(NotificationSettings.Vibration, notify: false);
			SetLabelKey($"settings.entry.{string.Join(".", GetPath())}.label");
		}

		protected override void OnValueChanged(bool value)
			=> NotificationSettings.Vibration = value;
	}
}
