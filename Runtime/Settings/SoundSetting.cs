using Nox.CCK.Notifications;
using Nox.CCK.Settings;
using UnityEngine;

namespace Nox.Notifications.Runtime.Settings {
	/// <summary>
	/// Toggle: play a sound when a notification is posted.
	/// Path: notifications > sound
	/// </summary>
	internal sealed class SoundSetting : ToggleHandler {
		public override string[] Path
			=> new[] { "notifications", "sound" };

		public override int Order => 90000;

		protected override GameObject GetPrefab()
			=> Main.CoreAPI.AssetAPI.GetAsset<GameObject>("settings:prefabs/toggle.prefab");

		public SoundSetting() {
			SetValue(NotificationSettings.Sound, notify: false);
			SetLabelKey($"settings.entry.{string.Join(".", Path)}.label");
		}

		protected override void OnValueChanged(bool value)
			=> NotificationSettings.Sound = value;
	}
}
