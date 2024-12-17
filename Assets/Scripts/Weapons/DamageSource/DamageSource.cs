using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageSource : MonoBehaviour
{
    [SerializeField] private float damageAmount = 1;
    [SerializeField] private string weaponName = "Default";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.GetComponent<EnemyHealth>() && (weaponName == "Sword" || weaponName == "Arrow"))
        {
            EnemyHealth enemyHealth = collision.gameObject.GetComponent<EnemyHealth>();
            enemyHealth.TakeDamage(damageAmount);

            KnockBack knockBack = collision.gameObject.GetComponent<KnockBack>();
            if(knockBack != null)
            {
                knockBack.ApplyKnockBack(collision.transform.position - transform.position);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<EnemyHealth>() && weaponName == "FlameThrower")
        {
            EnemyHealth enemyHealth = collision.gameObject.GetComponent<EnemyHealth>();
            enemyHealth.TakeDamage(damageAmount);
        }
    }

}
