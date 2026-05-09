using UnityEngine;
using UnityEngine.InputSystem;

public class BarrelBehaviour : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private string hiddenTag = "Untagged";
    [SerializeField] private Animator barrelAnimator;
    [SerializeField] private float hideAnimDuration = 0.5f;
    [SerializeField] private float unhideAnimDuration = 0.5f;

    private GameObject playerInRange;
    private GameObject hiddenPlayerRef;
    private bool isPlayerHidden = false;
    private bool savedRigidbodySimulated;
    private Collider2D[] cachedColliders = new Collider2D[0];
    private bool[] cachedColliderStates = new bool[0];
    private SpriteRenderer[] cachedRenderers = new SpriteRenderer[0];
    private bool[] cachedRendererStates = new bool[0];

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = other.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (playerInRange != null && other.gameObject == playerInRange)
        {
            playerInRange = null;
        }
    }

    void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (!keyboard.iKey.wasPressedThisFrame) return;

        if (isPlayerHidden && hiddenPlayerRef != null)
        {
            StartUnhideSequence(hiddenPlayerRef);
            return;
        }

        if (playerInRange != null)
        {
            StartHideSequence(playerInRange);
        }
    }

    private void StartHideSequence(GameObject player)
    {
        if (barrelAnimator != null)
        {
            barrelAnimator.SetBool("isHiding", true);
            barrelAnimator.SetBool("isUnhiding", false);
        }
        StartCoroutine(HideAfterDelay(player, hideAnimDuration));
    }

    private System.Collections.IEnumerator HideAfterDelay(GameObject player, float delay)
    {
        yield return new WaitForSeconds(delay);
        PerformHide(player);
        if (barrelAnimator != null)
        {
            barrelAnimator.SetBool("isHiding", false);
        }
    }

    private void PerformHide(GameObject player)
    {
        isPlayerHidden = true;
        hiddenPlayerRef = player;

        var movement = player.GetComponent<PlatPlayerMovement>();
        if (movement != null) movement.enabled = false;

        cachedColliders = player.GetComponentsInChildren<Collider2D>(true);
        cachedColliderStates = new bool[cachedColliders.Length];
        for (int i = 0; i < cachedColliders.Length; i++)
        {
            cachedColliderStates[i] = cachedColliders[i].enabled;
            cachedColliders[i].enabled = false;
        }

        cachedRenderers = player.GetComponentsInChildren<SpriteRenderer>(true);
        cachedRendererStates = new bool[cachedRenderers.Length];
        for (int i = 0; i < cachedRenderers.Length; i++)
        {
            cachedRendererStates[i] = cachedRenderers[i].enabled;
            cachedRenderers[i].enabled = false;
        }

        var rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            savedRigidbodySimulated = rb.simulated;
            rb.simulated = false;
        }

        player.tag = hiddenTag;
    }

    private void StartUnhideSequence(GameObject player)
    {
        if (barrelAnimator != null)
        {
            barrelAnimator.SetBool("isUnhiding", true);
            barrelAnimator.SetBool("isHiding", false);
        }
        StartCoroutine(UnhideAfterDelay(player, unhideAnimDuration));
    }

    private System.Collections.IEnumerator UnhideAfterDelay(GameObject player, float delay)
    {
        yield return new WaitForSeconds(delay);
        PerformUnhide(player);
        if (barrelAnimator != null)
        {
            barrelAnimator.SetBool("isUnhiding", false);
        }
    }

    private void PerformUnhide(GameObject player)
    {
        isPlayerHidden = false;
        hiddenPlayerRef = null;

        var movement = player.GetComponent<PlatPlayerMovement>();
        if (movement != null) movement.enabled = true;

        for (int i = 0; i < cachedColliders.Length; i++)
        {
            if (cachedColliders[i] != null)
            {
                cachedColliders[i].enabled = cachedColliderStates[i];
            }
        }

        for (int i = 0; i < cachedRenderers.Length; i++)
        {
            if (cachedRenderers[i] != null)
            {
                cachedRenderers[i].enabled = cachedRendererStates[i];
            }
        }

        var rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.simulated = savedRigidbodySimulated;
        }

        player.tag = playerTag;
    }
}
