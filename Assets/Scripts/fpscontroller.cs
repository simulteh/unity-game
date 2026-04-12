using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float crouchSpeed = 2.5f;
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;

    [Header("Crouch Settings")]
    public float crouchHeight = 1f;
    public float standingHeight = 2f;
    public float crouchCenterY = 0.5f;
    public float standingCenterY = 0f;
    public float crouchTransitionSpeed = 10f;

    [Header("Mouse Look Settings")]
    public float mouseSensitivity = 2f;
    public float maxVerticalAngle = 80f;
    public bool invertY = false;
    public Transform cameraTransform; // Ссылка на Main Camera (или её родительский объект для вертикального вращения)

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private bool isCrouching;

    private float currentSpeed;
    private float currentHeight;
    private float currentCenterY;

    private float verticalRotation = 0f; // Накопленный вертикальный угол

    void Start()
    {
        controller = GetComponent<CharacterController>();

        // Сначала пробуем найти через тег MainCamera
        if (cameraTransform == null)
        {
            Camera mainCam = Camera.main;
            if (mainCam != null)
                cameraTransform = mainCam.transform;
            else
            {
                // Если не найдено, ищем камеру в дочерних объектах
                cameraTransform = GetComponentInChildren<Camera>()?.transform;
                if (cameraTransform == null)
                    Debug.LogWarning("Camera reference not assigned and no Camera found in children!");
            }
        }

        currentSpeed = walkSpeed;
        currentHeight = standingHeight;
        currentCenterY = standingCenterY;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // --- Movement & Jump ---
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 move = transform.right * horizontal + transform.forward * vertical;

        bool isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        bool crouchInput = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);

        if (isRunning && !isCrouching)
            currentSpeed = runSpeed;
        else if (isCrouching)
            currentSpeed = crouchSpeed;
        else
            currentSpeed = walkSpeed;

        controller.Move(move * currentSpeed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded && !isCrouching)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // --- Crouch ---
        if (crouchInput)
        {
            isCrouching = true;
            currentHeight = crouchHeight;
            currentCenterY = crouchCenterY;
        }
        else
        {
            if (CanStandUp())
            {
                isCrouching = false;
                currentHeight = standingHeight;
                currentCenterY = standingCenterY;
            }
        }

        controller.height = Mathf.Lerp(controller.height, currentHeight, Time.deltaTime * crouchTransitionSpeed);
        Vector3 newCenter = controller.center;
        newCenter.y = Mathf.Lerp(controller.center.y, currentCenterY, Time.deltaTime * crouchTransitionSpeed);
        controller.center = newCenter;

        // --- Mouse Look (First Person) ---
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        if (invertY)
            mouseY = -mouseY;

        // Горизонтальный поворот персонажа
        transform.Rotate(Vector3.up * mouseX);

        // Вертикальный поворот камеры (с ограничением)
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -maxVerticalAngle, maxVerticalAngle);

        if (cameraTransform != null)
        {
            // Применяем вертикальный поворот к камере (локально, если камера внутри Player)
            cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        }
    }

    private bool CanStandUp()
    {
        float checkDistance = standingHeight - crouchHeight;
        Vector3 startPoint = transform.position + Vector3.up * crouchHeight;
        return !Physics.SphereCast(startPoint, controller.radius, Vector3.up, out RaycastHit hit, checkDistance);
    }

    // Необязательно: отпускаем курсор по нажатию Escape
    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void UpdateCursorLock()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if (Input.GetMouseButtonDown(0) && Cursor.lockState == CursorLockMode.None)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}