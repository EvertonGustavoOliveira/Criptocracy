using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using Unity.Cinemachine; 

public class HatchTeleport : MonoBehaviour
{
    [Header("Teleport")]
    [SerializeField] private Transform destination;

    [Header("Gerenciamento de Áreas (Fases/Salas)")]
    [SerializeField] private GameObject areaAtual;
    [SerializeField] private GameObject areaDestino;

    [Header("Gerenciamento de Câmeras (Cinemachine)")]
    [SerializeField] private CinemachineCamera cameraAtual;
    [SerializeField] private CinemachineCamera cameraDestino;

    private Transform player;
    private bool playerInside;
    private bool isTeleporting;
    private CinemachineBrain cameraBrain;

    private void Update()
    {
        if (playerInside &&
            !isTeleporting &&
            Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame)
        {
            StartCoroutine(TeleportRoutine());
        }
    }

    // MÉTODO PÚBLICO NOVO: Permite que a Ventilação dispare o teleporte com segurança
    public void IniciarTeleporteExterno()
    {
        if (!isTeleporting)
        {
            StartCoroutine(TeleportRoutine());
        }
    }

    private IEnumerator TeleportRoutine()
    {
        if (player == null || destination == null || FadeController.Instance == null)
        {
            Debug.LogError("Faltam configurações essenciais no script de teleporte.");
            yield break;
        }

        isTeleporting = true;

        yield return StartCoroutine(FadeController.Instance.FadeOut(0.3f));

        // CORREÇÃO: Busca o Cérebro do Cinemachine na hora H, ignorando se o Start() rodou ou não
        if (cameraBrain == null && Camera.main != null)
        {
            cameraBrain = Camera.main.GetComponent<CinemachineBrain>();
        }

        // Desativa o deslize visual
        if (cameraBrain != null) cameraBrain.enabled = false;

        if (areaDestino != null) areaDestino.SetActive(true);

        if (cameraAtual != null && cameraDestino != null)
        {
            cameraAtual.Priority = 0;      
            cameraDestino.Priority = 10;   
        }

        player.position = destination.position;

        if (Camera.main != null)
        {
            Camera.main.transform.position = new Vector3(destination.position.x, destination.position.y, Camera.main.transform.position.z);
        }

        yield return new WaitForEndOfFrame();
        
        if (cameraBrain != null) cameraBrain.enabled = true;

        yield return StartCoroutine(FadeController.Instance.FadeIn(0.3f));

        if (areaAtual != null) areaAtual.SetActive(false);

        yield return new WaitForSeconds(0.5f);
        isTeleporting = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            player = other.transform;
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