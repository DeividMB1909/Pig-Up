using UnityEngine;

public class EnemySandbag : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float velocidad = 2f;
    [SerializeField] private float distanciaPatrulla = 5f;

    [Header("Daño")]
    [SerializeField] private float tiempoEntreDaños = 1.5f;

    private Vector2 puntoInicial;
    private float limiteIzquierdo;
    private float limiteDerecho;
    private int direccion = 1; // 1 = derecha, -1 = izquierda
    private float siguienteDaño;

    private Rigidbody2D rb2D;
    private Animator animator;
    private Transform cachedTransform;

    private void Awake()
    {
        // Cachear componentes en Awake para mejor rendimiento
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        cachedTransform = transform;
    }

    private void Start()
    {
        InicializarPatrulla();
    }

    private void FixedUpdate()
    {
        // Usar FixedUpdate para movimiento físico consistente
        Patrullar();
    }

    private void InicializarPatrulla()
    {
        puntoInicial = cachedTransform.position;
        limiteIzquierdo = puntoInicial.x - distanciaPatrulla;
        limiteDerecho = puntoInicial.x + distanciaPatrulla;
    }

    private void Patrullar()
    {
        // Aplicar velocidad constante en la dirección actual
        rb2D.linearVelocity = new Vector2(velocidad * direccion, rb2D.linearVelocity.y);

        float posX = cachedTransform.position.x;

        // Verificar límites y cambiar dirección si es necesario
        if ((direccion > 0 && posX >= limiteDerecho) ||
            (direccion < 0 && posX <= limiteIzquierdo))
        {
            CambiarDireccion();
        }
    }

    private void CambiarDireccion()
    {
        direccion *= -1;

        // Voltear el sprite: 1 para derecha, -1 para izquierda
        Vector3 escala = cachedTransform.localScale;
        escala.x = Mathf.Abs(escala.x) * direccion;
        cachedTransform.localScale = escala;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // Verificar colisión con el jugador
        if (collision.gameObject.CompareTag("Player"))
        {
            AplicarDañoAlJugador(collision.gameObject);
        }
    }

    private void AplicarDañoAlJugador(GameObject jugador)
    {
        // Solo aplicar daño si ha pasado el tiempo necesario
        if (Time.time < siguienteDaño) return;

        Player player = jugador.GetComponent<Player>();
        if (player != null)
        {
            player.RecibirDaño();
            siguienteDaño = Time.time + tiempoEntreDaños;

            // Opcional: trigger de animación de ataque
            if (animator != null)
            {
                animator.SetTrigger("Atacar");
            }
        }
    }

    // Método público para recibir daño (útil si agregas sistema de combate)
    public void RecibirDaño(int cantidad = 1)
    {
        // Aquí puedes implementar lógica de vida del enemigo
        if (animator != null)
        {
            animator.SetTrigger("Golpeado");
        }

        // Ejemplo: Destroy(gameObject); // Si muere en un golpe
    }

    private void OnDrawGizmosSelected()
    {
        Vector2 inicio = Application.isPlaying ? puntoInicial : (Vector2)transform.position;

        // Línea de patrulla
        Gizmos.color = Color.cyan;
        Vector2 izq = inicio - Vector2.right * distanciaPatrulla;
        Vector2 der = inicio + Vector2.right * distanciaPatrulla;

        Gizmos.DrawLine(izq, der);

        // Esferas en los límites
        Gizmos.DrawWireSphere(izq, 0.3f);
        Gizmos.DrawWireSphere(der, 0.3f);

        // Punto inicial
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(inicio, 0.2f);
    }
}
