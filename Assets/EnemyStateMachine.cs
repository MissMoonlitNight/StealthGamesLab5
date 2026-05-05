using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Управление состояниями врага: патруль, подозрение, тревога.
/// </summary>
public class EnemyStateMachine : MonoBehaviour
{
    // Перечисление состояний врага
    public enum State { Patrol, Suspicion, Alert }
    public State currentState;

    [Header("Patrol")]
    [SerializeField] private Transform[] waypoints; // Точки маршрута патрулирования
    private int currentWaypointIndex;
    private NavMeshAgent agent;

    [Header("Suspicion")]
    [SerializeField] private float suspicionDuration = 5f; // Время поиска после шума
    [SerializeField] private float searchRadius = 3f; // Радиус случайного поиска
    [SerializeField] private float hearingRange = 10f; // Радиус слышимости 

    private float suspicionTimer;
    private Vector3 lastKnownPosition;
    private Vector3 searchTarget;
    private bool isSearching = false;

    [Header("Alert")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float attackCooldown = 1f;
    private float lastAttackTime;

    private Transform player;
    private bool playerDetected = false;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        currentState = State.Patrol;
        SetNextWaypoint();
    }

    private void Update()
    {
        switch (currentState)
        {
            case State.Patrol:
                Patrol();
                CheckNoise(); //  проверка шума во время патруля
                break;

            case State.Suspicion:
                Suspicion();
                break;

            case State.Alert:
                Alert();
                break;
        }
    }

    // Патруль
    private void Patrol()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            SetNextWaypoint();
        }
    }

    private void SetNextWaypoint()
    {
        if (waypoints.Length == 0) return;

        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        agent.SetDestination(waypoints[currentWaypointIndex].position);
    }

    // Шум
    private void CheckNoise()
    {
        // Проверяем наличие шума в радиусе hearingRange 
        var noise = NoiseManager.GetClosestNoise(transform.position, hearingRange);

        if (noise.HasValue)
        {
            lastKnownPosition = noise.Value.position;
            suspicionTimer = suspicionDuration;
            currentState = State.Suspicion;
            isSearching = false;
            agent.SetDestination(lastKnownPosition);
        }
    }

    // Подозрительность
    private void Suspicion()
    {
        suspicionTimer -= Time.deltaTime;

      
        if (!isSearching && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            // Случайная точка в радиусе поиска
            Vector3 randomDir = Random.insideUnitSphere * searchRadius;
            randomDir.y = 0; // Только горизонтальная плоскость
            searchTarget = lastKnownPosition + randomDir;

          
            NavMeshHit hit;
            if (NavMesh.SamplePosition(searchTarget, out hit, searchRadius, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
                isSearching = true;
            }
        }

        // Если время вышло ИЛИ дошли до точки поиска — возвращаемся к патрулю
        if (suspicionTimer <= 0 ||
            (isSearching && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance))
        {
            currentState = State.Patrol;
            isSearching = false;
            SetNextWaypoint();
        }
    }

    /// <summary>
    /// Вызывается из EnemyVision, когда враг видит игрока.
    /// </summary>
    public void OnPlayerDetected(Vector3 playerPos)
    {
        lastKnownPosition = playerPos;
        playerDetected = true;
        currentState = State.Alert;
    }

    /// <summary>
    /// Вызывается из EnemyVision, когда враг теряет игрока из виду.
    /// </summary>
    public void OnPlayerLost()
    {
        playerDetected = false;
    }

    private void Alert()
    {
        if (playerDetected && player != null)
        {
            // Преследуем игрока
            agent.SetDestination(player.position);

            // Проверка атаки
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer <= attackRange)
            {
                Attack();
            }
        }
        else
        {
            // Потеряли игрока — переходим в подозрение к последней известной позиции
            currentState = State.Suspicion;
            suspicionTimer = suspicionDuration;
            isSearching = false;
            agent.SetDestination(lastKnownPosition);
        }
    }

    private void Attack()
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            // Логика атаки (можно подключить скрипт здоровья из Лабораторной №3)
            Debug.Log($"Враг [{name}] атакует игрока! Урон: {attackDamage}");

            // Пример подключения здоровья игрока:
            // PlayerHealth playerHealth = player?.GetComponent<PlayerHealth>();
            // if (playerHealth != null)
            //     playerHealth.TakeDamage(attackDamage);

            lastAttackTime = Time.time;
        }
    }
}