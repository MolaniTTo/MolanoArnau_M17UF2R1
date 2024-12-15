using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class FlameThrower : MonoBehaviour, IWeapon
{
    [SerializeField] private WeaponSO weaponSO;
    private ParticleSystem flameThrowerParticles;
    private PolygonCollider2D flameThrowerCollider;

    bool isAttacking;
    

    private void Awake()
    {
        flameThrowerParticles = GetComponentInChildren<ParticleSystem>();
        flameThrowerParticles.Stop();

        flameThrowerCollider = GetComponentInChildren<PolygonCollider2D>();
        if(flameThrowerCollider != null)
        {
            flameThrowerCollider.enabled = false;
        }
        else
        {
            Debug.LogError("FlameThrower Collider not found");
        }
    }

    public void Update()
    {
        if (!isAttacking && flameThrowerParticles.isPlaying)
        {
            flameThrowerParticles.Stop();
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
            if(flameThrowerCollider != null)
            {
                flameThrowerCollider.enabled = true;
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
}
