using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
public class Sword : MonoBehaviour, IWeapon
{
    [SerializeField] private GameObject slashAnimPrefab;
    [SerializeField] private Transform slashAnimSpawnPoint;
    [SerializeField] private WeaponSO weaponSO;

    private Transform weaponCollider;
    private Animator animator;
    private bool isAttaking;

    private GameObject slashAnim;

    private float currentCooldown;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        currentCooldown = weaponSO.weaponCooldown;
    }

    private void Start()
    {
        weaponCollider = PlayerController.Instance.GetWeaponCollider();
        slashAnimSpawnPoint = PlayerController.Instance.GetSlashAnimSpawnPoint();
        if (slashAnimSpawnPoint == null)
        {
            return;
        }

        if (weaponCollider == null)
        {
            return;
        }
    }

    private void Update()
    { 
        MouseFollowWithOffset(); //Aquesta funció fa que l'arma segueixi el ratolí amb un offset
    }

    public WeaponSO GetWeaponSO()
    {
        return weaponSO;
    }

    public void SetWeaponCooldown(float newCooldown) //de la tenda
    {
        currentCooldown = newCooldown;
        weaponSO.weaponCooldown = newCooldown;
    }

    public void Attack()
    {
        animator.SetTrigger("Attack");
        weaponCollider.gameObject.SetActive(true);
        slashAnim = Instantiate(slashAnimPrefab, slashAnimSpawnPoint.position, Quaternion.identity); //Instanciem l'animació de l'espasa
        slashAnim.transform.parent = this.transform.parent;
    }


    public void DoneAttackingAnimEvent() //quan acaba la animacio
    {
        weaponCollider.gameObject.SetActive(false);
    }

    public void SwingUpFlipAnimEvent() //quan la animació de l'espasa fa un swing cap amunt
    {
        slashAnim.gameObject.transform.rotation = Quaternion.Euler(-180, 0, 0);

        if (PlayerController.Instance.FacingLeft)
        {
            slashAnim.GetComponent<SpriteRenderer>().flipX = true; //flipem l'animació si el player esta mirant a l'esquerra
        }
    }

    public void SwingDownFlipAnimEvent() //quan la animació de l'espasa fa un swing cap avall
    {
        slashAnim.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);

        if (PlayerController.Instance.FacingLeft)
        {
            slashAnim.GetComponent<SpriteRenderer>().flipX = true; //flipem l'animació si el player esta mirant a l'esquerra
        }
    }

    private void MouseFollowWithOffset() //Aquesta funció fa que l'arma segueixi el ratolí amb un offset
    {
        if (isAttaking) return;
        Vector3 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 playerScreenPoint = Camera.main.WorldToScreenPoint(PlayerController.Instance.transform.position);    

        float angle = Mathf.Atan2(mouseScreenPos.y, mouseScreenPos.x) * Mathf.Rad2Deg;

        if (mouseScreenPos.x < playerScreenPoint.x) //si el ratolí esta a l'esquerra del player
        {
            ActiveWeapon.Instance.transform.rotation = Quaternion.Euler(0, -180, angle); //flipem l'arma
            weaponCollider.transform.rotation = Quaternion.Euler(0, -180, 0);
        }
        else
        {
            ActiveWeapon.Instance.transform.rotation = Quaternion.Euler(0, 0, angle); //no flipem l'arma
            weaponCollider.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }


}
