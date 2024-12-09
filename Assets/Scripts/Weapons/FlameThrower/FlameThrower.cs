using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameThrower : MonoBehaviour, IWeapon
{
    public void Attack()
    {
        Debug.Log("FlameThrower Attack");
        ActiveWeapon.Instance.ToggleIsAttacking(false);
    }
}
