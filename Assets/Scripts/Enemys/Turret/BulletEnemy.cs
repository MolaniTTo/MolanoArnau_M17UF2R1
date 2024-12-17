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

    private Vector3 startPosition;
    private Vector2 moveDirection;
    
    void Start()
    {
        startPosition = transform.position; 
    }

    void Update()
    {
        transform.Translate(moveDirection * speed * Time.deltaTime);

        if(Vector3.Distance(startPosition, transform.position) > maxDistance)
        {
            Explode();
        }
       
    }

    public void SetDirection(Vector2 direction)
    {
        moveDirection = direction.normalized;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Obstacle"))
        {
            Explode();
        }
    }


    void Explode()
    {
        if(explosionEffect != null)
        {
            GameObject explosionInstance = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            Destroy(explosionInstance, 1.0f);
        }

        Destroy(gameObject);
    }
}
