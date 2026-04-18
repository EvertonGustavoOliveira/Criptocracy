using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Configurações de Patrulha")]
    [SerializeField] private float speed = 3f;
    [SerializeField] private float waitBeforeTurn = 2f;
    [SerializeField] private float waitAfterTurn = 1f;

    private Rigidbody2D rb;
    private bool isWaiting = false;
    private int direction = 1; 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true; 
    }

    void FixedUpdate()
    {
        if (!isWaiting)
        {
            rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);
        }
        else
        {
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

        yield return new WaitForSeconds(waitBeforeTurn);
        
        direction *= -1;

        if (direction == -1)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        yield return new WaitForSeconds(waitAfterTurn);

        isWaiting = false;
    }

    public void GameOver()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}