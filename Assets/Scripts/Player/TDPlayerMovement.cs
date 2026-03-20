using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color alternateColor = Color.red;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Vector2 input;
    private bool isAlternateColor;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            spriteRenderer.color = defaultColor;
        }
        else
        {
            Debug.LogWarning("SpriteRenderer nao encontrado no PlayerMovement.");
        }
    }

    void Update()
    {
        var keyboard = Keyboard.current;

        if (keyboard == null)
        {
            input = Vector2.zero;
            return;
        }

        input = Vector2.zero;

        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) input.x -= 1f;
        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) input.x += 1f;
        if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) input.y -= 1f;
        if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) input.y += 1f;

        input.Normalize();

        if (keyboard.iKey.wasPressedThisFrame)
        {
            TogglePlayerColor();
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = input * speed;
    }

    private void TogglePlayerColor()
    {
        if (spriteRenderer == null)
        {
            return;
        }

        isAlternateColor = !isAlternateColor;
        spriteRenderer.color = isAlternateColor ? alternateColor : defaultColor;
    }
}
