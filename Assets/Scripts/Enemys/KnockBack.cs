using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockBack : MonoBehaviour
{
    [SerializeField] private float knockBackForce = 5f;
    [SerializeField] private float knockBachDuration = 0.2f;

    private Rigidbody2D rb;
    private bool isKnockedBack = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void ApplyKnockBack(Vector2 direction)
    {
        if(!isKnockedBack)
        {
            isKnockedBack = true;
            rb.velocity = Vector2.zero;
            rb.AddForce(direction.normalized * knockBackForce, ForceMode2D.Impulse); //Apliquem la força de knockback
            StartCoroutine(EndKnockBack());
        }
    }

    private IEnumerator EndKnockBack()
    {
        yield return new WaitForSeconds(knockBachDuration); //Esperem el temps de knockback
        rb.velocity = Vector2.zero; //Detenem l'empenta
        isKnockedBack = false;
    }
}
