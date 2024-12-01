using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraConfinerManager : MonoBehaviour
{
    public CinemachineConfiner2D confiner;

    public void UpdateConfiner(PolygonCollider2D roomConfiner)
    {
        if (confiner == null)
        {
            Debug.LogError("No se ha asignado un CinemachineConfiner.");
            return;
        }

        if (roomConfiner == null)
        {
            Debug.LogError("No se ha asignado un PolygonCollider2D.");
            return;
        }

        // Asignar null y luego el nuevo confiner
        confiner.m_BoundingShape2D = null;
        confiner.m_BoundingShape2D = roomConfiner;

        Debug.Log("Confiner de la cámara actualizado.");
    }
}