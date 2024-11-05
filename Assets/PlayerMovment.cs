using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovment : MonoBehaviour
{
    private float _speed = 3f;

    private Rigidbody2D _rbPlayer;
    private Vector2 movmentImput;
    private Animator _animatorPlayer;

    void Start()
    {
        _rbPlayer = GetComponent<Rigidbody2D>();
        _animatorPlayer = GetComponent<Animator>();
    }

    private void Update()
    {
        float movmentX = Input.GetAxisRaw("Horizontal");
        float movmentY = Input.GetAxisRaw("Vertical");
        movmentImput = new Vector2(movmentX, movmentY).normalized;
        _animatorPlayer.SetFloat("Horizontal", movmentX); 
        _animatorPlayer.SetFloat("Vertical", movmentY);
        _animatorPlayer.SetFloat("Speed", movmentImput.sqrMagnitude); //ho poso aixi perq sino ha de fer el calcul al quadrat del vector
    }

    private void FixedUpdate()
    {
        _rbPlayer.MovePosition(_rbPlayer.position + movmentImput * _speed * Time.fixedDeltaTime);
    }



}
