using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public BulletPool bulletPool;
    public Transform[] firePoints;
    private Transform player;
    public float minDistance = 15f;
    public Animator myAnimator;
    PlayerController playerController;


    [System.Serializable]
    public class ShootingConfiguration
    {
        public float minAngle;         //angle mínim
        public float maxAngle;         //angle maxim
        public int[] bulletIndices;    //index de la bala
        public Vector2[] directions;  //direccions de les bales
        public int[] firePointIndices; //index dels firepoints
    }

    public List<ShootingConfiguration> shootingConfigurations; //llista de configuracions de dispar

    private void Start()
    {
        myAnimator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerController = player.GetComponent<PlayerController>();
    }

    public void IsInArea()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        if (Vector2.Distance(transform.position, player.position) < minDistance)
        {
            myAnimator.SetTrigger("Attack");
        }
    }

    public void Shoot()
    {
        if (playerController == null || playerController.isPlayerActive == false) return; //verifiquem si el player està actiu
        //calcular la direcció del jugador
        Vector2 direction = player.position - transform.position;

        //calcular l'angle entre la direcció de la torreta (transform.up) i la direcció cap al jugador
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        if (angle < 0)
        {
            angle += 360f; //si l'angle és negatiu, sumem 360 per a que sigui positiu
        }

        //busquem la configuració de dispar adequada segons l'angle
        foreach (var config in shootingConfigurations)
        {
            if (angle > config.minAngle && angle <= config.maxAngle)
            {
                //dispars segons la configuració
                for (int i = 0; i < config.bulletIndices.Length; i++)
                {
                    int bulletIndex = config.bulletIndices[i];
                    int firePointIndex = config.firePointIndices[i];
                    Vector2 bulletDirection = config.directions[i];

                    GameObject bullet = bulletPool.GetBullet(bulletIndex);
                    bullet.transform.position = firePoints[firePointIndex].position;
                    bullet.GetComponent<BulletEnemy>().SetDirection(bulletDirection);
                    bullet.GetComponent<BulletEnemy>().InitializeBullet(bulletIndex);
                }
                break; //sortim del bucle
            }
        }
    }

    /*public void Shoot() //aixi de pocho ho havia fet al principi, feia temps q no feia tants else if xd
    {
        // Calcular la dirección del jugador
        Vector2 direction = player.position - transform.position;

        // Calcular el ángulo entre la dirección de la torreta (transform.up) y la dirección hacia el jugador
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        if (angle < 0)
        {
            angle += 360f;
        }

        GameObject bullet1 = null;
        GameObject bullet2 = null;

        if (angle > 22.5f && angle < 67.5f)
        {
            bullet1 = bulletPool.GetBullet(0);  // Tipo 0 de bala
            bullet1.transform.position = firePoints[0].position;
            bullet1.GetComponent<BulletEnemy>().SetDirection(new Vector2(1, 1));
            bullet1.GetComponent<BulletEnemy>().InitializeBullet(0);  // Asignamos el índice
        }

        else if (angle > 67.5f && angle < 112.5f)
        {
            bullet1 = bulletPool.GetBullet(0);  // Tipo 0 de bala
            bullet1.transform.position = firePoints[0].position;
            bullet1.GetComponent<BulletEnemy>().SetDirection(new Vector2(0, 1));
            bullet1.GetComponent<BulletEnemy>().InitializeBullet(0);  // Asignamos el índice

            bullet2 = bulletPool.GetBullet(1);  // Tipo 1 de bala
            bullet2.transform.position = firePoints[1].position;
            bullet2.GetComponent<BulletEnemy>().SetDirection(new Vector2(0, 1));
            bullet2.GetComponent<BulletEnemy>().InitializeBullet(1);  // Asignamos el índice
        }

        else if (angle > 112.5f && angle < 157.5f)
        {
            bullet1 = bulletPool.GetBullet(1);  // Tipo 0 de bala
            bullet1.transform.position = firePoints[1].position;
            bullet1.GetComponent<BulletEnemy>().SetDirection(new Vector2(-1, 1));
            bullet1.GetComponent<BulletEnemy>().InitializeBullet(1);  // Asignamos el índice
        }

        else if (angle > 157.5f && angle < 202.5f)
        {
            bullet1 = bulletPool.GetBullet(1);  // Tipo 0 de bala
            bullet1.transform.position = firePoints[1].position;
            bullet1.GetComponent<BulletEnemy>().SetDirection(new Vector2(-1, 0));
            bullet1.GetComponent<BulletEnemy>().InitializeBullet(1);  // Asignamos el índice

            bullet2 = bulletPool.GetBullet(2);  // Tipo 1 de bala
            bullet2.transform.position = firePoints[2].position;
            bullet2.GetComponent<BulletEnemy>().SetDirection(new Vector2(-1, 0));
            bullet2.GetComponent<BulletEnemy>().InitializeBullet(2);  // Asignamos el índice
        }

        else if (angle > 202.5f && angle < 247.5f)
        {
            bullet1 = bulletPool.GetBullet(2);  // Tipo 0 de bala
            bullet1.transform.position = firePoints[2].position;
            bullet1.GetComponent<BulletEnemy>().SetDirection(new Vector2(-1, -1));
            bullet1.GetComponent<BulletEnemy>().InitializeBullet(2);  // Asignamos el índice
        }

        else if (angle > 247.5f && angle < 292.5f)
        {
            bullet1 = bulletPool.GetBullet(2);  // Tipo 0 de bala
            bullet1.transform.position = firePoints[2].position;
            bullet1.GetComponent<BulletEnemy>().SetDirection(new Vector2(0, -1));
            bullet1.GetComponent<BulletEnemy>().InitializeBullet(2);  // Asignamos el índice

            bullet2 = bulletPool.GetBullet(3);  // Tipo 1 de bala
            bullet2.transform.position = firePoints[3].position;
            bullet2.GetComponent<BulletEnemy>().SetDirection(new Vector2(0, -1));
            bullet2.GetComponent<BulletEnemy>().InitializeBullet(3);  // Asignamos el índice
        }

        else if (angle > 292.5f && angle < 337.5f)
        {
            bullet1 = bulletPool.GetBullet(3);  // Tipo 0 de bala
            bullet1.transform.position = firePoints[3].position;
            bullet1.GetComponent<BulletEnemy>().SetDirection(new Vector2(1, -1));
            bullet1.GetComponent<BulletEnemy>().InitializeBullet(3);  // Asignamos el índice
        }

        else
        {
            bullet1 = bulletPool.GetBullet(3);  // Tipo 0 de bala
            bullet1.transform.position = firePoints[3].position;
            bullet1.GetComponent<BulletEnemy>().SetDirection(new Vector2(1, 0));
            bullet1.GetComponent<BulletEnemy>().InitializeBullet(3);  // Asignamos el índice

            bullet2 = bulletPool.GetBullet(0);  // Tipo 1 de bala
            bullet2.transform.position = firePoints[0].position;
            bullet2.GetComponent<BulletEnemy>().SetDirection(new Vector2(1, 0));
            bullet2.GetComponent<BulletEnemy>().InitializeBullet(0);  // Asignamos el índice
        }

    }*/

}
