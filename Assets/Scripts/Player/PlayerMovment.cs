using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    private Animator animator;
    public bool isDied = false;

    public float live = 30;

    private Vector2 moveDirection;    // Dirección del movimiento
    private Vector2 lookDirection;    // Dirección del cursor
    private bool isRightClickPressed; // Estado del clic derecho
    private bool isLeftClickPressed;  // Estado del clic izquierd

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }


    void Update()
    {
        if (isDied) return;

        MovePlayer();

        HandleAnimations();
    }


    private void MovePlayer()
    {
        // Aplicar el movimiento
        Vector3 movement = new Vector3(moveDirection.x, moveDirection.y, 0f);
        transform.position += movement * speed * Time.deltaTime;
    }

    private void HandleAnimations()
    {
        if (isRightClickPressed) // Si se presiona el clic derecho
        {
            // Animación basada en la dirección del cursor
            animator.SetFloat("AnimationDirectionX", lookDirection.x);
            animator.SetFloat("AnimationDirectionY", lookDirection.y);
        }
        else if (moveDirection != Vector2.zero) // Movimiento con teclado
        {
            animator.SetFloat("AnimationDirectionX", moveDirection.x);
            animator.SetFloat("AnimationDirectionY", moveDirection.y);
        }

        // Establecer la velocidad en el Blend Tree
        animator.SetFloat("Speed", moveDirection.magnitude);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        // Actualizar dirección de movimiento desde el Input System
        moveDirection = context.ReadValue<Vector2>();

    }

    public void OnLook(InputAction.CallbackContext context)
    {
        // Calcular la dirección del cursor desde la posición del jugador
        Vector2 mousePosition = context.ReadValue<Vector2>();
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        Vector3 direction = (worldPosition - transform.position).normalized;
        lookDirection = new Vector2(direction.x, direction.y);
    }

    public void OnRightClick(InputAction.CallbackContext context)
    {
        // Actualizar el estado del clic derecho
        isRightClickPressed = context.phase == InputActionPhase.Performed;
    }

    public void OnLeftClick(InputAction.CallbackContext context)
    {
        // Actualizar el estado del clic izquierdo
        isLeftClickPressed = context.phase == InputActionPhase.Performed;

        if (isLeftClickPressed)
        {
            Debug.Log("Attack performed!");
            // Aquí podrías llamar a un método para manejar el ataque
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            Debug.Log("Player hit");
           

            live -= 10;
            if (live <= 0)
            {
                animator.SetTrigger("Die");
                isDied = true;
            }
            Debug.Log(live);

        }
    }
}


