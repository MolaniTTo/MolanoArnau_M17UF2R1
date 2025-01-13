using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageSource : MonoBehaviour
{
    [SerializeField] private float damageAmount = 1;
    [SerializeField] private float flameThrowerDamageInterval = 0.1f;
    [SerializeField] private string weaponName = "Default";
    private List<EnemyHealth> enemiesInRange = new List<EnemyHealth>(); //enemics que estan dins del rang del flameThrower
    private Coroutine flameThrowerCoroutine;
    private void OnTriggerEnter2D(Collider2D collision) //si li fem mal a un enemic amb l'arma sword o arrow
    {
        if(collision.gameObject.GetComponent<EnemyHealth>() && (weaponName == "Sword" || weaponName == "Arrow")) 
        {
            EnemyHealth enemyHealth = collision.gameObject.GetComponent<EnemyHealth>();
            enemyHealth.TakeDamage(damageAmount); //els hi treiem vida

            KnockBack knockBack = collision.gameObject.GetComponent<KnockBack>();//la pijada esta q tirin enrere
            if(knockBack != null)
            {
                knockBack.ApplyKnockBack(collision.transform.position - transform.position);
            }
        }
        if(collision.gameObject.GetComponent<EnemyHealth>() && weaponName == "FlameThrower")//si colisiona amb un enemic amb el flameThrower
        {
            EnemyHealth enemyHealth = collision.gameObject.GetComponent<EnemyHealth>();
            if(!enemiesInRange.Contains(enemyHealth))
            {
                enemiesInRange.Add(enemyHealth);
            }
            if(flameThrowerCoroutine == null)
            {
                flameThrowerCoroutine = StartCoroutine(FlameThrowerDamage()); //comença a fer-li mal
            }
           
        }
    }

    private void OnTriggerExit2D(Collider2D collision) //quan l'enemic surt del rang del flameThrower
    {
        if(collision.gameObject.GetComponent<EnemyHealth>() && weaponName == "FlameThrower")
        {
            EnemyHealth enemyHealth = collision.gameObject.GetComponent<EnemyHealth>();
            if(enemiesInRange.Contains(enemyHealth))
            {
                enemiesInRange.Remove(enemyHealth); //l'enemic ja no esta dins del rang
            }
            if(enemiesInRange.Count == 0 && flameThrowerCoroutine != null)
            {
                StopCoroutine(flameThrowerCoroutine); //parem de fer-li mal
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
                    enemy.TakeDamage(damageAmount);//per cada enemic dins de l'array li fem mal
                }
            }
            yield return new WaitForSeconds(flameThrowerDamageInterval); //cada 0.1 segons, li poso un interval perq si ho feia amb el triggerStay anava fatal
        }

        flameThrowerCoroutine = null;
    }

    public void IncreaseDamage(float additionalDamage) //Per la tenda
    {
        damageAmount += additionalDamage;
    }



}
