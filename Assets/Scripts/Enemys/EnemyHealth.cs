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
    
    }

    public void TakeDamage(float damage) //Aquesta funció s'executarà quan l'enemic rebi mal
    {
        if (isDeath) return;

        currentHealth -= damage;
        StartCoroutine(flash.FlashRoutine()); //Flash de l'enemic

        //Instanciem la barra de vida si no existeix encara
        if (instantiatedHealthBar == null)
        {
            instantiatedHealthBar = Instantiate(healthBarPrefab, healthBarPosition.position, Quaternion.identity, healthBarPosition);

            // Busquem el component Image de la barra de vida
            Transform healthBar = instantiatedHealthBar.transform.Find("HealthBar");
            if (healthBar != null)
            {
                healthBarBackGround = healthBar.Find("BackGround").GetComponent<Image>();
            }
        }

        //actualotzem la barra de vida
        UpdateHealthBar();
    }

    public void UpdateHealthBar()
    {
        if (instantiatedHealthBar != null)
        {
            healthBarBackGround.fillAmount = currentHealth / startingHealth; //fem que la barra de vida sigui proporcional a la vida actual
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
        //disparem l'event de mort de l'enemic Carlos odio els events :)
        OnEnemyDeath?.Invoke(gameObject);
        if (instantiatedHealthBar != null)
        {
            Destroy(instantiatedHealthBar);
        }
        Destroy(gameObject);
    }
}
