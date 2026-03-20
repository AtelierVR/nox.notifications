using System;
using System.Collections.Generic;
using Nox.Notifications;

namespace Nox.CCK.Notifications {
	// ─────────────────────────────────────────────────────────────────────────
	// Internal concrete data classes used as outputs of the CCK builders.
	// External code only sees these through their SDK interfaces.
	// ─────────────────────────────────────────────────────────────────────────

	internal sealed class NotificationActionData : INotificationAction {
		public string ActionId { get; set; }
		public string Title    { get; set; }
		public string Icon     { get; set; }
	}

	internal class NotificationData : INotification {
		public int                    Id          { get; set; }
		public string                 Tag         { get; set; }
		public string                 ChannelId   { get; set; }
		public string                 Title       { get; set; }
		public string                 ContentText { get; set; }
		public string                 SmallIcon   { get; set; }
		public string                 LargeIcon   { get; set; }
		public DateTime               When        { get; set; }
		public bool                   AutoCancel  { get; set; }
		public bool                   IsOngoing   { get; set; }
		public bool                   IsSilent    { get; set; }
		public NotificationPriority  Priority   { get; set; }
		public string                SenderId   { get; set; }
		public string                SenderName { get; set; }
		public string                Category   { get; set; }
		public INotificationAction[] Actions    { get; set; }
	}

	internal sealed class NotificationCompatData : NotificationData, INotificationCompat {
		public string    BigText               { get; set; }
		public string    BigPicture            { get; set; }
		public string    SubText               { get; set; }
		public string    ContentInfo           { get; set; }
		public int       ProgressMax           { get; set; }
		public int       Progress              { get; set; }
		public bool      ProgressIndeterminate { get; set; }
		public string    GroupKey              { get; set; }
		public string    SortKey               { get; set; }
		public TimeSpan? TimeoutAfter          { get; set; }
		public bool      ShowWhen              { get; set; } = true;

		private readonly IReadOnlyDictionary<string, object> _extras;
		public IReadOnlyDictionary<string, object> Extras => _extras;

		internal NotificationCompatData(IReadOnlyDictionary<string, object> extras) {
			_extras = extras ?? new Dictionary<string, object>();
		}
	}

	internal sealed class NotificationChannelData : INotificationChannel {
		public string                 Id                   { get; set; }
		public string                 Name                 { get; set; }
		public string                 Description          { get; set; }
		public NotificationImportance Importance           { get; set; } = NotificationImportance.Default;
		public string GroupId         { get; set; }
		public bool   EnableVibration { get; set; }
	}

	internal sealed class NotificationChannelGroupData : INotificationChannelGroup {
		public string                            Id          { get; set; }
		public string                            Name        { get; set; }
		public string                            Description { get; set; }
		public bool                              IsBlocked   { get; set; }
		public IReadOnlyList<INotificationChannel> Channels  { get; set; }
	}
}
