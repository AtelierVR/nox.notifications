using System;
using System.Collections.Generic;
using Nox.Notifications;

namespace Nox.CCK.Notifications {
	/// <summary>
	/// Fluent builder that produces an <see cref="INotificationCompat"/> with expanded styles,
	/// progress bar, grouping and arbitrary extras.
	/// Extends <see cref="NotificationBuilderBase{TSelf}"/> so all base setters return
	/// <see cref="NotificationCompatBuilder"/> — no type casting needed when chaining.
	/// </summary>
	public sealed class NotificationCompatBuilder
		: NotificationBuilderBase<NotificationCompatBuilder>, INotificationCompatBuilder {

		// ── Compat-specific fields ─────────────────────────────────────────────

		private string    _bigText;
		private string    _bigPicture;
		private string    _subText;
		private string    _contentInfo;
		private int       _progressMax;
		private int       _progress;
		private bool      _progressIndeterminate;
		private string    _groupKey;
		private string    _sortKey;
		private TimeSpan? _timeoutAfter;
		private bool      _showWhen = true;
		private readonly Dictionary<string, object> _extras = new();

		// ── Strongly-typed compat setters (return NotificationCompatBuilder) ──

		public NotificationCompatBuilder SetBigText(string bigText)   { _bigText    = bigText;   return this; }
		public NotificationCompatBuilder SetBigPicture(string key)    { _bigPicture = key;       return this; }
		public NotificationCompatBuilder SetSubText(string subText)   { _subText    = subText;   return this; }
		public NotificationCompatBuilder SetContentInfo(string info)  { _contentInfo = info;     return this; }
		public NotificationCompatBuilder SetGroup(string groupKey)    { _groupKey   = groupKey;  return this; }
		public NotificationCompatBuilder SetSortKey(string sortKey)   { _sortKey    = sortKey;   return this; }
		public NotificationCompatBuilder SetTimeoutAfter(TimeSpan? t) { _timeoutAfter = t;       return this; }
		public NotificationCompatBuilder SetShowWhen(bool v)          { _showWhen   = v;         return this; }

		public NotificationCompatBuilder SetProgress(int max, int progress, bool indeterminate) {
			_progressMax           = max;
			_progress              = progress;
			_progressIndeterminate = indeterminate;
			return this;
		}

		public NotificationCompatBuilder PutExtra(string key, object value) {
			_extras[key] = value;
			return this;
		}

		// ── Explicit INotificationCompatBuilder interface implementation ──────

		INotificationCompatBuilder INotificationCompatBuilder.SetBigText(string v)         => SetBigText(v);
		INotificationCompatBuilder INotificationCompatBuilder.SetBigPicture(string v)      => SetBigPicture(v);
		INotificationCompatBuilder INotificationCompatBuilder.SetSubText(string v)         => SetSubText(v);
		INotificationCompatBuilder INotificationCompatBuilder.SetContentInfo(string v)     => SetContentInfo(v);
		INotificationCompatBuilder INotificationCompatBuilder.SetGroup(string v)           => SetGroup(v);
		INotificationCompatBuilder INotificationCompatBuilder.SetSortKey(string v)         => SetSortKey(v);
		INotificationCompatBuilder INotificationCompatBuilder.SetTimeoutAfter(TimeSpan? v) => SetTimeoutAfter(v);
		INotificationCompatBuilder INotificationCompatBuilder.SetShowWhen(bool v)          => SetShowWhen(v);
		INotificationCompatBuilder INotificationCompatBuilder.PutExtra(string k, object v) => PutExtra(k, v);
		INotificationCompatBuilder INotificationCompatBuilder.SetProgress(int m, int p, bool i)
			=> SetProgress(m, p, i);

		// ── Build ──────────────────────────────────────────────────────────────

		public override INotification Build() => BuildCompat();

		INotificationCompat INotificationCompatBuilder.Build() => BuildCompat();

		private NotificationCompatData BuildCompat() {
			var f = BaseFields;
			return new NotificationCompatData(new Dictionary<string, object>(_extras)) {
				Id                    = f.Id,
				Tag                   = f.Tag,
				ChannelId             = f.ChannelId,
				Title                 = f.Title,
				ContentText           = f.ContentText,
				SmallIcon             = f.SmallIcon,
				LargeIcon             = f.LargeIcon,
				When                  = f.When,
				AutoCancel            = f.AutoCancel,
				IsOngoing             = f.IsOngoing,
				IsSilent              = f.IsSilent,
				Priority              = f.Priority,
				SenderId              = f.SenderId,
				SenderName            = f.SenderName,
				Category              = f.Category,
				Actions               = f.Actions,
				BigText               = _bigText,
				BigPicture            = _bigPicture,
				SubText               = _subText,
				ContentInfo           = _contentInfo,
				ProgressMax           = _progressMax,
				Progress              = _progress,
				ProgressIndeterminate = _progressIndeterminate,
				GroupKey              = _groupKey,
				SortKey               = _sortKey,
				TimeoutAfter          = _timeoutAfter,
				ShowWhen              = _showWhen
			};
		}
	}
}
