using System.Collections.Generic;

namespace Nox.Notifications.Runtime {
	/// <summary>
	/// Concrete implementation of <see cref="INotificationChannelGroup"/>.
	/// </summary>
	internal sealed class NotificationChannelGroup : INotificationChannelGroup {
		public string Id          { get; internal set; }
		public string Name        { get; internal set; }
		public string Description { get; internal set; }
		public bool   IsBlocked   { get; internal set; }

		private readonly List<INotificationChannel> _channels = new();
		public IReadOnlyList<INotificationChannel> Channels => _channels;

		internal NotificationChannelGroup(string id, string name) {
			Id   = id;
			Name = name;
		}

		internal void AddChannel(INotificationChannel channel)
			=> _channels.Add(channel);

		internal void RemoveChannel(string channelId)
			=> _channels.RemoveAll(c => c.Id == channelId);
	}
}
