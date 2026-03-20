using Nox.CCK.Utils;

namespace Nox.CCK.Notifications {
	/// <summary>
	/// Persisted configuration values for the notification system.
	/// Properties read and write via <see cref="Config"/> so values survive restarts.
	/// </summary>
	public static class NotificationSettings {
		// ── Config keys ───────────────────────────────────────────────────────

		public const string EnabledKey            = "settings.notifications.enabled";
		public const string DoNotDisturbKey       = "settings.notifications.do_not_disturb";
		public const string SoundKey              = "settings.notifications.sound";
		public const string VibrationKey          = "settings.notifications.vibration";
		public const string BadgeKey              = "settings.notifications.badge";

		// ── Defaults ──────────────────────────────────────────────────────────

		public const bool EnabledDefault            = true;
		public const bool DoNotDisturbDefault       = false;
		public const bool SoundDefault              = true;
		public const bool VibrationDefault          = true;
		public const bool BadgeDefault              = true;

		// ── Properties ────────────────────────────────────────────────────────

		/// <summary>Whether the notification system is globally enabled.</summary>
		public static bool Enabled {
			get => Config.Load().Get(EnabledKey, EnabledDefault);
			set { var c = Config.Load(); c.Set(EnabledKey, value); c.Save(); }
		}

		/// <summary>
		/// Do Not Disturb mode — when true all notifications are silently suppressed.
		/// </summary>
		public static bool DoNotDisturb {
			get => Config.Load().Get(DoNotDisturbKey, DoNotDisturbDefault);
			set { var c = Config.Load(); c.Set(DoNotDisturbKey, value); c.Save(); }
		}

		/// <summary>Whether to play a sound when a notification is posted.</summary>
		public static bool Sound {
			get => Config.Load().Get(SoundKey, SoundDefault);
			set { var c = Config.Load(); c.Set(SoundKey, value); c.Save(); }
		}

		/// <summary>Whether to vibrate (XR controllers) when a notification is posted.</summary>
		public static bool Vibration {
			get => Config.Load().Get(VibrationKey, VibrationDefault);
			set { var c = Config.Load(); c.Set(VibrationKey, value); c.Save(); }
		}

		/// <summary>Whether to show a badge count on the application icon.</summary>
		public static bool Badge {
			get => Config.Load().Get(BadgeKey, BadgeDefault);
			set { var c = Config.Load(); c.Set(BadgeKey, value); c.Save(); }
		}
	}
}
