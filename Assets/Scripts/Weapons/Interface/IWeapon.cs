using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeapon
{   
    public void Attack(); //M�tode per atacar
    public WeaponSO GetWeaponSO(); //M�tode per obtenir l'objecte Scriptable Object de l'arma

    public void SetWeaponCooldown(float newCooldown); //M�tode per canviar el cooldown de l'arma

}
