using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ActiveWeapon : MonoBehaviour 
{

    [SerializeField] private MonoBehaviour currentActiveWeapon;

    private PlayerInputActions playerInputActions;

    private bool attackButtonDown, isAttacking = false;

    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
    }


    private void OnEnable()
    {
        playerInputActions.Enable();
    }

    private void Start()
    {
        playerInputActions.Combat.Attack.started += ctx => StartAttacking();
        playerInputActions.Combat.Attack.canceled += ctx => StopAttacking();
    }

    private void Update()
    {
        Attack();
    }

    public void ToggleIsAttacking(bool value) { isAttacking = value; }

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
        if (attackButtonDown && !isAttacking)
        {
            isAttacking = true;
            (currentActiveWeapon as IWeapon).Attack();
        }
    }

}

