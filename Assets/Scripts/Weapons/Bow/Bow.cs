using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : MonoBehaviour, IWeapon
{
    [SerializeField] private WeaponSO weaponSO;
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform arrowSpawnPoint;

    private float currentCooldown;


    private Animator animator;

    private void Awake()
    {
        currentCooldown = weaponSO.weaponCooldown;
        animator = GetComponent<Animator>();
    }

    public void Attack()
    {
        animator.SetTrigger("Attack"); //Dispara l'animació de l'atac
        GameObject arrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, arrowSpawnPoint.rotation);

    }

    public WeaponSO GetWeaponSO()
    {
        return weaponSO;
    }

    public GameObject GetArrow()
    {
        return arrowPrefab;
    }

    public void SetWeaponCooldown(float newCooldown) //Aquesta funció la cridarem des de la shop per canviar el cooldown de l'arc
    {
        currentCooldown = newCooldown;
        weaponSO.weaponCooldown = newCooldown;
    }

}
