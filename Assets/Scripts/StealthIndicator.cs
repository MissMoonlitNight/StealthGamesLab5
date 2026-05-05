using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ќтображает текущий уровень скрытности игрока (зелЄный Ц безопасно, красный Ц обнаружен).
/// (јдаптировано из методички, раздел 7.5)
/// </summary>
public class StealthIndicator : MonoBehaviour
{
    [SerializeField] private Image indicatorImage;
    [SerializeField] private Color safeColor = Color.green;
    [SerializeField] private Color suspiciousColor = Color.yellow;
    [SerializeField] private Color detectedColor = Color.red;

    private bool isDetected = false;
    private bool isSuspicious = false;

    private void Start()
    {
        if (indicatorImage == null)
            indicatorImage = GetComponent<Image>();
    }

    private void Update()
    {
        // ќпредел€ем состо€ние, опрашива€ всех врагов в сцене
        EnemyStateMachine[] enemies = FindObjectsOfType<EnemyStateMachine>();
        isDetected = false;
        isSuspicious = false;

        foreach (var enemy in enemies)
        {
            if (enemy.currentState == EnemyStateMachine.State.Alert)
            {
                isDetected = true;
                break; // ≈сли хот€ бы один враг в тревоге Ч дальше провер€ть не нужно
            }
            else if (enemy.currentState == EnemyStateMachine.State.Suspicion)
            {
                isSuspicious = true;
            }
        }

        // ѕримен€ем цвет в зависимости от состо€ни€
        if (isDetected)
            indicatorImage.color = detectedColor;
        else if (isSuspicious)
            indicatorImage.color = suspiciousColor;
        else
            indicatorImage.color = safeColor;
    }
}