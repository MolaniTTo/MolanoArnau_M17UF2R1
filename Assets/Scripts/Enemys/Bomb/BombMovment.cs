using System.Collections;
using UnityEngine;

public class BombMovment : MonoBehaviour
{
    public float detectionRadious = 5f; //radi de detecció
    public float explosionRadious = 3f;//radi d'explosió
    public float explosionDelay = 2f;//temps d'espera per a l'explosió
    public float moveSpeed = 2f;//velocitat de moviment

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

    private void Update() //se que son moltes coses dins l'update i no es la state machine de tus sueños pero estic death amb aquest projece ya
    {

        if (isDead || isFadingOut || playerController == null || playerController.isPlayerActive == false) return; //verifiquem si el player està actiu
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        //si el player està dins del radi de detecció
        if (distanceToPlayer <= detectionRadious)
        {
            //si el player esta dins del radi d'explosió
            if (distanceToPlayer <= explosionRadious)
            {
                timeInExplosionRadius += Time.deltaTime; //incrementem el temps dins del radi d'explosió

                if (timeInExplosionRadius >= explosionDelay)
                {
                    StartExplosion(); //explota si el temps dins del radi d'explosió és suficient
                }
                else
                {
                    animator.SetBool("IsWalking", true); //cambiem l'estat de l'animator
                    FollowPlayer();
                }
            }
            else
            {
                //resetegem el temps dins del radi d'explosió
                timeInExplosionRadius = 0f;
                FollowPlayer();
            }
        }
        else
        {
            animator.SetBool("IsWalking", false); //no es camina si el player no està dins del radi de detecció
        }
    }

    void FollowPlayer()
    {
        if(isFadingOut) return;
        if (player == null) return;
        animator.SetBool("IsWalking", true);

        //Moviment cap al player
        Vector2 direction = (player.position - transform.position).normalized; //direcció cap al player
        transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;
    }

    void StartExplosion()
    {
        isExploding = true;
        animator.SetTrigger("Explode");
    }

    public void HandleExplosionDamage()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer <= explosionRadious) //si el player esta dins del radi d'explosió
        {
            PlayerController playerController = player.GetComponent<PlayerController>(); 
            if (playerController != null)
            {
                playerController.TakeDamage(damage); //Apliquem dany al player
                if(!playerController.shieldActive)
                {
                    KnockBack knockBack = player.GetComponent<KnockBack>(); //parides que he posat pero estan chuleta
                    if (knockBack != null)
                    {
                        Vector2 knockBackDirection = (player.position - transform.position).normalized;
                        knockBack.ApplyKnockBack(knockBackDirection); //emputxa al player
                    }
                }
            }
        }
        isFadingOut = true;

    }
    IEnumerator FadeOut() //funció per fer el fade out al enemic (ns pq la vaig posar) pero aqui esta
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

    private void OnDrawGizmosSelected() //aixo m'ha ajudat amb els radis
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadious);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadious);
    }
}


