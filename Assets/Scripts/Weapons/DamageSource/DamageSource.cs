using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageSource : MonoBehaviour
{
    [SerializeField] private float damageAmount = 1;
    [SerializeField] private float flameThrowerDamageInterval = 0.1f;
    [SerializeField] private string weaponName = "Default";
    private List<EnemyHealth> enemiesInRange = new List<EnemyHealth>();
    private Coroutine flameThrowerCoroutine;
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
        if(collision.gameObject.GetComponent<EnemyHealth>() && weaponName == "FlameThrower")
        {
            EnemyHealth enemyHealth = collision.gameObject.GetComponent<EnemyHealth>();
            if(!enemiesInRange.Contains(enemyHealth))
            {
                enemiesInRange.Add(enemyHealth);
            }
            if(flameThrowerCoroutine == null)
            {
                flameThrowerCoroutine = StartCoroutine(FlameThrowerDamage());
            }
           
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.GetComponent<EnemyHealth>() && weaponName == "FlameThrower")
        {
            EnemyHealth enemyHealth = collision.gameObject.GetComponent<EnemyHealth>();
            if(enemiesInRange.Contains(enemyHealth))
            {
                enemiesInRange.Remove(enemyHealth);
            }
            if(enemiesInRange.Count == 0 && flameThrowerCoroutine != null)
            {
                StopCoroutine(flameThrowerCoroutine);
                flameThrowerCoroutine = null;
            }
        }
        
    }
    private IEnumerator FlameThrowerDamage()
    {
        while(enemiesInRange.Count > 0)
        {
            foreach(EnemyHealth enemy in enemiesInRange)
            {
                if(enemy != null)
                {
                    enemy.TakeDamage(damageAmount);
                }
            }
            yield return new WaitForSeconds(flameThrowerDamageInterval);
        }

        flameThrowerCoroutine = null;
    }

    public void IncreaseDamage(float additionalDamage) //Per la tenda
    {
        damageAmount += additionalDamage;
        Debug.Log("Damage of " + weaponName + " increased to " + damageAmount);
    }



}
