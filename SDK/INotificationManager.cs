using System;

namespace Nox.Notifications {
	/// <summary>
	/// Central manager for the Nox notification system.
	/// Similar to Android's NotificationManager — handles posting, querying,
	/// and cancelling notifications, as well as managing channels and channel groups.
	/// </summary>
	public interface INotificationManager {
		// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
		// Posting
		// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

		/// <summary>
		/// Post or update a notification with the specified ID.
		/// </summary>
		/// <param name="id">Unique numeric identifier for the notification.</param>
		/// <param name="notification">The notification to post.</param>
		public void Notify(int id, INotification notification);

		/// <summary>
		/// Post or update a notification with the specified tag and ID.
		/// The tag allows two notifications to share the same ID without collision.
		/// </summary>
		/// <param name="tag">Optional tag for the notification.</param>
		/// <param name="id">Unique numeric identifier for the notification.</param>
		/// <param name="notification">The notification to post.</param>
		public void Notify(string tag, int id, INotification notification);

		// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
		// Cancelling
		// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

		/// <summary>Cancel a previously posted notification by ID.</summary>
		/// <param name="id">The ID of the notification to cancel.</param>
		public void Cancel(int id);

		/// <summary>Cancel a previously posted notification by tag and ID.</summary>
		/// <param name="tag">The tag of the notification to cancel.</param>
		/// <param name="id">The ID of the notification to cancel.</param>
		public void Cancel(string tag, int id);

		/// <summary>Cancel all notifications posted by the calling mod.</summary>
		public void CancelAll();

		// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
		// Querying
		// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

		/// <summary>
		/// Returns whether a notification with the given ID is currently active.
		/// </summary>
		public bool IsPosted(int id);

		/// <summary>
		/// Returns whether a notification with the given tag and ID is currently active.
		/// </summary>
		public bool IsPosted(string tag, int id);

		/// <summary>
		/// Returns all active (non-cancelled) notifications.
		/// </summary>
		public INotification[] GetActiveNotifications();

		// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
		// Channel management
		// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

		/// <summary>Create or update a notification channel.</summary>
		public void CreateNotificationChannel(INotificationChannel channel);

		/// <summary>Create or update several notification channels at once.</summary>
		public void CreateNotificationChannels(INotificationChannel[] channels);

		/// <summary>
		/// Delete the notification channel with the given ID.
		/// Any notifications posted to this channel are also cancelled.
		/// </summary>
		public void DeleteNotificationChannel(string channelId);

		/// <summary>Returns the channel with the given ID, or <c>null</c> if not found.</summary>
		public INotificationChannel GetNotificationChannel(string channelId);

		/// <summary>Returns all registered notification channels.</summary>
		public INotificationChannel[] GetNotificationChannels();

		// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
		// Channel group management
		// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

		/// <summary>Create or update a notification channel group.</summary>
		public void CreateNotificationChannelGroup(INotificationChannelGroup group);

		/// <summary>
		/// Delete the channel group with the given ID.
		/// Channels belonging to the group are not deleted.
		/// </summary>
		public void DeleteNotificationChannelGroup(string groupId);

		/// <summary>Returns the channel group with the given ID, or <c>null</c> if not found.</summary>
		public INotificationChannelGroup GetNotificationChannelGroup(string groupId);

		/// <summary>Returns all registered notification channel groups.</summary>
		public INotificationChannelGroup[] GetNotificationChannelGroups();

		// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
		// Builders
		// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

		/// <summary>
		/// Creates a new <see cref="INotificationBuilder"/> pre-configured with the given channel ID.
		/// </summary>
		public INotificationBuilder CreateBuilder(string channelId);

		/// <summary>
		/// Creates a new <see cref="INotificationCompatBuilder"/> pre-configured with the given channel ID,
		/// giving access to extended styling options.
		/// </summary>
		public INotificationCompatBuilder CreateCompatBuilder(string channelId);

		// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
		// Events
		// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

		/// <summary>
		/// Raised when a notification is successfully posted.
		/// The argument contains the posted notification.
		/// </summary>
		public event Action<INotification> OnNotificationPosted;

		/// <summary>
		/// Raised when a notification is cancelled.
		/// Arguments: (tag, id) of the cancelled notification.
		/// </summary>
		public event Action<string, int> OnNotificationCancelled;

		/// <summary>
		/// Raised when a notification action is invoked by the user.
		/// Arguments: (notification, actionId).
		/// </summary>
		public event Action<INotification, string> OnNotificationActionInvoked;
	}
}
