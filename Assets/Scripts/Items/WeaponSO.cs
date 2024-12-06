using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Inventory/Weapon")]

public class WeaponSO : ScriptableObject
{
    public GameObject weaponPrefab;
    public float weaponColldown;
}

