using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon")]

public class WeaponSO : ScriptableObject
{
    public GameObject weaponPrefab;
    public float weaponCooldown;
}
