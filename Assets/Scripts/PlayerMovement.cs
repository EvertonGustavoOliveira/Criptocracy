using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    float speed = 5f;
    private Rigidbody2D rb;
    private Vector2 input;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        input.x = Input.GetAxis("Horizontal");
        input.y = Input.GetAxis("Vertical");

        input.Normalize();
    }

    void FixedUpdate()
    {
        rb.linearVelocity = input * speed;
    }
}
