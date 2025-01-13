using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public Transform spawnPoint;
    public PolygonCollider2D roomConfiner;
    public Transform[] EnemySpawnPoints;
    public Transform[] DoorsToUnlock; //portes a desbloquejar per sortir de la sala

    private CinemachineVirtualCamera virtualCamera;
    private Transform playerTransform;
    [SerializeField] private float focusDuration = 1f;
    private WeaponAudio weaponAudio;
    private MusicZone musicZone;

    private List<GameObject> activeEnemies = new List<GameObject>();
    private bool isCleared = false;
    public bool isLastRoom = false;

    private void Awake()
    {
        weaponAudio = FindObjectOfType<WeaponAudio>();
        musicZone = transform.GetComponentInChildren<MusicZone>();
    }
    private void Start()
    {
        virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        SetDoorsActive(false);
    }

    public void AddEnemy(GameObject enemy)
    {
        activeEnemies.Add(enemy);

        //Ens subscrivim a l'event de mort de l'enemic (aqui tens lo del event q volies Carlos)
        EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.OnEnemyDeath += RemoveEnemy;
        }
    }

    private void RemoveEnemy(GameObject enemy) //Aquesta funció s'executarà quan l'enemic mori
    {
        activeEnemies.Remove(enemy);

        if (activeEnemies.Count == 0 && !isCleared)
        {
            OnRoomCleared();
        }
    }

    private void OnRoomCleared() //Quan la sala estigui buida d'enemics
    {
        isCleared = true;

        if (isLastRoom)
        {
            GameManager.Instance.StartCoroutine(GameManager.Instance.StartNewRound());
        }
        else
        {
            StartCoroutine(AnimateDoorOpening());
        }
    }


    private void SetDoorsActive(bool active) //Funció per activar o desactivar les portes
    {
        foreach (Transform door in DoorsToUnlock)
        {
            if (door != null)
            {
                door.gameObject.SetActive(active);

                Transform colliders = door.GetComponentInParent<Room>()?.transform.Find("colliders"); //Busquem els colliders de la porta
                if (colliders != null && colliders.Find(door.name) != null)
                {
                    colliders.Find(door.name).gameObject.SetActive(!active);
                }

                Transform connectedDoor = FindConnectedDoor(door);
                if (connectedDoor != null)
                {
                    connectedDoor.gameObject.SetActive(active);
                    Transform connectedColliders = connectedDoor.GetComponentInParent<Room>()?.transform.Find("colliders"); //Busquem els colliders de la porta connectada
                    if (connectedColliders != null && connectedColliders.Find(connectedDoor.name) != null)
                    {
                        connectedColliders.Find(connectedDoor.name).gameObject.SetActive(!active); //Desactivem els colliders de la porta connectada
                    }
                }
            }
        }
    }
    private IEnumerator AnimateDoorOpening() //Funció per obrir les portes (es una pijada q em va fer gracia posar de la camara)
    {
        Vector3 originalCameraPosition = virtualCamera.transform.position;
        Quaternion originalCameraRotation = virtualCamera.transform.rotation;

        foreach (Transform door in DoorsToUnlock) //Per cada porta a desbloquejar
        {
            if (door != null)
            {
                //Buscar l'AreaExit de la porta
                AreaExit areaExit = door.GetComponentInChildren<AreaExit>();

                if (areaExit != null)
                {
                    //Utilitzar la posició de l'AreaExit com a posició de la porta
                    Vector3 areaExitPosition = new Vector3(areaExit.transform.position.x, areaExit.transform.position.y, originalCameraPosition.z); //Posició de la porta

                    //Detenem el seguiment del jugador per la càmera
                    virtualCamera.Follow = null;

                    //Movem la càmera a la posició de la porta
                    if (musicZone != null)
                    {
                        StartCoroutine(musicZone.FadeOutMusic());
                    }
                    yield return MoveCameraToPosition(areaExitPosition, 1f);

                    //Mirem la porta com s'obre
                    yield return new WaitForSeconds(focusDuration);
                    door.gameObject.SetActive(true);
                    
                    weaponAudio.PlayPlayerSound(weaponAudio.Solution);
                    yield return new WaitForSeconds(3f);
                    if (musicZone != null)
                    {
                        StartCoroutine(musicZone.FadeInMusic());
                    }
                    //Gestionem els colliders de la porta
                    Transform colliders = door.GetComponentInParent<Room>()?.transform.Find("colliders");
                    if (colliders != null && colliders.Find(door.name) != null)
                    {
                        colliders.Find(door.name).gameObject.SetActive(false);
                    }

                    //Aqui gestionem els colliders de la porta connectada
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

            //Movem la camara a la posició del jugador
            yield return MoveCameraToPosition(playerCameraPosition, 1f);

            //Restableix el seguiment del jugador per la càmera
            virtualCamera.Follow = playerTransform;
        }
    }

    private IEnumerator MoveCameraToPosition(Vector3 targetPosition, float duration) //Funció per moure la càmera a una posició
    {
        Vector3 startPosition = virtualCamera.transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            virtualCamera.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;

            yield return null;
        }
        virtualCamera.transform.position = targetPosition;
    }



    private Transform FindConnectedDoor(Transform door)
    {
        //Busquem l'AreaExit de la porta per trobar la porta connectada
        AreaExit exit = door.GetComponentInChildren<AreaExit>();
        if (exit != null && exit.areaEntrance != null)
        {
            return exit.areaEntrance.parent; //la porta connectada
        }
        return null;
    }
}