using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Configurações de Patrulha")]
    [SerializeField] private float speed = 3f;
    [SerializeField] private float waitTime = 2f;
    
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private bool isWaiting = false;
    private int direction = 1; 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    void FixedUpdate()
    {
        if (!isWaiting)
        {
            // Movimenta o inimigo sempre para a frente baseada na direction
            rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);
        }
        else
        {
            // Para completamente enquanto espera
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Parede") && !isWaiting)
        {
            StartCoroutine(ChangeDirectionRoutine());
        }
    }

    private IEnumerator ChangeDirectionRoutine()
    {
        isWaiting = true;
        
        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(waitTime);
        
        direction *= -1;

        transform.localScale = new Vector3(direction, 1, 1);

        isWaiting = false;
    }

    public void GameOver()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}