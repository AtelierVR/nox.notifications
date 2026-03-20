namespace Nox.Notifications {
	/// <summary>
	/// Semantic category constants for notifications in a social game context.
	/// Used to help the system rank, filter, and route notifications.
	/// </summary>
	public static class NotificationCategory {
		/// <summary>Incoming friend request from another user.</summary>
		public const string FriendRequest = "friend_request";

		/// <summary>Invitation to join a party or group.</summary>
		public const string PartyInvite = "party_invite";

		/// <summary>Invitation to join a world or instance.</summary>
		public const string WorldInvite = "world_invite";

		/// <summary>Direct message or chat notification.</summary>
		public const string Message = "message";

		/// <summary>Achievement or milestone unlocked.</summary>
		public const string Achievement = "achievement";

		/// <summary>Progress of a long-running background operation (e.g. asset loading).</summary>
		public const string Progress = "progress";

		/// <summary>System or moderation alert.</summary>
		public const string Alert = "alert";

		/// <summary>General system or service notification.</summary>
		public const string System = "system";
	}
}
