using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Latigo : MonoBehaviour
{
    private Transform player;
    PlayerController playerController;
    private Animator animator;
    public float detectionRadious = 5f;  // Radio de detección para atacar

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerController = player.GetComponent<PlayerController>();
        animator = GetComponentInParent<Animator>();
    }

    private void Update()
    {
        if (playerController == null || playerController.isPlayerActive == false) return;
        if(Vector2.Distance(transform.position, player.position) <= detectionRadious)
        {
            Attack();
        }

    }

    private void Attack()
    {
        animator.SetTrigger("Attack");
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
