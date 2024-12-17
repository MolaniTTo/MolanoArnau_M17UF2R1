using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public event Action<GameObject> OnEnemyDeath;

    public void Die()
    {
        Debug.Log($"{name} ha muerto.");
        // Disparar el evento
        OnEnemyDeath?.Invoke(gameObject);
        Destroy(gameObject);
    }

    [SerializeField] private int startingHealth = 3;

    private Flash flash;
    private float currentHealth;

    private void Awake()
    {
        flash = GetComponent<Flash>();

    }

    private void Start()
    {
        currentHealth = startingHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        StartCoroutine(flash.FlashRoutine());
        
    }

    public void DetectDeath()
    {
        if (currentHealth <= 0)
        {
            Die();
        }
    }


    
}
