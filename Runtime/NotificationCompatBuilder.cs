using System;
using System.Collections.Generic;

namespace Nox.Notifications.Runtime {
	/// <summary>
	/// Fluent builder for <see cref="INotificationCompat"/> — extends <see cref="NotificationBuilder"/>
	/// and implements <see cref="INotificationCompatBuilder"/>.
	/// </summary>
	public sealed class NotificationCompatBuilder : NotificationBuilder, INotificationCompatBuilder {
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

		// ── Wrappers that coerce return type back to INotificationCompatBuilder ──

		INotificationCompatBuilder INotificationCompatBuilder.SetBigText(string bigText) {
			_bigText = bigText;
			return this;
		}

		INotificationCompatBuilder INotificationCompatBuilder.SetBigPicture(string key) {
			_bigPicture = key;
			return this;
		}

		INotificationCompatBuilder INotificationCompatBuilder.SetSubText(string subText) {
			_subText = subText;
			return this;
		}

		INotificationCompatBuilder INotificationCompatBuilder.SetContentInfo(string contentInfo) {
			_contentInfo = contentInfo;
			return this;
		}

		INotificationCompatBuilder INotificationCompatBuilder.SetProgress(int max, int progress, bool indeterminate) {
			_progressMax          = max;
			_progress             = progress;
			_progressIndeterminate = indeterminate;
			return this;
		}

		INotificationCompatBuilder INotificationCompatBuilder.SetGroup(string groupKey) {
			_groupKey = groupKey;
			return this;
		}

		INotificationCompatBuilder INotificationCompatBuilder.SetSortKey(string sortKey) {
			_sortKey = sortKey;
			return this;
		}

		INotificationCompatBuilder INotificationCompatBuilder.SetTimeoutAfter(TimeSpan? timeout) {
			_timeoutAfter = timeout;
			return this;
		}

		INotificationCompatBuilder INotificationCompatBuilder.SetShowWhen(bool showWhen) {
			_showWhen = showWhen;
			return this;
		}

		INotificationCompatBuilder INotificationCompatBuilder.PutExtra(string key, object value) {
			_extras[key] = value;
			return this;
		}

		INotificationCompat INotificationCompatBuilder.Build() => BuildCompat();

		public override INotification Build() => BuildCompat();

		private NotificationCompat BuildCompat() {
			var n = new NotificationCompat {
				Id                    = _id,
				Tag                   = _tag,
				ChannelId             = _channelId,
				Title                 = _title,
				ContentText           = _contentText,
				SmallIcon             = _smallIcon,
				LargeIcon             = _largeIcon,
				When                  = _when,
				AutoCancel            = _autoCancel,
				IsOngoing             = _ongoing,
				IsSilent              = _silent,
				Priority              = _priority,
				SenderId              = _senderId,
				SenderName            = _senderName,
				Category              = _category,
				Actions               = _actions.ToArray(),
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

			foreach (var kv in _extras)
				n.AddExtra(kv.Key, kv.Value);

			return n;
		}
	}
}
