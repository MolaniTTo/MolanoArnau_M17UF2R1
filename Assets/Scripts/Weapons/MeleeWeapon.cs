using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    public Transform player;              // Referencia al jugador
    public float distanceFromPlayer = 1.5f;  // Distancia fija del arma al jugador
    public float pushForce = 5f;          // Fuerza de empuje
    public GameObject melee; // Referencia al arma
    public SpriteRenderer spriteRenderer; // Referencia al sprite del arma
    public Collider2D weaponCollider;     // Collider del arma

    private bool isAttacking = false;     // Estado del golpe

    private void Start()
    {
        // Asegurarse de que el arma comience desactivada
        DeactivateWeapon();
    }

    private void Update()
    {
        if (Input.GetMouseButton(1)) // Mientras se mantenga el clic derecho
        {
            ActivateWeapon();
            FollowCursor();

            // Solo permite golpear mientras el clic derecho esté activo
            if (Input.GetMouseButtonDown(0)) // Detectar clic izquierdo
            {
                StartAttack();
            }
        }

        if (Input.GetMouseButtonUp(1)) // Al soltar el clic derecho
        {
            DeactivateWeapon();
        }

        if (Input.GetMouseButtonUp(0)) // Al soltar el clic izquierdo
        {
            StopAttack();
        }
    }

    private void FollowCursor()
    {
        // Obtener la posición del cursor en el mundo
        Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        cursorPosition.z = 0f; // Mantener en el plano 2D

        // Calcular la posición del arma alrededor del jugador
        Vector3 direction = (cursorPosition - player.position).normalized;
        transform.position = player.position + direction * distanceFromPlayer;

        // Rotar el arma hacia el cursor
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void ActivateWeapon()
    {
       spriteRenderer.enabled = true;    // Mostrar el sprite
    }

    private void DeactivateWeapon()
    {
        spriteRenderer.enabled = false;  // Ocultar el sprite
        weaponCollider.enabled = false; // Desactivar el collider
        StopAttack(); // Por si estaba atacando, detener el ataque
    }

    private void StartAttack()
    {
        isAttacking = true;
        weaponCollider.enabled = true;   // Activar el collider solo durante el golpe
        Debug.Log("Ataque iniciado.");
    }

    private void StopAttack()
    {
        isAttacking = false;
        weaponCollider.enabled = false;  // Desactivar el collider después del golpe
        Debug.Log("Ataque detenido.");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isAttacking && collision.CompareTag("Enemy")) // Golpea un enemigo solo si está atacando
        {
            Rigidbody2D enemyRb = collision.GetComponent<Rigidbody2D>();
            if (enemyRb != null)
            {
                // Aplicar fuerza en la dirección del golpe
                Vector2 pushDirection = (collision.transform.position - transform.position).normalized;
                enemyRb.AddForce(pushDirection * pushForce, ForceMode2D.Impulse);
                Debug.Log("Golpe exitoso al enemigo.");
            }
        }
    }
}
