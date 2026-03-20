using System;

namespace Nox.Notifications {
	/// <summary>
	/// Extended fluent builder for constructing <see cref="INotificationCompat"/> instances.
	/// Adds expanded styles, progress, grouping and extras on top of <see cref="INotificationBuilder"/>.
	/// Similar to Android's NotificationCompat.Builder from the support library.
	/// </summary>
	public interface INotificationCompatBuilder : INotificationBuilder {
		// ── Expanded styles ───────────────────────────────────────────────────

		/// <summary>
		/// Sets the expanded big-text body (BigTextStyle).
		/// When set, the notification will show this text when expanded.
		/// </summary>
		public INotificationCompatBuilder SetBigText(string bigText);

		/// <summary>
		/// Sets a large image shown in the expanded view (BigPictureStyle).
		/// </summary>
		public INotificationCompatBuilder SetBigPicture(string bigPictureKey);

		// ── Extra text ────────────────────────────────────────────────────────

		/// <summary>Sets a third line of text below the content text.</summary>
		public INotificationCompatBuilder SetSubText(string subText);

		/// <summary>Sets the right-hand side info text (e.g., "12 new").</summary>
		public INotificationCompatBuilder SetContentInfo(string contentInfo);

		// ── Progress ──────────────────────────────────────────────────────────

		/// <summary>
		/// Shows a progress bar in the notification.
		/// Set <paramref name="max"/> to 0 to hide it.
		/// </summary>
		public INotificationCompatBuilder SetProgress(int max, int progress, bool indeterminate);

		// ── Grouping ──────────────────────────────────────────────────────────

		/// <summary>Sets the group key for bundling related notifications together.</summary>
		public INotificationCompatBuilder SetGroup(string groupKey);

		/// <summary>Sets the sort key used to order notifications within the same group.</summary>
		public INotificationCompatBuilder SetSortKey(string sortKey);

		// ── Lifecycle ─────────────────────────────────────────────────────────

		/// <summary>
		/// Sets a duration after which the notification is automatically cancelled.
		/// Pass <c>null</c> to disable the timeout.
		/// </summary>
		public INotificationCompatBuilder SetTimeoutAfter(TimeSpan? timeout);

		/// <summary>Controls whether the When timestamp is shown in the notification header.</summary>
		public INotificationCompatBuilder SetShowWhen(bool showWhen);

		// ── Extras ────────────────────────────────────────────────────────────

		/// <summary>Attaches an arbitrary key-value pair to the notification extras.</summary>
		public INotificationCompatBuilder PutExtra(string key, object value);

		/// <summary>Constructs the <see cref="INotificationCompat"/> from the current builder state.</summary>
		public new INotificationCompat Build();
	}
}
