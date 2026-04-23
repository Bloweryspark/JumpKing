using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuController : MonoBehaviour
{
    [Header("Referencias")]
    public CanvasGroup menuCanvasGroup;
    public CanvasGroup opcionesCanvasGroup;
    public ToasterAnimator toasterAnimator;

    [Header("Selector")]
    public MenuSelector menuSelector;
    public MenuSelector selectorOpciones;

    [Header("Configuración")]
    public float fadeInDuration = 0.2f;

    void Awake()
    {
        SetCanvasGroup(menuCanvasGroup, 0f, false);
        SetCanvasGroup(opcionesCanvasGroup, 0f, false);
    }

    public void ShowMenu()
    {
        StartCoroutine(FadeIn(menuCanvasGroup, () => menuSelector.Activar()));
    }

    // Recibe el índice y el selector que lo llamó para saber el contexto
    public void EjecutarAccion(int indice, MenuSelector selector)
    {
        if (selector == menuSelector)
        {
            if (indice == 0) OnNewGame();
            if (indice == 1) OnOptions();
        }
        else if (selector == selectorOpciones)
        {
            // indice 0 = MusicVolume (slider, no hace nada aquí)
            // indice 1 = Back
            if (indice == 1) StartCoroutine(VolverAlMenu());
        }
    }

    void OnNewGame()
    {
        SceneManager.LoadScene("Game");
    }

    void OnOptions()
    {
        StartCoroutine(TransicionAOpciones());
    }

    IEnumerator TransicionAOpciones()
    {
        menuSelector.Desactivar();
        SetCanvasGroup(menuCanvasGroup, 0f, false);
        yield return StartCoroutine(toasterAnimator.BajarPan());
        yield return StartCoroutine(toasterAnimator.SubirPanOpciones());
        yield return StartCoroutine(FadeIn(opcionesCanvasGroup, () => selectorOpciones.Activar()));
    }

    IEnumerator VolverAlMenu()
    {
        selectorOpciones.Desactivar();
        SetCanvasGroup(opcionesCanvasGroup, 0f, false);
        yield return StartCoroutine(toasterAnimator.BajarPanOpciones());
        yield return StartCoroutine(toasterAnimator.SubirPanMenu());
        yield return StartCoroutine(FadeIn(menuCanvasGroup, () => menuSelector.Activar()));
    }

    IEnumerator FadeIn(CanvasGroup cg, System.Action onComplete = null)
    {
        float elapsed = 0f;
        cg.alpha = 0f;
        while (elapsed < fadeInDuration)
        {
            cg.alpha = elapsed / fadeInDuration;
            elapsed += Time.deltaTime;
            yield return null;
        }
        SetCanvasGroup(cg, 1f, true);
        onComplete?.Invoke();
    }

    void SetCanvasGroup(CanvasGroup cg, float alpha, bool interactable)
    {
        if (cg == null) return;
        cg.alpha = alpha;
        cg.interactable = interactable;
        cg.blocksRaycasts = interactable;
    }
}