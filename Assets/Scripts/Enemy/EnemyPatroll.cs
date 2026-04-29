using UnityEngine;
using System.Collections;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Configurações de Patrulha")]
    [SerializeField] private float speed = 3f;
    [SerializeField] private float waitBeforeTurn = 2f;
    [SerializeField] private float waitAfterTurn = 1f;

    [Header("Animação")]
    [SerializeField] private Animator animator;

    private Rigidbody2D rb;
    private bool isWaiting = false;
    private int direction = 1; 

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
            animator.SetBool("isWalking", !isWaiting);
        }
    }

    void FixedUpdate()
    {
        if (!isWaiting)
        {
            rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
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

        float escalaX = Mathf.Abs(transform.localScale.x) * direction;
        transform.localScale = new Vector3(escalaX, transform.localScale.y, transform.localScale.z);

        yield return new WaitForSeconds(waitAfterTurn);

        isWaiting = false;
    }
    public void GameOver()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

}