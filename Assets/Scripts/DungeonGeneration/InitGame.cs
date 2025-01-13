using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InitGame : MonoBehaviour
{
    public GameObject playerPrefab;
    public CinemachineVirtualCamera virtualCamera;
    public GameObject[] enemyPrefabs; // Array de prefabs d'enemics
    private CinemachineConfiner2D confiner;
    private List<GameObject> activeEnemies = new List<GameObject>(); // Lista dels enemics actius

    private int round = 1;  

    private void Awake()
    {
        confiner = FindObjectOfType<CinemachineConfiner2D>();
        confiner.m_BoundingShape2D = null;
    }


    public void StartGame()
    {
        if (playerPrefab == null || virtualCamera == null)
        {
            return; //he tret els debugs.log perque em vas dir que consumien molt pero on veus returns de normal hi havia debugs.log
        }

        StartCoroutine(SpawnEnemiesInAllRooms());
        SetUpPlayer();
    }

    private void SetUpPlayer()
    {
        GameObject startRoom = GameObject.Find("Start");
        GameObject existingPlayer = GameObject.FindGameObjectWithTag("Player");
        if (startRoom != null)
        {
            Room roomComponent = startRoom.GetComponent<Room>();
            if (roomComponent != null)
            {
                if (existingPlayer == null) // Si no existeix el jugador, el creem
                {
                    existingPlayer = Instantiate(playerPrefab, roomComponent.spawnPoint.position, Quaternion.identity);
                }
                else // Si ja existeix, el movem a la posició de spawn
                {
                    existingPlayer.transform.position = roomComponent.spawnPoint.position;
                } 
                virtualCamera.Follow = existingPlayer.transform; // Assignem la càmera al jugador
                // Asignar el confiner inicial
                if (roomComponent.roomConfiner != null) // Si la sala té un confiner assignat, l'assignem a la càmera
                {
                    confiner.m_BoundingShape2D = roomComponent.roomConfiner;
                }
            }
        }
    }

    public void FlickerAndActive()
    {
        GameObject existingPlayer = GameObject.FindGameObjectWithTag("Player");
        if (existingPlayer != null)
        {
            Flash flash = existingPlayer.GetComponent<Flash>();
            if (flash != null)
            {
                StartCoroutine(flash.PlayerFlicker()); //parpadeja el jugador quan es fa dany o quan es inici de ronda a modo de invulnerabilitat
            }
        }
    }

    public void SetRound(int newRound) //funcio per actualitzar la ronda
    {
        round = newRound;
    }


    private IEnumerator SpawnEnemiesInAllRooms()
    {  
        Room[] allRooms = FindObjectsOfType<Room>();

        foreach (Room room in allRooms) //per cada habitacio de la llista de totes les habitacions cridem a la funcio SpawnEnemiesInAllRooms
        {
            if (room == null)
            {
                continue;
            }
            SpawnEnemiesInAllRooms(room, round);
            yield return null;

        }
    }

    private void SpawnEnemiesInAllRooms(Room room, int round) //funcio a millorar ja que no controlo que de manera random es puguin instanciar molts enemics iguals
    {
        if (room == null) return;
        if (enemyPrefabs == null || enemyPrefabs.Length == 0)
        {
            return;
        }

        if (room.EnemySpawnPoints == null || room.EnemySpawnPoints.Length == 0)
        {
            return;
        }

        int enemyCount = Mathf.Min(round * 5, room.EnemySpawnPoints.Length); //5 enemics per ronda i habitacio pero com a maxim el nombre de punts de spawn
        List<Transform> avalibleSpawnPoints = new List<Transform>(room.EnemySpawnPoints); //Llista de punts de spawn

        for(int i=0; i<enemyCount; i++) //per cada enemic que volem spawnear
        {
            int spawnIndex = Random.Range(0, avalibleSpawnPoints.Count); //agafem un index random de la llista de punts de spawn
            Transform spawnpoint = avalibleSpawnPoints[spawnIndex]; //agafem el punt de spawn amb l'index random
            avalibleSpawnPoints.RemoveAt(spawnIndex); //eliminem el punt de spawn de la llista perque no es pugui repetir

            int enemyIndex = Random.Range(0, enemyPrefabs.Length); //agafem un index random dels prefabs d'enemics
            GameObject enemyPrefab = enemyPrefabs[enemyIndex]; //agafem el prefab d'enemic amb l'index random

            GameObject enemy = Instantiate(enemyPrefab, spawnpoint.position, Quaternion.identity); //instanciem l'enemic al punt de spawn
            room.AddEnemy(enemy); //afegim l'enemic a la llista d'enemics de la habitacio
        }
    }
}
