
using UnityEngine;
using TMPro;

public class EnemyCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI counterText;
    private int deathCount = 0;
    private int totalEnemies = 0;   

    private void Start()
    {
        UpdateCounter();
    }

    public void RegisterEnemy(EnemyHealth enemy)
    {
        totalEnemies++;
        enemy.OnEnemyDeath += OnEnemyDied;
        UpdateCounter();
    }

    private void OnEnemyDied(GameObject enemy)
    {
        deathCount++;
        UpdateCounter();
    }

    private void UpdateCounter()
    {
        counterText.text = $"{deathCount} / {totalEnemies}";
    }
}
