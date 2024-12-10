using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
public class Sword : MonoBehaviour, IWeapon
{
    [SerializeField] private GameObject slashAnimPrefab;
    [SerializeField] private Transform slashAnimSpawnPoint;
    [SerializeField] private float swordAttackCD = .5f;

    private Transform weaponCollider;
    private Animator animator;

    private GameObject slashAnim;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        weaponCollider = PlayerController.Instance.GetWeaponCollider();
        slashAnimSpawnPoint = PlayerController.Instance.GetSlashAnimSpawnPoint();
        if (slashAnimSpawnPoint == null)
        {
            Debug.LogError("SlashSpawnPoint not found");
            return;
        }

        if (weaponCollider == null)
        {
            Debug.LogError("Weapon Collider not found");
            return;
        }
    }

    private void Update()
    { 
        MouseFollowWithOffset();
    }

    public void Attack()
    {

        //isAttacking = true;
        animator.SetTrigger("Attack");
        weaponCollider.gameObject.SetActive(true);
        slashAnim = Instantiate(slashAnimPrefab, slashAnimSpawnPoint.position, Quaternion.identity);
        slashAnim.transform.parent = this.transform.parent;
        StartCoroutine(AttackCDRoutine());
    }

    private IEnumerator AttackCDRoutine()
    {
        yield return new WaitForSeconds(swordAttackCD);
        ActiveWeapon.Instance.ToggleIsAttacking(false);
    }

    public void DoneAttackingAnimEvent()
    {
        weaponCollider.gameObject.SetActive(false);
    }


    public void SwingUpFlipAnimEvent()
    {
        slashAnim.gameObject.transform.rotation = Quaternion.Euler(-180, 0, 0);

        if (PlayerController.Instance.FacingLeft)
        {
            slashAnim.GetComponent<SpriteRenderer>().flipX = true;
        }
    }

    public void SwingDownFlipAnimEvent()
    {
        slashAnim.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);

        if (PlayerController.Instance.FacingLeft)
        {
            slashAnim.GetComponent<SpriteRenderer>().flipX = true;
        }
    }

    private void MouseFollowWithOffset()
    {
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector2 playerScreenPoint = Camera.main.WorldToScreenPoint(transform.position);

        float angle = Mathf.Atan2(mouseScreenPos.y, mouseScreenPos.x) * Mathf.Rad2Deg;

        if (mouseScreenPos.x < playerScreenPoint.x)
        {
            ActiveWeapon.Instance.transform.rotation = Quaternion.Euler(0, -180, angle);
            weaponCollider.transform.rotation = Quaternion.Euler(0, -180, 0);
        }
        else
        {
            ActiveWeapon.Instance.transform.rotation = Quaternion.Euler(0, 0, angle);
            weaponCollider.transform.rotation = Quaternion.Euler(0, 0, 0);
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
        throw new System.NotImplementedException();
    }
}
