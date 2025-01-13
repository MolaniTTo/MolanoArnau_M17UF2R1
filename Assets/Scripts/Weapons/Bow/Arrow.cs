using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float maxDistance = 20f;
    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        MoveArrow();
        CheckMaxDistance();
    }

    private void MoveArrow() //Movem la fletxa
    {
        transform.Translate(Vector3.right * Time.deltaTime * moveSpeed);
    }

    private void CheckMaxDistance() //Comprovem si la fletxa ha recorregut la distància màxima
    {
        float distanceTraveled = Vector3.Distance(startPosition, transform.position);
        if(distanceTraveled >= maxDistance)
        {
            Destroy(gameObject); //aqui no he fet una pool, ho he fet amb el torreta
        }
    }


    private void OnTriggerEnter2D(Collider2D collision) //quan la fletxa colisiona amb un enemic, la destruim
    {
        if(collision.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }
}




