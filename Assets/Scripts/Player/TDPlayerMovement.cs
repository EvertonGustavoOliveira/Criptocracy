using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement; 

public class PlayerMovement : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private string menuSceneName = "MenuFases"; 

    [SerializeField] private Animator animator;
    [Header("Cores")]
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color alternateColor = Color.red;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Vector2 input;
    private bool isAlternateColor;
    private bool isGrounded;
    private bool isMenuScene;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb.freezeRotation = true;

        isMenuScene = SceneManager.GetActiveScene().name == menuSceneName;

        if (isMenuScene)
        {
            rb.gravityScale = 0f; 
        }
        else
        {
            rb.gravityScale = 3f; 
        }

        if (spriteRenderer != null) spriteRenderer.color = defaultColor;
    }

    void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        input = Vector2.zero;

        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) {
            animator.SetBool("isRunning", true);
            input.x -= 1f;
        }


        
        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) {
            animator.SetBool("isRunning", true);
            input.x += 1f;
        }
        if (isMenuScene)
        {
            if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) input.y -= 1f;
            if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) input.y += 1f;
            input.Normalize();
        }
        else
        {
            if (keyboard.spaceKey.wasPressedThisFrame && isGrounded)
            {
                Jump();
            }
        }

        if (keyboard.iKey.wasPressedThisFrame) TogglePlayerColor();
    }

    void FixedUpdate()
    {
        if (isMenuScene)
        {
            rb.linearVelocity = input * speed;
        }
        else
        {
            rb.linearVelocity = new Vector2(input.x * speed, rb.linearVelocity.y);
        }
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f); // Reseta o Y para pulo consistente
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void TogglePlayerColor()
    {
        if (spriteRenderer == null) return;
        isAlternateColor = !isAlternateColor;
        spriteRenderer.color = isAlternateColor ? alternateColor : defaultColor;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isMenuScene && collision.gameObject.CompareTag("Chao")) isGrounded = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!isMenuScene && collision.gameObject.CompareTag("Chao")) isGrounded = false;
    }
}