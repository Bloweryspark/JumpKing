using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Source")]
    public AudioSource musicaSource;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void SetVolumen(float valor)
    {
        musicaSource.volume = valor;
    }

    public float GetVolumen()
    {
        return musicaSource.volume;
    }

     public void SetMute(bool mute)
    {
        if (musicaSource != null)
            musicaSource.mute = mute;
    }
}