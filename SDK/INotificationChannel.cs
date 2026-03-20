namespace Nox.Notifications {
	/// <summary>
	/// A channel through which notifications are delivered.
	/// Equivalent to Android's NotificationChannel — allows users to control
	/// notification behaviour per logical category.
	/// </summary>
	public interface INotificationChannel {
		/// <summary>Unique identifier of this channel.</summary>
		public string Id { get; }

		/// <summary>User-visible name of this channel.</summary>
		public string Name { get; }

		/// <summary>User-visible description shown in the settings UI.</summary>
		public string Description { get; }

		/// <summary>
		/// Importance level that controls how interruptive notifications in this channel are.
		/// </summary>
		public NotificationImportance Importance { get; }

		/// <summary>
		/// Identifier of the <see cref="INotificationChannelGroup"/> this channel belongs to, or null.
		/// </summary>
		public string GroupId { get; }

		/// <summary>Whether to enable vibration for this channel.</summary>
		public bool EnableVibration { get; }
	}
}
