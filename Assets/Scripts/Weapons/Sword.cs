using UnityEngine;
using UnityEngine.InputSystem;
public class Sword : MonoBehaviour , PlayerInputActions.ICombatActions
{
    [SerializeField] private GameObject slashAnimPrefab;
    [SerializeField] private Transform slashAnimSpawnPoint;
    [SerializeField] private Transform weaponCollider;


    private PlayerInputActions ic; // Referencia al Input Action
    private Animator animator;
    private PlayerController playerController;
    private ActiveWeapon activeWeapon;
    private Vector2 mousePosition;
    bool isAttacking = false;

    private GameObject slashAnim;

    private void Awake()
    {
        mousePosition = Vector2.zero;
        animator = GetComponent<Animator>();
        ic = new PlayerInputActions();
        ic.Combat.SetCallbacks(this);
    }



    private void OnEnable()
    {
        ic.Enable();
    }
    
    private void OnDisable()
    {
        ic.Disable();
    }

    private void Start()
    {
        playerController = GetComponentInParent<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("No se ha encontrado el componente PlayerController");
        }
        activeWeapon = GetComponentInParent<ActiveWeapon>();
        if (activeWeapon == null)
        {
            Debug.LogError("No se ha encontrado el componente ActiveWeapon");
        }
    }
    private void Update()
    {
        MouseFollowWithOffset();
    }

    private void Attack()
    {
        if (isAttacking) return;
        isAttacking = true;
        animator.SetTrigger("Attack");
        weaponCollider.gameObject.SetActive(true);
        slashAnim = Instantiate(slashAnimPrefab, slashAnimSpawnPoint.position, Quaternion.identity);
        slashAnim.transform.parent = this.transform.parent;
    }

    public void DoneAttackingAnimEvent()
    {
        weaponCollider.gameObject.SetActive(false);
        Destroy(slashAnim);
        isAttacking = false;

    }

    public void SwingUpFlipAnimEvent()
    {
        slashAnim.gameObject.transform.rotation = Quaternion.Euler(-180, 0, 0);

        if(playerController.facingLeft)
        {
            slashAnim.GetComponent<SpriteRenderer>().flipX = true;
        }
    }

    public void SwingDownFlipAnimEvent()
    {
        slashAnim.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);

        if(playerController.facingLeft)
        {
            slashAnim.GetComponent<SpriteRenderer>().flipX = true; 
        }
    }

    private void MouseFollowWithOffset()
    {
        Vector3 playerScreenPoint = Camera.main.WorldToScreenPoint(playerController.transform.position);
        float angle = Mathf.Atan2(mousePosition.y, mousePosition.x) * Mathf.Rad2Deg;

        if(mousePosition.x < playerScreenPoint.x)
        {
            activeWeapon.transform.rotation = (Quaternion.Euler(0, 180, angle));
            weaponCollider.transform.rotation = (Quaternion.Euler(0, 180, 0));
        }
        else
        {
            activeWeapon.transform.rotation = (Quaternion.Euler(0, 0, angle));
            weaponCollider.transform.rotation = (Quaternion.Euler(0, 0, 0));
        }

    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            Attack();
        }
    }

    public void OnLookCombat(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            mousePosition = context.ReadValue<Vector2>();
        }
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        ((PlayerInputActions.ICombatActions)playerController).OnDash(context);
    }
}
