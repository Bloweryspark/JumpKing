using UnityEngine;
using UnityEngine.SceneManagement;

public class MovPlayer : MonoBehaviour
{
    public float speed = 5f;

    private Rigidbody2D rb;
    private float moveInput;
    private bool suelo;
    private bool mirandoDerecha = true;
    private Animator animator;
    private float direccionSalto = 0f;
    private bool EstabaEnSuelo;

    public AudioClip sonidoCaida;
    public AudioSource audioSource;
    public AudioClip jump_00;

    public Transform wallCheckLeft;
    public Transform wallCheckRight;
    public float wallCheckDistance = 0.2f;
    public LayerMask wallLayer;

    private bool isTouchingWall;

    private bool cargandoSalto = false;
    private float tiempoCarga = 0f;

    public float tiempoMaxCarga = 1f;
    public float fuerzaMaxima = 15f;
    public float fuerzaMinima = 5f;
    public float fuerzaRebote = 12f;

    public float maxFallSpeed = -10f;

    public Transform groundCheck;
    public float Radio = 0.3f;
    public LayerMask capaPiso;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Reiniciar el juego con R
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        EstabaEnSuelo = suelo;

        // Detecta si toca el suelo

        suelo = Physics2D.OverlapCircle(groundCheck.position, Radio, capaPiso);

        if (suelo)
        {
            animator.SetBool("IsGrounded", true);
            animator.SetBool("IsJumping", false);
            animator.SetBool("IsFalling", false);
        }
        else
        {
            animator.SetBool("IsGrounded", false);

            if (rb.linearVelocity.y > 0.1f)
            {
                animator.SetBool("IsJumping", true);
                animator.SetBool("IsFalling", false);
            }

            if (rb.linearVelocity.y < -0.1f)
            {
                animator.SetBool("IsFalling", true);
                animator.SetBool("IsJumping", false);
            }
        }

        if (!suelo && rb.linearVelocity.y > 0.01f)

        {
            animator.SetBool("IsJumping", true);
        }

        if (!EstabaEnSuelo && suelo)
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(sonidoCaida);
        }

        // Movimiento
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        if (suelo && !cargandoSalto)
        {
            moveInput = horizontalInput;
        }
        else
        {
            moveInput = 0;
        }

        // Animación correr
        animator.SetBool("IsRunning", moveInput != 0 && suelo);

        // INICIAR carga de salto
        if (suelo && Input.GetButtonDown("Jump"))
        {
            cargandoSalto = true;
            tiempoCarga = 0f;
            animator.SetBool("IsCharging", true);
        }

        // CARGAR salto
        if (cargandoSalto && Input.GetButton("Jump"))
        {
            tiempoCarga += Time.deltaTime;
            tiempoCarga = Mathf.Clamp(tiempoCarga, 0, tiempoMaxCarga);
        }

        // SOLTAR salto
        if (cargandoSalto && Input.GetButtonUp("Jump"))
        {

            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(jump_00);

            float fuerzaSalto = Mathf.Lerp(fuerzaMinima, fuerzaMaxima, tiempoCarga / tiempoMaxCarga);


            direccionSalto = Mathf.Sign(Input.GetAxisRaw("Horizontal"));
            rb.linearVelocity = new Vector2(direccionSalto * speed, fuerzaSalto);

            cargandoSalto = false;
            animator.SetBool("IsCharging", false);
            animator.SetBool("IsJumping", true);
            animator.SetBool("IsRunning", false);
        }

        Orientacion(horizontalInput);

        // Verificar contacto con paredes
        bool touchingLeft = Physics2D.Raycast(wallCheckLeft.position, Vector2.left, wallCheckDistance, wallLayer);
        bool touchingRight = Physics2D.Raycast(wallCheckRight.position, Vector2.right, wallCheckDistance, wallLayer);

        if ((touchingLeft || touchingRight) && Input.GetKeyDown(KeyCode.Space))
        {
            WallBounce(touchingLeft, touchingRight);
        }
    }

    void FixedUpdate()
    {
        if (suelo && !cargandoSalto)
        {
            rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);
        }

        // Limitar velocidad máxima de caída
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, maxFallSpeed));
    }

    void Orientacion(float moveInput)
    {
        if ((mirandoDerecha && moveInput < 0) || (!mirandoDerecha && moveInput > 0))
        {
            mirandoDerecha = !mirandoDerecha;
            transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
        }
    }
    void WallBounce(bool touchingLeft, bool touchingRight)
    {
        float incomingSpeed = Mathf.Abs(rb.linearVelocity.x);
        float bounceForce = Mathf.Clamp(10 - incomingSpeed, 4, 10);

        if (touchingLeft && moveInput > 0)
        {
            rb.linearVelocity = new Vector2(bounceForce, fuerzaMaxima * 0.8f);
        }
        else if (touchingRight && moveInput < 0)
        {
            rb.linearVelocity = new Vector2(-bounceForce, fuerzaMaxima * 0.8f);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, Radio);
        }

        if (wallCheckLeft != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(wallCheckLeft.position, wallCheckLeft.position + Vector3.left * wallCheckDistance);
        }

        if (wallCheckRight != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(wallCheckRight.position, wallCheckRight.position + Vector3.right * wallCheckDistance);
        }
    }
}