using UnityEngine;
using TMPro;
using System.Collections;

public class SplashScreen : MonoBehaviour
{
    [Header("Referencias")]
    public ToasterAnimator toasterAnimator;
    public CanvasGroup splashCanvasGroup;
    public TextMeshProUGUI primerTxt; // arrastra PrimerTxt aquí en el Inspector

    [Header("Configuración")]
    public float titleFadeInDuration = 1.5f;
    public float blinkInterval = 0.5f;

    private bool inputEnabled = false;

    void Start()
    {
        primerTxt.alpha = 0f;
        StartCoroutine(ShowSplash());
    }

    IEnumerator ShowSplash()
    {
        // Fade in del título completo
        float t = 0;
        while (t < titleFadeInDuration)
        {
            splashCanvasGroup.alpha = t / titleFadeInDuration;
            t += Time.deltaTime;
            yield return null;
        }
        splashCanvasGroup.alpha = 1f;

        // Pequeña pausa antes de que aparezca el texto
        yield return new WaitForSeconds(0.3f);

        // Habilita input y empieza el titileo
        inputEnabled = true;
        StartCoroutine(BlinkPrompt());
    }

    IEnumerator BlinkPrompt()
    {
        while (true)
        {
            // Aparece
            primerTxt.alpha = 1f;
            yield return new WaitForSeconds(blinkInterval);
            // Desaparece
            primerTxt.alpha = 0f;
            yield return new WaitForSeconds(blinkInterval);
        }
    }

    void Update()
    {
        if (!inputEnabled) return;

        if (Input.anyKeyDown)
        {
            inputEnabled = false;
            StopAllCoroutines();
            StartCoroutine(TransitionToMenu());
        }
    }

    IEnumerator TransitionToMenu()
    {
        // Asegura que el texto quede visible durante el fade out
        primerTxt.alpha = 1f;

        float t = 1f;
        while (t > 0)
        {
            splashCanvasGroup.alpha = t;
            t -= Time.deltaTime * 2f;
            yield return null;
        }
        splashCanvasGroup.alpha = 0f;
        splashCanvasGroup.gameObject.SetActive(false);

        toasterAnimator.PlayToastSequence();
    }
}