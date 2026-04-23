using UnityEngine;
using UnityEngine.UI;

public class VolumenController : MonoBehaviour
{
    private Slider slider;

    [Header("Ajuste con teclado")]
    public float pasoVolumen = 0.05f;

    void Awake()
    {
        slider = GetComponent<Slider>();
    }

    void OnEnable()
    {
        if (AudioManager.Instance != null)
            slider.value = AudioManager.Instance.GetVolumen();

        slider.onValueChanged.AddListener(OnSliderCambiado);
    }

    void OnDisable()
    {
        slider.onValueChanged.RemoveListener(OnSliderCambiado);
    }

    void OnSliderCambiado(float valor)
    {
        AudioManager.Instance?.SetVolumen(valor);
    }

    // Llamado por MenuSelector cuando esta opción está seleccionada
    public void AjustarConTeclado(float direccion)
    {
        slider.value = Mathf.Clamp01(slider.value + direccion * pasoVolumen);
    }
}