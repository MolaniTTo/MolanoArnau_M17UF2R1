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

    public void Reset() //al final ns si crido aquest metode ara q ho penso (estic muerto)
    {
        deathCount = 0;
        roundDeathCount = 0;
        totalEnemies = 0;
        headCount = 0;
        inventorySystem.ResetSlots();
        inventorySystem.ToggleActiveSlot(1);
        UpdateKillsToUnlockForRound(1);
        UpdateCounter();
    }

    public void StartNewRound(int currentRound) //per la nova ronda
    {
        previousRoundsDeathCount = deathCount;
        roundDeathCount = 0;

        inventorySystem.ResetSlots();

        inventorySystem.ToggleActiveSlot(1);

        UpdateKillsToUnlockForRound(currentRound);
    }

    public void RegisterEnemy(EnemyHealth enemy) //registra el nou enemic i crida a l'event 
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

    private void CheckForUnlocks() //comprova si s'han de desbloquejar slots
    {
        for (int i = 0; i < killsToUnlockSlots.Length; i++)
        {
            if (roundDeathCount >= killsToUnlockSlots[i])
            {
                inventorySystem.UnlockSlot(i); //notificia el desbloqueig del slot
            }
        }
    }

    public void UpdateKillsToUnlockForRound(int currentRound) //actualitza el nombre d'enemics a matar per a desbloquejar cada slot, en cas de fer moooooltes rondes, no podra desbloquejar ni el flamethrower ni l'arc i mabye esta una mica desequilibrat, pero depen de lo chetao q vaya
    {
        for (int i = 0; i < killsToUnlockSlots.Length; i++)
        {
            killsToUnlockSlots[i] = basekillsToUnlockSlots[i] * currentRound;
        }
    }

    private void UpdateCounter()
    {
        counterText.text = $"{deathCount} / {totalEnemies}"; //actualitza el text del contador

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
        int targetHeadCount = Mathf.Max(0, headCount - amount); //assegurar que el contador no baixi de 0

        float elapsedTime = 0f;
        float duration = 2f;

        //animacio de baixada del contador
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            headCount = (int)Mathf.Lerp(initialHeadCount, targetHeadCount, elapsedTime / duration);
            UpdateCounter();
            yield return null;
        }

        //ens assegurem que el contador es el correcte
        headCount = targetHeadCount;
        UpdateCounter();
    }
}
