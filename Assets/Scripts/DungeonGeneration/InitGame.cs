using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InitGame : MonoBehaviour
{
    public GameObject playerPrefab;
    public CinemachineVirtualCamera virtualCamera;
    public GameObject[] enemyPrefabs;
    private CinemachineConfiner2D confiner;
    private List<GameObject> activeEnemies = new List<GameObject>();

    private int round = 5;

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
        StartCoroutine(SpawnEnemiesInAllRooms());
    }


    private IEnumerator SpawnEnemiesInAllRooms()
    {
        Room[] allRooms = FindObjectsOfType<Room>();

        foreach (Room room in allRooms)
        {
            Debug.Log("Spawning enemies in room " + room.name);
            SpawnEnemiesInAllRooms(room, round);
            yield return null;

        }
    }

    private void SpawnEnemiesInAllRooms(Room room, int round)
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0)
        {
            Debug.LogError("No se han asignado prefabs de enemigos.");
            return;
        }

        if (room.EnemySpawnPoints == null || room.EnemySpawnPoints.Length == 0)
        {
            Debug.LogWarning($"La habitación {room.name} no tiene puntos de spawn de enemigos.");
            return;
        }

        int enemyCount = Mathf.Min(round * 5, room.EnemySpawnPoints.Length);
        List<Transform> avalibleSpawnPoints = new List<Transform>(room.EnemySpawnPoints);

        for(int i=0; i<enemyCount; i++)
        {
            int spawnIndex = Random.Range(0, avalibleSpawnPoints.Count);
            Transform spawnpoint = avalibleSpawnPoints[spawnIndex];
            avalibleSpawnPoints.RemoveAt(spawnIndex);

            int enemyIndex = Random.Range(0, enemyPrefabs.Length);
            GameObject enemyPrefab = enemyPrefabs[enemyIndex];

            GameObject enemy = Instantiate(enemyPrefab, spawnpoint.position, Quaternion.identity);
            room.AddEnemy(enemy);

        }
    }
}
