using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveWeapon : Singleton<ActiveWeapon>
{
    public MonoBehaviour CurrentActiveWeapon { get; private set; }

    private PlayerInputActions playerInputActions;

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
    }

    private void Update()
    {
        Attack();
    }

    public void NewWeapon(MonoBehaviour newWeapon)
    {
        CurrentActiveWeapon = newWeapon;
    }

    public void WeaponNull()
    {
        CurrentActiveWeapon = null;
    }

    public void ToggleIsAttacking(bool value)
    {
        isAttacking = value;
    }

    private void StartAttacking()
    {
        attackButtonDown = true;
    }

    private void StopAttacking()
    {
        attackButtonDown = false;
    }

    private void Attack()
    {
        if(attackButtonDown && !isAttacking)
        {
            isAttacking = true;
            if(CurrentActiveWeapon == null)
            {
                Debug.LogError("No weapon is assigned to the player!");
                return;
            }
            (CurrentActiveWeapon as IWeapon)?.Attack();
        }
    }
}
