using UnityEngine;

public class testInteraction : MonoBehaviour
{
    [SerializeField] private Color interactionColor = Color.green;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryColorPlayer(collision.collider);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryColorPlayer(other);
    }

    private void TryColorPlayer(Collider2D other)
    {
        if (other == null || !other.CompareTag("Player"))
        {
            return;
        }

        SpriteRenderer playerSprite = other.GetComponent<SpriteRenderer>();
        if (playerSprite != null)
        {
            playerSprite.color = interactionColor;
        }
    }
}
