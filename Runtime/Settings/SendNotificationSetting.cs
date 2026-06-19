using System;
using Nox.CCK.Settings;
using Nox.UI;
using UnityEngine;
using Logger = Nox.CCK.Utils.Logger;

namespace Nox.Notifications.Runtime.Settings
{
    /// <summary>
    /// Debug button that sends a test notification for development and QA.
    /// Path: debug > send_notification
    /// </summary>
    internal sealed class SendNotificationSetting : ButtonHandler
    {
        public override string[] GetPath()
            => new[] { "debug", "send_notification" };

        public override int GetOrder() => 100;

        public SendNotificationSetting()
        {
            SetLabel("settings.debug.send_notification");
            SetButtonText("settings.debug.send_notification.send");
        }

        protected override GameObject GetPrefab()
            => Main.CoreAPI.AssetAPI.GetAsset<GameObject>("settings:prefabs/button.prefab");

        protected override void OnClick(IMenu menu)
        {
            var builder = Main.Instance.CreateBuilder("default");
            builder.SetTitle("Test Notification")
                   .SetContentText($"This is a test notification sent at {DateTime.Now:T}.")
                   .SetSmallIcon("ic_notification")
                   .SetAutoCancel(true)
                   .SetPriority(NotificationPriority.Default);

            var notification = builder.Build();
            var id = Main.Instance.Notify(notification);

            Logger.Log($"Test notification #{id} sent.", tag: "Notifications");
        }
    }
}
