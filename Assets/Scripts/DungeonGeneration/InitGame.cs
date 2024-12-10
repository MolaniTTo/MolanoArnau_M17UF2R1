using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InitGame : MonoBehaviour
{
    public GameObject playerPrefab;
    public CinemachineVirtualCamera virtualCamera;
    private CinemachineConfiner2D confiner;

    private void Awake()
    {
        confiner = FindObjectOfType<CinemachineConfiner2D>();
        confiner.m_BoundingShape2D = null;
    }


    public void StartGame()
    {
        if (playerPrefab == null || virtualCamera == null)
        {
            Debug.LogError("No se ha asignado el prefab del jugador o la cámara virtual.");
            return;
        }

        GameObject startRoom = GameObject.Find("Start");
        if (startRoom != null)
        {
            Room roomComponent = startRoom.GetComponent<Room>();
            if (roomComponent != null)
            {
                GameObject player = Instantiate(playerPrefab, roomComponent.spawnPoint.position, Quaternion.identity);
                virtualCamera.Follow = player.transform;

                // Asignar el confiner inicial
                if (roomComponent.roomConfiner != null)
                {
                    confiner.m_BoundingShape2D = roomComponent.roomConfiner;
                    Debug.Log("Confiner inicial asignado a StartRoom.");
                }
                else
                {
                    Debug.LogError("StartRoom no tiene un confiner asignado.");
                }
            }
        }
    }

   
}
