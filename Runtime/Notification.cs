using System;

namespace Nox.Notifications.Runtime {
	/// <summary>
	/// Concrete implementation of <see cref="INotification"/>.
	/// </summary>
	internal class Notification : INotification {
		public int                    Id         { get; internal set; }
		public string                 Tag        { get; internal set; }
		public string                 ChannelId  { get; internal set; }
		public string                 Title      { get; internal set; }
		public string                 ContentText { get; internal set; }
		public string                 SmallIcon  { get; internal set; }
		public string                 LargeIcon  { get; internal set; }
		public DateTime               When       { get; internal set; }
		public bool                   AutoCancel { get; internal set; }
		public bool                   IsOngoing  { get; internal set; }
		public bool                   IsSilent   { get; internal set; }
		public NotificationPriority  Priority   { get; internal set; }
		public string                SenderId   { get; internal set; }
		public string                SenderName { get; internal set; }
		public string                Category   { get; internal set; }
		public INotificationAction[] Actions    { get; internal set; }
	}
}
