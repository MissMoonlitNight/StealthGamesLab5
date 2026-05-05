using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Отображает текущий уровень скрытности игрока .
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
        // Определяем состояние, опрашивая всех врагов в сцене
        EnemyStateMachine[] enemies = FindObjectsOfType<EnemyStateMachine>();
        isDetected = false;
        isSuspicious = false;

        foreach (var enemy in enemies)
        {
            if (enemy.currentState == EnemyStateMachine.State.Alert)
            {
                isDetected = true;
                break; // Если хотя бы один враг в тревоге — дальше проверять не нужно
            }
            else if (enemy.currentState == EnemyStateMachine.State.Suspicion)
            {
                isSuspicious = true;
            }
        }

        // Применяем цвет в зависимости от состояния
        if (isDetected)
            indicatorImage.color = detectedColor;
        else if (isSuspicious)
            indicatorImage.color = suspiciousColor;
        else
            indicatorImage.color = safeColor;
    }
}