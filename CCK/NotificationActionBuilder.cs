using Nox.Notifications;

namespace Nox.CCK.Notifications {
	/// <summary>
	/// Fluent builder for <see cref="INotificationAction"/> (action buttons on a notification).
	/// </summary>
	public sealed class NotificationActionBuilder {
		private readonly string _actionId;
		private readonly string _title;
		private          string _icon;

		/// <param name="actionId">Unique identifier used to distinguish this action when invoked.</param>
		/// <param name="title">Label shown on the action button.</param>
		public NotificationActionBuilder(string actionId, string title) {
			_actionId = actionId;
			_title    = title;
		}

		/// <summary>Sets an optional icon key for the action button.</summary>
		public NotificationActionBuilder SetIcon(string icon) {
			_icon = icon;
			return this;
		}

		/// <summary>Builds an <see cref="INotificationAction"/> from the current state.</summary>
		public INotificationAction Build() => new NotificationActionData {
			ActionId = _actionId,
			Title    = _title,
			Icon     = _icon
		};
	}
}
