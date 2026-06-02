using UnityEngine;
using System.Collections;
using Unity.Cinemachine;

public class DoorTeleport : MonoBehaviour
{
    [Header("Configurações de Teleporte")]
    [SerializeField] private Transform destination;

    [Header("Gerenciamento de Áreas (Fases/Salas)")]
    [Tooltip("O objeto pai que segura todos os inimigos/luzes da sala ATUAL")]
    [SerializeField] private GameObject areaAtual;
    
    [Tooltip("O objeto pai que segura todos os inimigos/luzes da PRÓXIMA sala")]
    [SerializeField] private GameObject areaDestino;

    [Header("Gerenciamento de Câmeras (Cinemachine)")]
    [SerializeField] private CinemachineCamera cameraAtual;
    [SerializeField] private CinemachineCamera cameraDestino;

    private Transform player;
    private bool isTeleporting = false;
    private CinemachineBrain cameraBrain;

    private void Start()
    {
        if (Camera.main != null)
        {
            cameraBrain = Camera.main.GetComponent<CinemachineBrain>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isTeleporting)
        {
            player = other.transform;
            StartCoroutine(TeleportRoutine());
        }
    }

    private IEnumerator TeleportRoutine()
    {
        if (player == null || destination == null || FadeController.Instance == null)
        {
            Debug.LogError("Faltam configurações essenciais no script.");
            yield break;
        }

        isTeleporting = true;

        // 1. Fecha a tela
        yield return StartCoroutine(FadeController.Instance.FadeOut(0.3f));

        // 2. Desativa o Cinemachine Brain para evitar o deslize
        if (cameraBrain != null) cameraBrain.enabled = false;

        // 3. ATIVA a nova área antes do player chegar (assim os inimigos nascem nos lugares certos)
        if (areaDestino != null)
        {
            areaDestino.SetActive(true);
        }

        // 4. Troca as prioridades das câmeras
        if (cameraAtual != null && cameraDestino != null)
        {
            cameraAtual.Priority = 0;      
            cameraDestino.Priority = 10;   
        }

        // 5. Teleporta o jogador
        player.position = destination.position;

        // 6. Move a câmera principal para o destino
        if (Camera.main != null)
        {
            Camera.main.transform.position = new Vector3(destination.position.x, destination.position.y, Camera.main.transform.position.z);
        }

        // 7. DESATIVA a área antiga (economiza memória e processamento)
        if (areaAtual != null)
        {
            areaAtual.SetActive(false);
        }

        // 8. Espera o frame renderizar e liga o Cinemachine de volta
        yield return new WaitForEndOfFrame();
        if (cameraBrain != null) cameraBrain.enabled = true;

        yield return new WaitForSeconds(0.05f);

        // 9. Abre a tela
        yield return StartCoroutine(FadeController.Instance.FadeIn(0.3f));

        yield return new WaitForSeconds(0.5f);
        isTeleporting = false;
    }
}