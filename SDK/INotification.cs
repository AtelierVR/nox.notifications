using System;

namespace Nox.Notifications {
	/// <summary>
	/// Core notification interface.
	/// Represents the minimum data needed to display a notification.
	/// Similar to Android's Notification class.
	/// </summary>
	public interface INotification {
		/// <summary>Numeric identifier for this notification (unique per tag).</summary>
		public int Id { get; }

		/// <summary>
		/// Optional tag that, combined with <see cref="Id"/>, uniquely identifies a notification.
		/// </summary>
		public string Tag { get; }

		/// <summary>
		/// Identifier of the <see cref="INotificationChannel"/> this notification is posted to.
		/// </summary>
		public string ChannelId { get; }

		/// <summary>Title line of the notification.</summary>
		public string Title { get; }

		/// <summary>Body text of the notification.</summary>
		public string ContentText { get; }

		/// <summary>
		/// Key/name of the small icon displayed in the status bar.
		/// </summary>
		public string SmallIcon { get; }

		/// <summary>
		/// Key/name of the large icon shown in the notification body, or null.
		/// </summary>
		public string LargeIcon { get; }

		/// <summary>
		/// Timestamp associated with this notification (shown in the header).
		/// </summary>
		public DateTime When { get; }

		/// <summary>
		/// Whether the notification is automatically cancelled when the user taps it.
		/// </summary>
		public bool AutoCancel { get; }

		/// <summary>
		/// Whether this notification is ongoing (cannot be dismissed by the user).
		/// </summary>
		public bool IsOngoing { get; }

		/// <summary>
		/// Whether this notification should be silent (no sound/vibration).
		/// </summary>
		public bool IsSilent { get; }

		/// <summary>
		/// Relative priority of this notification.
		/// </summary>
		public NotificationPriority Priority { get; }

		/// <summary>
		/// Semantic category of this notification. Use <see cref="NotificationCategory"/> constants.
		/// </summary>
		public string Category { get; }

		/// <summary>
		/// Actions that can be taken on this notification.
		/// </summary>
		public INotificationAction[] Actions { get; }

		/// <summary>
		/// Identifier of the user who triggered this notification, or null for system notifications.
		/// </summary>
		public string SenderId { get; }

		/// <summary>
		/// Display name of the user who triggered this notification, or null.
		/// </summary>
		public string SenderName { get; }
	}
}
