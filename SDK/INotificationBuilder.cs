using System;

namespace Nox.Notifications {
	/// <summary>
	/// Fluent builder for constructing <see cref="INotification"/> instances.
	/// Similar to Android's Notification.Builder.
	/// </summary>
	public interface INotificationBuilder {
		/// <summary>Sets the identifier of the channel to deliver this notification to.</summary>
		public INotificationBuilder SetChannelId(string channelId);

		/// <summary>Sets the title line of the notification.</summary>
		public INotificationBuilder SetTitle(string title);

		/// <summary>Sets the body text of the notification.</summary>
		public INotificationBuilder SetContentText(string text);

		/// <summary>Sets the small icon key used in the status bar.</summary>
		public INotificationBuilder SetSmallIcon(string icon);

		/// <summary>Sets the large icon key shown in the notification body.</summary>
		public INotificationBuilder SetLargeIcon(string icon);

		/// <summary>Sets the timestamp associated with this notification.</summary>
		public INotificationBuilder SetWhen(DateTime when);

		/// <summary>
		/// Whether the notification is automatically cancelled when tapped.
		/// Default is <c>false</c>.
		/// </summary>
		public INotificationBuilder SetAutoCancel(bool autoCancel);

		/// <summary>
		/// Marks the notification as ongoing (cannot be dismissed by the user).
		/// </summary>
		public INotificationBuilder SetOngoing(bool ongoing);

		/// <summary>Marks the notification as silent (no sound/vibration).</summary>
		public INotificationBuilder SetSilent(bool silent);

		/// <summary>Sets the relative priority of this notification.</summary>
		public INotificationBuilder SetPriority(NotificationPriority priority);

		/// <summary>
		/// Sets the semantic category. Use <see cref="NotificationCategory"/> constants.
		/// </summary>
		public INotificationBuilder SetCategory(string category);

		/// <summary>Adds an action button to the notification.</summary>
		public INotificationBuilder AddAction(INotificationAction action);

		/// <summary>Sets the user ID of the sender (e.g. for friend requests, invites).</summary>
		public INotificationBuilder SetSenderId(string senderId);

		/// <summary>Sets the display name of the sender.</summary>
		public INotificationBuilder SetSenderName(string senderName);

		/// <summary>Sets a specific numeric ID for this notification (default is auto-assigned).</summary>
		public INotificationBuilder SetId(int id);

		/// <summary>Sets the tag that, combined with the ID, uniquely identifies this notification.</summary>
		public INotificationBuilder SetTag(string tag);

		/// <summary>Constructs the <see cref="INotification"/> from the current builder state.</summary>
		public INotification Build();
	}
}
