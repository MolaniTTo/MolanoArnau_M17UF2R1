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
    [SerializeField] private float focusDuration = 1f;

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

        if (isLastRoom)
        {
            Debug.Log("Starting new round...");
            GameManager.Instance.StartCoroutine(GameManager.Instance.StartNewRound());
        }
        else
        {
            StartCoroutine(AnimateDoorOpening());
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
                    // Usar la posición global del AreaExit
                    Vector3 areaExitPosition = new Vector3(areaExit.transform.position.x, areaExit.transform.position.y, originalCameraPosition.z);

                    // Detener el seguimiento temporalmente
                    virtualCamera.Follow = null;

                    // Mover la cámara suavemente hacia la puerta
                    yield return MoveCameraToPosition(areaExitPosition, 1f);

                    // Permanecer enfocado en la puerta mientras se abre
                    yield return new WaitForSeconds(focusDuration);
                    door.gameObject.SetActive(true);

                    // Gestionar los colliders
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

                    yield return new WaitForSeconds(0.5f);
                }
            }
        }

        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        if (playerTransform != null)
        {
            Vector3 playerCameraPosition = new Vector3(playerTransform.position.x, playerTransform.position.y, originalCameraPosition.z);

            // Mover la cámara de vuelta al jugador
            yield return MoveCameraToPosition(playerCameraPosition, 1f);

            // Restaurar el seguimiento al jugador
            virtualCamera.Follow = playerTransform;
        }
    }

    private IEnumerator MoveCameraToPosition(Vector3 targetPosition, float duration)
    {
        Vector3 startPosition = virtualCamera.transform.position;
        float elapsedTime = 0f;

        Debug.Log($"Starting camera move from {startPosition} to {targetPosition}.");

        while (elapsedTime < duration)
        {
            virtualCamera.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        virtualCamera.transform.position = targetPosition;
        Debug.Log($"Camera moved to {targetPosition}.");
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