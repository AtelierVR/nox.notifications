using System.Collections.Generic;

namespace Nox.Notifications {
	/// <summary>
	/// A logical grouping of <see cref="INotificationChannel"/>s.
	/// Equivalent to Android's NotificationChannelGroup — lets users
	/// collapse/manage multiple channels under one label in the settings UI.
	/// </summary>
	public interface INotificationChannelGroup {
		/// <summary>Unique identifier of this group.</summary>
		public string Id { get; }

		/// <summary>User-visible name of this group.</summary>
		public string Name { get; }

		/// <summary>User-visible description shown in the settings UI.</summary>
		public string Description { get; }

		/// <summary>Whether this group has been blocked by the user.</summary>
		public bool IsBlocked { get; }

		/// <summary>
		/// The channels that belong to this group.
		/// </summary>
		public IReadOnlyList<INotificationChannel> Channels { get; }
	}
}
