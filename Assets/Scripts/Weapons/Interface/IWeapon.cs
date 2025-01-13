using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeapon
{   
    public void Attack(); //Mètode per atacar
    public WeaponSO GetWeaponSO(); //Mètode per obtenir l'objecte Scriptable Object de l'arma

    public void SetWeaponCooldown(float newCooldown); //Mètode per canviar el cooldown de l'arma

}
