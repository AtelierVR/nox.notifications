using Nox.CCK.Players;
using Nox.Controllers;
using UnityEngine;

namespace Nox.Notifications.Runtime
{
    /// <summary>
    /// Behavior modes for how the canvas follows, inspired by
    /// spatial OS notification patterns (Android XR, Meta Quest, Apple visionOS).
    /// </summary>
    public enum FollowMode
    {
        /// <summary>
        /// Smoothly lerps position and rotation toward the camera target.
        /// Always follows — best for notifications that need constant visibility.
        /// </summary>
        SmoothFollow,

        /// <summary>
        /// Smoothly follows while the head is moving, but locks in place
        /// after a period of inactivity. Ideal for interactive panels where
        /// the user needs to click UI elements without the panel drifting.
        /// Resumes following as soon as the head moves again.
        /// </summary>
        SmartFollow,
    }

    /// <summary>
    /// Preset distances matching spatial OS design guidelines.
    /// </summary>
    public enum FollowDistancePreset
    {
        /// <summary>0.75 m — minimum comfortable reading distance in XR.</summary>
        Near,
        /// <summary>1.75 m — standard spatial panel distance (Android XR default).</summary>
        Default,
        /// <summary>3.00 m — ambient / secondary info.</summary>
        Far,
        /// <summary>Use the custom distance value defined on the component.</summary>
        Custom,
    }

