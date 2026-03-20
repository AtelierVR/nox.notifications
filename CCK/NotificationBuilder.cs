using System;
using System.Collections.Generic;
using Nox.Notifications;

namespace Nox.CCK.Notifications {
	/// <summary>
	/// Generic fluent base builder for notifications.
	/// <typeparamref name="TSelf"/> is the concrete builder type, which lets every method
	/// return the exact subtype so chained calls never lose the derived API.
	/// </summary>
	/// <typeparam name="TSelf">The concrete builder class inheriting from this base.</typeparam>
	public abstract class NotificationBuilderBase<TSelf> : INotificationBuilder
		where TSelf : NotificationBuilderBase<TSelf> {

		private static int _autoId = 1;

		protected string                 _channelId   = string.Empty;
		protected string                 _title       = string.Empty;
		protected string                 _contentText = string.Empty;
		protected string                 _smallIcon   = string.Empty;
		protected string                 _largeIcon;
		protected DateTime               _when        = DateTime.Now;
		protected bool                   _autoCancel;
		protected bool                   _ongoing;
		protected bool                   _silent;
		protected NotificationPriority               _priority   = NotificationPriority.Default;
		protected string                             _senderId;
		protected string                             _senderName;
		protected string                             _category;
		protected readonly List<INotificationAction> _actions    = new();
		protected int                                _id         = _autoId++;
		protected string                             _tag;

		// ── Strongly-typed fluent setters ─────────────────────────────────────

		public TSelf SetChannelId(string channelId)  { _channelId   = channelId;  return (TSelf)this; }
		public TSelf SetTitle(string title)           { _title       = title;      return (TSelf)this; }
		public TSelf SetContentText(string text)      { _contentText = text;       return (TSelf)this; }
		public TSelf SetSmallIcon(string icon)        { _smallIcon   = icon;       return (TSelf)this; }
		public TSelf SetLargeIcon(string icon)        { _largeIcon   = icon;       return (TSelf)this; }
		public TSelf SetWhen(DateTime when)           { _when        = when;       return (TSelf)this; }
		public TSelf SetAutoCancel(bool autoCancel)   { _autoCancel  = autoCancel; return (TSelf)this; }
		public TSelf SetOngoing(bool ongoing)         { _ongoing     = ongoing;    return (TSelf)this; }
		public TSelf SetSilent(bool silent)           { _silent      = silent;     return (TSelf)this; }
		public TSelf SetPriority(NotificationPriority p)   { _priority   = p;          return (TSelf)this; }
		public TSelf SetSenderId(string senderId)          { _senderId   = senderId;   return (TSelf)this; }
		public TSelf SetSenderName(string senderName)      { _senderName = senderName; return (TSelf)this; }
		public TSelf SetCategory(string category)          { _category   = category;   return (TSelf)this; }
		public TSelf SetId(int id)                         { _id         = id;         return (TSelf)this; }
		public TSelf SetTag(string tag)                    { _tag        = tag;        return (TSelf)this; }

		public TSelf AddAction(INotificationAction action) {
			_actions.Add(action);
			return (TSelf)this;
		}

		// ── Explicit INotificationBuilder implementation (returns interface type) ──

		INotificationBuilder INotificationBuilder.SetChannelId(string v)         => SetChannelId(v);
		INotificationBuilder INotificationBuilder.SetTitle(string v)             => SetTitle(v);
		INotificationBuilder INotificationBuilder.SetContentText(string v)       => SetContentText(v);
		INotificationBuilder INotificationBuilder.SetSmallIcon(string v)         => SetSmallIcon(v);
		INotificationBuilder INotificationBuilder.SetLargeIcon(string v)         => SetLargeIcon(v);
		INotificationBuilder INotificationBuilder.SetWhen(DateTime v)            => SetWhen(v);
		INotificationBuilder INotificationBuilder.SetAutoCancel(bool v)          => SetAutoCancel(v);
		INotificationBuilder INotificationBuilder.SetOngoing(bool v)             => SetOngoing(v);
		INotificationBuilder INotificationBuilder.SetSilent(bool v)              => SetSilent(v);
		INotificationBuilder INotificationBuilder.SetPriority(NotificationPriority v)  => SetPriority(v);
		INotificationBuilder INotificationBuilder.SetSenderId(string v)               => SetSenderId(v);
		INotificationBuilder INotificationBuilder.SetSenderName(string v)             => SetSenderName(v);
		INotificationBuilder INotificationBuilder.SetCategory(string v)               => SetCategory(v);
		INotificationBuilder INotificationBuilder.SetId(int v)                        => SetId(v);
		INotificationBuilder INotificationBuilder.SetTag(string v)                    => SetTag(v);
		INotificationBuilder INotificationBuilder.AddAction(INotificationAction v)    => AddAction(v);

		// ── Build ─────────────────────────────────────────────────────────────

		public abstract INotification Build();

		/// <summary>
		/// Exposes the current base-field values as a named tuple so subclasses can
		/// pass them into any <see cref="NotificationData"/> object initializer.
		/// </summary>
		protected (int Id, string Tag, string ChannelId, string Title, string ContentText,
			string SmallIcon, string LargeIcon, DateTime When, bool AutoCancel, bool IsOngoing,
			bool IsSilent, NotificationPriority Priority, string SenderId, string SenderName,
			string Category, INotificationAction[] Actions) BaseFields =>
			(_id, _tag, _channelId, _title, _contentText, _smallIcon, _largeIcon, _when,
				_autoCancel, _ongoing, _silent, _priority, _senderId, _senderName,
				_category, _actions.ToArray());

		/// <summary>Builds a plain <see cref="NotificationData"/> from the current base state.</summary>
		private protected NotificationData BuildBase() => new NotificationData {
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

	/// <summary>
	/// Concrete notification builder — produces a plain <see cref="INotification"/>.
	/// Use this when you don't need expanded styles or extras.
	/// </summary>
	public sealed class NotificationBuilder : NotificationBuilderBase<NotificationBuilder> {
		public override INotification Build() => BuildBase();
	}
}
