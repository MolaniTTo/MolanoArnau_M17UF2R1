using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    private Animator animator;
    public bool isDied = false;

    public float live = 30;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if(isDied == false)
        {
            // Obtenir la direcció de moviment dels inputs de l'usuari amb normalized, pq si es vertical, el vector resultant seria més gran
            Vector2 moveDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

            // Moure el personatge en la direccio dels inputs
            transform.position += (Vector3)moveDirection * speed * Time.deltaTime;

            //si el clic dret del mouse esta presionat
            bool isRightClickPressed = Input.GetMouseButton(1);

            // Només miro la direccio del cursor per fer l'animació si faig clic dret, sino, es mou on marquen les tecles
            if (isRightClickPressed) //Si es presiona el clic dret (apuntar)
            {
                // Obtenir la posicio del ratoli dins del marc de la camara
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                // Resto el vector que es genera de la posicio del ratolí amb la posicio que té el personatge per treure el vector de on mirara el cursor
                Vector2 lookDirection = (mousePosition - transform.position).normalized;

                // Passar els parametres al Blend Tree en base a la direccio del cursor
                animator.SetFloat("AnimationDirectionX", lookDirection.x);
                animator.SetFloat("AnimationDirectionY", lookDirection.y);
            }

            //sino es controla default per les tecles WASD
            else if (moveDirection != Vector2.zero)
            {
                animator.SetFloat("AnimationDirectionX", moveDirection.x);
                animator.SetFloat("AnimationDirectionY", moveDirection.y);
            }

            // Establir la velocitat de l'animació en base a la magnitud del vector de moviment
            animator.SetFloat("Speed", moveDirection.magnitude);

        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            Debug.Log("Player hit");
           

            live -= 10;
            if (live <= 0)
            {
                animator.SetTrigger("Die");
                isDied = true;
            }
            Debug.Log(live);

        }
    }



}


/*using System.Collections;
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



}*/
