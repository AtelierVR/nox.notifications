using Nox.Notifications;

namespace Nox.CCK.Notifications {
	/// <summary>
	/// Fluent builder for <see cref="INotificationChannel"/>.
	/// </summary>
	public sealed class NotificationChannelBuilder {
		private readonly string              _id;
		private readonly string              _name;
		private readonly NotificationImportance _importance;
		private string _description;
		private string _groupId;
		private bool   _enableVibration;

		/// <param name="id">Unique identifier of the channel.</param>
		/// <param name="name">User-visible name.</param>
		/// <param name="importance">Importance level controlling how interruptive this channel is.</param>
		public NotificationChannelBuilder(string id, string name,
			NotificationImportance importance = NotificationImportance.Default) {
			_id         = id;
			_name       = name;
			_importance = importance;
		}

		public NotificationChannelBuilder SetDescription(string description) { _description    = description; return this; }
		public NotificationChannelBuilder SetGroup(string groupId)           { _groupId        = groupId;     return this; }
		public NotificationChannelBuilder SetEnableVibration(bool enable)    { _enableVibration = enable;    return this; }

		/// <summary>Builds an <see cref="INotificationChannel"/> from the current state.</summary>
		public INotificationChannel Build() => new NotificationChannelData {
			Id              = _id,
			Name            = _name,
			Importance      = _importance,
			Description     = _description,
			GroupId         = _groupId,
			EnableVibration = _enableVibration
		};
	}
}
