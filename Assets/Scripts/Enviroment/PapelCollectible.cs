using UnityEngine;
using UnityEngine.InputSystem;

public class PaperCollectible : MonoBehaviour
{
    [Header("Visual Feedback")]
    [Tooltip("O objeto/sprite que vai SUMIR (ex: o próprio papel)")]
    [SerializeField] private GameObject objetoParaEsconder;

    [Tooltip("O objeto/sprite que vai APARECER (ex: uma poça de sangue revelada, ou nada)")]
    [SerializeField] private GameObject objetoParaMostrar;

    private bool playerInside = false;
    private PlayerInventory playerInventory;

    private void Update()
    {
        if (playerInside && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            ColetarPapel();
        }
    }

    private void ColetarPapel()
    {
        if (playerInventory != null)
        {
            playerInventory.temPapel = true;
            Debug.Log("Papel coletado!");
        }

        if (objetoParaEsconder != null) objetoParaEsconder.SetActive(false);
        if (objetoParaMostrar != null) objetoParaMostrar.SetActive(true);

        if (GetComponent<Collider2D>() != null)
        {
            GetComponent<Collider2D>().enabled = false;
        }

        playerInside = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            playerInventory = other.GetComponent<PlayerInventory>();
            Debug.Log("Pressione E para pegar o papel");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
        }
    }
}