using System;
using System.Collections.Generic;
using System.Linq;
using Nox.CCK.Events;
using UnityEngine;

namespace Nox.Notifications.Runtime {
	/// <summary>
	/// Core implementation of <see cref="INotificationManager"/>.
	/// Manages channels, channel groups, and active notifications in-memory.
	/// </summary>
	internal sealed class NotificationManager : INotificationManager {
		// ── Active notifications ──────────────────────────────────────────────
		private readonly Dictionary<int, INotification> _active = new();

		// ── Channels ──────────────────────────────────────────────────────────
		private readonly Dictionary<string, NotificationChannel>      _channels = new();
		private readonly Dictionary<string, NotificationChannelGroup> _groups   = new();

		// ── Events ────────────────────────────────────────────────────────────
		public NoxEvent<INotification>      OnNotificationPosted        { get; } = new();
		public NoxEvent<int>                OnNotificationCancelled     { get; } = new();
		public NoxEvent<INotification, string> OnNotificationActionInvoked { get; } = new();

		// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
		// Posting
		// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

		public int Notify(INotification notification) {
			if (notification == null) {
				Debug.LogWarning("[Notifications] Notify called with a null notification.");
				return -1;
			}

			_active[notification.Id] = notification;

			ScheduleTimeout(notification.Id, notification);
			OnNotificationPosted?.Invoke(notification);
			Debug.Log($"[Notifications] Posted: id={notification.Id} title=\"{notification.Title}\"");
			return notification.Id;
		}

		// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
		// Cancelling
		// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

		public void Cancel(int id) {
			if (!_active.Remove(id)) return;
			OnNotificationCancelled?.Invoke(id);
			Debug.Log($"[Notifications] Cancelled: id={id}");
		}

		public void CancelAll() {
			foreach (var id in _active.Keys.ToArray())
				Cancel(id);
		}

		// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
		// Querying
		// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

		public bool IsPosted(int id) 
			=> _active.ContainsKey(id);

		public INotification[] GetActiveNotifications()
			=> _active.Values.ToArray();

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
			foreach (var id in _active.Keys
				         .Where(k => _active[k].ChannelId == channelId)
				         .ToArray())
				Cancel(id);

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
		internal void InvokeAction(int id, string actionId) {
			if (!_active.TryGetValue(id, out var notification)) return;
			OnNotificationActionInvoked?.Invoke(notification, actionId);

			if (notification.AutoCancel)
				Cancel(id);
		}

		private void ScheduleTimeout(int id, INotification notification) {
			if (notification is not INotificationCompat compat) return;
			if (compat.TimeoutAfter == null) return;

			// Use a simple fire-and-forget coroutine via UniTask-free approach
			var timeout = compat.TimeoutAfter.Value;
			ScheduleCancelAfter(id, timeout);
		}

		private async void ScheduleCancelAfter(int id, TimeSpan delay) {
			await System.Threading.Tasks.Task.Delay(delay);
			if (IsPosted(id))
				Cancel(id);
		}

		internal void Dispose() {
			_active.Clear();
			_channels.Clear();
			_groups.Clear();
		}
	}
}
