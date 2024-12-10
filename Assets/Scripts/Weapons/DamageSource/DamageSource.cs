using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageSource : MonoBehaviour
{
    [SerializeField] private int damageAmount = 1;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.GetComponent<EnemyHealth>())
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

}
