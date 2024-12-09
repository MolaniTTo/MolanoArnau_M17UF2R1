using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour , PlayerInputActions.IPlayerActions , PlayerInputActions.ICombatActions
{
    public static PlayerController Instance { get; private set; } //Singleton
    public bool FacingLeft { get { return facingLeft; } } // Propiedad para obtener la direcci�n del jugador

    private PlayerInputActions ic; // Referencia al Input Action
    private Animator animator; // Referencia al componente Animator

    [SerializeField] private float dashSpeed = 9f;
    [SerializeField] private float speed = 5f;
    [SerializeField] private int live = 100;
    [SerializeField] private TrailRenderer mytrailRenderer;
    [SerializeField] private Transform weaponCollider;

    private Vector2 moveDirection;    // Direcci�n del movimiento
    public Vector2 lookDirection { get; private set;}

    public bool isRightClickPressed { get; private set; }// Estado del clic derecho
    public bool isLeftClickPressed { get; private set; }  // Estado del clic izquierd
    public bool facingLeft = false;
    public bool isDashing = false;
    public bool isDied = false;

    private Sword sword; // Referencia a la espasa


    private void Awake()
    {
        ic = new PlayerInputActions();
        ic.Player.SetCallbacks(this);
        animator = GetComponent<Animator>();

        sword = GetComponentInChildren<Sword>(); //Busquem la espasa com a fill del jugador
    }

    public void Start()
    {
        ic.Combat.Dash.performed += _ => Dash();
       
    }

    void Update()
    {
        if (isDied) return;
        MovePlayer();
        HandleAnimations();
    }

    private void OnEnable()
    {
        ic.Enable();
    }

    private void OnDisable()
    {
        ic.Disable();
    }

    public Transform GetWeaponCollider()
    {
        if (weaponCollider == null)
        {
            Debug.LogError("Weapon Collider is not assigned in PlayerController!");
        }
        return weaponCollider;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            moveDirection = context.ReadValue<Vector2>();
        }
        else if(context.canceled)
        {
            moveDirection = Vector2.zero;
        }

    }

    public void OnLook(InputAction.CallbackContext context)
    {
        if(context.performed)
        {

            // Calcular la direcci�n del cursor desde la posici�n del jugador
            Vector2 mousePosition = context.ReadValue<Vector2>();
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 0));
            Vector3 direction = (worldPosition - transform.position).normalized;
            lookDirection = new Vector2(direction.x, direction.y);

            if(lookDirection.x < 0 && !facingLeft)
            {
                facingLeft = true;
               
            }
            else if(lookDirection.x > 0 && facingLeft)
            {
                facingLeft = false;
            }
        }


    }

    public void OnRightClick(InputAction.CallbackContext context)
    {
        isRightClickPressed = context.phase == InputActionPhase.Performed;
    }

    public void OnLeftClick(InputAction.CallbackContext context)
    {
        // Actualizar el estado del clic izquierdo
        isLeftClickPressed = context.phase == InputActionPhase.Performed;
        if (isLeftClickPressed)
        {
            sword?.Attack();
        }
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
            // Animaci�n basada en la direcci�n del cursor
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

    public void OnAttack(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnLookCombat(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }


    public void OnDash(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            Dash();
        }
    }

    private void Dash()
    {
        if(!isDashing)
        {
            isDashing = true;
            speed*= dashSpeed;
            mytrailRenderer.emitting = true;
            StartCoroutine(EndDashRoutine());
        } 
    }

    private IEnumerator EndDashRoutine()
    {   
        float dashTime = 0.2f;
        float dashCD = 0.25f;
        yield return new WaitForSeconds(dashTime);
        speed /= dashSpeed;
        mytrailRenderer.emitting = false;
        yield return new WaitForSeconds(dashCD);
        isDashing = false;
    }
}


