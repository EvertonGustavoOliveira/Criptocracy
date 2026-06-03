using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections; 

public class PlatPlayerMovement : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    [SerializeField] private float speed = 7f;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private string groundTag = "Chao";
    [SerializeField] private string boxTag = "Caixa";
    
    [Tooltip("Tempo em segundos do frame de agachamento na animação")]
    [SerializeField] private float tempoDeAgachamento = 0.15f; 

    [Header("Física e Colisores")]
    [Tooltip("Arraste aqui o colisor específico dos PÉS do seu Player")]
    [SerializeField] private Collider2D coliserPe; 

    [Header("Animação")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Rigidbody2D rb;
    private float horizontalInput;
    private bool isGrounded;
    private bool estaPreparandoPulo = false;
    
    // Referência para o script de puxar caixa
    private PlayerPull scriptDePuxar; 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        
        isGrounded = true; 

        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Pega o script PlayerPull automaticamente no início
        scriptDePuxar = GetComponent<PlayerPull>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.otherCollider == coliserPe && (collision.gameObject.CompareTag(groundTag) || collision.gameObject.CompareTag(boxTag))) 
        {
            isGrounded = true;
            if (animator != null) animator.SetFloat("yVelocity", 0);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.otherCollider == coliserPe && (collision.gameObject.CompareTag(groundTag) || collision.gameObject.CompareTag(boxTag))) 
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.otherCollider == coliserPe && collision.gameObject.CompareTag(groundTag)) 
        {
            isGrounded = false;
        }
    }

    void Update()
    {
        ManejarInput();
        ManejarAnimacoes();
        
        // Verifica se o script de puxar existe e se ele está segurando a caixa
        bool segurandoCaixa = (scriptDePuxar != null && scriptDePuxar.isHoldingBox);
        
        // Trava o pulo se estiver segurando a caixa (!segurandoCaixa)
        if (Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded && !estaPreparandoPulo && !segurandoCaixa)
        {
            StartCoroutine(SequenciaDePulo());
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(horizontalInput * speed, rb.linearVelocity.y);
        
        if (animator != null)
        {
            animator.SetFloat("yVelocity", rb.linearVelocity.y);
        }
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

        bool segurandoCaixa = (scriptDePuxar != null && scriptDePuxar.isHoldingBox);

        // Só vira o personagem automaticamente se não estiver segurando a caixa
        if (!segurandoCaixa)
        {
            if (horizontalInput > 0.1f)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else if (horizontalInput < -0.1f)
            {
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
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
}