using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveWeapon : Singleton<ActiveWeapon>
{
    public MonoBehaviour CurrentActiveWeapon { get; private set; }

    private PlayerInputActions playerInputActions;

    private float TimeBetweenAttacks;

    private bool attackButtonDown, isAttacking = false;

    protected override void Awake()
    {
        base.Awake();

        playerInputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        playerInputActions.Enable();
    }

    private void Start()
    {
        playerInputActions.Combat.Attack.started += _ => StartAttacking();
        playerInputActions.Combat.Attack.canceled += _ => StopAttacking();

        AttackCooldown();
    }

    private void Update()
    {
        Attack();
    }

    public void NewWeapon(MonoBehaviour newWeapon)
    {
        CurrentActiveWeapon = newWeapon;

        if(CurrentActiveWeapon is IWeapon weapon)
        {
            WeaponSO weaponSO = weapon.GetWeaponSO();
            AttackCooldown();
            TimeBetweenAttacks = weaponSO.weaponCooldown;
            UpdateActiveWeaponProperties();
        }

    }

    public void UpdateActiveWeaponProperties()
    {
        if (CurrentActiveWeapon is IWeapon weapon)
        {
            WeaponSO weaponSO = weapon.GetWeaponSO();

            // Actualizar el cooldown del arma activa
            weapon.SetWeaponCooldown(weaponSO.weaponCooldown);

            // Recalcular el tiempo entre ataques
            TimeBetweenAttacks = weaponSO.weaponCooldown;
        }
    }

    public void WeaponNull()
    {
        CurrentActiveWeapon = null;
    }

    public void AttackCooldown()
    {
        isAttacking = true;
        StopAllCoroutines();
        StartCoroutine(TimeBetAttacksR());
    }

    private IEnumerator TimeBetAttacksR()
    {
        yield return new WaitForSeconds(TimeBetweenAttacks);
        isAttacking = false;
    }


    private void StartAttacking()
    {
        attackButtonDown = true;
    }

    private void StopAttacking()
    {
        attackButtonDown = false;

        if (CurrentActiveWeapon is IWeapon weapon)
        {
            (CurrentActiveWeapon as FlameThrower)?.StopAttacking();
        }
    }

    private void Attack()
    {
        if(attackButtonDown && !isAttacking)
        {
            AttackCooldown();
            if(CurrentActiveWeapon == null)
            {
                Debug.LogError("No weapon is assigned to the player!");
                return;
            }
            (CurrentActiveWeapon as IWeapon)?.Attack();
        }
    }

    public WeaponSO GetCurrentWeaponSO()
    {
        if (CurrentActiveWeapon is IWeapon weapon)
        {
            return weapon.GetWeaponSO();
        }
        return null;
    }
}
