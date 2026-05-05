using UnityEngine;

/// <summary>
/// Предотвращает прохождение камеры сквозь стены (эффект "рентгена").
/// Вешается на объект Player. Работает в LateUpdate, чтобы не конфликтовать с движением.
/// </summary>
public class CameraWallClip : MonoBehaviour
{
    [Header("Settings")]
    public Transform cameraTransform;       // Ссылка на MainCamera
    [SerializeField] private float checkDistance = 0.25f; // Дистанция "вплотную" к стене
    [SerializeField] private LayerMask wallLayer;         // Слой препятствий (Obstacle/Default)

    void LateUpdate()
    {
        if (cameraTransform == null) return;

        // Проверяем, не оказалась ли камера внутри стены прямо перед собой
        Vector3 forward = cameraTransform.forward;
        if (Physics.Raycast(cameraTransform.position, forward, checkDistance, wallLayer))
        {
            // Отодвигаем камеру чуть назад, чтобы она не рендерила текстуры изнутри коллайдера
            cameraTransform.position -= forward * 0.15f;
        }
    }
}