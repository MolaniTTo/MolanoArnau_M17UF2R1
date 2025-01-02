using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public event Action<GameObject> OnEnemyDeath;
    [SerializeField] private EnemyCounter enemyCounter;
    [SerializeField] private int startingHealth = 3;
    private Flash flash;
    private float currentHealth;
    private bool isDeath = false;

    private void Awake()
    {
        flash = GetComponent<Flash>();
    }
    private void Start()
    {
        currentHealth = startingHealth;

        if (enemyCounter == null)
        {
            enemyCounter = FindObjectOfType<EnemyCounter>();
        }
        if (enemyCounter != null)
        {
            enemyCounter.RegisterEnemy(this);
        }
        else
        {
            Debug.LogWarning("No se ha asignado el EnemyCounter");
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDeath) return;
        currentHealth -= damage;
        StartCoroutine(flash.FlashRoutine());
    }

    public void DetectDeath()
    {
        if (currentHealth <= 0 && !isDeath)
        {
            Die();
        }
    }

    public void Die()
    {
        if(isDeath) return;
        isDeath = true;
        Debug.Log($"{name} ha muerto.");
        // Disparar el evento
        OnEnemyDeath?.Invoke(gameObject);
        Destroy(gameObject);
    }
}
