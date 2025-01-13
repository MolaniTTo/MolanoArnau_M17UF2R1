using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseFollow : MonoBehaviour
{
    private void Update()
    {
        FaceMouse();
    }

    private void FaceMouse()
    {
        Vector3 mousePos = Mouse.current.position.ReadValue(); //Agafem la posici� del ratol�
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);//Convertim la posici� del ratol� a la posici� del m�n

        Vector2 direction = transform.position - mousePos; //Calculem la direcci� de la mira

        transform.right = -direction; //Apuntem la mira cap a la direcci� del ratol�


    }
}


