using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class FlameThrower : MonoBehaviour, IWeapon
{
    [SerializeField] private WeaponSO weaponSO;
    private ParticleSystem flameThrowerParticles;
    private PolygonCollider2D flameThrowerCollider;
    private WeaponAudio weaponAudio;

    bool isAttacking;
    

    private void Awake()
    {
        flameThrowerParticles = GetComponentInChildren<ParticleSystem>();
        flameThrowerParticles.Stop();

        flameThrowerCollider = GetComponentInChildren<PolygonCollider2D>();
        weaponAudio = GetComponent<WeaponAudio>();
        if(flameThrowerCollider != null)
        {
            flameThrowerCollider.enabled = false;
        }
    }

    public void Update()
    {
        if (!isAttacking && flameThrowerParticles.isPlaying) //si no estem atacant i les particules estan activades
        {
            flameThrowerParticles.Stop();
            weaponAudio.StopSound();
            if(flameThrowerCollider != null)
            {
                flameThrowerCollider.enabled = false;
            }
            
        }
    }

    public void Attack()
    {
        isAttacking = true;
        if(!flameThrowerParticles.isPlaying)
        {
            flameThrowerParticles.Play();
            weaponAudio.StartSound();
            if(flameThrowerCollider != null)
            {
                flameThrowerCollider.enabled = true;//activem el collider que esta com a fill del flameThrower
            }
            
        } 
    }

    public void StopAttacking()
    {
        isAttacking = false;
    }

    public WeaponSO GetWeaponSO()
    {
        return weaponSO;
    }

    public void SetWeaponCooldown(float newCooldown)
    {
        Debug.Log("No implementat amb aquesta arma");
    }
}
