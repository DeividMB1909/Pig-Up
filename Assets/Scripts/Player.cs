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

    private Rigidbody2D rb2D;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private SistemaVidas sistemaVidas;

    private float move;
    private bool isGrounded;
    private int coins;
    private bool esInvulnerable = false;
    private bool estaMuerto = false;

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        sistemaVidas = FindObjectOfType<SistemaVidas>();

        rb2D.interpolation = RigidbodyInterpolation2D.Interpolate;
        ActualizarUI();
    }

    void Update()
    {
        if (estaMuerto) return;

        move = Input.GetAxis("Horizontal");

        if (move != 0)
            transform.localScale = new Vector3(Mathf.Sign(move), 1, 1);

        if (Input.GetButtonDown("Jump") && isGrounded)
            rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, jumpForce);

        animator.SetFloat("Speed", Mathf.Abs(move));
        animator.SetFloat("VerticalVelocity", rb2D.linearVelocity.y);
        animator.SetBool("IsGrounded", isGrounded);
    }

    void FixedUpdate()
    {
        if (estaMuerto)
        {
            rb2D.linearVelocity = Vector2.zero;
            return;
        }

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
        rb2D.linearVelocity = new Vector2(move * speed, rb2D.linearVelocity.y);
    }

    public void RecibirDaño()
    {
        if (estaMuerto || esInvulnerable) return;

        sistemaVidas.PerderVida();
        ActualizarUI();

        if (sistemaVidas.vidasActuales <= 0)
        {
            Morir();
        }
        else
        {
            animator.SetTrigger("Hit");
            StartCoroutine(InvulnerabilidadTemporal());
        }
    }

    void Morir()
    {
        estaMuerto = true;

        rb2D.linearVelocity = Vector2.zero;
        rb2D.bodyType = RigidbodyType2D.Kinematic;

        foreach (var col in GetComponents<Collider2D>())
            col.enabled = false;

        animator.SetTrigger("Die");
        Invoke("ReiniciarEscena", 1.2f);
    }

    IEnumerator InvulnerabilidadTemporal()
    {
        esInvulnerable = true;

        for (float t = 0; t < tiempoInvulnerable; t += 0.1f)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(0.1f);
        }

        spriteRenderer.enabled = true;
        esInvulnerable = false;
    }

    void ActualizarUI()
    {
        if (textCoins != null)
            textCoins.text = coins.ToString();
    }

    void ReiniciarEscena()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Monedas
        if (other.CompareTag("Coin"))
        {
            coins++;
            ActualizarUI();
            audioSource.PlayOneShot(coinClip);
            Destroy(other.gameObject);
        }

        // Pinchos
        if (other.CompareTag("Spikes"))
        {
            RecibirDaño();
        }

        // Barriles Restaurado
        if (other.CompareTag("Barrel"))
        {
            audioSource.PlayOneShot(barrelClip);

            Animator barrelAnimator = other.GetComponent<Animator>();
            if (barrelAnimator != null)
                barrelAnimator.enabled = true;

            // Rebote / knockback pequeño
            rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, 2f);

            // Quitar colisiones
            foreach (var c in other.GetComponents<Collider2D>())
                c.enabled = false;

            Destroy(other.gameObject, 0.4f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
    }
}
