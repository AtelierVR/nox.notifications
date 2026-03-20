namespace Nox.Notifications {
	/// <summary>
	/// Represents an action button attached to a notification.
	/// Similar to Android's Notification.Action.
	/// </summary>
	public interface INotificationAction {
		/// <summary>
		/// Unique identifier used to distinguish this action when it is invoked.
		/// </summary>
		public string ActionId { get; }

		/// <summary>
		/// Label shown on the action button.
		/// </summary>
		public string Title { get; }

		/// <summary>
		/// Optional icon name/key for the action button.
		/// </summary>
		public string Icon { get; }
	}
}
