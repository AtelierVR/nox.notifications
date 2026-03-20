using System.Collections.Generic;
using Nox.Notifications;

namespace Nox.CCK.Notifications {
	/// <summary>
	/// Fluent builder for <see cref="INotificationChannelGroup"/>.
	/// </summary>
	public sealed class NotificationChannelGroupBuilder {
		private readonly string _id;
		private readonly string _name;
		private string          _description;
		private readonly List<INotificationChannel> _channels = new();

		/// <param name="id">Unique identifier of the group.</param>
		/// <param name="name">User-visible name.</param>
		public NotificationChannelGroupBuilder(string id, string name) {
			_id   = id;
			_name = name;
		}

		public NotificationChannelGroupBuilder SetDescription(string description) {
			_description = description;
			return this;
		}

		/// <summary>
		/// Pre-populates the group with an existing channel.
		/// Channels are typically assigned by setting their <c>GroupId</c> when registering them
		/// with <see cref="INotificationManager.CreateNotificationChannel"/>; this overload is a
		/// convenience for building the group data object with known channels.
		/// </summary>
		public NotificationChannelGroupBuilder AddChannel(INotificationChannel channel) {
			if (channel != null) _channels.Add(channel);
			return this;
		}

		/// <summary>Builds an <see cref="INotificationChannelGroup"/> from the current state.</summary>
		public INotificationChannelGroup Build() => new NotificationChannelGroupData {
			Id          = _id,
			Name        = _name,
			Description = _description,
			IsBlocked   = false,
			Channels    = _channels.AsReadOnly()
		};
	}
}
