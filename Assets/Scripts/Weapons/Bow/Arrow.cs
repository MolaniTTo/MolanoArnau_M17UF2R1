using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;

    private void Update()
    {
        MoveArrow();
    }

    private void MoveArrow()
    {
        transform.Translate(Vector3.right * Time.deltaTime * moveSpeed);
    }

}




