using Nox.CCK.Notifications;
using Nox.CCK.Settings;
using UnityEngine;

namespace Nox.Notifications.Runtime.Settings {
	/// <summary>
	/// Toggle: globally enable or disable the notification system.
	/// Path: notifications > enabled
	/// </summary>
	internal sealed class EnabledSetting : ToggleHandler {
		public override string[] GetPath()
			=> new[] { "notifications", "enabled" };

		protected override GameObject GetPrefab()
			=> Main.CoreAPI.AssetAPI.GetAsset<GameObject>("settings:prefabs/toggle.prefab");

		public EnabledSetting() {
			SetValue(NotificationSettings.Enabled, notify: false);
			SetLabelKey($"settings.entry.{string.Join(".", GetPath())}.label");
		}

		protected override void OnValueChanged(bool value)
			=> NotificationSettings.Enabled = value;
	}
}
