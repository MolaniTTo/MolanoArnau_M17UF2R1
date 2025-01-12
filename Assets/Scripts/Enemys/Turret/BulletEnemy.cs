using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class BulletEnemy : MonoBehaviour
{
    public float speed = 5f;
    public float maxDistance = 10f;
    public GameObject explosionEffect;
    private Transform player;
    private Vector3 startPosition;
    private Vector2 moveDirection;
    private BulletPool bulletPool;

    private int bulletIndex;  // Almacena el índice de la bala

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        startPosition = transform.position;
        bulletPool = GameObject.FindObjectOfType<BulletPool>();
    }

    void Update()
    {
        transform.Translate(moveDirection * speed * Time.deltaTime);
        if(Vector3.Distance(startPosition, transform.position) > maxDistance)
        {
            ReturnToPool();
        }
       
    }

    public void SetDirection(Vector2 direction)
    {
        moveDirection = direction.normalized;
    }

    public void SetBulletIndex(int index)
    {
        bulletIndex = index;
    }



    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            ReturnToPool();
        }
        if(collision.gameObject.CompareTag("Player"))
        {
            
            collision.gameObject.GetComponent<PlayerController>().TakeDamage(5);
            KnockBack knockBack = player.GetComponent<KnockBack>();
            if (knockBack != null)
            {
                Vector2 knockBackDirection = (player.position - transform.position).normalized;
                knockBack.ApplyKnockBack(knockBackDirection); // Empujar al jugador
            }
            ReturnToPool();
        }
    }


    void ReturnToPool()
    {
        if (explosionEffect != null)
        {
            GameObject explosionInstance = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            Destroy(explosionInstance, 1.5f);
        }

        // Devolver la bala al pool usando el índice correspondiente
        bulletPool.ReturnBullet(bulletIndex, gameObject);
        gameObject.SetActive(false);
    }

    // Cuando obtengas la bala desde el pool, asegúrate de pasarle el índice correcto
    public void InitializeBullet(int index)
    {
        bulletIndex = index;  // Guarda el índice de la bala al ser inicializada
    }
}
