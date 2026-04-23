using UnityEngine;

public class PanObjetivo : MonoBehaviour
{
    public float floatSpeed = 2f;
    public float floatHeight = 0.25f;

    public AudioClip sonidoVictoria;

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

            // ocultar pan
            sr.enabled = false;
            col.enabled = false;

            // destruir después de que termine el sonido
            Destroy(gameObject, sonidoVictoria.length);
        }
    }
}