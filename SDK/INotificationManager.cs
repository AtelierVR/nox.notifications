using Nox.CCK.Events;

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
		/// <returns>The notification id.</returns>
		public int Notify(INotification notification);

		// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
		// Cancelling
		// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

		/// <summary>Cancel a previously posted notification by ID.</summary>
		/// <param name="id">The ID of the notification to cancel.</param>
		public void Cancel(int id);

		/// <summary>Cancel all notifications.</summary>
		public void CancelAll();

		// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
		// Querying
		// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

		/// <summary>
		/// Returns whether a notification with the given ID is currently active.
		/// </summary>
		public bool IsPosted(int id);

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
		public NoxEvent<INotification> OnNotificationPosted { get; }

		/// <summary>
		/// Raised when a notification is cancelled.
		/// Argument: id of the cancelled notification.
		/// </summary>
		public NoxEvent<int> OnNotificationCancelled { get; }

		/// <summary>
		/// Raised when a notification action is invoked by the user.
		/// Arguments: (notification, actionId).
		/// </summary>
		public NoxEvent<INotification, string> OnNotificationActionInvoked { get; }
	}
}
