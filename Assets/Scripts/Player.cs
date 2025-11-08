using UnityEngine;

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

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Movimiento horizontal
        move = Input.GetAxis("Horizontal");
        rb2D.linearVelocity = new Vector2(move * speed, rb2D.linearVelocity.y);

        // Voltear el sprite según la dirección
        if (move != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(move), 1, 1);
        }

        // Salto (FUERA del if de movimiento)
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, jumpForce);
        }
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
    }
}