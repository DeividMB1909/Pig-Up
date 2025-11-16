using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Player : MonoBehaviour
{
    [Header("Movimiento")]
    public float speed = 5;
    public float jumpForce = 4;

    [Header("Detección de Suelo")]
    public Transform groundCheck;
    public float groundRadius = 0.1f;
    public LayerMask groundLayer;

    [Header("UI y Sistema")]
    public TMP_Text textCoins;
    public float tiempoInvulnerable = 1.5f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip coinClip;
    public AudioClip barrelClip;

    // Componentes
    private Rigidbody2D rb2D;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private SistemaVidas sistemaVidas;

    // Variables de estado
    private float move;
    private bool isGrounded;
    private int coins;
    private bool esInvulnerable = false;

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        sistemaVidas = FindObjectOfType<SistemaVidas>();

        // Ajustar interpolación para movimiento más suave
        rb2D.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    void Update()
    {
        // Movimiento horizontal
        move = Input.GetAxis("Horizontal");

        // Voltear sprite
        if (move != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(move), 1, 1);
        }

        // SALTO
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, jumpForce);
        }

        // Cancelar salto si se suelta el botón
        if (Input.GetButtonUp("Jump") && rb2D.linearVelocity.y > 0f)
        {
            rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, rb2D.linearVelocity.y * 0.5f);
        }

        // Parámetros del animator
        animator.SetFloat("Speed", Mathf.Abs(move));
        animator.SetFloat("VerticalVelocity", rb2D.linearVelocity.y);
        animator.SetBool("IsGrounded", isGrounded);
    }

    private void FixedUpdate()
    {
        // Detectar suelo
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);

        // Movimiento horizontal
        rb2D.linearVelocity = new Vector2(move * speed, rb2D.linearVelocity.y);
    }

    public int GetCoins()
    {
        return coins;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Coin"))
        {
            // Audio de moneda
            if (audioSource != null && coinClip != null)
            {
                audioSource.PlayOneShot(coinClip);
            }

            Destroy(collision.gameObject);
            coins++;
            textCoins.text = coins.ToString();
        }

        // DAÑO POR PINCHOS
        if (collision.CompareTag("Spikes"))
        {
            if (!esInvulnerable)
            {
                RecibirDaño();
            }
        }

        if (collision.CompareTag("Barrel"))
        {
            // Audio de barril
            if (audioSource != null && barrelClip != null)
            {
                audioSource.PlayOneShot(barrelClip);
            }

            // Knockback
            Vector2 knockbackDir = (rb2D.position - (Vector2)collision.transform.position).normalized;
            rb2D.linearVelocity = Vector2.zero;
            rb2D.AddForce(knockbackDir * 3, ForceMode2D.Impulse);

            // Destruir barril
            BoxCollider2D[] colliders = collision.gameObject.GetComponents<BoxCollider2D>();
            foreach (BoxCollider2D col in colliders)
            {
                col.enabled = false;
            }

            Animator barrelAnimator = collision.GetComponent<Animator>();
            if (barrelAnimator != null)
            {
                barrelAnimator.enabled = true;
            }

            Destroy(collision.gameObject, 0.5f);
        }
    }

    public void RecibirDaño()
    {
        if (sistemaVidas != null)
        {
            sistemaVidas.PerderVida();

            // Si aún tiene vidas, activar invulnerabilidad temporal
            if (sistemaVidas.vidasActuales > 0)
            {
                StartCoroutine(InvulnerabilidadTemporal());
            }
            else
            {
                // Game Over - reiniciar escena
                Invoke("ReiniciarEscena", 1f);
            }
        }
        else
        {
            // Si no hay sistema de vidas, reiniciar directamente (comportamiento original)
            ReiniciarEscena();
        }
    }

    IEnumerator InvulnerabilidadTemporal()
    {
        esInvulnerable = true;

        // Efecto de parpadeo
        if (spriteRenderer != null)
        {
            for (float i = 0; i < tiempoInvulnerable; i += 0.1f)
            {
                spriteRenderer.enabled = !spriteRenderer.enabled;
                yield return new WaitForSeconds(0.1f);
            }
            spriteRenderer.enabled = true;
        }

        esInvulnerable = false;
    }

    void ReiniciarEscena()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Visualizar en el editor
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
        }
    }
}