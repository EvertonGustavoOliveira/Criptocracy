using UnityEngine;
using UnityEngine.InputSystem;

public class BarrelBehaviour : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private string hiddenTag = "Untagged";
    [SerializeField] private Animator barrelAnimator;

    [Header("Configurações de Tempo Manual")]
    [Tooltip("Tempo exato (em segundos) que a animação de SAÍDA demora para fechar o barril completamente.")]
    [SerializeField] private float unhideDuration = 0.6f; 

    private GameObject playerInRange;
    private GameObject hiddenPlayerRef;
    private bool isPlayerHidden = false;
    private bool isTransitioning = false; 
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
        if (keyboard == null || isTransitioning) return; 

        if (!keyboard.iKey.wasPressedThisFrame) return;

        if (isPlayerHidden && hiddenPlayerRef != null)
        {
            StartUnhideSequence();
            return;
        }

        if (playerInRange != null)
        {
            StartHideSequence(playerInRange);
        }
    }

    private void StartHideSequence(GameObject player)
    {
        isTransitioning = true;
        
        var movement = player.GetComponent<PlatPlayerMovement>();
        if (movement != null) movement.enabled = false;

        hiddenPlayerRef = player;

        if (barrelAnimator != null)
        {
            barrelAnimator.SetBool("isHiding", true);
            barrelAnimator.SetBool("isUnhiding", false);
        }
    }

    // Mantido via Animation Event na animação de ENTRADA (Hiding)
    public void PerformHide()
    {
        if (hiddenPlayerRef == null) return;

        isPlayerHidden = true;

        cachedColliders = hiddenPlayerRef.GetComponentsInChildren<Collider2D>(true);
        cachedColliderStates = new bool[cachedColliders.Length];
        for (int i = 0; i < cachedColliders.Length; i++)
        {
            cachedColliderStates[i] = cachedColliders[i].enabled;
            cachedColliders[i].enabled = false;
        }

        cachedRenderers = hiddenPlayerRef.GetComponentsInChildren<SpriteRenderer>(true);
        cachedRendererStates = new bool[cachedRenderers.Length];
        for (int i = 0; i < cachedRenderers.Length; i++)
        {
            cachedRendererStates[i] = cachedRenderers[i].enabled;
            cachedRenderers[i].enabled = false;
        }

        var rb = hiddenPlayerRef.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            savedRigidbodySimulated = rb.simulated;
            rb.simulated = false;
        }

        hiddenPlayerRef.tag = hiddenTag;

        if (barrelAnimator != null)
        {
            barrelAnimator.SetBool("isHiding", false);
        }
        
        isTransitioning = false; 
    }

    private void StartUnhideSequence()
    {
        isTransitioning = true;
        
        if (barrelAnimator != null)
        {
            barrelAnimator.SetBool("isUnhiding", true);
            barrelAnimator.SetBool("isHiding", false);
        }
        
        // Dispara a rotina com base no tempo manual estrito que você colocar no Inspector
        StartCoroutine(WaitForUnhideManual(unhideDuration));
    }

    private System.Collections.IEnumerator WaitForUnhideManual(float delay)
    {
        // Garante a espera absoluta do tempo estipulado, independente do estado do Animator
        yield return new WaitForSeconds(delay);

        PerformUnhide();
    }

    private void PerformUnhide()
    {
        if (hiddenPlayerRef == null) return;

        // 1. Reseta os parâmetros do Animator PRIMEIRO para sumir com o frame antigo do barril
        if (barrelAnimator != null)
        {
            barrelAnimator.SetBool("isUnhiding", false);
            barrelAnimator.SetBool("isHiding", false);
        }

        // 2. Só agora devolve os Sprites e Colisores do Personagem
        for (int i = 0; i < cachedColliders.Length; i++)
        {
            if (cachedColliders[i] != null) cachedColliders[i].enabled = cachedColliderStates[i];
        }

        for (int i = 0; i < cachedRenderers.Length; i++)
        {
            if (cachedRenderers[i] != null) cachedRenderers[i].enabled = cachedRendererStates[i];
        }

        var rb = hiddenPlayerRef.GetComponent<Rigidbody2D>();
        if (rb != null) rb.simulated = savedRigidbodySimulated;

        var movement = hiddenPlayerRef.GetComponent<PlatPlayerMovement>();
        if (movement != null) movement.enabled = true;

        hiddenPlayerRef.tag = playerTag;

        isPlayerHidden = false;
        hiddenPlayerRef = null;
        
        isTransitioning = false; 
    }
}