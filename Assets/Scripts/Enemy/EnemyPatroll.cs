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
    [Tooltip("Arraste aqui o objeto do 'inimigo_farelo' que JÁ ESTÁ na cena (desativado)")]
    [SerializeField] private GameObject inimigoFareloObj;

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
        if (isStationary || isWaiting)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);
        }
    }

    // Usado para bater em Paredes Sólidas do cenário
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // LOG 1: Mostra tudo o que o corpo SÓLIDO do inimigo está esbarrando
        Debug.Log($"[SÓLIDO] Inimigo trombou em: {collision.gameObject.name} | Tag: {collision.gameObject.tag}");

        if (!isStationary && !isWaiting)
        {
            if (collision.gameObject.CompareTag("ParedeInimigo") || collision.gameObject.CompareTag("Parede"))
            {
                Debug.Log("Parede detectada na colisão sólida! Iniciando curva...");
                StartCoroutine(ChangeDirectionRoutine());
            }
        }
    }
    
    // MÁGICA NOVA: Detecta a caixa passando por dentro dele (precisa que um dos colisores seja Trigger)
    private void OnTriggerEnter2D(Collider2D other)
    {
        // LOG 2: Mostra tudo o que entrou no sensor (Trigger) do inimigo
        Debug.Log($"[TRIGGER] Algo entrou no sensor fantasma do Inimigo: {other.name} | Tag: {other.tag}");

        if (other.CompareTag("Caixa"))
        {
            Rigidbody2D caixaRb = other.GetComponent<Rigidbody2D>();
            
            if (caixaRb != null)
            {
                // LOG 3: Fofoca os números exatos da caixa na hora que ela encostou
                Debug.Log($"[CAIXA DETECTADA] Altura da Caixa: {other.transform.position.y} | Altura Inimigo: {transform.position.y} | Velocidade Y da Caixa: {caixaRb.linearVelocity.y}");

                // Se a caixa estiver mais alta que o centro do inimigo E estiver caindo (velocidade Y negativa)
                if (other.transform.position.y > transform.position.y && caixaRb.linearVelocity.y < -0.1f)
                {
                    Debug.Log(">>> CONDIÇÃO DE MORTE ATENDIDA! Esmagando inimigo... <<<");
                    MorrerEsmagado(other.gameObject);
                }
                else
                {
                    Debug.Log(">>> CAIXA IGNORADA! A caixa encostou, mas ou não estava caindo (velocidade Y insuficiente) ou não bateu por cima. <<<");
                }
            }
            else
            {
                Debug.LogWarning("ERRO: A caixa que tocou no inimigo não tem um Rigidbody2D!");
            }
        }
    }

    private void MorrerEsmagado(GameObject caixa)
    {
        if (inimigoFareloObj != null)
        {
            inimigoFareloObj.SetActive(true);
        }

        Destroy(caixa);
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