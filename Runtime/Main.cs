using Nox.CCK.Language;
using Nox.CCK.Mods.Cores;
using Nox.CCK.Mods.Initializers;
using Nox.CCK.Settings;
using Nox.Settings;
using UnityEngine;

namespace Nox.Notifications.Runtime {
	/// <summary>
	/// Mod entry point for <c>nox.notifications</c>.
	/// Implements <see cref="IMainModInitializer"/> and exposes <see cref="INotificationManager"/>
	/// as the mod's primary instance.
	/// </summary>
	public class Main : IMainModInitializer, INotificationManager {
		public static Main Instance { get; private set; }
		internal static IMainModCoreAPI CoreAPI { get; private set; }

		// Delegate all work to the internal manager
		private NotificationManager _manager;
		private LanguagePack _lang;

		// ── Settings ──────────────────────────────────────────────────────────

		internal static ISettingAPI SettingAPI
			=> CoreAPI?.ModAPI?.GetMod("settings")?.GetInstance<ISettingAPI>();

		private IHandler[] _settingHandlers = System.Array.Empty<IHandler>();

		// ── Lifecycle ─────────────────────────────────────────────────────────

		public void OnInitializeMain(IMainModCoreAPI api) {
			CoreAPI   = api;
			Instance  = this;
			_manager  = new NotificationManager();
			_lang     = CoreAPI.AssetAPI.GetAsset<LanguagePack>("lang.asset");
			LanguageManager.AddPack(_lang);

			_settingHandlers = new IHandler[] {
				new Settings.EnabledSetting(),
				new Settings.DoNotDisturbSetting(),
				new Settings.SoundSetting(),
				new Settings.VibrationSetting(),
				new Settings.BadgeSetting(),
			};
			foreach (var h in _settingHandlers)
				SettingAPI?.Add(h);

			Debug.Log("[Notifications] Mod initialized.");
		}

		public void OnDisposeMain() {
			foreach (var h in _settingHandlers)
				SettingAPI?.Remove(h.GetPath());
			_settingHandlers = System.Array.Empty<IHandler>();

			LanguageManager.RemovePack(_lang);
			_lang = null;

			_manager?.Dispose();
			_manager = null;
			Instance = null;
			CoreAPI  = null;
			Debug.Log("[Notifications] Mod disposed.");
		}

		// ── INotificationManager delegation ───────────────────────────────────

		public void Notify(int id, INotification notification)
			=> _manager.Notify(id, notification);

		public void Notify(string tag, int id, INotification notification)
			=> _manager.Notify(tag, id, notification);

		public void Cancel(int id)
			=> _manager.Cancel(id);

		public void Cancel(string tag, int id)
			=> _manager.Cancel(tag, id);

		public void CancelAll()
			=> _manager.CancelAll();

		public bool IsPosted(int id)
			=> _manager.IsPosted(id);

		public bool IsPosted(string tag, int id)
			=> _manager.IsPosted(tag, id);

		public INotification[] GetActiveNotifications()
			=> _manager.GetActiveNotifications();

		public void CreateNotificationChannel(INotificationChannel channel)
			=> _manager.CreateNotificationChannel(channel);

		public void CreateNotificationChannels(INotificationChannel[] channels)
			=> _manager.CreateNotificationChannels(channels);

		public void DeleteNotificationChannel(string channelId)
			=> _manager.DeleteNotificationChannel(channelId);

		public INotificationChannel GetNotificationChannel(string channelId)
			=> _manager.GetNotificationChannel(channelId);

		public INotificationChannel[] GetNotificationChannels()
			=> _manager.GetNotificationChannels();

		public void CreateNotificationChannelGroup(INotificationChannelGroup group)
			=> _manager.CreateNotificationChannelGroup(group);

		public void DeleteNotificationChannelGroup(string groupId)
			=> _manager.DeleteNotificationChannelGroup(groupId);

		public INotificationChannelGroup GetNotificationChannelGroup(string groupId)
			=> _manager.GetNotificationChannelGroup(groupId);

		public INotificationChannelGroup[] GetNotificationChannelGroups()
			=> _manager.GetNotificationChannelGroups();

		public INotificationBuilder CreateBuilder(string channelId)
			=> _manager.CreateBuilder(channelId);

		public INotificationCompatBuilder CreateCompatBuilder(string channelId)
			=> _manager.CreateCompatBuilder(channelId);

		public event System.Action<INotification> OnNotificationPosted {
			add    => _manager.OnNotificationPosted += value;
			remove => _manager.OnNotificationPosted -= value;
		}

		public event System.Action<string, int> OnNotificationCancelled {
			add    => _manager.OnNotificationCancelled += value;
			remove => _manager.OnNotificationCancelled -= value;
		}

		public event System.Action<INotification, string> OnNotificationActionInvoked {
			add    => _manager.OnNotificationActionInvoked += value;
			remove => _manager.OnNotificationActionInvoked -= value;
		}

		// ── Helper used by the UI layer ───────────────────────────────────────

		/// <summary>
		/// Triggers a notification action (called by the UI when the user taps an action button).
		/// </summary>
		public void InvokeAction(string tag, int id, string actionId)
			=> _manager.InvokeAction(tag, id, actionId);
	}
}
