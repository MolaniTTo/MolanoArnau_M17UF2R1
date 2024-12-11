using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameThrower : MonoBehaviour, IWeapon
{
    [SerializeField] private WeaponSO weaponSO;
    public void Attack()
    {
        Debug.Log("FlameThrower Attack");
    }

    public WeaponSO GetWeaponSO()
    {
        return weaponSO;
    }
}
