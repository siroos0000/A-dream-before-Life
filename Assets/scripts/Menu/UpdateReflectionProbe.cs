using UnityEngine;

public class UpdateReflectionProbe : MonoBehaviour
{
    [SerializeField] private ReflectionProbe probe;
    [SerializeField] private Camera targetCamera;

    [Header("Update Settings")]
    public float positionThreshold = 0.5f;
    public float rotationThreshold = 5f;
    public float minUpdateDelay = 0.2f;

    private Vector3 lastPosition;
    private Quaternion lastRotation;
    private float nextUpdateTime;

    void Awake()
    {
        if (probe == null)
            probe = GetComponent<ReflectionProbe>();

        if (targetCamera == null)
            targetCamera = Camera.main;

        lastPosition = targetCamera.transform.position;
        lastRotation = targetCamera.transform.rotation;
    }

    void LateUpdate()
    {
        if (Time.time < nextUpdateTime)
            return;

        Transform cam = targetCamera.transform;

        bool moved =
            Vector3.Distance(cam.position, lastPosition) >= positionThreshold;

        bool rotated =
            Quaternion.Angle(cam.rotation, lastRotation) >= rotationThreshold;

        if (moved || rotated)
        {
            probe.RenderProbe();

            lastPosition = cam.position;
            lastRotation = cam.rotation;
            nextUpdateTime = Time.time + minUpdateDelay;
        }
    }
}