namespace Nox.Notifications.Runtime {
	/// <summary>
	/// Fluent builder for creating <see cref="INotificationAction"/> instances.
	/// </summary>
	public sealed class NotificationActionBuilder {
		private readonly string _actionId;
		private readonly string _title;
		private          string _icon;

		public NotificationActionBuilder(string actionId, string title) {
			_actionId = actionId;
			_title    = title;
		}

		/// <summary>Sets an optional icon key for the action button.</summary>
		public NotificationActionBuilder SetIcon(string icon) {
			_icon = icon;
			return this;
		}

		public INotificationAction Build()
			=> new NotificationAction(_actionId, _title, _icon);
	}
}
