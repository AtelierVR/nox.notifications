namespace Nox.Notifications {
	/// <summary>
	/// Importance levels for notification channels, similar to Android's NotificationManager importance.
	/// Controls how interruptive a notification is.
	/// </summary>
	public enum NotificationImportance {
		/// <summary>Do not show notifications at all.</summary>
		None = 0,

		/// <summary>Show in the status bar; no sound or visual interruption.</summary>
		Min = 1,

		/// <summary>Show in the notification shade; no sound.</summary>
		Low = 2,

		/// <summary>Show notification with sound (default behavior).</summary>
		Default = 3,

		/// <summary>Show notification with sound and appear on screen.</summary>
		High = 4,

		/// <summary>Show as a heads-up notification; highest interruption.</summary>
		Max = 5
	}
}
