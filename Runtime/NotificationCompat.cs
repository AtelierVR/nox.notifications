using System;
using System.Collections.Generic;

namespace Nox.Notifications.Runtime {
	/// <summary>
	/// Concrete implementation of <see cref="INotificationCompat"/>; extends <see cref="Notification"/>.
	/// </summary>
	internal sealed class NotificationCompat : Notification, INotificationCompat {
		public string    BigText               { get; internal set; }
		public string    BigPicture            { get; internal set; }
		public string    SubText               { get; internal set; }
		public string    ContentInfo           { get; internal set; }
		public int       ProgressMax           { get; internal set; }
		public int       Progress              { get; internal set; }
		public bool      ProgressIndeterminate { get; internal set; }
		public string    GroupKey              { get; internal set; }
		public string    SortKey               { get; internal set; }
		public TimeSpan? TimeoutAfter          { get; internal set; }
		public bool      ShowWhen              { get; internal set; } = true;

		private readonly Dictionary<string, object> _extras = new();
		public IReadOnlyDictionary<string, object> Extras => _extras;

		internal void AddExtra(string key, object value)
			=> _extras[key] = value;
	}
}
