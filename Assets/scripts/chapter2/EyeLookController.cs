
using UnityEngine;
[DisallowMultipleComponent]
[DefaultExecutionOrder(500)]
public class EyeLookController : MonoBehaviour
{
    public enum Axis
    {
        [InspectorName("+X")] PlusX,
        [InspectorName("-X")] MinusX,
        [InspectorName("+Y")] PlusY,
        [InspectorName("-Y")] MinusY,
        [InspectorName("+Z")] PlusZ,
        [InspectorName("-Z")] MinusZ
    }

    public enum LimitMode
    {
        None,
        HardClamp,
        SoftClamp
    }
    public enum OutOfRangeBehavior
    {
        ClampToLimit,
        ReturnToCenter,
        Freeze
    }
    [Header("Target")]
    [Tooltip("Transform to look at. If empty in Awake and 'Default To Main Camera' is on, Camera.main is cached once.")]
    [SerializeField] private Transform target;

    [Tooltip("When no target is assigned in Awake, use Camera.main (looked up once, never again). SetTarget(null) always means 'no target'.")]
    [SerializeField] private bool defaultToMainCamera = true;

    [Tooltip("Aim point offset in the TARGET's local axes (rotates with the target, is not scaled). Example: (0, 0.1, 0) to aim at the head instead of the pivot.")]
    [SerializeField] private Vector3 targetOffset = Vector3.zero;

    [Header("Axes (eye-local)")]
    [Tooltip("Local axis the pupil points along at rest. Change this if your model's pupil faces another direction.")]
    [SerializeField] private Axis forwardAxis = Axis.PlusX;

    [Tooltip("Local axis for yaw (left/right). Must differ from Forward Axis and Pitch Axis (different X/Y/Z axis, sign is free).")]
    [SerializeField] private Axis yawAxis = Axis.PlusY;

    [Tooltip("Local axis for pitch (up/down). Must differ from Forward Axis and Yaw Axis. Flip the sign if the eye looks up when it should look down.")]
    [SerializeField] private Axis pitchAxis = Axis.PlusZ;

    [Header("Limits (degrees, relative to rest pose)")]
    [Tooltip("None: no limits. HardClamp: stop dead at the limit. SoftClamp: ease out over the last 'Soft Zone' degrees before the limit.")]
    [SerializeField] private LimitMode limitMode = LimitMode.SoftClamp;

    [Tooltip("SoftClamp only: size of the easing zone in degrees. Automatically reduced to half the range if the range is smaller.")]
    [SerializeField, Range(0f, 90f)] private float softZone = 10f;

    [Tooltip("When on, a single 'Yaw Limit' is used for both sides and Yaw Min / Yaw Max are ignored.")]
    [SerializeField] private bool symmetricYaw = false;

    [Tooltip("Symmetric yaw limit in degrees (used only while 'Symmetric Yaw' is on).")]
    [SerializeField, Range(0f, 90f)] private float yawLimit = 35f;

    [Tooltip("Minimum yaw in degrees (negative = toward the -yaw side). Ignored while 'Symmetric Yaw' is on.")]
    [SerializeField, Range(-90f, 90f)] private float yawMin = -35f;

    [Tooltip("Maximum yaw in degrees. Ignored while 'Symmetric Yaw' is on.")]
    [SerializeField, Range(-90f, 90f)] private float yawMax = 35f;

    [Tooltip("Minimum pitch in degrees.")]
    [SerializeField, Range(-90f, 90f)] private float pitchMin = -30f;

    [Tooltip("Maximum pitch in degrees.")]
    [SerializeField, Range(-90f, 90f)] private float pitchMax = 25f;

    [Header("Out Of Range")]
    [Tooltip("ClampToLimit: point at the nearest allowed angle. ReturnToCenter: look straight ahead. Freeze: keep the last angle. (With Limit Mode None there is nothing to clamp to.)")]
    [SerializeField] private OutOfRangeBehavior outOfRangeBehavior = OutOfRangeBehavior.ClampToLimit;

    [Tooltip("If the target is further than this many degrees from the rest forward, 'Out Of Range Behavior' is applied. 180 = disabled.")]
    [SerializeField, Range(0f, 180f)] private float ignoreBehindAngle = 120f;

