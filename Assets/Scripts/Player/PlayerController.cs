using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour , PlayerInputActions.IPlayerActions , PlayerInputActions.ICombatActions
{
    public static PlayerController Instance { get; private set; } //Singleton
    public bool FacingLeft { get { return facingLeft; } } // Propiedad para obtener la dirección del jugador

    private PlayerInputActions ic; // Referencia al Input Action
    private Animator animator; // Referencia al componente Animator
    private Flash flash; // Referencia al componente Flash
    private ScreenFade screenFade; // Referencia al componente ScreenFade
    private EnemyCounter enemyCounter; // Referencia al contador de enemigos

    [SerializeField] private float dashSpeed = 9f;
    [SerializeField] private float speed = 5f;
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int shield = 150;
    private int currentHealth;

    [SerializeField] private Image healthBar;
    [SerializeField] private GameObject shieldBarObj;
    [SerializeField] private Image shieldBar;

    [SerializeField] private TrailRenderer mytrailRenderer;
    [SerializeField] private Transform weaponCollider;
    [SerializeField] private Transform SlashAnimSpawnPoint;


    private Vector2 moveDirection;    // Dirección del movimiento
    public bool isMovementBlocked { get; set; } = false;  // Estado del movimiento
    public Vector2 lookDirection { get; private set;}

    public bool isRightClickPressed { get; private set; }// Estado del clic derecho
    public bool isLeftClickPressed { get; private set; }  // Estado del clic izquierd
    public bool facingLeft = false;
    public bool isDashing = false;
    public bool isDied = false;
    public bool isPlayerActive = false;
    public bool shieldActive = false;

    private Sword sword; // Referencia a la espasa


    private void Awake()
    {
        Instance = this;
        ic = new PlayerInputActions();
        ic.Player.SetCallbacks(this);
        animator = GetComponent<Animator>();
        flash = GetComponent<Flash>();
        sword = GetComponentInChildren<Sword>(); //Busquem la espasa com a fill del jugador
        screenFade = FindObjectOfType<ScreenFade>();
        enemyCounter = FindObjectOfType<EnemyCounter>();

    }

    public void Start()
    {
        currentHealth = maxHealth;

        healthBar = GameObject.Find("HealthBackGround").GetComponent<Image>();
        shieldBar = GameObject.Find("ShieldBarBackGround").GetComponent<Image>();
        shieldBarObj = GameObject.Find("ShieldBar");
        shieldBarObj.SetActive(false);
        UpdateHealthBar();

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

    public void SetPlayerActive(bool isActive)
    {
        isPlayerActive = isActive;
    }

    public Transform GetWeaponCollider()
    {
        if (weaponCollider == null)
        {
            Debug.LogError("Weapon Collider is not assigned in PlayerController!");
        }
        return weaponCollider;
    }

    public Transform GetSlashAnimSpawnPoint()
    {
        if (SlashAnimSpawnPoint == null)
        {
            Debug.LogError("SlashAnimSpawnPoint is not assigned in PlayerController!");
        }
        return SlashAnimSpawnPoint;
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
            // Calcular la dirección del cursor desde la posición del jugador
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
        if (isMovementBlocked) return;
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            Debug.Log("Player hit");
            TakeDamage(5);
        }
    }
    public void TakeDamage(int damage)
    {
        if(shieldActive)
        {
            shield -= damage;
            StartCoroutine(flash.PlayerFlicker());
            if (shield <= 0)
            {
                shield = 0;
                shieldActive = false;
                shieldBarObj.SetActive(false);
            }
            UpdateShieldBar();
            return;
        }
        else
        {
            currentHealth -= damage;
            StartCoroutine(flash.PlayerFlicker());
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                StartCoroutine(Die());
            }
            UpdateHealthBar();
        }
    }

   public void Heal(int healAmount)
   {
        Debug.Log("Player healed for " + healAmount + " health.");
        currentHealth += healAmount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        UpdateHealthBar();
    }

    private IEnumerator Die()
    {
        UpdateHealthBar();
        animator.SetTrigger("Die");
        isDied = true;
        Debug.Log("Player has died.");
        SetPlayerActive(false);
        isMovementBlocked = true;
        yield return new WaitForSeconds(2f);
        StartCoroutine(GameManager.Instance.InitializeGameWithDelay());
        yield return new WaitForSeconds(2f);
        animator.ResetTrigger("Die");
        animator.SetTrigger("Respawn");
        currentHealth = maxHealth;
        isDied = false;
        isMovementBlocked = false;
        enemyCounter.Reset();
        UpdateHealthBar();
        SetPlayerActive(true);

    }
    private void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.fillAmount = (float)currentHealth / maxHealth;
        }
    }

    public void ActivateShield()
    {
        Debug.Log("Player shield activated.");
        shieldBarObj.SetActive(true);
        shieldActive = true;
        shield = 150;
        UpdateShieldBar();
    }

    private void UpdateShieldBar()
    {
        if (shieldBar != null)
        {
            shieldBar.fillAmount = (float)shield / 150;
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