    /// <summary>
    /// World-space follow container for notification panels.
    /// Managed by <see cref="Client"/> (the IClientModInitializer entrypoint)
    /// which wires the active controller for orbit tracking.
    ///
    /// Holds a <see cref="NotificationRoot"/> RectTransform where individual
    /// notification UI elements can be instantiated.
    ///
    [AddComponentMenu("Nox/Notifications/Notification Container")]
    [DefaultExecutionOrder(100)] // Run after camera tracking updates
    public class NotificationContainer : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("Camera the panel faces for rotation. Auto-detected if null " +
                 "(Camera.current, then Camera.main fallback).")]
        [SerializeField]
        private Camera m_LookTarget;

        [Tooltip("Transform the panel orbits around (e.g. a controller). " +
                 "If null, the look target position is used as the orbit center. " +
                 "Can be overridden at runtime via OrbitPositionOverride.")]
        [SerializeField]
        private Transform m_OrbitCenter;

        [Tooltip("RectTransform where notification UI elements are instantiated.")]
        public NotificationStack NotificationRoot;

        [Header("Follow Behavior")]
        [Tooltip("How the canvas follows.")]
        [SerializeField]
        private FollowMode m_FollowMode = FollowMode.SmoothFollow;

        [Tooltip("Distance preset from the orbit center.")]
        [SerializeField]
        private FollowDistancePreset m_DistancePreset = FollowDistancePreset.Default;

        [Tooltip("Custom distance in meters (used when Distance Preset = Custom).")]
        [SerializeField]
        [Range(0.3f, 10f)]
        private float m_CustomDistance = 1.75f;

        [Tooltip("How quickly the canvas rotation catches up to the target. " +
                 "Higher = snappier. Used in SmoothFollow and SmartFollow modes.")]
        [SerializeField]
        [Range(0.5f, 20f)]
        private float m_SmoothSpeed = 6f;

        [Header("Orbit Constraints")]
        [Tooltip("Maximum horizontal orbiting speed in degrees per second. " +
                 "Prevents the panel from passing through the orbit center on fast turns " +
                 "by forcing it to orbit around rather than cut through.")]
        [SerializeField]
        [Range(30f, 720f)]
        private float m_MaxHorizontalSpeed = 270f;

        [Tooltip("Fixed height offset from the orbit center in meters. " +
                 "Negative = below eye level (more ergonomic, less obstructive). " +
                 "Keeps the panel from moving too high or low when looking up/down.")]
        [SerializeField]
        [Range(-2f, 2f)]
        private float m_HeightOffset = -0.2f;

        [Header("Constraints")]
        [Tooltip("When enabled, the canvas stays upright (no roll).")]
        [SerializeField]
        private bool m_KeepUpright = true;

        [Tooltip("Maximum horizontal angle deviation from the camera forward. " +
                 "Prevents the panel from swiveling too far when the camera looks " +
                 "at a sharp angle. Set to 0 to lock, 180 to disable.")]
        [SerializeField]
        [Range(0f, 180f)]
        private float m_MaxLookAtAngle = 20f;

        [Tooltip("Minimum distance from orbit center (clamp).")]
        [SerializeField]
        [Range(0.3f, 2f)]
        private float m_MinDistance = 0.5f;

        [Tooltip("Maximum distance from orbit center (clamp).")]
        [SerializeField]
        [Range(2f, 10f)]
        private float m_MaxDistance = 5f;

        [Header("Smart Follow")]
        [Tooltip("Seconds of head near-stillness before the panel locks in place.")]
        [SerializeField]
        [Range(0.1f, 5f)]
        private float m_StandbyDelay = 0.5f;

        [Tooltip("Horizontal head angle in degrees needed to exit standby " +
                 "and resume following. A larger value requires a bigger head turn " +
                 "to unlock the panel.")]
        [SerializeField]
        [Range(10f, 120f)]
        private float m_WakeAngleThreshold = 45f;

        [Header("Controller")]
        [Tooltip("Which controller part to use as the orbit center. Default: RightHand (13).")]
        [SerializeField]
        private PlayerRig m_OrbitPart = PlayerRig.RightHand;

        // ── Runtime references (set by Client.cs) ─────────────────────────

        /// <summary>
        /// Controller used to read orbit center position and forward direction
        /// each frame. Set by <see cref="Client"/> on controller change.
        /// When null, falls back to <see cref="m_OrbitCenter"/> Transform.
        /// </summary>
        public IController OrbitController { get; set; }

        // ── Public properties ──────────────────────────────────────────────

        /// <summary>Current effective distance in meters.</summary>
        public float CurrentDistance => GetDistance();

        /// <summary>The follow mode.</summary>
        public FollowMode Mode
        {
            get => m_FollowMode;
            set => m_FollowMode = value;
        }

        /// <summary>Smooth speed for SmoothFollow mode.</summary>
        public float SmoothSpeed
        {
            get => m_SmoothSpeed;
            set => m_SmoothSpeed = Mathf.Max(0.5f, value);
        }

        /// <summary>The look target camera (can be set at runtime).</summary>
        public Camera LookTarget
        {
            get => m_LookTarget;
            set => m_LookTarget = value;
        }

        // Cached transform
        private Transform m_Transform;

        // Cached last valid horizontal forward (avoids degenerate direction
        // when the camera looks straight up or down).
        private Vector3 m_LastValidForward = Vector3.forward;

        // SmartFollow state
        private Vector3 m_LastCameraForward;
        private Vector3 m_LastOrbitCenter;
        private float   m_StandbyTimer;
        private float   m_StandbyElapsed; // time spent in current standby
        private bool    m_IsInStandby;
        private bool    m_IsRecovering;   // lerping back to orbit band after standby
        private Vector3 m_StandbyPosition;
        private Quaternion m_StandbyRotation;
        private Vector3 m_StandbyForward;  // horizontal forward when locked
        private Vector3 m_StandbyCenter;   // orbit center position when locked

        private void Awake()
        {
            m_Transform = transform;
        }

        private void LateUpdate()
        {
            Camera lookTarget = ResolveLookTarget();
            if (lookTarget == null)
                return;

            Vector3 orbitCenter = GetOrbitCenter(lookTarget);
            float distance = GetDistance();

            switch (m_FollowMode)
            {
                case FollowMode.SmoothFollow:
                {
                    ApplySmoothFollow(lookTarget, orbitCenter, distance);
                    break;
                }

                case FollowMode.SmartFollow:
                {
                    UpdateSmartFollowState(lookTarget, orbitCenter);

                    if (m_IsInStandby)
                    {
                        // Smooth-lock: lerp toward the frozen standby position.
                        float t = Mathf.Clamp01(m_SmoothSpeed * Time.deltaTime);
                        m_Transform.position = Vector3.Lerp(
                            m_Transform.position, m_StandbyPosition, t);
                        m_Transform.rotation = Quaternion.Slerp(
                            m_Transform.rotation, m_StandbyRotation, t);
                    }
                    else if (m_IsRecovering)
                    {
                        // Recovering: lerp from frozen position back toward the
                        // orbit ring. Once inside the soft band, resume normal follow.
                        Vector3 desiredDir = ComputeTargetDirection(lookTarget, orbitCenter);
                        Vector3 targetPos = ComputeOrbitPosition(orbitCenter, desiredDir, distance);

                        float t = Mathf.Clamp01(m_SmoothSpeed * Time.deltaTime);
                        m_Transform.position = Vector3.Lerp(m_Transform.position, targetPos, t);
                        m_Transform.rotation = Quaternion.Slerp(m_Transform.rotation,
                            ComputeTargetRotation(lookTarget, m_Transform.position), t);

                        // Check if we're back inside the soft orbit band.
                        Vector3 toCenter = m_Transform.position - orbitCenter;
                        float hDist = new Vector2(toCenter.x, toCenter.z).magnitude;
                        if (hDist > distance - 0.3f && hDist < distance + 0.3f)
                        {
                            m_IsRecovering = false;
                        }
                    }
                    else
                    {
                        ApplySmoothFollow(lookTarget, orbitCenter, distance);
                        // Keep standby snapshot updated for smooth entry.
                        m_StandbyPosition = m_Transform.position;
                        m_StandbyRotation = m_Transform.rotation;
                    }
                    break;
                }
            }
        }

        private void ApplySmoothFollow(Camera lookTarget, Vector3 orbitCenter, float distance)
        {
            Vector3 desiredDir = ComputeTargetDirection(lookTarget, orbitCenter);
            Vector3 targetPos = ComputeOrbitPosition(orbitCenter, desiredDir, distance);

            float t = Mathf.Clamp01(m_SmoothSpeed * Time.deltaTime);
            m_Transform.position = new Vector3(
                Mathf.Lerp(m_Transform.position.x, targetPos.x, t),
                Mathf.Lerp(m_Transform.position.y, targetPos.y, t),
                Mathf.Lerp(m_Transform.position.z, targetPos.z, t));

            // Soft orbit band
            Vector3 toCenter = m_Transform.position - orbitCenter;
            float hDist = new Vector2(toCenter.x, toCenter.z).magnitude;
            if (hDist > 0.0001f)
            {
                float band = 0.3f;
                float clampedDist = Mathf.Clamp(hDist, distance - band, distance + band);
                Vector2 hDir = new Vector2(toCenter.x, toCenter.z) / hDist;
                m_Transform.position = new Vector3(
                    orbitCenter.x + hDir.x * clampedDist,
                    m_Transform.position.y,
                    orbitCenter.z + hDir.y * clampedDist);
            }

            Quaternion targetRot = ComputeTargetRotation(lookTarget, m_Transform.position);
            m_Transform.rotation = Quaternion.Slerp(m_Transform.rotation, targetRot, t);
        }

        /// <summary>
        /// Manages the standby/lock state for SmartFollow mode.
        /// Enters standby after <see cref="m_StandbyDelay"/> seconds of head stillness.
        /// Exits standby when the horizontal head angle from the locked direction
        /// exceeds <see cref="m_WakeAngleThreshold"/>.
        /// </summary>
        private void UpdateSmartFollowState(Camera lookTarget, Vector3 orbitCenter)
        {
            Vector3 currentForward = lookTarget.transform.forward;

            if (m_IsInStandby)
            {
                m_StandbyElapsed += Time.deltaTime;

                // Exit standby if head turned enough horizontally.
                Vector3 curFlat = currentForward;
                curFlat.y = 0f;
                Vector3 standbyFlat = m_StandbyForward;

                bool wokeByAngle = false;
                if (curFlat.sqrMagnitude > 0.0001f && standbyFlat.sqrMagnitude > 0.0001f)
                {
                    float angle = Vector3.Angle(curFlat, standbyFlat);
                    wokeByAngle = angle >= m_WakeAngleThreshold;
                }

                // Exit standby if the orbit center moved significantly.
                bool wokeByPosition = Vector3.Distance(orbitCenter, m_StandbyCenter) > 1.0f;

                // Minimum standby duration before allowing wake (prevents rapid cycling).
                if ((wokeByAngle || wokeByPosition) && m_StandbyElapsed > 0.3f)
                {
                    m_IsInStandby = false;
                    m_IsRecovering = true;
                    m_StandbyTimer = 0f;
                    m_StandbyElapsed = 0f;
                }
            }
            else
            {
                // Track stillness to enter standby.
                float angleDelta = 0f;
                if (m_LastCameraForward != Vector3.zero)
                    angleDelta = Vector3.Angle(m_LastCameraForward, currentForward);

                float instantVelocity = angleDelta / Mathf.Max(Time.deltaTime, 0.0001f);

                // Also check if the orbit center is moving (walking).
                float posDelta = 0f;
                if (m_LastOrbitCenter != Vector3.zero)
                    posDelta = Vector3.Distance(orbitCenter, m_LastOrbitCenter);
                float instantPosSpeed = posDelta / Mathf.Max(Time.deltaTime, 0.0001f);

                bool headStill = instantVelocity < 5f;
                bool posStable = instantPosSpeed < 0.3f; // 0.3 m/s = barely moving

                if (headStill && posStable)
                {
                    m_StandbyTimer += Time.deltaTime;
                    if (m_StandbyTimer >= m_StandbyDelay)
                    {
                        m_IsInStandby = true;
                        m_IsRecovering = false;
                        m_StandbyElapsed = 0f;

                        m_StandbyForward = currentForward;
                        m_StandbyForward.y = 0f;
                        if (m_StandbyForward.sqrMagnitude < 0.0001f)
                            m_StandbyForward = m_LastValidForward;
                        else
                            m_StandbyForward.Normalize();

                        m_StandbyCenter = orbitCenter;
                    }
                }
                else
                {
                    m_StandbyTimer = 0f;
                }
            }

            m_LastCameraForward = currentForward;
            m_LastOrbitCenter = orbitCenter;
        }

        // ── Camera resolution ──────────────────────────────────────────────

        /// <summary>
        /// Resolves the look target: Camera.current (rendering camera) first,
        /// Camera.main as fallback.
        /// </summary>
        private Camera ResolveLookTarget()
        {
            if (m_LookTarget != null)
                return m_LookTarget;

            Camera cam = Camera.current;
            if (cam != null)
                return cam;

            return Camera.main;
        }

        // ── Orbit center resolution ────────────────────────────────────────

        /// <summary>
        /// Returns the orbit center position. Priority:
        /// 1. <see cref="OrbitPositionOverride"/> (set from code)
        /// 2. <see cref="m_OrbitCenter"/> Transform
        /// 3. Look target (camera) position
        /// </summary>
        private Vector3 GetOrbitCenter(Camera lookTarget)
        {
            // Priority: Controller part > Transform > Camera
            if (OrbitController != null
                && OrbitController.TryGetPart(m_OrbitPart.ToIndex(), out var part))
                return part.GetPosition();

            if (m_OrbitCenter != null)
                return m_OrbitCenter.position;

            return lookTarget.transform.position;
        }

        private float GetDistance()
        {
            float d = m_DistancePreset switch
            {
                FollowDistancePreset.Near => 0.75f,
                FollowDistancePreset.Default => 1.75f,
                FollowDistancePreset.Far => 3.00f,
                FollowDistancePreset.Custom => m_CustomDistance,
                _ => 1.75f,
            };
            return Mathf.Clamp(d, m_MinDistance, m_MaxDistance);
        }

        // ── Direction computation ──────────────────────────────────────────

        /// <summary>
        /// Returns the desired direction from the orbit center to the panel,
        /// projected onto the horizontal plane (Y = 0).
        /// Always driven by the camera forward so the panel stays in the
        /// user's line of sight, even when the controller points elsewhere.
        /// </summary>
        private Vector3 ComputeTargetDirection(Camera lookTarget, Vector3 orbitCenter)
        {
            Vector3 forward = lookTarget.transform.forward;
            forward.y = 0f;

            if (forward.sqrMagnitude > 0.0001f)
            {
                // Valid horizontal direction: cache and return.
                m_LastValidForward = forward.normalized;
                return m_LastValidForward;
            }

            // Camera is looking straight up/down — reuse last valid direction.
            return m_LastValidForward;
        }

        // ── Position / Rotation computation ────────────────────────────────

        /// <summary>
        /// Places the object at the given direction and distance from the orbit center,
        /// with the Y position clamped to orbit center height + offset.
        /// </summary>
        private Vector3 ComputeOrbitPosition(Vector3 orbitCenter, Vector3 direction, float distance)
        {
            Vector3 pos = orbitCenter + direction.normalized * distance;
            pos.y = orbitCenter.y + m_HeightOffset;
            return pos;
        }

        /// <param name="lookTarget">The camera to face.</param>
        /// <param name="objectPosition">The actual (or target) world position of the panel.</param>
        private Quaternion ComputeTargetRotation(Camera lookTarget, Vector3 objectPosition)
        {
            Vector3 dirToLookTarget = objectPosition - lookTarget.transform.position;

            // Always use world-up to avoid degenerate orientations when
            // the camera pitches steeply up or down.
            Quaternion lookRot = Quaternion.LookRotation(dirToLookTarget, Vector3.up);

            if (m_KeepUpright)
            {
                // Flatten to world-up: keep only Y rotation.
                Vector3 euler = lookRot.eulerAngles;
                euler.z = 0f;
                euler.x = 0f;

                // Clamp horizontal look-at angle relative to camera forward.
                float camY = lookTarget.transform.eulerAngles.y;
                float delta = Mathf.DeltaAngle(camY, euler.y);
                delta = Mathf.Clamp(delta, -m_MaxLookAtAngle, m_MaxLookAtAngle);
                euler.y = camY + delta;

                return Quaternion.Euler(euler);
            }

            return lookRot;
        }

