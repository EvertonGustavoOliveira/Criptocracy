using UnityEngine;
using System.Collections;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Configurações de Patrulha")]
    [Tooltip("Marque para o inimigo ficar parado no lugar")]
    [SerializeField] private bool isStationary = false; 
    [SerializeField] private float speed = 3f;
    [SerializeField] private float waitBeforeTurn = 2f;
    [SerializeField] private float waitAfterTurn = 1f;

    [Header("Morte por Esmagamento")]
    [Tooltip("Coloque aqui o prefab/objeto do inimigo morto que vai aparecer no chão")]
    [SerializeField] private GameObject deadEnemyPrefab;

    [Header("Animação")]
    [SerializeField] private Animator animator;

    private Rigidbody2D rb;
    private bool isWaiting = false;
    
    [SerializeField] private int direction = 1; 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true; 

        if (animator == null) animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (animator != null)
        {
            // Se for estático, a animação de andar fica sempre falsa (ele fica em Idle)
            if (isStationary)
            {
                animator.SetBool("isWalking", false);
            }
            else
            {
                animator.SetBool("isWalking", !isWaiting);
            }
        }
    }

    void FixedUpdate()
    {
        // Se for estático ou estiver esperando, zera a velocidade horizontal
        if (isStationary || isWaiting)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 1. CHECAGEM DE MORTE PELA CAIXA
        if (collision.gameObject.CompareTag("Caixa"))
        {
            // Analisa os pontos de contato da colisão
            foreach (ContactPoint2D ponto in collision.contacts)
            {
                // Se a direção do impacto (normal) estiver apontando para baixo,
                // significa que a caixa caiu em cima da cabeça do inimigo
                if (ponto.normal.y < -0.5f)
                {
                    MorrerEsmagado();
                    return; // Interrompe o resto do código para ele não tentar virar
                }
            }
        }

        // 2. LÓGICA NORMAL DE PATRULHA (Só acontece se ele não for estático)
        if (!isStationary && !isWaiting)
        {
            if (collision.gameObject.CompareTag("ParedeInimigo") || collision.gameObject.CompareTag("Parede"))
            {
                StartCoroutine(ChangeDirectionRoutine());
            }
        }
    }
    
    private void MorrerEsmagado()
    {
        // Cria o corpo morto exatamente na mesma posição e rotação que o inimigo está agora
        if (deadEnemyPrefab != null)
        {
            Instantiate(deadEnemyPrefab, transform.position, transform.rotation);
        }

        // Destrói o objeto deste inimigo vivo
        Destroy(gameObject);
    }

    private IEnumerator ChangeDirectionRoutine()
    {
        isWaiting = true;
        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(waitBeforeTurn);
        
        direction *= -1;
        
        Flip();

        yield return new WaitForSeconds(waitAfterTurn);

        isWaiting = false;
    }

    private void Flip()
    {
        Vector3 currentScale = transform.localScale;
        currentScale.x *= -1; 
        transform.localScale = currentScale;
    }

    public void GameOver()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}