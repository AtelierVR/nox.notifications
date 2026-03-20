namespace Nox.Notifications.Runtime {
	/// <summary>
	/// Fluent builder for creating <see cref="INotificationChannel"/> instances.
	/// </summary>
	public sealed class NotificationChannelBuilder {
		private readonly string              _id;
		private          string              _name;
		private          string              _description;
		private          NotificationImportance _importance = NotificationImportance.Default;
		private          string              _groupId;
		private bool _enableVibration;

		public NotificationChannelBuilder(string id, string name,
			NotificationImportance importance = NotificationImportance.Default) {
			_id         = id;
			_name       = name;
			_importance = importance;
		}

		public NotificationChannelBuilder SetDescription(string description) {
			_description = description;
			return this;
		}

		public NotificationChannelBuilder SetGroup(string groupId) {
			_groupId = groupId;
			return this;
		}

		public NotificationChannelBuilder SetEnableVibration(bool enable) {
			_enableVibration = enable;
			return this;
		}

		public INotificationChannel Build() => new NotificationChannel(_id, _name, _importance) {
			Description     = _description,
			GroupId         = _groupId,
			EnableVibration = _enableVibration
		};
	}
}
