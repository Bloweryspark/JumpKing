using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BotonSalir : MonoBehaviour
{
    private Image imagenBoton;
    private Color colorOriginal;
    public float duracionAnimacion = 0.2f;
    public float oscurecimiento = 0.6f;

    void Start()
    {
        imagenBoton = GetComponent<Image>();
        if (imagenBoton != null)
        {
            colorOriginal = imagenBoton.color;
            // Agregar componente Button si no lo tiene
            if (GetComponent<Button>() == null)
            {
                Button boton = gameObject.AddComponent<Button>();
                boton.onClick.AddListener(OnBotonClick);
            }
            else
            {
                GetComponent<Button>().onClick.AddListener(OnBotonClick);
            }
        }
    }

    void OnBotonClick()
    {
        StartCoroutine(AnimarYSalir());
    }

    IEnumerator AnimarYSalir()
    {
        // Cambiar a color oscuro
        Color colorOscuro = new Color(colorOriginal.r * oscurecimiento, colorOriginal.g * oscurecimiento, colorOriginal.b * oscurecimiento, colorOriginal.a);
        
        float tiempoTranscurrido = 0f;
        
        // Animar transición al color oscuro
        while (tiempoTranscurrido < duracionAnimacion)
        {
            tiempoTranscurrido += Time.deltaTime;
            float t = tiempoTranscurrido / duracionAnimacion;
            imagenBoton.color = Color.Lerp(colorOriginal, colorOscuro, t);
            yield return null;
        }
        
        // Asegurar que está en el color oscuro
        imagenBoton.color = colorOscuro;
        
        Debug.Log("¡Saliendo del juego!");
        
        // Salir del juego
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
