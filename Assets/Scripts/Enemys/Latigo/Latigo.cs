using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Latigo : MonoBehaviour //aquest enemic ni el demanaves ni re pero em vaig enchichar pq em molava molt el sprite
{
    private Transform player;
    PlayerController playerController;
    private Animator animator;
    public float detectionRadious = 5f; //radi de detecció per a l'atac

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerController = player.GetComponent<PlayerController>();
        animator = GetComponentInParent<Animator>();
    }

    private void Update()
    {
        if (playerController == null || playerController.isPlayerActive == false) return; //verifiquem si el player està actiu
        if(Vector2.Distance(transform.position, player.position) <= detectionRadious)
        {
            Attack(); //ataquem si el player està dins del radi de detecció
        }

    }

    private void Attack()
    {
        animator.SetTrigger("Attack");
    }


    public void OnCollisionEnter2D(Collision2D collision) //quan el latigo colisiona amb el jugador li fa mal
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Latigo ha golpeado al jugador");
            collision.gameObject.GetComponent<PlayerController>().TakeDamage(10);
            KnockBack knockBack = player.GetComponent<KnockBack>();
            if (knockBack != null)
            {
                Vector2 knockBackDirection = (player.position - transform.position).normalized;
                knockBack.ApplyKnockBack(knockBackDirection); //Emputxem al jugador
            }
        }
    }
}
