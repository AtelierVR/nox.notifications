using Nox.CCK.Notifications;
using Nox.CCK.Settings;
using UnityEngine;

namespace Nox.Notifications.Runtime.Settings {
	/// <summary>
	/// Toggle: Do Not Disturb — when active all notifications are silently suppressed.
	/// Path: notifications > do_not_disturb
	/// </summary>
	internal sealed class DoNotDisturbSetting : ToggleHandler {
		public override string[] GetPath()
			=> new[] { "notifications", "do_not_disturb" };

		protected override GameObject GetPrefab()
			=> Main.CoreAPI.AssetAPI.GetAsset<GameObject>("settings:prefabs/toggle.prefab");

		public DoNotDisturbSetting() {
			SetValue(NotificationSettings.DoNotDisturb, notify: false);
			SetLabelKey($"settings.entry.{string.Join(".", GetPath())}.label");
		}

		protected override void OnValueChanged(bool value)
			=> NotificationSettings.DoNotDisturb = value;
	}
}
