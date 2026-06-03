using UnityEngine;
using UnityEngine.InputSystem;

public class VentilationLock : MonoBehaviour
{
    private bool playerInside = false;
    private PlayerInventory playerInventory;
    private bool ventilacaoAberta = false;

    [Header("Ação de Sucesso (Opcional)")]
    [Tooltip("Se você quiser que ele se teleporte ao abrir, arraste o script HatchTeleport aqui")]
    [SerializeField] private HatchTeleport scriptTeleporte;

    private void Update()
    {
        if (playerInside && !ventilacaoAberta && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            TentarEntrarNaVentilacao();
        }
    }

    private void TentarEntrarNaVentilacao()
    {
        // Checa se o inventário do player existe e se ele tem o papel
        if (playerInventory != null && playerInventory.temPapel)
        {
            Debug.Log("Você usou o papel e abriu a ventilação!");
            ventilacaoAberta = true;

            // Se você tiver um script de teleporte atrelado, ativa ele aqui
            // Chama a função pública do teleporte
            if (scriptTeleporte != null)
            {
                // Como o script pode estar desativado no Inspector, ativamos ele por 1 frame para garantir
                scriptTeleporte.enabled = true; 
                scriptTeleporte.IniciarTeleporteExterno(); 
            }
        }
        else
        {
            // Caso não tenha o papel, exibe a mensagem de aviso
            Debug.Log("Não posso ir ainda.");
            
            // DICA: Se você tiver um sistema de texto na tela (UI Text), 
            // você pode chamar a ativação dele exatamente nesta linha.
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            playerInventory = other.GetComponent<PlayerInventory>();
            Debug.Log("Pressione E para inspecionar a ventilação");
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