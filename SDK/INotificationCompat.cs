using System;
using System.Collections.Generic;

namespace Nox.Notifications {
	/// <summary>
	/// Extended notification interface with richer styling options.
	/// Similar to Android's NotificationCompat from the support library —
	/// adds expanded views, progress bars, grouping and extras.
	/// </summary>
	public interface INotificationCompat : INotification {
		// ── Expanded styles ───────────────────────────────────────────────────

		/// <summary>
		/// Expanded big-text body. When non-null, the notification expands to show this text
		/// instead of <see cref="INotification.ContentText"/>.
		/// </summary>
		public string BigText { get; }

		/// <summary>
		/// Key/name of a large image shown when the notification is expanded (BigPictureStyle).
		/// </summary>
		public string BigPicture { get; }

		// ── Extra text fields ──────────────────────────────────────────────────

		/// <summary>
		/// Third line of text in the notification displayed below the content text.
		/// </summary>
		public string SubText { get; }

		/// <summary>
		/// Right-hand side text (e.g., "12 new" in an email notification).
		/// </summary>
		public string ContentInfo { get; }

		// ── Progress bar ──────────────────────────────────────────────────────

		/// <summary>Maximum value of the progress bar; 0 hides the progress bar.</summary>
		public int ProgressMax { get; }

		/// <summary>Current progress value.</summary>
		public int Progress { get; }

		/// <summary>Whether the progress bar is indeterminate.</summary>
		public bool ProgressIndeterminate { get; }

		// ── Grouping ──────────────────────────────────────────────────────────

		/// <summary>
		/// Key used to group several notifications together in the shade.
		/// </summary>
		public string GroupKey { get; }

		/// <summary>
		/// Key used to sort this notification relative to others in the same group.
		/// </summary>
		public string SortKey { get; }

		// ── Timing & lifecycle ────────────────────────────────────────────────

		/// <summary>
		/// Optional duration after which the notification is automatically cancelled.
		/// </summary>
		public TimeSpan? TimeoutAfter { get; }

		/// <summary>
		/// Whether to show the <see cref="INotification.When"/> timestamp.
		/// </summary>
		public bool ShowWhen { get; }

		// ── Extras ────────────────────────────────────────────────────────────

		/// <summary>
		/// Arbitrary key-value data attached to the notification.
		/// </summary>
		public IReadOnlyDictionary<string, object> Extras { get; }
	}
}
