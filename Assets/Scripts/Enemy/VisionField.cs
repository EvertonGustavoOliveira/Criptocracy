using UnityEngine;

public class VisionField : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GetComponentInParent<EnemyPatrol>().GameOver();
        }
    }
}