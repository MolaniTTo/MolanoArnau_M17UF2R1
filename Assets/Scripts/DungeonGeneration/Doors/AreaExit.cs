using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaExit : MonoBehaviour
{
    public Transform areaEntrance; // Se asignar� din�micamente
    public PolygonCollider2D roomConfiner; // Se asignar� din�micamente
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
        if (other.CompareTag("Player")) // Aseg�rate de que el jugador tiene el tag "Player"
        {
            StartCoroutine(HandleRoomTransition(other.transform));
        }
    }

    private IEnumerator HandleRoomTransition(Transform player)
    {
        screenFade.FadeOut();
        yield return new WaitForSeconds(2f);

        player.position = areaEntrance.position;
        confinerManager.UpdateConfiner(roomConfiner);
        yield return new WaitForSeconds(0.5f);

        screenFade.FadeIn();
       
    }
}
