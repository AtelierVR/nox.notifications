using System;
using Cysharp.Threading.Tasks;
using Nox.CCK.Mods.Cores;
using Nox.CCK.Mods.Events;
using Nox.CCK.Mods.Initializers;
using Nox.CCK.Utils;
using Nox.Controllers;
using UnityEngine;
using Logger = Nox.CCK.Utils.Logger;

namespace Nox.Notifications.Runtime
{
    /// <summary>
    /// <para>
    /// Client entrypoint for <c>nox.notifications</c>.
    /// Implements <see cref="IClientModInitializer"/> to manage the
    /// <see cref="NotificationContainer"/> lifecycle.
    /// </para>
    /// <para>
    /// On initialization, instantiates the <c>follower.prefab</c> notification
    /// panel (with a child Canvas already set up), wires it to the active
    /// controller for orbit tracking, and places it in DontDestroyOnLoad.
    /// </para>
    /// <para>
    /// Controller changes are detected via <c>controller_changed</c> event
    /// on <see cref="IClientModCoreAPI.EventAPI"/>.
    /// </para>
    /// </summary>
    public class Client : IClientModInitializer
    {
        public static Client Instance { get; private set; }
        static internal IClientModCoreAPI CoreAPI { get; private set; }

        /// <summary>
        /// The active notification follower panel (DontDestroyOnLoad).
        /// </summary>
        [NoxPublic(NoxAccess.Read)]
        public NotificationContainer Container { get; private set; }

        // ── Getters (resolve lazily from CoreAPI) ──────────────────────────

        private static IControllerAPI ControllerAPI
            => CoreAPI?.ModAPI?.GetMod("controllers")
                ?.GetInstance<IControllerAPI>();

        private static IController CurrentController
            => ControllerAPI?.Current;

        // ── Event subscriptions ────────────────────────────────────────────

        private EventSubscription[] _events = Array.Empty<EventSubscription>();

        // ── IClientModInitializer ─────────────────────────────────────────

        public async UniTask OnInitializeClientAsync(IClientModCoreAPI api)
        {
            CoreAPI  = api;
            Instance = this;

            // Load and instantiate the notification follower panel.
            await CreateFollowerAsync();

            // Subscribe to controller changes via EventAPI.
            _events = new[] {
                api.EventAPI.Subscribe("controller_changed", OnControllerChanged)
            };

            // Apply current controller immediately if already active.
            if (CurrentController != null)
                OnControllerChanged(CurrentController);

            Logger.Log("Client initialized.", tag: "Notifications");
        }

        public void OnPreDisposeClient()
        {
            // Unsubscribe all events.
            foreach (var ev in _events)
                CoreAPI?.EventAPI?.Unsubscribe(ev);
            _events = Array.Empty<EventSubscription>();

            if (Container != null)
            {
                Container.gameObject.Destroy();
                Container = null;
            }

            Instance = null;
            CoreAPI  = null;
            Logger.Log("Client disposed.", tag: "Notifications");
        }

        // ── Controller change handling ─────────────────────────────────────

        /// <summary>
        /// Called by EventAPI when <c>controller_changed</c> is emitted.
        /// </summary>
        private static void OnControllerChanged(EventData context)
        {
            if (!context.TryGet(0, out IController controller))
                return;

            if (Instance == null)
                return;

            Instance.OnControllerChanged(controller);
        }

        private void OnControllerChanged(IController controller)
        {
            if (controller == null || Container == null)
                return;
            // Wire the controller to the container so it reads orbit position
            // and forward direction directly in its own LateUpdate.
            Container.OrbitController = controller;
            // Set the camera as the look target so the panel faces the headset.
            Container.LookTarget = controller.GetCamera();
        }

        // ── Follower creation ──────────────────────────────────────────────

        /// <summary>
        /// Asynchronously loads and instantiates the notification follower
        /// from <c>Assets/notifications/follower.prefab</c>.
        /// The prefab has a <see cref="NotificationContainer"/> component and a child Canvas.
        /// </summary>
        private async UniTask CreateFollowerAsync()
        {
            var prefab = await CoreAPI.AssetAPI.GetAssetAsync<GameObject>("follower.prefab");

            Container = await prefab.InstantiateAsync<NotificationContainer>();
            Container.name = "[Notifications] Container";
            Container.NotificationRoot.Initialize();
        }
    }
}