    [Header("Motion")]
    [Tooltip("Smoothing time per axis in seconds (Mathf.SmoothDampAngle). 0 = no smoothing.")]
    [SerializeField, Min(0f)] private float smoothTime = 0.08f;

    [Tooltip("Maximum angular speed per axis in degrees per second. 0 = unlimited.")]
    [SerializeField, Min(0f)] private float maxAngularSpeed = 720f;

    [Tooltip("0 = keep the animated pose, 1 = full look-at. Fade this to blend the effect in or out.")]
    [SerializeField, Range(0f, 1f)] private float weight = 1f;

    [Header("Micro-saccades")]
    [Tooltip("Adds a small random offset to the desired angles every few moments. Only active while a target is tracked.")]
    [SerializeField] private bool enableSaccades = false;

    [Tooltip("Random time between saccades in seconds (x = min, y = max).")]
    [SerializeField] private Vector2 saccadeInterval = new Vector2(0.4f, 1.6f);

    [Tooltip("Maximum saccade offset in degrees. The result is still clamped by the limits.")]
    [SerializeField, Range(0f, 5f)] private float saccadeAmplitude = 0.8f;

    [Header("Gizmos")]
    [Tooltip("Radius of the editor gizmos in metres. Set it to roughly the eyeball radius.")]
    [SerializeField, Min(0.001f)] private float gizmoRadius = 0.1f;

    private const float MinDirectionSqr = 1e-8f;       
    private const float PoleEpsilon = 1e-4f;           
    private const float BehindHysteresisAngle = 175f;   
    private const float WeightEpsilon = 1e-3f;
    private const float PoseTolerance = 1e-5f;         


    private Transform _tr;
    private Quaternion _restLocal = Quaternion.identity;
    private bool _initialized;
    private bool _targetAssignedByCode;
    private bool _holdCenter;
    private bool _axesValid = true;
    private Vector3 _fwd = Vector3.right;
    private Vector3 _yawVec = Vector3.up;
    private Vector3 _pitchVec = Vector3.forward;
    private Vector3 _side = Vector3.back;   
    private float _pitchSign = -1f;         
    private float _currentYaw;
    private float _currentPitch;
    private float _yawVel;
    private float _pitchVel;
    private float _desiredYaw;
    private float _desiredPitch;
    private float _lastRawYaw;
    private bool _isAtLimit;

    private float _saccadeYaw;
    private float _saccadePitch;
    private float _saccadeTimer;

    private bool _hasWritten;
    private Quaternion _lastWritten = Quaternion.identity;

