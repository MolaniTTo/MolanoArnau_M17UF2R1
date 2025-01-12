using System.Collections;
using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;

public class EnemyCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI counterText;
    [SerializeField] private TextMeshProUGUI headCounterText;
    [SerializeField] private InventorySystem inventorySystem;
    private int deathCount = 0;
    private int roundDeathCount = 0;
    private int previousRoundsDeathCount = 0;
    private int totalEnemies = 0;
    public int headCount = 0;

    [SerializeField] private int[] basekillsToUnlockSlots; //arrat amb el nombre d'enemics a matar per a desbloquejar cada slot
    private int[] killsToUnlockSlots;

    private void Start()
    {
        killsToUnlockSlots = (int[])basekillsToUnlockSlots.Clone(); //Clona l'array per a evitar modificar l'original
        UpdateCounter();
    }

    public void Reset()
    {
        deathCount = 0;
        roundDeathCount = 0;
        totalEnemies = 0;
        headCount = 0;
        inventorySystem.ResetSlots();
        inventorySystem.ToggleActiveSlot(1);
        UpdateKillsToUnlockForRound(0);
        UpdateCounter();
    }

    public void StartNewRound(int currentRound)
    {
        previousRoundsDeathCount = deathCount;
        roundDeathCount = 0;

        inventorySystem.ResetSlots();

        inventorySystem.ToggleActiveSlot(1);

        UpdateKillsToUnlockForRound(currentRound);
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
        roundDeathCount++;
        headCount++;
        UpdateCounter();
        CheckForUnlocks();
    }

    private void CheckForUnlocks()
    {
        for (int i = 0; i < killsToUnlockSlots.Length; i++)
        {
            if (roundDeathCount >= killsToUnlockSlots[i])
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

        headCounterText.text = $"{headCount}";
    }

    public int GetHeadCount()
    {
        return headCount;
    }

    public void DecreaseHeadCountGradually(int amount)
    {
        StartCoroutine(DecreaseHeadCountCoroutine(amount));
    }

    private IEnumerator DecreaseHeadCountCoroutine(int amount)
    {
        int initialHeadCount = headCount;
        int targetHeadCount = Mathf.Max(0, headCount - amount); // Asegurarnos de que el contador no sea negativo

        float elapsedTime = 0f;
        float duration = 2f; // Duración de la animación (en segundos)

        // Animación de la disminución gradual del contador
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            headCount = (int)Mathf.Lerp(initialHeadCount, targetHeadCount, elapsedTime / duration);
            UpdateCounter();
            yield return null;
        }

        // Asegurarnos de que el contador llegue al valor final
        headCount = targetHeadCount;
        UpdateCounter();
    }
}
