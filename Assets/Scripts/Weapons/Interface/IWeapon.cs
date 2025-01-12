using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeapon
{   
    public void Attack();
    public WeaponSO GetWeaponSO();

    public void SetWeaponCooldown(float newCooldown);

}
