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
        if(rb == null)
        {
            Debug.LogError("Rigidbody2D not found");
        }
    }

    public void ApplyKnockBack(Vector2 direction)
    {
        if(!isKnockedBack)
        {
            isKnockedBack = true;
            rb.velocity = Vector2.zero;
            rb.AddForce(direction.normalized * knockBackForce, ForceMode2D.Impulse);
            StartCoroutine(EndKnockBack());
        }
    }

    private IEnumerator EndKnockBack()
    {
        yield return new WaitForSeconds(knockBachDuration);
        rb.velocity = Vector2.zero; //Detenem la empenta
        isKnockedBack = false;
    }
}
