
using UnityEngine;
using TMPro;

public class EnemyCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI counterText;
    [SerializeField] private InventorySystem inventorySystem;
    private int deathCount = 0;
    private int totalEnemies = 0;

    [SerializeField] private int[] basekillsToUnlockSlots; //arrat amb el nombre d'enemics a matar per a desbloquejar cada slot
    private int[] killsToUnlockSlots;

    private void Start()
    {
        killsToUnlockSlots = (int[])basekillsToUnlockSlots.Clone(); //Clona l'array per a evitar modificar l'original
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
        CheckForUnlocks();
    }

    private void CheckForUnlocks()
    {
        for (int i = 0; i < killsToUnlockSlots.Length; i++)
        {
            if (deathCount >= killsToUnlockSlots[i])
            {
                inventorySystem.UnlockSlot(i); // Notifica el desbloqueo al sistema de inventario.
            }
        }
    }

    public void UpdateKillsToUnlockForRound(int currentRound)
    {
        for (int i = 0; i < killsToUnlockSlots.Length; i++)
        {
            killsToUnlockSlots[i] = basekillsToUnlockSlots[i] * currentRound;
        }
        Debug.Log($"Kills para desbloqueo actualizados para la ronda {currentRound}: {string.Join(", ", killsToUnlockSlots)}");
    }

    private void UpdateCounter()
    {
        counterText.text = $"{deathCount} / {totalEnemies}";
    }
}
