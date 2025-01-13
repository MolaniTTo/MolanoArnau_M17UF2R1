using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraConfinerManager : MonoBehaviour
{
    public CinemachineConfiner2D confiner; // Referencia al CinemachineConfiner

    public void UpdateConfiner(PolygonCollider2D roomConfiner) //per actualitzar el confiner de la càmera
    {
        if (confiner == null)
        {
            return;
        }

        if (roomConfiner == null)
        {
            return;
        }

        
        confiner.m_BoundingShape2D = null; // Eliminem el confiner actual i assignem el nou
        confiner.m_BoundingShape2D = roomConfiner;
    }
}