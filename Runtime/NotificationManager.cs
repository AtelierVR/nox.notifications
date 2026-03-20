using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Nox.Notifications.Runtime {
	/// <summary>
	/// Core implementation of <see cref="INotificationManager"/>.
	/// Manages channels, channel groups, and active notifications in-memory.
	/// </summary>
	internal sealed class NotificationManager : INotificationManager {
		// ── Active notifications ──────────────────────────────────────────────
		private readonly Dictionary<(string tag, int id), INotification> _active = new();

		// ── Channels ──────────────────────────────────────────────────────────
		private readonly Dictionary<string, NotificationChannel>      _channels = new();
		private readonly Dictionary<string, NotificationChannelGroup> _groups   = new();

		// ── Events ────────────────────────────────────────────────────────────
		public event Action<INotification>      OnNotificationPosted;
		public event Action<string, int>        OnNotificationCancelled;
		public event Action<INotification, string> OnNotificationActionInvoked;

		// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
		// Posting
		// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

		public void Notify(int id, INotification notification)
			=> Notify(null, id, notification);

		public void Notify(string tag, int id, INotification notification) {
			if (notification == null) {
				Debug.LogWarning("[Notifications] Notify called with a null notification.");
				return;
			}

			var key = (tag, id);
			_active[key] = notification;

			ScheduleTimeout(tag, id, notification);
			OnNotificationPosted?.Invoke(notification);
			Debug.Log($"[Notifications] Posted: tag={tag ?? "(none)"} id={id} title=\"{notification.Title}\"");
		}

		// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
		// Cancelling
		// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

		public void Cancel(int id)
			=> Cancel(null, id);

		public void Cancel(string tag, int id) {
			var key = (tag, id);
			if (!_active.Remove(key)) return;
			OnNotificationCancelled?.Invoke(tag, id);
			Debug.Log($"[Notifications] Cancelled: tag={tag ?? "(none)"} id={id}");
		}

		public void CancelAll() {
			foreach (var key in _active.Keys.ToArray())
				Cancel(key.tag, key.id);
		}

		// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
		// Querying
		// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

		public bool IsPosted(int id)          => _active.ContainsKey((null, id));
		public bool IsPosted(string tag, int id) => _active.ContainsKey((tag, id));
		public INotification[] GetActiveNotifications() => _active.Values.ToArray();

		// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
		// Channels
		// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

		public void CreateNotificationChannel(INotificationChannel channel) {
			if (channel == null || string.IsNullOrEmpty(channel.Id)) {
				Debug.LogWarning("[Notifications] CreateNotificationChannel: channel is null or has no Id.");
				return;
			}

			var impl = new NotificationChannel(channel.Id, channel.Name, channel.Importance) {
				Description     = channel.Description,
				GroupId         = channel.GroupId,
				EnableVibration = channel.EnableVibration
			};

			_channels[channel.Id] = impl;

			// Register channel into its group if applicable
			if (!string.IsNullOrEmpty(channel.GroupId)
			    && _groups.TryGetValue(channel.GroupId, out var grp))
				grp.AddChannel(impl);

			Debug.Log($"[Notifications] Channel created: {channel.Id} (\"{channel.Name}\")");
		}

		public void CreateNotificationChannels(INotificationChannel[] channels) {
			if (channels == null) return;
			foreach (var ch in channels)
				CreateNotificationChannel(ch);
		}

		public void DeleteNotificationChannel(string channelId) {
			if (!_channels.TryGetValue(channelId, out var ch)) return;
			_channels.Remove(channelId);

			// Remove from group
			if (!string.IsNullOrEmpty(ch.GroupId)
			    && _groups.TryGetValue(ch.GroupId, out var grp))
				grp.RemoveChannel(channelId);

			// Cancel all active notifications on this channel
			foreach (var key in _active.Keys
				         .Where(k => _active[k].ChannelId == channelId)
				         .ToArray())
				Cancel(key.tag, key.id);

			Debug.Log($"[Notifications] Channel deleted: {channelId}");
		}

		public INotificationChannel GetNotificationChannel(string channelId)
			=> _channels.TryGetValue(channelId, out var ch) ? ch : null;

		public INotificationChannel[] GetNotificationChannels()
			=> _channels.Values.Cast<INotificationChannel>().ToArray();

		// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
		// Channel groups
		// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

		public void CreateNotificationChannelGroup(INotificationChannelGroup group) {
			if (group == null || string.IsNullOrEmpty(group.Id)) {
				Debug.LogWarning("[Notifications] CreateNotificationChannelGroup: group is null or has no Id.");
				return;
			}

			var impl = new NotificationChannelGroup(group.Id, group.Name) {
				Description = group.Description
			};

			_groups[group.Id] = impl;
			Debug.Log($"[Notifications] Channel group created: {group.Id} (\"{group.Name}\")");
		}

		public void DeleteNotificationChannelGroup(string groupId) {
			if (!_groups.ContainsKey(groupId)) return;
			_groups.Remove(groupId);
			Debug.Log($"[Notifications] Channel group deleted: {groupId}");
		}

		public INotificationChannelGroup GetNotificationChannelGroup(string groupId)
			=> _groups.TryGetValue(groupId, out var g) ? g : null;

		public INotificationChannelGroup[] GetNotificationChannelGroups()
			=> _groups.Values.Cast<INotificationChannelGroup>().ToArray();

		// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
		// Builders
		// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

		public INotificationBuilder CreateBuilder(string channelId)
			=> new Nox.CCK.Notifications.NotificationBuilder().SetChannelId(channelId);

		public INotificationCompatBuilder CreateCompatBuilder(string channelId)
			=> new Nox.CCK.Notifications.NotificationCompatBuilder().SetChannelId(channelId);

		// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
		// Internal helpers
		// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

		/// <summary>
		/// Invokes an action on a notification and raises the action event.
		/// Called externally by the UI layer when the user taps an action button.
		/// </summary>
		internal void InvokeAction(string tag, int id, string actionId) {
			if (!_active.TryGetValue((tag, id), out var notification)) return;
			OnNotificationActionInvoked?.Invoke(notification, actionId);

			if (notification.AutoCancel)
				Cancel(tag, id);
		}

		private void ScheduleTimeout(string tag, int id, INotification notification) {
			if (notification is not INotificationCompat compat) return;
			if (compat.TimeoutAfter == null) return;

			// Use a simple fire-and-forget coroutine via UniTask-free approach
			var timeout = compat.TimeoutAfter.Value;
			ScheduleCancelAfter(tag, id, timeout);
		}

		private async void ScheduleCancelAfter(string tag, int id, TimeSpan delay) {
			await System.Threading.Tasks.Task.Delay(delay);
			if (IsPosted(tag, id))
				Cancel(tag, id);
		}

		internal void Dispose() {
			_active.Clear();
			_channels.Clear();
			_groups.Clear();
		}
	}
}
