using UnityEngine;

/// <summary>
/// Компонент врага для проверки видимости игрока.
/// </summary>
public class EnemyVision : MonoBehaviour
{
    [Header("Vision Settings")]
    [SerializeField] private float viewRadius = 10f;
    [SerializeField] private float viewAngle = 90f;
    [SerializeField] private LayerMask obstacleMask; // Слои, блокирующие обзор
    [SerializeField] private LayerMask targetMask;   // Слой игрока

    private Transform player;
    private EnemyStateMachine stateMachine;

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        stateMachine = GetComponent<EnemyStateMachine>();
    }

    private void Update()
    {
        if (player == null || stateMachine == null) return;

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // 1. Проверка расстояния
        if (distanceToPlayer <= viewRadius)
        {
            // 2. Проверка угла обзора
            float angle = Vector3.Angle(transform.forward, directionToPlayer);
            if (angle <= viewAngle * 0.5f)
            {
                // 3. Проверка прямой видимости (луч не должен упираться в препятствия)
                if (!Physics.Raycast(transform.position, directionToPlayer, distanceToPlayer, obstacleMask))
                {
                    // Игрок виден ? отправляем сигнал машине состояний
                    stateMachine.OnPlayerDetected(player.position);
                }
            }
        }
    }

    // Визуализация конуса в редакторе (при выделении объекта)
    private void OnDrawGizmosSelected()
    {
        // Сфера радиуса обзора
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        // Границы угла обзора
        Vector3 leftBoundary = Quaternion.Euler(0, -viewAngle * 0.5f, 0) * transform.forward * viewRadius;
        Vector3 rightBoundary = Quaternion.Euler(0, viewAngle * 0.5f, 0) * transform.forward * viewRadius;

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary);
    }
}