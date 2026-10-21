using UnityEngine;

/// <summary>
/// Smoothly rotates a neck bone to look toward a target (e.g. the player),
/// clamped to anatomically plausible limits.
/// </summary>
public class PigNeckLook : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The neck bone to rotate.")]
    public Transform neck;

    [Tooltip("The target to look at.")]
    public Transform target;

    [Header("Rotation Limits (degrees)")]
    [Range(0f, 180f)] public float maxLeftRight = 60f;
    [Range(0f, 90f)] public float maxUpDown = 40f;

    [Header("Smoothing")]
    [Range(1f, 30f)] public float smoothSpeed = 10f;

    [Header("Idle Behaviour")]
    [Tooltip("Seconds with no valid target before returning to rest pose.")]
    public float returnToRestDelay = 2f;

    private Quaternion _initialLocalRotation;
    private float _timeSinceValidTarget;

    // -------------------------------------------------------------------------

    void Start()
    {
        if (neck == null)
        {
            Debug.LogError($"[PigNeckLook] 'neck' is not assigned on {gameObject.name}. Disabling.", this);
            enabled = false;
            return;
        }

        _initialLocalRotation = neck.localRotation;
    }

    void LateUpdate()
    {
        if (neck == null) return;          // safety — neck could be destroyed at runtime

        bool hasValidTarget = target != null;
        Quaternion desiredLocalRotation;

        if (hasValidTarget)
        {
            _timeSinceValidTarget = 0f;
            desiredLocalRotation = ComputeClampedLocalRotation();
        }
        else
        {
            // Count up and blend back to rest after the delay expires
            _timeSinceValidTarget += Time.deltaTime;
            desiredLocalRotation = _timeSinceValidTarget >= returnToRestDelay
                ? _initialLocalRotation
                : neck.localRotation;   // hold current pose during grace period
        }

        neck.localRotation = Quaternion.Slerp(
            neck.localRotation,
            desiredLocalRotation,
            Time.deltaTime * smoothSpeed
        );
    }

    // -------------------------------------------------------------------------

    Quaternion ComputeClampedLocalRotation()
    {
        Transform parent = neck.parent;
        Quaternion parentRotation = parent != null ? parent.rotation : Quaternion.identity;

        Vector3 direction = target.position - neck.position;

        // Guard against target sitting exactly on the neck pivot
        if (direction.sqrMagnitude < 0.0001f)
            return neck.localRotation;

        Quaternion worldTarget = Quaternion.LookRotation(direction);
        Quaternion localTarget = Quaternion.Inverse(parentRotation) * worldTarget;

        Vector3 euler = localTarget.eulerAngles;
        euler.x = ClampAngle(euler.x, -maxUpDown, maxUpDown);
        euler.y = ClampAngle(euler.y, -maxLeftRight, maxLeftRight);
        euler.z = 0f;                                 // no roll

        return Quaternion.Euler(euler);
    }

    /// <summary>
    /// Remaps a Unity euler angle from [0, 360] to [-180, 180] then clamps it.
    /// </summary>
    static float ClampAngle(float angle, float min, float max)
    {
        // Unity returns eulerAngles in [0, 360]; remap to [-180, 180]
        if (angle > 180f) angle -= 360f;
        return Mathf.Clamp(angle, min, max);
    }
}