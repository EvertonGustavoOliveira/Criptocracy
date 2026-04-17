using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPlatformer : MonoBehaviour
{
    [Header("Configurações")]
    [SerializeField] private float speed = 8f;
    [SerializeField] private float jumpForce = 12f;
    
    private Rigidbody2D rb;
    private float moveX;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true; 
        
        // Garante que a gravidade esteja ativa para o modo plataforma
        rb.gravityScale = 3f; 
    }

    void Update()
    {
        if (Keyboard.current != null)
        {
            // Movimento Horizontal
            float left = Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed ? 1f : 0f;
            float right = Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed ? 1f : 0f;
            moveX = right - left;

            // Pulo
            if (Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded)
            {
                Jump();
            }
        }
    }

    void FixedUpdate()
    {
        // No Unity 2026, usamos velocity (ou linearVelocity dependendo da versão específica)
        rb.linearVelocity = new Vector2(moveX * speed, rb.linearVelocity.y);
    }

    void Jump()
    {
        // Zera a velocidade vertical antes de pular para o pulo ser sempre consistente
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    // --- SÓ PODE TER UM DE CADA ABAIXO ---

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Chao"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Chao"))
        {
            isGrounded = false;
        }
    }
}