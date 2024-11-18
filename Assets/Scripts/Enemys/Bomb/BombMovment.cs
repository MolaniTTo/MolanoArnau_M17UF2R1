using System.Collections;
using UnityEngine;

public class BombMovment : MonoBehaviour
{
    public float detectionRadious = 5f;  // Radio de detección para seguir al jugador
    public float explosionRadious = 3f; // Radio de explosión
    public float explosionDelay = 10f;  // Tiempo necesario dentro del radio de explosión para detonar
    public float moveSpeed = 2f;        // Velocidad de movimiento del enemigo

    public int damage = 50;
    public float fadeDuration = 2.0f;

    private Animator animator;
    private Transform player;
    private bool isExploding = false;
    private float timeInExplosionRadius = 0f;
    SpriteRenderer spriteRenderer;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Si ya está explotando, no hacer nada más
        if (isExploding)
            return;

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
                    Debug.Log($"Time in explosion radius: {timeInExplosionRadius}/{explosionDelay}");
                    animator.SetBool("IsWalking", false); // Cambiar animación si no está caminando
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
        //esperar a que termine la animación
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(2.5f);
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
        Destroy(gameObject);
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


