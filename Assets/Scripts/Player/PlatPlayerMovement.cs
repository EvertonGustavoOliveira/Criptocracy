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

    [Tooltip("Ângulo mínimo da normal do chão para considerar como 'chão' (0 = plano horizontal, 90 = parede vertical). 60 é um bom valor padrão.")]
    [SerializeField] private float anguloMinimoChao = 60f;

    [Header("Animação")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Rigidbody2D rb;
    private float horizontalInput;
    private bool isGrounded;
    private bool estaPreparandoPulo = false;
    
    // Rastreia se o player está tocando uma parede e de qual lado
    private bool tocandoParedeDireita = false;
    private bool tocandoParedeEsquerda = false;
    
    private PlayerPull scriptDePuxar; 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        
        isGrounded = true; 

        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        scriptDePuxar = GetComponent<PlayerPull>();
    }

    private bool ColisaoEhChao(Collision2D collision)
    {
        if (collision.otherCollider != coliserPe) return false;

        bool tagValida = collision.gameObject.CompareTag(groundTag) || 
                         collision.gameObject.CompareTag(boxTag);
        if (!tagValida) return false;

        foreach (ContactPoint2D contato in collision.contacts)
        {
            float angulo = Vector2.Angle(contato.normal, Vector2.up);
            if (angulo < anguloMinimoChao)
                return true;
        }

        return false;
    }

    // Verifica se a colisão é com uma parede (normal aproximadamente horizontal)
    private void VerificarParede(Collision2D collision)
    {
        bool tagValida = collision.gameObject.CompareTag(groundTag) || 
                         collision.gameObject.CompareTag(boxTag);
        if (!tagValida) return;

        foreach (ContactPoint2D contato in collision.contacts)
        {
            float angulo = Vector2.Angle(contato.normal, Vector2.up);
            // Ângulo próximo de 90° indica parede
            if (angulo >= anguloMinimoChao)
            {
                // Normal apontando para a direita = parede à esquerda do player
                if (contato.normal.x > 0.5f)
                    tocandoParedeEsquerda = true;
                // Normal apontando para a esquerda = parede à direita do player
                else if (contato.normal.x < -0.5f)
                    tocandoParedeDireita = true;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (ColisaoEhChao(collision))
        {
            isGrounded = true;
            if (animator != null) animator.SetFloat("yVelocity", 0);
        }
        VerificarParede(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (ColisaoEhChao(collision))
        {
            isGrounded = true;
        }
        VerificarParede(collision);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.otherCollider == coliserPe &&
            (collision.gameObject.CompareTag(groundTag) || collision.gameObject.CompareTag(boxTag)))
        {
            isGrounded = false;
        }

        // Reseta as flags de parede ao sair da colisão
        tocandoParedeDireita = false;
        tocandoParedeEsquerda = false;
    }

    void Update()
    {
        ManejarInput();
        ManejarAnimacoes();
        
        bool segurandoCaixa = (scriptDePuxar != null && scriptDePuxar.isHoldingBox);
        
        if (Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded && !estaPreparandoPulo && !segurandoCaixa)
        {
            StartCoroutine(SequenciaDePulo());
        }
    }

    void FixedUpdate()
    {
        float velocidadeHorizontal = horizontalInput * speed;

        // Bloqueia o movimento na direção da parede enquanto estiver no ar
        if (!isGrounded)
        {
            if (tocandoParedeDireita && velocidadeHorizontal > 0f)
                velocidadeHorizontal = 0f;
            if (tocandoParedeEsquerda && velocidadeHorizontal < 0f)
                velocidadeHorizontal = 0f;
        }

        rb.linearVelocity = new Vector2(velocidadeHorizontal, rb.linearVelocity.y);
        
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

        if (!segurandoCaixa)
        {
            if (horizontalInput > 0.1f)
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            else if (horizontalInput < -0.1f)
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    private IEnumerator SequenciaDePulo()
    {
        estaPreparandoPulo = true;

        if (animator != null)
            animator.SetTrigger("jumpTrigger");

        yield return new WaitForSeconds(tempoDeAgachamento);

        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        estaPreparandoPulo = false;
    }
}