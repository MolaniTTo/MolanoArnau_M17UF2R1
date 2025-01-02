using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Latigo : MonoBehaviour
{
    private Transform player;
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Latigo ha golpeado al jugador");
            collision.gameObject.GetComponent<PlayerController>().TakeDamage(10);
            KnockBack knockBack = player.GetComponent<KnockBack>();
            if (knockBack != null)
            {
                Vector2 knockBackDirection = (player.position - transform.position).normalized;
                knockBack.ApplyKnockBack(knockBackDirection); // Empujar al jugador
            }
        }
    }
}
