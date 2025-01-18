using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour , PlayerInputActions.IPlayerActions , PlayerInputActions.ICombatActions
{
    public static PlayerController Instance { get; private set; } //Singleton
    public bool FacingLeft { get { return facingLeft; } } //per saber si el jugador mira a l'esquerra

    private PlayerInputActions ic; //referencia al inputactions
    private Animator animator; //animator
    private Flash flash; //flash
    private ScreenFade screenFade; //component de fade
    private WeaponAudio weaponAudio; //al weapon audio
    private MusicZone musicZone; //al music zone

    [SerializeField] private float dashSpeed = 9f;
    [SerializeField] private float speed = 5f;
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int shield = 150;
    private int currentHealth;

    [SerializeField] private Image healthBar;
    [SerializeField] private GameObject shieldBarObj;
    [SerializeField] private Image shieldBar;

    [SerializeField] private TrailRenderer mytrailRenderer; //trail renderer del dash
    [SerializeField] private Transform weaponCollider; //collider de l'arma sword
    [SerializeField] private Transform SlashAnimSpawnPoint;


    private Vector2 moveDirection; //direcció del moviment
    public bool isMovementBlocked { get; set; } = false;  //estat del moviment
    public Vector2 lookDirection { get; private set;}

    public bool isRightClickPressed { get; private set; }//estat del clic dret
    public bool isLeftClickPressed { get; private set; }  //estat del clic esquerre
    public bool facingLeft = false;
    public bool isDashing = false;
    public bool isDied = false;
    public bool isPlayerActive = false;
    public bool shieldActive = false;
    private bool isWalkingSoundPlaying = false;

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
        weaponAudio = GetComponent<WeaponAudio>();
        musicZone = FindObjectOfType<MusicZone>();

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

    public Transform GetWeaponCollider() //Retornem el collider de l'arma sword
    {
        return weaponCollider;
    }

    public Transform GetSlashAnimSpawnPoint() //el punt d'espawn del slash
    {
        return SlashAnimSpawnPoint;
    }

    public void OnMove(InputAction.CallbackContext context) //Callback del moviment
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

    public void OnLook(InputAction.CallbackContext context) //IMPORTANT!!! he posat aquesta funcio que nomes funcioni quan mantinguis el click dret (com si fos apuntar) perque no m'agradava que nomes mires al cursor per atacar, aixi que si ataques amb el click dret mantingut sempre seguira al cursor
    {

        if(context.performed) //lo de adal ho dic perq vegis q el requisit el cumpleixo pero ho he posat amb aquesta funcionalitat de mantenir com si fos apuntar
        {
            //calcular la direcció del cursor respecte el jugador
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

    public void OnRightClick(InputAction.CallbackContext context) //Callback del clic dret
    {
        isRightClickPressed = context.phase == InputActionPhase.Performed;
    }

    public void OnLeftClick(InputAction.CallbackContext context) //Callback del clic esquerre
    {
        isLeftClickPressed = context.phase == InputActionPhase.Performed;
        if (isLeftClickPressed)
        {
            sword?.Attack();
        }
    }

    private void MovePlayer()
    {
        if (isMovementBlocked) return;
        //aplica el moviment del jugador
        Vector3 movement = new Vector3(moveDirection.x, moveDirection.y, 0f);
        transform.position += movement * speed * Time.deltaTime;

        if(moveDirection != Vector2.zero)
        {
            if (!isWalkingSoundPlaying)
            {
                weaponAudio.PlayPlayerSound(weaponAudio.WalkSound);
                isWalkingSoundPlaying = true;
            }
        }
        else
        {
            if(isWalkingSoundPlaying)
            {
                weaponAudio.StopPlayerSound();
                isWalkingSoundPlaying = false;
            }
        }
    }
    private void HandleAnimations()
    {
        if (isRightClickPressed) //si es prem el clic dret
        {
            // Animacio es basa en la direcció del cursor
            animator.SetFloat("AnimationDirectionX", lookDirection.x);
            animator.SetFloat("AnimationDirectionY", lookDirection.y);
        }
        else if (moveDirection != Vector2.zero) //sino, la animacio es basa en la direcció del moviment
        {
            animator.SetFloat("AnimationDirectionX", moveDirection.x);
            animator.SetFloat("AnimationDirectionY", moveDirection.y);
        }

        //estableix la velocitat al blend tree
        animator.SetFloat("Speed", moveDirection.magnitude);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            TakeDamage(5); //si colisiona amb un enemic, el jugador rep mal
        }
    }
    public void TakeDamage(int damage) //funció per rebre dany
    {
       
        if (shieldActive && !isDied) //si tenim escut
        {
            weaponAudio.PlayPlayerSound(weaponAudio.Damage);
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
        else if(!isDied && !shieldActive) //si no tenim escut
        {
            weaponAudio.PlayPlayerSound(weaponAudio.Damage);
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

   public void Heal(int healAmount) //pocio de curació
   {
        currentHealth += healAmount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        UpdateHealthBar();
    }

    private IEnumerator Die() //funció per morir i carregar la escena de tornar a jugar o pa casa
    {
        if(isDied) yield break;
        if (musicZone != null)
        {
            StartCoroutine(musicZone.FadeOutMusic());
        }
        isDied = true;

        UpdateHealthBar();
        animator.SetTrigger("Die");
        weaponAudio.PlayPlayerSound(weaponAudio.Death);
        isMovementBlocked = true;
        yield return new WaitForSeconds(2f);
        screenFade.FadeOut();
        yield return new WaitForSeconds(2f);
        GameManager.Instance.ClearGame();
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("MainMenu");


    }
    private void UpdateHealthBar() //actualitzar la barra de vida
    {
        if (healthBar != null)
        {
            healthBar.fillAmount = (float)currentHealth / maxHealth;
        }
    }

    public void ActivateShield() //activar l'escut (ho crida la tenda)
    {
        if (shieldActive) return;
        shieldBarObj.SetActive(true);
        shieldActive = true;
        shield = 150;
        UpdateShieldBar();
    }

    private void UpdateShieldBar() //actualitzar la barra de l'escut
    {
        if (shieldBar != null)
        {
            shieldBar.fillAmount = (float)shield / 150;
        }
    }

    public void OnAttack(InputAction.CallbackContext context) //funcionalitats que vaig posar al mapa al principi i les vaig acabar fent de diferent manera
    {
        throw new System.NotImplementedException();
    }

    public void OnLookCombat(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }


    public void OnDash(InputAction.CallbackContext context) //Callback del dash
    {
        if(context.performed)
        {
            Dash();
        }
    }

    private void Dash() //funció del dash
    {
        if(!isDashing)
        {
            isDashing = true;
            weaponAudio.PlayPlayerSound(weaponAudio.DashSound);
            speed*= dashSpeed;
            mytrailRenderer.emitting = true;
            StartCoroutine(EndDashRoutine());
        } 
    }

    private IEnumerator EndDashRoutine() //rutina del dash
    {   
        float dashTime = 0.2f;
        float dashCD = 0.25f;
        yield return new WaitForSeconds(dashTime);
        speed /= dashSpeed;
        mytrailRenderer.emitting = false;
        yield return new WaitForSeconds(dashCD);
        isDashing = false;
        if(isWalkingSoundPlaying)
        {
            weaponAudio.StopPlayerSound();
            isWalkingSoundPlaying = false;
        }
    }
}