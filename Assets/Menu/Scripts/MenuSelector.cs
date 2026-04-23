using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class MenuSelector : MonoBehaviour
{
    [Header("Opciones del menú")]
    public List<TextMeshProUGUI> opciones = new List<TextMeshProUGUI>();

    [Header("Colores")]
    public Color colorNormal    = Color.white;
    public Color colorSeleccion = new Color(1f, 0.85f, 0.1f);

    [Header("Efecto brillo")]
    public float velocidadParpadeo = 8f;
    public float minBrillo         = 0.4f;
    public float maxBrillo         = 1.0f;

    [Header("Escala al seleccionar")]
    public float escalaSeleccion = 1.08f;

    [Header("Slider opcional")]
    public VolumenController sliderVolumen;
    public int indiceSlider = -1;

    private int indiceActual = 0;
    private bool activo = false;
    private Coroutine coroutineParpadeo = null;
    private MainMenuController menuController;

    void Awake()
    {
        menuController = FindFirstObjectByType<MainMenuController>();

        
        
    }

    public void Activar()
    {
        if (opciones == null || opciones.Count == 0)
        {
            Debug.LogWarning("MenuSelector: no hay opciones asignadas.");
            return;
        }
        activo = true;
        indiceActual = 0;
        ActualizarSeleccion();
    }

    public void Desactivar()
    {
        activo = false;
        PararParpadeo();
        foreach (var op in opciones)
        {
            if (op == null) continue;
            op.color = colorNormal;
            op.transform.localScale = Vector3.one;
        }
    }

    void Update()
    {
        if (!activo || opciones.Count == 0) return;

        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            CambiarSeleccion(1);
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            CambiarSeleccion(-1);

        if (indiceActual == indiceSlider && sliderVolumen != null)
        {
            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
                sliderVolumen.AjustarConTeclado(1f);
            else if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
                sliderVolumen.AjustarConTeclado(-1f);
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            if (indiceActual != indiceSlider)
                EjecutarOpcion(indiceActual);
        }
    }

    void CambiarSeleccion(int direccion)
    {
        if (opciones.Count == 0) return;
        indiceActual = (indiceActual + direccion + opciones.Count) % opciones.Count;
        ActualizarSeleccion();
    }

    void ActualizarSeleccion()
    {
        PararParpadeo();
        for (int i = 0; i < opciones.Count; i++)
        {
            if (opciones[i] == null) continue;
            if (i == indiceActual)
            {
                opciones[i].transform.localScale = Vector3.one * escalaSeleccion;
                coroutineParpadeo = StartCoroutine(ParpadeoBrillo(opciones[i]));
            }
            else
            {
                opciones[i].color = colorNormal;
                opciones[i].transform.localScale = Vector3.one;
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

    IEnumerator ParpadeoBrillo(TextMeshProUGUI texto)
    {
        while (true)
        {
            float alpha = Mathf.Lerp(minBrillo, maxBrillo,
                          (Mathf.Sin(Time.time * velocidadParpadeo) + 1f) / 2f);
            texto.color = new Color(colorSeleccion.r, colorSeleccion.g, colorSeleccion.b, alpha);
            yield return null;
        }
    }

    public void OnHover(int indice)
    {
        if (!activo || indice == indiceActual) return;
        indiceActual = indice;
        ActualizarSeleccion();
    }

    public void EjecutarOpcion(int indice)
    {
        if (!activo || opciones.Count == 0) return;
        StartCoroutine(EfectoConfirmar(indice));
    }

    IEnumerator EfectoConfirmar(int indice)
    {
        activo = false;
        PararParpadeo();

        for (int i = 0; i < 4; i++)
        {
            opciones[indice].color = colorNormal;
            yield return new WaitForSeconds(0.07f);
            opciones[indice].color = colorSeleccion;
            yield return new WaitForSeconds(0.07f);
        }

        menuController?.EjecutarAccion(indice, this);
    }
}