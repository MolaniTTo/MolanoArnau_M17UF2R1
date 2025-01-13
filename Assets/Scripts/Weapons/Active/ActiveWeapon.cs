using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveWeapon : Singleton<ActiveWeapon>
{
    public MonoBehaviour CurrentActiveWeapon { get; private set; } // Arma activa

    private PlayerInputActions playerInputActions;

    private float TimeBetweenAttacks;

    private bool attackButtonDown, isAttacking = false;

    protected override void Awake() //com que és un singleton, el awake s'ha de cridar així
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

    public void NewWeapon(MonoBehaviour newWeapon) // Assignar un nou arma
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

    public void UpdateActiveWeaponProperties() // Actualitzar les propietats de l'arma activa
    {
        if (CurrentActiveWeapon is IWeapon weapon)
        {
            WeaponSO weaponSO = weapon.GetWeaponSO();

            //el cooldown de l'arma activa s'actualitza
            weapon.SetWeaponCooldown(weaponSO.weaponCooldown);

            //recalcula el temps entre atacs
            TimeBetweenAttacks = weaponSO.weaponCooldown;
        }
    }

    public void WeaponNull() // Eliminar l'arma activa
    {
        CurrentActiveWeapon = null;
    }

    public void AttackCooldown() // Cooldown entre atacs
    {
        isAttacking = true;
        StopAllCoroutines();
        StartCoroutine(TimeBetAttacksR());
    }

    private IEnumerator TimeBetAttacksR() //acabo de veure q se'm va colar una R al final del nom de la funció
    {
        yield return new WaitForSeconds(TimeBetweenAttacks);
        isAttacking = false;
    }


    private void StartAttacking()
    {
        attackButtonDown = true;
    }

    private void StopAttacking() // Deixar d'atacar
    {
        attackButtonDown = false;

        if (CurrentActiveWeapon is IWeapon weapon)
        {
            (CurrentActiveWeapon as FlameThrower)?.StopAttacking();
        }
    }

    private void Attack() // Atacar
    {
        if(attackButtonDown && !isAttacking)
        {
            AttackCooldown();
            if(CurrentActiveWeapon == null)
            {
                return;
            }
            (CurrentActiveWeapon as IWeapon)?.Attack();
        }
    }

    public WeaponSO GetCurrentWeaponSO() // Obtenir l'arma actual
    {
        if (CurrentActiveWeapon is IWeapon weapon)
        {
            return weapon.GetWeaponSO();
        }
        return null;
    }
}
