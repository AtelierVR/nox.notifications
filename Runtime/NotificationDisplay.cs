using Nox.CCK.Language;
using Nox.CCK.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Nox.Notifications.Runtime
{
    /// <summary>
    /// Manages visual elements of a single notification instance.
    /// Installed on <c>notification.prefab</c>.
    /// Containers are SetActive based on content; text and images
    /// are assigned via serialized fields already set up in the prefab.
    /// </summary>
    public class NotificationDisplay : MonoBehaviour
    {
        [Header("Containers (SetActive based on content)")]
        [SerializeField] private GameObject m_TitleContainer;
        [SerializeField] private GameObject m_MessageContainer;
        [SerializeField] private GameObject m_ImageContainer;
        // [SerializeField] private GameObject m_IconContainer;

        [Header("Text")]
        [SerializeField] private TextLanguage m_TitleText;
        [SerializeField] private TextLanguage m_MessageText;

        [Header("Images")]
        [SerializeField] private Image m_Image;
        // [SerializeField] private Image m_Icon;

        /// <summary>Duration in seconds before auto-dismiss. 0 = until cancelled.</summary>
        public float Duration { get; set; } = 5f;

        /// <summary>The notification ID for cancellation tracking.</summary>
        public int NotificationId { get; set; }

        private float _elapsed;

        // ── Initialization ─────────────────────────────────────────────────

        /// <summary>
        /// Populates the display with notification data.
        /// </summary>
        public void SetNotification(INotification notification)
        {
            NotificationId = notification.Id;

            // Title
            bool hasTitle = !string.IsNullOrEmpty(notification.Title);
            m_TitleContainer.SetActive(hasTitle);
            m_TitleText.UpdateText("value", new[] { notification.Title });

            // Message (content text)
            bool hasMessage = !string.IsNullOrEmpty(notification.ContentText);
            m_MessageContainer.SetActive(hasMessage);
            m_MessageText.UpdateText("value", new[] { notification.ContentText });

            // Icon (small icon)
            // bool hasIcon = !string.IsNullOrEmpty(notification.SmallIcon);
            // if (m_IconContainer) m_IconContainer.SetActive(hasIcon);
            // // Icon loading would be handled by an image loader (TBD)

            // Image (large icon / picture)
            bool hasImage = !string.IsNullOrEmpty(notification.LargeIcon);
            m_ImageContainer.SetActive(hasImage);
            // Image loading would be handled by an image loader (TBD)

            // Duration from compat notification
            if (notification is INotificationCompat compat && compat.TimeoutAfter.HasValue)
                Duration = (float)compat.TimeoutAfter.Value.TotalSeconds;
        }

        // ── Lifecycle ─────────────────────────────────────────────────────

        private void Update()
        {
            if (Duration <= 0f) return;

            _elapsed += Time.deltaTime;
            if (_elapsed >= Duration)
            {
                if (NotificationId > 0)
                    Main.Instance?.Cancel(NotificationId);
                else
                    gameObject.Destroy();
            }
        }
    }
}
