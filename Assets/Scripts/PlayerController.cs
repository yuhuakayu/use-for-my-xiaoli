using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 4.2f;
    public float sprintSpeed = 6f;
    public float gravity = -20f;
    public float turnSpeed = 18f;

    [Header("Jump")]
    public float jumpHeight = 1.2f;
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Aiming")]
    public Transform cameraTransform;

    private CharacterController controller;
    private Vector3 verticalVelocity;
    private bool jumpRequested;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Start()
    {
        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameState.Playing)
        {
            return;
        }

        // 在 Update 里检测按键按下的瞬间，避免在 FixedUpdate 或多次调用中漏掉输入
        if (Input.GetKeyDown(jumpKey))
        {
            jumpRequested = true;
        }

        Move();
        FaceCameraAim();
    }

    private void Move()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 input = new Vector3(horizontal, 0f, vertical).normalized;
        Vector3 moveDirection = input;

        if (cameraTransform != null)
        {
            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();
            moveDirection = forward * input.z + right * input.x;
        }

        bool grounded = controller.isGrounded;

        if (grounded && verticalVelocity.y < 0f)
        {
            verticalVelocity.y = -2f;
        }

        // 只有在地面上才能起跳，避免空中连跳
        if (jumpRequested)
        {
            if (grounded)
            {
                // 根据目标跳跃高度反推初速度：v = sqrt(2 * h * g)
                verticalVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
            jumpRequested = false; // 无论是否成功起跳，消耗掉这次请求，避免落地瞬间"补跳"
        }

        verticalVelocity.y += gravity * Time.deltaTime;

        float speed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;
        Vector3 movement = moveDirection.normalized * speed + verticalVelocity;
        controller.Move(movement * Time.deltaTime);
    }

    private void FaceCameraAim()
    {
        if (cameraTransform == null)
        {
            return;
        }

        Vector3 aimDirection = cameraTransform.forward;
        aimDirection.y = 0f;

        if (aimDirection.sqrMagnitude < 0.01f)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(aimDirection.normalized);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
    }
}