using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AreaExit : MonoBehaviour
{
    public Transform areaEntrance; //l'assignem dinamicament
    public PolygonCollider2D roomConfiner; //tambe dinamicament
    public CameraConfinerManager confinerManager;
    public ScreenFade screenFade;


    private void Start()
    {
        confinerManager = FindObjectOfType<CameraConfinerManager>();
        if(screenFade == null)
        {
            screenFade = FindObjectOfType<ScreenFade>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(HandleRoomTransition(other.transform));
        }
    }

    private IEnumerator HandleRoomTransition(Transform player)
    {
        //Connecto l'area d'entrada amb l'area de sortida de la room1 amb la store
        if (gameObject.CompareTag("EntryStore"))
        {
            areaEntrance = GameObject.FindGameObjectWithTag("EntranceStore").transform;
            roomConfiner = GameObject.FindGameObjectWithTag("Store").GetComponentInChildren<PolygonCollider2D>();
        }
        if (gameObject.CompareTag("ExitStore"))
        {
            areaEntrance = GameObject.FindGameObjectWithTag("EntryDungeon").transform;
            roomConfiner = GameObject.FindGameObjectWithTag("ConfinerConectionWithStore").GetComponentInParent<PolygonCollider2D>();
        }
        PlayerController.Instance.isMovementBlocked = true;
        PlayerController.Instance.isPlayerActive = false;
        screenFade.FadeOut();
        yield return new WaitForSeconds(2f);

        player.position = areaEntrance.position; //teletransportem el player
        confinerManager.UpdateConfiner(roomConfiner); //actualitzem el confiner de la camera
        yield return new WaitForSeconds(1.3f);

        screenFade.FadeIn();
        PlayerController.Instance.isMovementBlocked = false;
        yield return new WaitForSeconds(0.8f);
        PlayerController.Instance.isPlayerActive = true;
       
    }

}
