using UnityEngine;

public class OverShoulderCamera : MonoBehaviour
{
    [Header("Target")]
    public Transform target;
    public float lookHeight = 1.45f;

    [Header("Camera")]
    public Vector3 shoulderOffset = new Vector3(0.75f, 1.75f, -3.2f);
    public float smoothTime = 0.06f;
    public float mouseSensitivity = 2.2f;
    public float minPitch = -35f;
    public float maxPitch = 60f;

    [Header("Debug")]
    public bool showDebug = true;
    public float largeDeltaThreshold = 0.05f; // 单帧位移超过这个值就标红提示

    private float yaw;
    private float pitch = 12f;
    private Vector3 velocity;

    // debug 用
    private Vector3 lastPosition;
    private Quaternion lastRotation;
    private float lastPosDelta;
    private float lastRotDeltaDeg;

    private void Start()
    {
        if (target != null)
        {
            yaw = target.eulerAngles.y;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        lastPosition = transform.position;
        lastRotation = transform.rotation;
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameState.Playing)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            return;
        }

        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        Quaternion orbit = Quaternion.Euler(pitch, yaw, 0f);

        Vector3 targetPosition = target.position + orbit * shoulderOffset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        transform.rotation = orbit;

        // 记录本帧相机实际的位移量和旋转变化量
        lastPosDelta = Vector3.Distance(transform.position, lastPosition);
        lastRotDeltaDeg = Quaternion.Angle(transform.rotation, lastRotation);

        if (lastPosDelta > largeDeltaThreshold)
        {
            Debug.Log($"[Camera] Large position jump: {lastPosDelta:F4}m  frame={Time.frameCount}  " +
                       $"targetVel={velocity}  smoothTime={smoothTime}");
        }

        lastPosition = transform.position;
        lastRotation = transform.rotation;
    }

    private void OnGUI()
    {
        if (!showDebug) return;

        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.fontSize = 16;
        style.normal.textColor = Color.cyan;

        GUI.Label(new Rect(10, 150, 500, 24), $"[Camera] pos delta this frame: {lastPosDelta:F4}", style);
        GUI.Label(new Rect(10, 172, 500, 24), $"[Camera] rot delta this frame: {lastRotDeltaDeg:F4}°", style);
        GUI.Label(new Rect(10, 194, 500, 24), $"[Camera] velocity (SmoothDamp): {velocity}", style);
        GUI.Label(new Rect(10, 216, 500, 24), $"[Camera] yaw={yaw:F2}  pitch={pitch:F2}", style);

        if (lastPosDelta > largeDeltaThreshold)
        {
            GUIStyle warn = new GUIStyle(style);
            warn.normal.textColor = Color.red;
            GUI.Label(new Rect(10, 238, 500, 24), $"⚠ Camera jumped {lastPosDelta:F4}m this frame!", warn);
        }
    }
}