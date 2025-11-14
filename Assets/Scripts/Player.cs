using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; // ← ESTA LÍNEA FALTABA

public class Player : MonoBehaviour
{
    public float speed = 5;
    private Rigidbody2D rb2D;
    private float move;
    public float jumpForce = 4;
    private bool isGrounded;
    public Transform groundCheck;
    public float groundRadius = 0.1f;
    public LayerMask groundLayer;
    private Animator animator;
    private int coins;
    public TMP_Text textCoins;

    // Sistema de vidas
    private SistemaVidas sistemaVidas;
    public float tiempoInvulnerable = 1.5f;
    private bool esInvulnerable = false;

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sistemaVidas = FindObjectOfType<SistemaVidas>();
    }

    void Update()
    {
        // Movimiento horizontal
        move = Input.GetAxis("Horizontal");
        rb2D.linearVelocity = new Vector2(move * speed, rb2D.linearVelocity.y);

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

        // Parámetros del animator
        animator.SetFloat("Speed", Mathf.Abs(move));
        animator.SetFloat("VerticalVelocity", rb2D.linearVelocity.y);
        animator.SetBool("IsGrounded", isGrounded);
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
    }

    public int GetCoins()
    {
        return coins;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Coin"))
        {
            Destroy(collision.gameObject);
            coins++;
            textCoins.text = coins.ToString();
        }

        // DAÑO POR PINCHOS
        if (collision.transform.CompareTag("Spikes"))
        {
            if (!esInvulnerable)
            {
                RecibirDaño();
            }
        }

        if (collision.transform.CompareTag("Barrel"))
        {
            Vector2 knockbackDir = (rb2D.position - (Vector2)collision.transform.position).normalized;
            rb2D.linearVelocity = Vector2.zero;
            rb2D.AddForce(knockbackDir * 3, ForceMode2D.Impulse);

            BoxCollider2D[] colliders = collision.gameObject.GetComponents<BoxCollider2D>();
            foreach (BoxCollider2D col in colliders)
            {
                col.enabled = false;
            }
            collision.GetComponent<Animator>().enabled = true;
            Destroy(collision.gameObject, 0.5f);

            // DAÑO POR BARRIL
            if (!esInvulnerable)
            {
                RecibirDaño();
            }
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
            // Si no hay sistema de vidas, reiniciar directamente
            ReiniciarEscena();
        }
    }

    IEnumerator InvulnerabilidadTemporal()
    {
        esInvulnerable = true;

        // Efecto de parpadeo
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        for (float i = 0; i < tiempoInvulnerable; i += 0.1f)
        {
            sprite.enabled = !sprite.enabled;
            yield return new WaitForSeconds(0.1f);
        }

        sprite.enabled = true;
        esInvulnerable = false;
    }

    void ReiniciarEscena()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
