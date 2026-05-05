using UnityEngine;

public class CoverChecker : MonoBehaviour
{
    [SerializeField] private LayerMask coverLayer; // Назначьте слой Obstacle/Cover

    public bool IsInCoverFrom(Transform enemy)
    {
        Vector3 directionToEnemy = (enemy.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, enemy.position);

        RaycastHit hit;
        if (Physics.Raycast(enemy.position, directionToEnemy, out hit, distance, coverLayer))
        {
            // Если луч от врага попал в укрытие до игрока -> вы в укрытии
            return true;
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        EnemyStateMachine[] enemies = FindObjectsOfType<EnemyStateMachine>();
        foreach (var enemy in enemies)
        {
            if (enemy != null)
            {
                bool inCover = IsInCoverFrom(enemy.transform);
                Gizmos.color = inCover ? Color.green : Color.red;
                Gizmos.DrawLine(transform.position, enemy.transform.position);
            }
        }
    }
}