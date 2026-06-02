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

        // 1. Fecha a tela completamente (Blackout)
        yield return StartCoroutine(FadeController.Instance.FadeOut(0.3f));

        // 2. Desativa o Cinemachine Brain para evitar o deslize visual
        if (cameraBrain != null) cameraBrain.enabled = false;

        // 3. ATIVA a nova área (os inimigos aparecem nos postos deles)
        if (areaDestino != null)
        {
            areaDestino.SetActive(true);
        }

        // 4. Troca as prioridades das câmeras virtuais
        if (cameraAtual != null && cameraDestino != null)
        {
            cameraAtual.Priority = 0;      
            cameraDestino.Priority = 10;   
        }

        // 5. Teleporta o jogador para o destino
        player.position = destination.position;

        // 6. Move a câmera principal fisicamente para o destino
        if (Camera.main != null)
        {
            Camera.main.transform.position = new Vector3(destination.position.x, destination.position.y, Camera.main.transform.position.z);
        }

        // 7. Espera um frame para a Unity estabilizar a nova física e posicionar a câmera
        yield return new WaitForEndOfFrame();
        if (cameraBrain != null) cameraBrain.enabled = true;

        // 8. CRUCIAL: Abre a tela PRIMEIRO, enquanto este script ainda está ativo e vivo
        yield return StartCoroutine(FadeController.Instance.FadeIn(0.3f));

        // 9. SÓ AGORA desativamos a área antiga com segurança
        if (areaAtual != null)
        {
            areaAtual.SetActive(false);
        }

        // Cooldown final
        yield return new WaitForSeconds(0.2f);
        isTeleporting = false;
        yield break;
    }
}