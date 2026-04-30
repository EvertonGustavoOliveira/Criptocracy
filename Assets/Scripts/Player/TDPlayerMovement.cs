using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private string menuSceneName = "MenuFases";

    [Header("Animação")]
    [SerializeField] private Animator animator;

    [Header("Cores")]
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color alternateColor = Color.red;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Vector2 input;
    private bool isAlternateColor;
    private bool isMenuScene;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb.freezeRotation = true;

        if (animator == null) animator = GetComponent<Animator>();

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
        ManejarInput();
        ManejarAnimacoes();
    }

    private void ManejarInput()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        input = Vector2.zero;

        if (!isMenuScene) return;

        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) input.x -= 1f;
        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) input.x += 1f;
        if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) input.y -= 1f;
        if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) input.y += 1f;
        
        input.Normalize();

        if (keyboard.iKey.wasPressedThisFrame) TogglePlayerColor();
    }

    private void ManejarAnimacoes()
    {
        if (animator == null) return;

        bool movendo = input.sqrMagnitude > 0.01f;
        animator.SetBool("isRunning", movendo);

        if (input.x > 0.1f)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (input.x < -0.1f)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    void FixedUpdate()
    {
        if (isMenuScene)
        {
            rb.linearVelocity = input * speed;
        }
    }

    private void TogglePlayerColor()
    {
        if (spriteRenderer == null) return;
        isAlternateColor = !isAlternateColor;
        spriteRenderer.color = isAlternateColor ? alternateColor : defaultColor;
    }
}