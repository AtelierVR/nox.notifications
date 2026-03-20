using Nox.Notifications;

namespace Nox.CCK.Notifications {
	/// <summary>
	/// Extension methods on <see cref="INotificationManager"/> for common notification patterns.
	/// </summary>
	public static class NotificationHelper {
		// ── Quick post helpers ────────────────────────────────────────────────

		/// <summary>
		/// Posts a minimal notification with just a title and body text.
		/// </summary>
		/// <param name="manager">The notification manager.</param>
		/// <param name="channelId">Destination channel identifier.</param>
		/// <param name="title">Notification title.</param>
		/// <param name="text">Notification body text.</param>
		/// <returns>The id assigned to the posted notification.</returns>
		public static int Notify(this INotificationManager manager,
			string channelId, string title, string text) {
			var n = new NotificationBuilder()
				.SetChannelId(channelId)
				.SetTitle(title)
				.SetContentText(text)
				.Build();
			manager.Notify(n.Id, n);
			return n.Id;
		}

		/// <summary>
		/// Posts a rich compat notification using a pre-built <see cref="INotificationCompat"/>.
		/// </summary>
		public static int NotifyCompat(this INotificationManager manager, INotificationCompat notification) {
			manager.Notify(notification.Tag, notification.Id, notification);
			return notification.Id;
		}

		// ── Channel convenience ───────────────────────────────────────────────

		/// <summary>
		/// Creates or updates a notification channel using a fluent builder action.
		/// </summary>
		/// <example><code>
		/// manager.CreateChannel("chat", "Chat", NotificationImportance.High,
		///     b => b.SetDescription("Messages from other users"));
		/// </code></example>
		public static void CreateChannel(this INotificationManager manager,
			string id, string name, NotificationImportance importance,
			System.Action<NotificationChannelBuilder> configure = null) {
			var builder = new NotificationChannelBuilder(id, name, importance);
			configure?.Invoke(builder);
			manager.CreateNotificationChannel(builder.Build());
		}

		/// <summary>
		/// Creates or updates a channel group using a fluent builder action.
		/// </summary>
		public static void CreateChannelGroup(this INotificationManager manager,
			string id, string name,
			System.Action<NotificationChannelGroupBuilder> configure = null) {
			var builder = new NotificationChannelGroupBuilder(id, name);
			configure?.Invoke(builder);
			manager.CreateNotificationChannelGroup(builder.Build());
		}

		// ── Builder factories ─────────────────────────────────────────────────

		/// <summary>
		/// Creates a <see cref="NotificationCompatBuilder"/> pre-configured with the given channel ID.
		/// The returned builder has a strongly-typed API — all setters return
		/// <see cref="NotificationCompatBuilder"/> so chaining never requires a cast.
		/// </summary>
		public static NotificationCompatBuilder CompatBuilder(this INotificationManager manager,
			string channelId)
			=> new NotificationCompatBuilder().SetChannelId(channelId);

		/// <summary>
		/// Creates an <see cref="INotificationAction"/> for a simple button.
		/// </summary>
		public static INotificationAction BuildAction(string actionId, string title,
			string icon = null)
			=> new NotificationActionBuilder(actionId, title)
				.SetIcon(icon)
				.Build();
	}
}
