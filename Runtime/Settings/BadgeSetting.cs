using Nox.CCK.Notifications;
using Nox.CCK.Settings;
using UnityEngine;

namespace Nox.Notifications.Runtime.Settings {
	/// <summary>
	/// Toggle: show a badge count on the app icon for unread notifications.
	/// Path: notifications > badge
	/// </summary>
	internal sealed class BadgeSetting : ToggleHandler {
		public override string[] Path
			=> new[] { "notifications", "badge" };

		public override int Order => 90000;

		protected override GameObject GetPrefab()
			=> Main.CoreAPI.AssetAPI.GetAsset<GameObject>("settings:prefabs/toggle.prefab");

		public BadgeSetting() {
			SetValue(NotificationSettings.Badge, notify: false);
			SetLabelKey($"settings.entry.{string.Join(".", Path)}.label");
		}

		protected override void OnValueChanged(bool value)
			=> NotificationSettings.Badge = value;
	}
}
