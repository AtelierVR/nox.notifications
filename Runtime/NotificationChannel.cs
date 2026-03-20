namespace Nox.Notifications.Runtime {
	/// <summary>
	/// Concrete implementation of <see cref="INotificationChannel"/>.
	/// </summary>
	internal sealed class NotificationChannel : INotificationChannel {
		public string                 Id          { get; internal set; }
		public string                 Name        { get; internal set; }
		public string                 Description { get; internal set; }
		public NotificationImportance Importance  { get; internal set; } = NotificationImportance.Default;
		public string                 GroupId     { get; internal set; }
		public bool                   EnableVibration { get; internal set; }

		internal NotificationChannel(string id, string name, NotificationImportance importance) {
			Id         = id;
			Name       = name;
			Importance = importance;
		}
	}
}
