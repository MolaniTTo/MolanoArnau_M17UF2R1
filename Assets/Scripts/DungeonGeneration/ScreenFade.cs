using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ScreenFade : MonoBehaviour
{
    public Image fadeImage; // La imagen que cubre la pantalla
    public float fadeDuration = 2f; // Duración del fade in y fade out

    private IEnumerator fadeRoutine;

    private void Awake()
    {
        fadeImage = GetComponent<Image>();
    }

    public void FadeOut()
    {
        if (fadeRoutine != null)
        {
            StopCoroutine(fadeRoutine);
        }

        fadeRoutine = FadeOutRoutine();
        StartCoroutine(fadeRoutine);
    }

    public void FadeIn()
    {
        if (fadeRoutine != null)
        {
            StopCoroutine(fadeRoutine);
        }

        fadeRoutine = FadeInRoutine();
        StartCoroutine(fadeRoutine);
    }


    private IEnumerator FadeOutRoutine()
    {
        float timer = 0f;
        Color color = fadeImage.color;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            color.a = Mathf.Lerp(0, 1, timer / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }
    }

    private IEnumerator FadeInRoutine()
    {
        float timer = 0f;
        Color color = fadeImage.color;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            color.a = Mathf.Lerp(1, 0, timer / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }
    }

   

   

   
}
