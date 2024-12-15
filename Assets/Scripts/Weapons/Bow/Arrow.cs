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

    private void MoveArrow()
    {
        transform.Translate(Vector3.right * Time.deltaTime * moveSpeed);
    }

    private void CheckMaxDistance()
    {
        float distanceTraveled = Vector3.Distance(startPosition, transform.position);
        if(distanceTraveled >= maxDistance)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }
}




