using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class PauseManager : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject menuPause;
    public AudioSource musicaJuego;
    public AudioSource efectosPersonaje;

    [Header("Botón sonido")]
    public Image imagenBtnSonido;
    public Sprite spriteConSonido;
    public Sprite spriteMute;

    [Header("Botones en orden: Play, Home, Sound")]
    public List<Image> botones = new List<Image>();

    [Header("Efecto selección")]
    public float escalaSeleccion    = 1.2f;
    public float velocidadParpadeo  = 8f;
    public float minBrillo          = 0.5f;
    public float maxBrillo          = 1.0f;
    public Color colorSeleccion     = new Color(1f, 0.85f, 0.1f);

    private bool pausado  = false;
    private bool muteado  = false;
    private int  indiceActual = 0;
    private bool selectorActivo = false;
    private Coroutine coroutineParpadeo = null;
    private List<Vector3> escalasOriginales = new List<Vector3>();

    void Awake()
{
    // Guarda las escalas originales al inicio antes de cualquier cosa
    escalasOriginales.Clear();
    foreach (var btn in botones)
    {
        if (btn != null)
            escalasOriginales.Add(btn.transform.localScale);
    }
}

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            TogglePausa();

        if (!selectorActivo || botones.Count == 0) return;

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            CambiarSeleccion(1);
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            CambiarSeleccion(-1);

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            EjecutarOpcion(indiceActual);
    }

    // ── Pausa ────────────────────────────────────────────

    public void TogglePausa()
    {
        pausado = !pausado;
        menuPause.SetActive(pausado);
        Time.timeScale = pausado ? 0f : 1f;

        if (pausado)
        {
            ActivarSelector();
            if (musicaJuego != null) musicaJuego.Pause();
            if (efectosPersonaje != null) efectosPersonaje.Pause();
        }
        else
        {
            DesactivarSelector();
            if (musicaJuego != null) musicaJuego.UnPause();
            if (efectosPersonaje != null) efectosPersonaje.UnPause();
        }
    }

    public void OnPlay()
    {
        pausado = false;
        menuPause.SetActive(false);
        Time.timeScale = 1f;
        DesactivarSelector();
        if (musicaJuego != null) musicaJuego.UnPause();
        if (efectosPersonaje != null) efectosPersonaje.UnPause();
    }

    public void OnHome()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }

    public void OnToggleSonido()
    {
        muteado = !muteado;
        if (musicaJuego != null)      musicaJuego.mute      = muteado;
        if (efectosPersonaje != null) efectosPersonaje.mute = muteado;
        if (AudioManager.Instance != null) AudioManager.Instance.SetMute(muteado);
        imagenBtnSonido.sprite = muteado ? spriteMute : spriteConSonido;
    }

    // ── Selector ─────────────────────────────────────────

    void ActivarSelector()
    {
        if (botones == null || botones.Count == 0) return;
        selectorActivo = true;
        indiceActual   = 0;
        ActualizarSeleccion();
    }

   void DesactivarSelector()
{
    selectorActivo = false;
    PararParpadeo();
    for (int i = 0; i < botones.Count; i++)
    {
        if (botones[i] == null) continue;
        botones[i].color = Color.white;
        if (escalasOriginales.Count > i)
            botones[i].transform.localScale = escalasOriginales[i];
    }
}

    void CambiarSeleccion(int direccion)
    {
        indiceActual = (indiceActual + direccion + botones.Count) % botones.Count;
        ActualizarSeleccion();
    }

    void ActualizarSeleccion()
    {
         PararParpadeo();
    for (int i = 0; i < botones.Count; i++)
    {
        if (botones[i] == null) continue;
        if (i == indiceActual)
        {
            botones[i].transform.localScale = escalasOriginales[i] * escalaSeleccion;
            coroutineParpadeo = StartCoroutine(ParpadeoBrillo(botones[i]));
        }
        else
        {
            botones[i].color = Color.white;
            botones[i].transform.localScale = escalasOriginales[i];
        }
    }
    }

    void PararParpadeo()
    {
        if (coroutineParpadeo != null)
        {
            StopCoroutine(coroutineParpadeo);
            coroutineParpadeo = null;
        }
    }

    IEnumerator ParpadeoBrillo(Image imagen)
    {
        while (true)
        {
            float alpha = Mathf.Lerp(minBrillo, maxBrillo,
                          (Mathf.Sin(Time.unscaledTime * velocidadParpadeo) + 1f) / 2f);
            imagen.color = new Color(colorSeleccion.r, colorSeleccion.g, colorSeleccion.b, alpha);
            yield return null;
        }
    }

    // ── Hover y click desde Event Trigger ────────────────

    public void OnHover(int indice)
    {
        if (!selectorActivo || indice == indiceActual) return;
        indiceActual = indice;
        ActualizarSeleccion();
    }

    public void EjecutarOpcion(int indice)
    {
        if (!selectorActivo) return;
        StartCoroutine(EfectoConfirmar(indice));
    }

    IEnumerator EfectoConfirmar(int indice)
    {
        selectorActivo = false;
        PararParpadeo();

        for (int i = 0; i < 4; i++)
        {
            botones[indice].color = Color.white;
            yield return new WaitForSecondsRealtime(0.07f);
            botones[indice].color = colorSeleccion;
            yield return new WaitForSecondsRealtime(0.07f);
        }

        switch (indice)
        {
            case 0: OnPlay();         break;
            case 1: OnHome();         break;
            case 2: OnToggleSonido();
                    selectorActivo = true;
                    ActualizarSeleccion(); break;
        }
    }
}