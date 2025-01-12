using System;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public event Action<GameObject> OnEnemyDeath;

    [SerializeField] private EnemyCounter enemyCounter;
    [SerializeField] private GameObject healthBarPrefab;
    [SerializeField] private Transform healthBarPosition;
    [SerializeField] private int startingHealth = 3;

    private Flash flash;
    private float currentHealth;
    private bool isDeath = false;
    private GameObject instantiatedHealthBar;
    private Image healthBarBackGround;

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

        // Instanciar la barra de vida si no existe
        if (instantiatedHealthBar == null)
        {
            instantiatedHealthBar = Instantiate(healthBarPrefab, healthBarPosition.position, Quaternion.identity, healthBarPosition);

            // Buscar el componente BackGround dentro de la jerarquía correctamente
            Transform healthBar = instantiatedHealthBar.transform.Find("HealthBar");
            if (healthBar != null)
            {
                healthBarBackGround = healthBar.Find("BackGround").GetComponent<Image>();
            }
            else
            {
                Debug.LogError("No se pudo encontrar el objeto HealthBar o BackGround en la barra de vida instanciada.");
            }
        }

        // Actualizar la barra de vida
        UpdateHealthBar();
    }

    public void UpdateHealthBar()
    {
        if (instantiatedHealthBar != null)
        {
            healthBarBackGround.fillAmount = currentHealth / startingHealth;
        }
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
        if (instantiatedHealthBar != null)
        {
            Destroy(instantiatedHealthBar);
        }
        Destroy(gameObject);
    }
}
