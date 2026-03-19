using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollow : MonoBehaviour
{
    [Header("Distance")]
    public float distance     = 8f;
    public float smoothSpeed  = 8f;

    [Header("Mouse Look")]
    public float sensitivity = 0.15f;
    public float pitchMin    = -15f;
    public float pitchMax    =  70f;
    public float initialPitch=  20f;

    private Transform target;
    private float yaw;
    private float pitch;
    private Vector3 currentVelocity;

    void Start()
    {
        BallController ball = FindFirstObjectByType<BallController>();
        if (ball != null) target = ball.transform;

        pitch = initialPitch;
        yaw   = transform.eulerAngles.y;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;

        if (target != null) SnapToTarget();
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector2 mouse = Mouse.current.delta.ReadValue();
        yaw   += mouse.x * sensitivity;
        pitch -= mouse.y * sensitivity;
        pitch  = Mathf.Clamp(pitch, pitchMin, pitchMax);

        Vector3 desired = ComputeDesiredPosition();
        transform.position = Vector3.SmoothDamp(
            transform.position, desired,
            ref currentVelocity, 1f / smoothSpeed
        );

        transform.LookAt(target.position + Vector3.up * 1f);
    }

    Vector3 ComputeDesiredPosition()
    {
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        return target.position + rotation * (Vector3.back * distance);
    }

    void SnapToTarget()
    {
        transform.position = ComputeDesiredPosition();
        transform.LookAt(target.position + Vector3.up * 1f);
    }
}
