using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class PlatPlayerMovement : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    [SerializeField] private float speed = 7f;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private string groundTag = "Chao";
    [SerializeField] private string wallTag = "Parede";

    [Tooltip("Tempo em segundos do frame de agachamento na animação")]
    [SerializeField] private float tempoDeAgachamento = 0.15f;

    [Header("Física e Colisores")]
    [Tooltip("Arraste aqui o colisor específico dos PÉS do seu Player")]
    [SerializeField] private Collider2D feetCollider;
    [SerializeField] private float tempoParaResetarOffsetDaPlataforma = 0.8f;
    [SerializeField] private float wallCheckDistance = 0.08f;

    [Header("Animação")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Rigidbody2D rb;
    private readonly List<Collider2D> bodyColliders = new List<Collider2D>();
    private readonly Dictionary<Collider2D, int> wallContacts = new Dictionary<Collider2D, int>();
    private float horizontalInput;
    private bool isGrounded;
    private bool isTouchingWallLeft;
    private bool isTouchingWallRight;
    private bool isTouchingWall;
    private bool estaPreparandoPulo = false;
    private PlatformEffector2D activePlatformEffector;
    private Collider2D activePlatformCollider;
    private Coroutine dropThroughCoroutine;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.freezeRotation = true;
        }
        else
        {
            Debug.LogWarning("PlatPlayerMovement: Rigidbody2D not found on GameObject.", this);
        }
    }

    void Start()
    {
        bodyColliders.Clear();
        wallContacts.Clear();

        Collider2D[] colliders = GetComponentsInChildren<Collider2D>(true);
        for (int i = 0; i < colliders.Length; i++)
        {
            Collider2D collider = colliders[i];
            if (collider != null && !collider.isTrigger && collider != feetCollider && !bodyColliders.Contains(collider))
            {
                bodyColliders.Add(collider);
            }
        }

        isGrounded = false;

        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();

        if (feetCollider == null)
        {
            Debug.LogWarning("PlatPlayerMovement: 'feetCollider' (feet collider) is not assigned in the Inspector.", this);
        }
    }

    private bool IsGroundTrigger(Collider2D other)
    {
        return feetCollider != null && other != null && (other.CompareTag(groundTag) || IsOneWayPlatform(other));
    }

    private bool IsOneWayPlatform(Collider2D other)
    {
        return other != null && other.GetComponent<PlatformEffector2D>() != null;
    }

    private bool IsWall(Collider2D other)
    {
        return other != null && other.CompareTag(wallTag);
    }

    private int GetWallSide(Collision2D collision)
    {
        if (collision == null || collision.contactCount == 0)
        {
            return 0;
        }

        ContactPoint2D contact = collision.GetContact(0);
        if (Mathf.Abs(contact.normal.x) <= Mathf.Abs(contact.normal.y))
        {
            return 0;
        }

        return contact.normal.x > 0f ? -1 : 1;
    }

    private void RefreshWallState()
    {
        isTouchingWallLeft = false;
        isTouchingWallRight = false;

        foreach (int side in wallContacts.Values)
        {
            if (side < 0)
            {
                isTouchingWallLeft = true;
            }
            else if (side > 0)
            {
                isTouchingWallRight = true;
            }
        }

        isTouchingWall = isTouchingWallLeft || isTouchingWallRight;
    }

    private void RegisterWallCollision(Collision2D collision)
    {
        if (collision == null || collision.collider == null)
        {
            return;
        }

        int wallSide = GetWallSide(collision);
        if (wallSide == 0)
        {
            return;
        }

        wallContacts[collision.collider] = wallSide;
        RefreshWallState();
    }

    private void ClearWallCollision(Collider2D other)
    {
        if (other == null)
        {
            return;
        }

        if (wallContacts.Remove(other))
        {
            RefreshWallState();
        }
    }

    private void RegisterPlatformCollision(Collider2D other)
    {
        if (other == null || bodyColliders.Count == 0)
        {
            return;
        }

        PlatformEffector2D platformEffector = other.GetComponent<PlatformEffector2D>();
        if (platformEffector == null)
        {
            return;
        }

        activePlatformEffector = platformEffector;
        activePlatformCollider = other;
    }

    private void ClearPlatformCollision(Collider2D other)
    {
        if (other == null)
        {
            return;
        }

        PlatformEffector2D platformEffector = other.GetComponent<PlatformEffector2D>();
        if (platformEffector != null && activePlatformEffector == platformEffector)
        {
            activePlatformEffector = null;
            activePlatformCollider = null;
        }
    }

    private void SetBodyCollisionWithPlatform(Collider2D platformCollider, bool ignore)
    {
        if (platformCollider == null)
        {
            return;
        }

        for (int i = 0; i < bodyColliders.Count; i++)
        {
            Collider2D bodyCollider = bodyColliders[i];
            if (bodyCollider != null)
            {
                Physics2D.IgnoreCollision(bodyCollider, platformCollider, ignore);
            }
        }
    }

    private void SetGrounded(bool grounded)
    {
        if (isGrounded == grounded)
        {
            return;
        }

        isGrounded = grounded;

        if (!isGrounded && animator != null)
        {
            animator.SetFloat("yVelocity", rb != null ? rb.linearVelocity.y : 0f);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsGroundTrigger(other))
        {
            SetGrounded(true);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (IsGroundTrigger(other))
        {
            SetGrounded(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (IsGroundTrigger(other))
        {
            SetGrounded(false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.otherCollider != null && bodyColliders.Contains(collision.otherCollider))
        {
            if (IsWall(collision.collider))
            {
                RegisterWallCollision(collision);
            }

            RegisterPlatformCollision(collision.collider);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.otherCollider != null && bodyColliders.Contains(collision.otherCollider))
        {
            if (IsWall(collision.collider))
            {
                RegisterWallCollision(collision);
            }

            RegisterPlatformCollision(collision.collider);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.otherCollider != null && bodyColliders.Contains(collision.otherCollider))
        {
            if (IsWall(collision.collider))
            {
                ClearWallCollision(collision.collider);
            }

            ClearPlatformCollision(collision.collider);
        }
    }

    void Update()
    {
        ManejarInput();
        ManejarAnimacoes();

        if (!estaPreparandoPulo && activePlatformEffector != null)
        {
            var keyboard = Keyboard.current;
            if (keyboard != null && isGrounded && (keyboard.sKey.wasPressedThisFrame || keyboard.downArrowKey.wasPressedThisFrame))
            {
                if (dropThroughCoroutine == null)
                {
                    dropThroughCoroutine = StartCoroutine(DropThroughPlatform());
                }
            }
        }

        var currentKeyboard = Keyboard.current;
        if (currentKeyboard != null && currentKeyboard.spaceKey.wasPressedThisFrame && isGrounded && !estaPreparandoPulo)
        {
            StartCoroutine(SequenciaDePulo());
        }
    }

    void FixedUpdate()
    {
        if (rb == null) return;

        float moveInput = horizontalInput;
        if (moveInput != 0f && IsWallAhead(moveInput))
        {
            moveInput = 0f;
        }

        rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);

        if (animator != null)
        {
            animator.SetFloat("yVelocity", rb.linearVelocity.y);
        }
    }

    private bool IsWallAhead(float moveInput)
    {
        if (rb == null || bodyColliders.Count == 0 || Mathf.Approximately(moveInput, 0f))
        {
            return false;
        }

        Vector2 direction = moveInput > 0f ? Vector2.right : Vector2.left;
        ContactFilter2D filter = new ContactFilter2D
        {
            useTriggers = false,
            useLayerMask = false
        };

        RaycastHit2D[] hits = new RaycastHit2D[8];
        int hitCount = rb.Cast(direction, filter, hits, wallCheckDistance);

        for (int i = 0; i < hitCount; i++)
        {
            RaycastHit2D hit = hits[i];
            if (hit.collider != null && IsWall(hit.collider))
            {
                return true;
            }
        }

        return false;
    }

    private void ManejarInput()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        horizontalInput = 0f;
        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) horizontalInput -= 1f;
        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) horizontalInput += 1f;
    }

    private void ManejarAnimacoes()
    {
        if (animator == null) return;

        animator.SetBool("isRunning", Mathf.Abs(horizontalInput) > 0.01f);
        animator.SetBool("isGrounded", isGrounded);

        if (horizontalInput > 0.1f)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (horizontalInput < -0.1f)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    private IEnumerator SequenciaDePulo()
    {
        estaPreparandoPulo = true;

        if (animator != null)
        {
            animator.SetTrigger("jumpTrigger");
        }

        yield return new WaitForSeconds(tempoDeAgachamento);

        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        estaPreparandoPulo = false;
    }

    private IEnumerator DropThroughPlatform()
    {
        if (activePlatformEffector == null || activePlatformCollider == null)
        {
            dropThroughCoroutine = null;
            yield break;
        }

        PlatformEffector2D platformEffector = activePlatformEffector;
        Collider2D platformCollider = activePlatformCollider;

        platformEffector.surfaceArc = 180f;
        SetBodyCollisionWithPlatform(platformCollider, true);

        yield return new WaitForSeconds(tempoParaResetarOffsetDaPlataforma);

        if (platformEffector != null)
        {
            platformEffector.surfaceArc = 180f;
        }

        SetBodyCollisionWithPlatform(platformCollider, false);

        if (activePlatformEffector == platformEffector)
        {
            activePlatformEffector = null;
            activePlatformCollider = null;
        }

        dropThroughCoroutine = null;
    }
}