using UnityEngine;

/// <summary>
/// Компонент врага для проверки видимости игрока.
/// </summary>
public class EnemyVision : MonoBehaviour
{
    [Header("Vision Settings")]
    [SerializeField] private float viewRadius = 10f;
    [SerializeField] private float viewAngle = 90f;
    [SerializeField] private LayerMask obstacleMask; // Слой стен и укрытий
    [SerializeField] private LayerMask targetMask;   // Слой игрока

    private Transform player;
    private EnemyStateMachine stateMachine;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        stateMachine = GetComponent<EnemyStateMachine>();
    }

    private void Update()
    {
        if (player == null || stateMachine == null) return;

        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        float distToPlayer = Vector3.Distance(transform.position, player.position);

        Vector3 eyePos = transform.position + Vector3.up * 1.5f;

        bool canSeePlayer = false;

        // 1. Проверка дистанции и угла
        if (distToPlayer <= viewRadius)
        {
            float angle = Vector3.Angle(transform.forward, dirToPlayer);
            if (angle <= viewAngle * 0.5f)
            { 
                if (!Physics.Raycast(eyePos, dirToPlayer, distToPlayer, obstacleMask))
                {
                    canSeePlayer = true;
                }
            }
        }

        // 3. Передача состояния в машину состояний
        if (canSeePlayer)
        {
            stateMachine.OnPlayerDetected(player.position);
        }
        else
        {
            // Сообщаем, что потеряли игрока из виду
            stateMachine.OnPlayerLost();
        }

        // 4. Отладка: зелёный луч = видит, красный = не видит
        Debug.DrawLine(eyePos, player.position, canSeePlayer ? Color.green : Color.red);
    }

    // Визуализация конуса в редакторе
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        Vector3 left = Quaternion.Euler(0, -viewAngle * 0.5f, 0) * transform.forward * viewRadius;
        Vector3 right = Quaternion.Euler(0, viewAngle * 0.5f, 0) * transform.forward * viewRadius;

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + left);
        Gizmos.DrawLine(transform.position, transform.position + right);
    }
}