    public float CurrentYaw => _currentYaw;
    public float CurrentPitch => _currentPitch;
    public bool IsAtLimit => _isAtLimit;
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        _targetAssignedByCode = true;
        _holdCenter = false;
    }
    public void SetLimits(float yawMin, float yawMax, float pitchMin, float pitchMax)
    {
        symmetricYaw = false;
        this.yawMin = Mathf.Clamp(Mathf.Min(yawMin, yawMax), -90f, 90f);
        this.yawMax = Mathf.Clamp(Mathf.Max(yawMin, yawMax), -90f, 90f);
        this.pitchMin = Mathf.Clamp(Mathf.Min(pitchMin, pitchMax), -90f, 90f);
        this.pitchMax = Mathf.Clamp(Mathf.Max(pitchMin, pitchMax), -90f, 90f);
    }
    public void SetWeight(float newWeight)
    {
        weight = float.IsNaN(newWeight) ? 0f : Mathf.Clamp01(newWeight);
    }
    public void ResetToCenter(bool instant)
    {
        _holdCenter = true;
        _isAtLimit = false;
        _desiredYaw = 0f;
        _desiredPitch = 0f;

        if (instant)
        {
            _currentYaw = 0f;
            _currentPitch = 0f;
            _yawVel = 0f;
            _pitchVel = 0f;
        }
    }
    private void Awake()
    {
        _tr = transform;
        _restLocal = _tr.localRotation;
        CacheAxes(true);

        if (!_targetAssignedByCode && target == null && defaultToMainCamera)
        {
            Camera cam = Camera.main; 
            if (cam != null)
            {
                target = cam.transform;
            }
        }

        _initialized = true;
    }

    private void OnEnable()
    {
        _currentYaw = 0f;
        _currentPitch = 0f;
        _yawVel = 0f;
        _pitchVel = 0f;
        _desiredYaw = 0f;
        _desiredPitch = 0f;
        _lastRawYaw = 0f;
        _saccadeYaw = 0f;
        _saccadePitch = 0f;
        _saccadeTimer = 0f;
        _isAtLimit = false;
        _hasWritten = false;
    }

    private void OnDisable()
    {
        if (_initialized && _hasWritten && _tr != null && SamePose(_tr.localRotation, _lastWritten))
        {
            _tr.localRotation = _restLocal;
        }

        _hasWritten = false;
    }

    private void LateUpdate()
    {
        float dt = Time.deltaTime;

        UpdateSaccades(dt);
        SolveDesiredAngles();
        SmoothAngles(dt);
        ApplyPose();
    }
    private void UpdateSaccades(float dt)
    {
        if (!enableSaccades)
        {
            _saccadeYaw = 0f;
            _saccadePitch = 0f;
            _saccadeTimer = 0f;
            return;
        }

        _saccadeTimer -= dt;
        if (_saccadeTimer > 0f)
        {
            return;
        }

        Vector2 offset = Random.insideUnitCircle * saccadeAmplitude;   
        _saccadeYaw = offset.x;
        _saccadePitch = offset.y;
        _saccadeTimer = Random.Range(saccadeInterval.x, saccadeInterval.y);
    }

    private void SolveDesiredAngles()
    {
        GetYawRange(out float yMin, out float yMax);
        float pMin = pitchMin;
        float pMax = pitchMax;
        bool tracking = false;
        float rawYaw = 0f;
        float rawPitch = 0f;
        float angleFromForward = 0f;

        if (!_holdCenter && target != null)
        {
            Transform parent = _tr.parent;
            Quaternion restWorld = parent != null ? parent.rotation * _restLocal : _restLocal;
            tracking = TryComputeAngles(_tr.position, restWorld, GetTargetPoint(target), _lastRawYaw,
                                        out rawYaw, out rawPitch, out angleFromForward);
        }

        float baseYaw = 0f;
        float basePitch = 0f;
        bool outOfRange = false;
        bool freeze = false;

        if (tracking)
        {
            if (limitMode != LimitMode.None && Mathf.Abs(rawYaw) > BehindHysteresisAngle && Mathf.Abs(_currentYaw) > 0.01f)
            {
                rawYaw = Mathf.Abs(rawYaw) * Mathf.Sign(_currentYaw);
            }

            _lastRawYaw = rawYaw;
            baseYaw = rawYaw;
            basePitch = rawPitch;
            outOfRange = IsOutOfRange(rawYaw, rawPitch, angleFromForward, yMin, yMax, pMin, pMax);
            if (outOfRange)
            {
                switch (outOfRangeBehavior)
                {
                    case OutOfRangeBehavior.ReturnToCenter:
                        baseYaw = 0f;
                        basePitch = 0f;
                        break;
                    case OutOfRangeBehavior.Freeze:
                        freeze = true;
                        break;
                }
            }
        }

        _isAtLimit = outOfRange;

        if (freeze)
        {
            return;     
        }

        float saccadeYaw = tracking ? _saccadeYaw : 0f;
        float saccadePitch = tracking ? _saccadePitch : 0f;

        _desiredYaw = ApplyLimit(baseYaw + saccadeYaw, yMin, yMax);
        _desiredPitch = ApplyLimit(basePitch + saccadePitch, pMin, pMax);
    }

    private bool TryComputeAngles(Vector3 pivot, Quaternion restWorld, Vector3 point, float fallbackYaw,
                                  out float yaw, out float pitch, out float angleFromForward)
    {
        yaw = 0f;
        pitch = 0f;
        angleFromForward = 0f;

        Vector3 toTarget = point - pivot;
        float sqr = toTarget.sqrMagnitude;
        if (!(sqr >= MinDirectionSqr) || float.IsInfinity(sqr))     
        {
            return false;
        }

        Vector3 dir = Quaternion.Inverse(restWorld) * (toTarget / Mathf.Sqrt(sqr));

        float dF = Vector3.Dot(dir, _fwd);
        float dS = Vector3.Dot(dir, _side);
        float dY = Vector3.Dot(dir, _yawVec);
        float planar = Mathf.Sqrt(dF * dF + dS * dS);

        pitch = Mathf.Atan2(-_pitchSign * dY, planar) * Mathf.Rad2Deg;
        yaw = planar > PoleEpsilon ? Mathf.Atan2(dS, dF) * Mathf.Rad2Deg : fallbackYaw;
        angleFromForward = Mathf.Atan2(Mathf.Sqrt(dS * dS + dY * dY), dF) * Mathf.Rad2Deg;
        return true;
    }

    private bool IsOutOfRange(float yaw, float pitch, float angleFromForward,
                              float yMin, float yMax, float pMin, float pMax)
    {
        if (ignoreBehindAngle < 180f && angleFromForward > ignoreBehindAngle)
        {
            return true;
        }

        if (limitMode == LimitMode.None)
        {
            return false;
        }

        return yaw < yMin || yaw > yMax || pitch < pMin || pitch > pMax;
    }

    private float ApplyLimit(float angle, float min, float max)
    {
        switch (limitMode)
        {
            case LimitMode.HardClamp:
                return Mathf.Clamp(angle, min, max);
            case LimitMode.SoftClamp:
                return SoftClamp(angle, min, max, softZone);
            default:
                return angle;
        }
    }
    private static float SoftClamp(float angle, float min, float max, float zone)
    {
        zone = Mathf.Min(zone, (max - min) * 0.5f);
        if (zone <= 1e-4f)
        {
            return Mathf.Clamp(angle, min, max);
        }

        float hi = max - zone;
        if (angle > hi)
        {
            float t = Mathf.Clamp01((angle - hi) / (2f * zone));
            float inv = 1f - t;
            return hi + zone * (1f - inv * inv);
        }

        float lo = min + zone;
        if (angle < lo)
        {
            float t = Mathf.Clamp01((lo - angle) / (2f * zone));
            float inv = 1f - t;
            return lo - zone * (1f - inv * inv);
        }

        return angle;
    }

    private void SmoothAngles(float dt)
    {
        float maxSpeed = maxAngularSpeed > 0f ? maxAngularSpeed : Mathf.Infinity;

        _currentYaw = Mathf.SmoothDampAngle(_currentYaw, _desiredYaw, ref _yawVel, smoothTime, maxSpeed, dt);
        _currentPitch = Mathf.SmoothDampAngle(_currentPitch, _desiredPitch, ref _pitchVel, smoothTime, maxSpeed, dt);
        _currentYaw = Mathf.DeltaAngle(0f, _currentYaw);
        _currentPitch = Mathf.DeltaAngle(0f, _currentPitch);

        if (limitMode != LimitMode.None)
        {
            GetYawRange(out float yMin, out float yMax);

            float clampedYaw = Mathf.Clamp(_currentYaw, yMin, yMax);
            if (clampedYaw != _currentYaw)
            {
                _currentYaw = clampedYaw;
                _yawVel = 0f;
            }

            float clampedPitch = Mathf.Clamp(_currentPitch, pitchMin, pitchMax);
            if (clampedPitch != _currentPitch)
            {
                _currentPitch = clampedPitch;
                _pitchVel = 0f;
            }
        }
    }

    private void ApplyPose()
    {
        Quaternion current = _tr.localRotation;
        bool external = !_hasWritten || !SamePose(current, _lastWritten);

        Quaternion result;
        if (weight <= WeightEpsilon)
        {
            if (external)
            {
                _hasWritten = false;    
                return;
            }

            result = _restLocal;      
        }
        else
        {
            Quaternion look = _restLocal
                              * Quaternion.AngleAxis(_currentYaw, _yawVec)
                              * Quaternion.AngleAxis(_currentPitch, _pitchVec);

            result = weight >= 1f - WeightEpsilon
                ? look
                : Quaternion.Slerp(external ? current : _restLocal, look, weight);
        }

        if (!SamePose(current, result))
        {
            _tr.localRotation = result;
        }

        _lastWritten = result;
        _hasWritten = true;
    }
    private Vector3 GetTargetPoint(Transform t)
    {
        return t.position + t.rotation * targetOffset;
    }

    private void GetYawRange(out float min, out float max)
    {
        if (symmetricYaw)
        {
            min = -yawLimit;
            max = yawLimit;
        }
        else
        {
            min = yawMin;
            max = yawMax;
        }
    }

    private static bool SamePose(Quaternion a, Quaternion b)
    {
        return Mathf.Abs(a.x - b.x) <= PoseTolerance
               && Mathf.Abs(a.y - b.y) <= PoseTolerance
               && Mathf.Abs(a.z - b.z) <= PoseTolerance
               && Mathf.Abs(a.w - b.w) <= PoseTolerance;
    }

    private static int AxisIndex(Axis axis)
    {
        return (int)axis / 2;   
    }

    private static Vector3 AxisToVector(Axis axis)
    {
        switch (axis)
        {
            case Axis.PlusX: return Vector3.right;
            case Axis.MinusX: return Vector3.left;
            case Axis.PlusY: return Vector3.up;
            case Axis.MinusY: return Vector3.down;
            case Axis.PlusZ: return Vector3.forward;
            default: return Vector3.back;
        }
    }

    private void CacheAxes(bool warn)
    {
        int f = AxisIndex(forwardAxis);
        int y = AxisIndex(yawAxis);
        int p = AxisIndex(pitchAxis);
        _axesValid = f != y && f != p && y != p;

        if (_axesValid)
        {
            _fwd = AxisToVector(forwardAxis);
            _yawVec = AxisToVector(yawAxis);
            _pitchVec = AxisToVector(pitchAxis);
        }
        else
        {
            _fwd = Vector3.right;
            _yawVec = Vector3.up;
            _pitchVec = Vector3.forward;

            if (warn)
            {
                Debug.LogWarning("EyeLookController: Forward, Yaw and Pitch axes must use three different local axes (X, Y, Z). Falling back to +X / Y / Z.", this);
            }
        }

        _side = Vector3.Cross(_yawVec, _fwd);
        _pitchSign = Vector3.Dot(_pitchVec, _side) >= 0f ? 1f : -1f;
    }

    private void OnValidate()
    {
        yawLimit = Mathf.Clamp(yawLimit, 0f, 90f);
        yawMin = Mathf.Clamp(yawMin, -90f, 90f);
        yawMax = Mathf.Clamp(yawMax, -90f, 90f);
        pitchMin = Mathf.Clamp(pitchMin, -90f, 90f);
        pitchMax = Mathf.Clamp(pitchMax, -90f, 90f);
        yawMin = Mathf.Min(yawMin, yawMax);
        pitchMin = Mathf.Min(pitchMin, pitchMax);

        softZone = Mathf.Clamp(softZone, 0f, 90f);
        ignoreBehindAngle = Mathf.Clamp(ignoreBehindAngle, 0f, 180f);
        smoothTime = Mathf.Max(0f, smoothTime);
        maxAngularSpeed = Mathf.Max(0f, maxAngularSpeed);
        weight = Mathf.Clamp01(weight);
        saccadeAmplitude = Mathf.Clamp(saccadeAmplitude, 0f, 5f);
        saccadeInterval.x = Mathf.Max(0.01f, saccadeInterval.x);
        saccadeInterval.y = Mathf.Max(saccadeInterval.x, saccadeInterval.y);
        gizmoRadius = Mathf.Max(0.001f, gizmoRadius);

        CacheAxes(true);
    }

    [ContextMenu("Preset: Human")]
    private void PresetHuman()
    {
        ApplyPreset("Eye Look Preset: Human", LimitMode.SoftClamp, -35f, 35f, -30f, 25f, 10f);
    }

    [ContextMenu("Preset: Wide (creepy)")]
    private void PresetWide()
    {
        ApplyPreset("Eye Look Preset: Wide", LimitMode.SoftClamp, -80f, 80f, -70f, 60f, 5f);
    }

    [ContextMenu("Preset: Narrow")]
    private void PresetNarrow()
    {
        ApplyPreset("Eye Look Preset: Narrow", LimitMode.SoftClamp, -15f, 15f, -10f, 10f, 6f);
    }

    private void ApplyPreset(string undoLabel, LimitMode mode, float yMin, float yMax, float pMin, float pMax, float zone)
    {
#if UNITY_EDITOR
        UnityEditor.Undo.RecordObject(this, undoLabel);
#endif
        limitMode = mode;
        symmetricYaw = false;
        yawMin = yMin;
        yawMax = yMax;
        pitchMin = pMin;
        pitchMax = pMax;
        softZone = zone;
        OnValidate();
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }

    private Vector3 LocalDirection(float yaw, float pitch)
    {
        return Quaternion.AngleAxis(yaw, _yawVec) * (Quaternion.AngleAxis(pitch, _pitchVec) * _fwd);
    }

    private void DrawCurve(bool varyYaw, float from, float to, float fixedAngle, float radius)
    {
        int steps = Mathf.Clamp(Mathf.CeilToInt(Mathf.Abs(to - from) / 5f), 1, 64);
        Vector3 previous = Vector3.zero;

        for (int i = 0; i <= steps; i++)
        {
            float a = Mathf.Lerp(from, to, i / (float)steps);
            Vector3 p = (varyYaw ? LocalDirection(a, fixedAngle) : LocalDirection(fixedAngle, a)) * radius;
            if (i > 0)
            {
                Gizmos.DrawLine(previous, p);
            }

            previous = p;
        }
    }

    private void DrawRangeFrame(float y0, float y1, float p0, float p1, float radius)
    {
        DrawCurve(true, y0, y1, p0, radius);
        DrawCurve(true, y0, y1, p1, radius);
        DrawCurve(false, p0, p1, y0, radius);
        DrawCurve(false, p0, p1, y1, radius);
    }

    private void OnDrawGizmosSelected()
    {
        CacheAxes(false);

        Transform tr = transform;
        Quaternion restLocal = Application.isPlaying && _initialized ? _restLocal : tr.localRotation;
        Transform parent = tr.parent;
        Quaternion restWorld = parent != null ? parent.rotation * restLocal : restLocal;
        Vector3 pivot = tr.position;
        float r = Mathf.Max(0.001f, gizmoRadius);

        GetYawRange(out float yMin, out float yMax);
        float pMin = pitchMin;
        float pMax = pitchMax;
        Gizmos.matrix = Matrix4x4.TRS(pivot, restWorld, Vector3.one);

        Gizmos.color = new Color(1f, 1f, 1f, 0.15f);
        Gizmos.DrawWireSphere(Vector3.zero, r);

        if (limitMode == LimitMode.None)
        {
            Gizmos.color = new Color(1f, 0.92f, 0.016f, 0.6f);
            Gizmos.DrawLine(Vector3.zero, _fwd * r);
        }
        else
        {
            Gizmos.color = Color.yellow;
            DrawRangeFrame(yMin, yMax, pMin, pMax, r);
            Gizmos.DrawLine(Vector3.zero, LocalDirection(yMin, pMin) * r);
            Gizmos.DrawLine(Vector3.zero, LocalDirection(yMax, pMin) * r);
            Gizmos.DrawLine(Vector3.zero, LocalDirection(yMax, pMax) * r);
            Gizmos.DrawLine(Vector3.zero, LocalDirection(yMin, pMax) * r);

            Gizmos.color = new Color(1f, 0.92f, 0.016f, 0.4f);
            DrawCurve(true, yMin, yMax, Mathf.Clamp(0f, pMin, pMax), r);
            DrawCurve(false, pMin, pMax, Mathf.Clamp(0f, yMin, yMax), r);

            if (limitMode == LimitMode.SoftClamp && softZone > 0f)
            {
                float zoneYaw = Mathf.Min(softZone, (yMax - yMin) * 0.5f);
                float zonePitch = Mathf.Min(softZone, (pMax - pMin) * 0.5f);
                Gizmos.color = new Color(1f, 0.92f, 0.016f, 0.25f);
                DrawRangeFrame(yMin + zoneYaw, yMax - zoneYaw, pMin + zonePitch, pMax - zonePitch, r);
            }
        }
        Gizmos.matrix = Matrix4x4.identity;
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(pivot, pivot + tr.TransformDirection(_fwd) * (r * 1.3f));

        if (target != null)
        {
            Vector3 point = GetTargetPoint(target);
            if (TryComputeAngles(pivot, restWorld, point, 0f, out float yaw, out float pitch, out float angle))
            {
                bool outside = IsOutOfRange(yaw, pitch, angle, yMin, yMax, pMin, pMax);
                Vector3 end = pivot + (point - pivot).normalized * (r * 1.6f);

                Gizmos.color = outside ? Color.red : Color.green;
                Gizmos.DrawLine(pivot, end);
                Gizmos.DrawWireSphere(end, r * 0.06f);
            }
        }
    }
}
