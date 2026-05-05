using UnityEngine;

/// <summary>
/// ”правление камерой мышью: горизонтальный поворот игрока, вертикальный наклон камеры.
/// </summary>
public class MouseLook : MonoBehaviour
{
    [Header("Mouse Settings")]
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float minVerticalAngle = -60f;
    [SerializeField] private float maxVerticalAngle = 60f;

    private float verticalRotation = 0f;
    private Transform playerTransform;
    private Transform cameraTransform;

    private void Start()
    {
        cameraTransform = transform;
        playerTransform = transform.parent;

        // Ѕлокируем курсор в центре экрана
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // √оризонтальный поворот (вращаем всего игрока)
        if (playerTransform != null)
        {
            playerTransform.Rotate(Vector3.up * mouseX);
        }

        // ¬ертикальный наклон (вращаем только камеру)
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, minVerticalAngle, maxVerticalAngle);

        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }
}