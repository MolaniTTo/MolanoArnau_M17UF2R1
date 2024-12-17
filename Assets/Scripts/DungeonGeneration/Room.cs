using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public Transform spawnPoint;
    public PolygonCollider2D roomConfiner;
    public Transform[] EnemySpawnPoints;
    public Transform[] DoorsToUnlock;

    private List<GameObject> activeEnemies = new List<GameObject>();
    private bool isCleared = false;

    private void Start()
    {
        SetDoorsActive(false);
    }

    public void AddEnemy(GameObject enemy)
    {
        activeEnemies.Add(enemy);

        //Ens subscrivim a l'event de mort de l'enemic
        EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.OnEnemyDeath += RemoveEnemy;
        }
    }

    private void RemoveEnemy(GameObject enemy)
    {
        activeEnemies.Remove(enemy);

        if (activeEnemies.Count == 0 && !isCleared)
        {
            OnRoomCleared();
        }
    }

    private void OnRoomCleared()
    {
        isCleared = true;
        Debug.Log($"Room {name} cleared!");

        // Abrir las puertas correspondientes
        SetDoorsActive(true);
    }

    private void SetDoorsActive(bool active)
    {
        foreach (Transform door in DoorsToUnlock)
        {
            if (door != null)
            {
                door.gameObject.SetActive(active);

                Transform connectedDoor = FindConnectedDoor(door);
                if (connectedDoor != null)
                {
                    connectedDoor.gameObject.SetActive(active);

                }
            }
        }
    }

    private Transform FindConnectedDoor(Transform door)
    {
        // Buscar la puerta conectada usando AreaExit
        AreaExit exit = door.GetComponentInChildren<AreaExit>();
        if (exit != null && exit.areaEntrance != null)
        {
            return exit.areaEntrance.parent; // La puerta conectada
        }
        return null;
    }
}