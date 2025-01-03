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

    private CinemachineVirtualCamera virtualCamera;
    private Transform playerTransform;
    [SerializeField] private float focusDuration = 2f;

    private List<GameObject> activeEnemies = new List<GameObject>();
    private bool isCleared = false;
    public bool isLastRoom = false;
    private void Start()
    {
        virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
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

        StartCoroutine(AnimateDoorOpening());
       
        if(isLastRoom)
        {
            Debug.Log("Starting new round...");
            GameManager.Instance.StartNewRound();
        }
    }


    private void SetDoorsActive(bool active)
    {
        foreach (Transform door in DoorsToUnlock)
        {
            if (door != null)
            {
                door.gameObject.SetActive(active);

                Transform colliders = door.GetComponentInParent<Room>()?.transform.Find("colliders");
                if (colliders != null && colliders.Find(door.name) != null)
                {
                    colliders.Find(door.name).gameObject.SetActive(!active);
                }

                Transform connectedDoor = FindConnectedDoor(door);
                if (connectedDoor != null)
                {
                    connectedDoor.gameObject.SetActive(active);
                    Transform connectedColliders = connectedDoor.GetComponentInParent<Room>()?.transform.Find("colliders");
                    if (connectedColliders != null && connectedColliders.Find(connectedDoor.name) != null)
                    {
                        connectedColliders.Find(connectedDoor.name).gameObject.SetActive(!active);
                    }
                }
            }
        }
    }
    private IEnumerator AnimateDoorOpening()
    {
        Vector3 originalCameraPosition = virtualCamera.transform.position;
        Quaternion originalCameraRotation = virtualCamera.transform.rotation;

        foreach (Transform door in DoorsToUnlock)
        {
            if (door != null)
            {
                // Buscar el componente AreaExit en el hijo de la puerta
                AreaExit areaExit = door.GetComponentInChildren<AreaExit>();

                if (areaExit != null)
                {
                    // Desplazar la cámara hacia la posición de AreaExit, ajustando el offset si es necesario
                    Vector3 areaExitPosition = areaExit.transform.position;
                    virtualCamera.transform.position = new Vector3(areaExitPosition.x, areaExitPosition.y, virtualCamera.transform.position.z);

                    // Cambiar el Follow a la puerta solo por un tiempo
                    if (virtualCamera != null)
                    {
                        virtualCamera.Follow = door;
                        yield return new WaitForSeconds(focusDuration);
                    }

                    // Abrir la puerta
                    door.gameObject.SetActive(true);

                    // Gestionar los coliders
                    Transform colliders = door.GetComponentInParent<Room>()?.transform.Find("colliders");
                    if (colliders != null && colliders.Find(door.name) != null)
                    {
                        colliders.Find(door.name).gameObject.SetActive(false);
                    }

                    // Gestionar la puerta conectada
                    Transform connectedDoor = FindConnectedDoor(door);
                    if (connectedDoor != null)
                    {
                        connectedDoor.gameObject.SetActive(true);
                        Transform connectedColliders = connectedDoor.GetComponentInParent<Room>()?.transform.Find("colliders");
                        if (connectedColliders != null && connectedColliders.Find(connectedDoor.name) != null)
                        {
                            connectedColliders.Find(connectedDoor.name).gameObject.SetActive(false);
                        }
                    }
                }
            }
        }

        // Regresar la cámara a su posición original y seguir al jugador
        if (virtualCamera != null)
        {
            virtualCamera.transform.position = originalCameraPosition;  // Restaurar la posición original
            virtualCamera.transform.rotation = originalCameraRotation;  // Restaurar la rotación original

            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
            virtualCamera.Follow = playerTransform;  // Volver a seguir al jugador
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