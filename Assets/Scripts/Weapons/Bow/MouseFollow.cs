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
        Vector3 mousePos = Mouse.current.position.ReadValue(); //Agafem la posició del ratolí
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);//Convertim la posició del ratolí a la posició del món

        Vector2 direction = transform.position - mousePos; //Calculem la direcció de la mira

        transform.right = -direction; //Apuntem la mira cap a la direcció del ratolí


    }
}


