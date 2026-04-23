using System.Collections;
using UnityEngine;

public class PanObjetivo : MonoBehaviour
{
    public float floatSpeed = 2f;
    public float floatHeight = 0.25f;

    public AudioClip sonidoVictoria;
    public GameObject winImage;
    public float blinkInterval = 0.2f;
    public int blinkCount = 6;

    private Vector3 startPos;
    public AudioSource audioSource;
    private SpriteRenderer sr;
    private Collider2D col;

    void Start()
    {
        startPos = transform.position;

        audioSource = GetComponent<AudioSource>();
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();

        if (winImage != null)
        {
            winImage.SetActive(false);
        }
    }

    void Update()
    {
        float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            Animator anim = other.GetComponent<Animator>();

            if(anim != null)
            {
                anim.SetTrigger("ganar");
            }

            // reproducir sonido
            audioSource.PlayOneShot(sonidoVictoria);

            sr.enabled = false;
            col.enabled = false;

            if (winImage != null)
            {
                StartCoroutine(ShowWinImageAndStop());
            }
            else
            {
                Debug.LogWarning("Win image not assigned in PanObjetivo.");
                Time.timeScale = 0f;
            }

            Destroy(gameObject, sonidoVictoria.length);
        }
    }

    private IEnumerator ShowWinImageAndStop()
    {
        winImage.SetActive(true);

        for (int i = 0; i < blinkCount; i++)
        {
            winImage.SetActive(i % 2 == 0);
            yield return new WaitForSecondsRealtime(blinkInterval);
        }

        winImage.SetActive(true);
        Time.timeScale = 0f;
    }
}