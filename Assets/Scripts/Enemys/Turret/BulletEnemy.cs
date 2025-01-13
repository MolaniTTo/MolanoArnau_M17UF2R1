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

    private int bulletIndex;  //emmagaatzemarà l'índex de la bala al ser inicialitzada

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        startPosition = transform.position;
        bulletPool = GameObject.FindObjectOfType<BulletPool>();
    }

    void Update()
    {
        transform.Translate(moveDirection * speed * Time.deltaTime); //com que es una bala en si que instanciare, ja poso q es mogui
        if(Vector3.Distance(startPosition, transform.position) > maxDistance)
        {
            ReturnToPool();//si la bala ha recorregut més de la distància màxima, la tornem al pool
        }
       
    }

    public void SetDirection(Vector2 direction) //per a que la bala es mogui en la direcció correcta
    {
        moveDirection = direction.normalized;
    }

    public void SetBulletIndex(int index) //li passem el índex de la bala al ser inicialitzada
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
                knockBack.ApplyKnockBack(knockBackDirection); //emputxem al player
            }
            ReturnToPool();
        }
    }


    void ReturnToPool() //funció per tornar la bala al pool
    {
        if (explosionEffect != null)
        {
            GameObject explosionInstance = Instantiate(explosionEffect, transform.position, Quaternion.identity); 
            Destroy(explosionInstance, 1.5f);
        }

        //retornem la bala al pool i la desactivem
        bulletPool.ReturnBullet(bulletIndex, gameObject);
        gameObject.SetActive(false);
    }

    //funció per inicialitzar la bala amb el índex de la bala
    public void InitializeBullet(int index)
    {
        bulletIndex = index;
    }
}