#if UNITY_EDITOR
        /// <summary>
        /// Draw the target orbit ring and placement in the editor for easy tuning.
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            Camera lookTarget = ResolveLookTarget();
            if (lookTarget == null)
                lookTarget = Camera.main;
            if (lookTarget == null)
                return;

            Vector3 orbitCenter = GetOrbitCenter(lookTarget);
            float dist = GetDistance();
            Vector3 targetDir = ComputeTargetDirection(lookTarget, orbitCenter);
            Vector3 targetPos = ComputeOrbitPosition(orbitCenter, targetDir, dist);

            // Draw orbit ring (horizontal circle at fixed height around orbit center)
            Gizmos.color = new Color(0f, 1f, 1f, 0.3f);
            DrawGizmoCircle(orbitCenter + Vector3.up * m_HeightOffset, dist, 48);

            // Draw orbit center marker
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(orbitCenter, 0.03f);

            // Draw line from orbit center to target
            Gizmos.color = Color.cyan;
            Vector3 centerFlat = orbitCenter;
            centerFlat.y = targetPos.y;
            Gizmos.DrawLine(centerFlat, targetPos);
            Gizmos.DrawWireSphere(targetPos, 0.05f);

            // Draw the look direction (toward camera)
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(targetPos, ComputeTargetRotation(lookTarget, targetPos) * Vector3.forward * 0.15f);
        }

        private static void DrawGizmoCircle(Vector3 center, float radius, int segments)
        {
            float angleStep = 360f / segments;
            Vector3 prev = center + new Vector3(radius, 0f, 0f);
            for (int i = 1; i <= segments; i++)
            {
                float angle = i * angleStep * Mathf.Deg2Rad;
                Vector3 next = center + new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius);
                Gizmos.DrawLine(prev, next);
                prev = next;
            }
        }
#endif
    }
}
