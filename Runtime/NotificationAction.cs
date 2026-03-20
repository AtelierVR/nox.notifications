using System;
using System.Collections.Generic;

namespace Nox.Notifications.Runtime {
	/// <summary>
	/// Concrete implementation of <see cref="INotificationAction"/>.
	/// </summary>
	internal sealed class NotificationAction : INotificationAction {
		public string ActionId { get; internal set; }
		public string Title    { get; internal set; }
		public string Icon     { get; internal set; }

		internal NotificationAction(string actionId, string title, string icon = null) {
			ActionId = actionId;
			Title    = title;
			Icon     = icon;
		}
	}
}
