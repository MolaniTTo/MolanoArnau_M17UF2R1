using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Inventory/Weapon")]

public class WeaponData : ItemData
{
    public int damage;
    public float attackSpeed;
}

