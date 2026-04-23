using UnityEngine;
using UnityEngine.Rendering.Universal; 

public class LuzParpadeante : MonoBehaviour
{
    private Light2D luz;
    
    [Header("Configuración")]
    public float intensidadMinima = 0.8f;
    public float intensidadMaxima = 1.2f;
    [Tooltip("Qué tan rápido cambia la luz")]
    public float velocidad = 0.1f;

    void Start()
    {
        luz = GetComponent<Light2D>();
    }

    void Update()
    {
        
        float ruido = Mathf.PerlinNoise(Time.time * (1 / velocidad), 0);
        luz.intensity = Mathf.Lerp(intensidadMinima, intensidadMaxima, ruido);
    }
}