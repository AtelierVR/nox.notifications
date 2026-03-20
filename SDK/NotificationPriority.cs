namespace Nox.Notifications {
	/// <summary>
	/// Priority levels for individual notifications, similar to Android's Notification priority.
	/// Lower-importance channels override these.
	/// </summary>
	public enum NotificationPriority {
		/// <summary>Lowest priority; not shown unless the user pulls down the shade.</summary>
		Min = -2,

		/// <summary>Low priority; shown at the bottom of the list.</summary>
		Low = -1,

		/// <summary>Default priority.</summary>
		Default = 0,

		/// <summary>High priority; shown at the top of the list.</summary>
		High = 1,

		/// <summary>Highest priority; may appear as a heads-up notification.</summary>
		Max = 2
	}
}
