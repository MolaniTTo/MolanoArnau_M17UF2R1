using System.Collections;
using UnityEngine;

public class BombMovment : MonoBehaviour
{
    public float detectionRadious = 5f;  // Radio de detección para seguir al jugador
    public float explosionRadious = 3f; // Radio de explosión
    public float explosionDelay = 2f;  // Tiempo necesario dentro del radio de explosión para detonar
    public float moveSpeed = 2f;        // Velocidad de movimiento del enemigo

    public int damage = 15;
    public float fadeDuration = 1f;

    private Animator animator;
    private Transform player;
    private bool isExploding = false;
    private bool isDead = false;
    private bool isFadingOut = false;
    private float timeInExplosionRadius = 0f;
    SpriteRenderer spriteRenderer;
    EnemyHealth enemyHealth;
    PlayerController playerController;


    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerController = player.GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyHealth = GetComponent<EnemyHealth>();
    }

    private void Update()
    {

        if (isDead || isFadingOut || playerController == null || playerController.isPlayerActive == false) return; // Verificar si el jugador está activo
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        // Si el jugador está dentro del radio de detección
        if (distanceToPlayer <= detectionRadious)
        {
            // Si el jugador está dentro del radio de explosión
            if (distanceToPlayer <= explosionRadious)
            {
                timeInExplosionRadius += Time.deltaTime; // Incrementar el tiempo dentro del radio

                if (timeInExplosionRadius >= explosionDelay)
                {
                    StartExplosion(); // Detonar si se alcanza el tiempo requerido
                }
                else
                {
                    animator.SetBool("IsWalking", true); // Cambiar animación si no está caminando
                    FollowPlayer();
                }
            }
            else
            {
                // Reseteamos el tiempo si el jugador abandona el radio de explosión
                timeInExplosionRadius = 0f;
                FollowPlayer();
            }
        }
        else
        {
            animator.SetBool("IsWalking", false); // No caminar si el jugador está fuera del radio
        }
    }

    void FollowPlayer()
    {
        if(isFadingOut) return;
        if (player == null) return;
        animator.SetBool("IsWalking", true);

        // Movimiento hacia el jugador
        Vector2 direction = (player.position - transform.position).normalized;
        transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;
    }

    void StartExplosion()
    {
        isExploding = true;
        Debug.Log("BOOOM! Explosion triggered.");
        animator.SetTrigger("Explode");
    }

    public void HandleExplosionDamage()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer <= explosionRadious)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.TakeDamage(damage); // Aplicar daño al jugador
                if(!playerController.shieldActive)
                {
                    KnockBack knockBack = player.GetComponent<KnockBack>();
                    if (knockBack != null)
                    {
                        Vector2 knockBackDirection = (player.position - transform.position).normalized;
                        knockBack.ApplyKnockBack(knockBackDirection); // Empujar al jugador
                    }
                }
            }
        }
        isFadingOut = true;

    }
    IEnumerator FadeOut()
    {

        animator.SetBool("IsWalking", false);
        float elapsedTime = 0f;
        Color color = spriteRenderer.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(1, 0, elapsedTime / fadeDuration);
            spriteRenderer.color = new Color(color.r, color.g, color.b, newAlpha);
            yield return null;
        }
        spriteRenderer.color = new Color(color.r, color.g, color.b, 0f);
        isDead = true;
        enemyHealth.Die();
    }

    private void OnDrawGizmosSelected()
    {
        // Dibujar radios en la vista de escena
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadious);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadious);
    }
}


