using System.Collections;
using UnityEngine;

public class MusicZone : MonoBehaviour
{
    public AudioClip musicClip; 
    public float fadeDuration = 1f; 
    private AudioSource audioSource; 
    private PlayerController playerController;

    
    private void OnTriggerEnter2D(Collider2D other) //Si entrem a la zona de música
    {
        if (other.CompareTag("Player")) 
        {
            StartCoroutine(PlayMusicWithFade());
        }
    }

   
    private void OnTriggerExit2D(Collider2D other) //si sortim de la zona de música
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(FadeOutMusic());
        }
    }

    private void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        audioSource = Camera.main.GetComponent<AudioSource>();
    }

    private IEnumerator PlayMusicWithFade()
    {
        if (audioSource == null) yield break;

        
        if (audioSource.isPlaying) //Fade out si ja hi ha musica
        {
            yield return StartCoroutine(FadeOutMusic());
        }

       
        audioSource.clip = musicClip; //Canviem la música
        audioSource.Play();

        yield return StartCoroutine(FadeInMusic());
    }

    public IEnumerator FadeOutMusic()
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }

        audioSource.volume = 0;
        audioSource.Stop();
    }

    public IEnumerator FadeInMusic()
    {
        if (audioSource == null) yield break;

        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }

        float targetVolume = 1f;

        while (audioSource.volume < targetVolume)
        {
            audioSource.volume += targetVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }

        audioSource.volume = targetVolume; 
    }
}
