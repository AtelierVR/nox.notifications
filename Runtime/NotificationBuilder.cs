using System;
using System.Collections.Generic;

namespace Nox.Notifications.Runtime {
	/// <summary>
	/// Fluent builder for <see cref="INotification"/> — implements <see cref="INotificationBuilder"/>.
	/// </summary>
	public class NotificationBuilder : INotificationBuilder {
		private static int _autoId = 1;

		protected string                  _channelId  = string.Empty;
		protected string                  _title      = string.Empty;
		protected string                  _contentText = string.Empty;
		protected string                  _smallIcon  = string.Empty;
		protected string                  _largeIcon;
		protected DateTime                _when       = DateTime.Now;
		protected bool                    _autoCancel;
		protected bool                    _ongoing;
		protected bool                    _silent;
		protected NotificationPriority               _priority   = NotificationPriority.Default;
		protected string                             _senderId;
		protected string                             _senderName;
		protected string                             _category;
		protected readonly List<INotificationAction> _actions    = new();
		protected int                                _id         = _autoId++;
		protected string                             _tag;

		public INotificationBuilder SetChannelId(string channelId)  { _channelId   = channelId;  return this; }
		public INotificationBuilder SetTitle(string title)           { _title       = title;      return this; }
		public INotificationBuilder SetContentText(string text)      { _contentText = text;       return this; }
		public INotificationBuilder SetSmallIcon(string icon)        { _smallIcon   = icon;       return this; }
		public INotificationBuilder SetLargeIcon(string icon)        { _largeIcon   = icon;       return this; }
		public INotificationBuilder SetWhen(DateTime when)           { _when        = when;       return this; }
		public INotificationBuilder SetAutoCancel(bool autoCancel)   { _autoCancel  = autoCancel; return this; }
		public INotificationBuilder SetOngoing(bool ongoing)         { _ongoing     = ongoing;    return this; }
		public INotificationBuilder SetSilent(bool silent)           { _silent      = silent;     return this; }
		public INotificationBuilder SetPriority(NotificationPriority p) { _priority   = p;          return this; }
		public INotificationBuilder SetSenderId(string senderId)        { _senderId   = senderId;   return this; }
		public INotificationBuilder SetSenderName(string senderName)    { _senderName = senderName; return this; }
		public INotificationBuilder SetCategory(string category)        { _category   = category;   return this; }
		public INotificationBuilder SetId(int id)                       { _id         = id;         return this; }
		public INotificationBuilder SetTag(string tag)                  { _tag        = tag;        return this; }

		public INotificationBuilder AddAction(INotificationAction action) {
			_actions.Add(action);
			return this;
		}

		public virtual INotification Build() => new Notification {
			Id          = _id,
			Tag         = _tag,
			ChannelId   = _channelId,
			Title       = _title,
			ContentText = _contentText,
			SmallIcon   = _smallIcon,
			LargeIcon   = _largeIcon,
			When        = _when,
			AutoCancel  = _autoCancel,
			IsOngoing   = _ongoing,
			IsSilent    = _silent,
			Priority    = _priority,
			SenderId    = _senderId,
			SenderName  = _senderName,
			Category    = _category,
			Actions     = _actions.ToArray()
		};
	}
}
