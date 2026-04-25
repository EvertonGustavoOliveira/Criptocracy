using UnityEngine;
using UnityEngine.InputSystem;

public class PlatPlayerMovement : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private string groundTag = "Chao";

    [Header("Animação")]
    [SerializeField] private Animator animator;

    private Rigidbody2D rb;
    private float horizontalInput;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
    }

    void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        horizontalInput = 0f;

        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) horizontalInput -= 1f;
        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) horizontalInput += 1f;

        if (animator != null)
        {
            animator.SetBool("isRunning", horizontalInput > 0.01f);
            animator.SetBool("isRunningL", horizontalInput < -0.01f);
        }

        if (keyboard.spaceKey.wasPressedThisFrame && isGrounded)
        {
            Jump();
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(horizontalInput * speed, rb.linearVelocity.y);
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(groundTag)) isGrounded = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(groundTag)) isGrounded = false;
    }
}
