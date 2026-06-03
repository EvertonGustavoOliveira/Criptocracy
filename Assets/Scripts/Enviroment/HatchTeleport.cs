using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using Unity.Cinemachine;

public class HatchTeleport : MonoBehaviour
{
    [Header("Identificação do Checkpoint")]
    [SerializeField] private int numeroDaAreaDestino;

    public int NumeroDaAreaDestino => numeroDaAreaDestino;

    [Header("Teleport")]
    [SerializeField] private Transform destination;

    [Header("Gerenciamento de Áreas")]
    [SerializeField] private GameObject areaAtual;
    [SerializeField] private GameObject areaDestino;

    [Header("Gerenciamento de Câmeras (Cinemachine)")]
    [SerializeField] private CinemachineCamera cameraAtual;
    [SerializeField] private CinemachineCamera cameraDestino;

    private Transform player;
    private bool playerInside;
    private bool isTeleporting;
    private CinemachineBrain cameraBrain;

    private void Start()
    {
        if (Camera.main != null)
            cameraBrain = Camera.main.GetComponent<CinemachineBrain>();
    }

    // CORREÇÃO PRINCIPAL: Mesma lógica do DoorTeleport — ativa o cenário de forma
    // síncrona e delega a coroutine de câmera para um objeto garantidamente ativo.
    public void ConfigurarCenarioNoNascimento(GameObject playerObj)
    {
        // 1. Ativa/desativa áreas imediatamente (síncrono)
        if (areaDestino != null) areaDestino.SetActive(true);
        if (areaAtual != null && areaAtual != areaDestino) areaAtual.SetActive(false);

        // 2. Prioridades de câmera imediatamente
        if (cameraDestino != null) cameraDestino.Priority = 30;
        if (cameraAtual != null && cameraAtual != cameraDestino) cameraAtual.Priority = 0;

        // 3. Delega coroutine para o GameManager (que é garantidamente ativo)
        GameManager gm = FindAnyObjectByType<GameManager>();
        if (gm != null)
        {
            gm.StartCoroutine(RotinaRespawnCamera(playerObj));
        }
        else
        {
            if (playerObj != null) playerObj.SetActive(true);
        }
    }

    private IEnumerator RotinaRespawnCamera(GameObject playerObj)
    {
        if (cameraBrain == null && Camera.main != null)
            cameraBrain = Camera.main.GetComponent<CinemachineBrain>();

        if (cameraBrain != null) cameraBrain.enabled = false;

        yield return new WaitForEndOfFrame();

        if (Camera.main != null && destination != null)
        {
            Camera.main.transform.position = new Vector3(
                destination.position.x,
                destination.position.y,
                Camera.main.transform.position.z
            );
        }

        if (cameraBrain != null) cameraBrain.enabled = true;

        yield return new WaitForEndOfFrame();

        // Reativa o player só depois do cenário e câmera estarem prontos
        if (playerObj != null) playerObj.SetActive(true);

        Debug.Log("[HATCH] Respawn configurado com sucesso.");
    }

    private void Update()
    {
        if (playerInside && !isTeleporting &&
            Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            SalvarEsteCheckpoint();
            StartCoroutine(TeleportRoutine());
        }
    }

    private void SalvarEsteCheckpoint()
    {
        if (destination != null)
        {
            PlayerPrefs.SetFloat("CheckpointX", destination.position.x);
            PlayerPrefs.SetFloat("CheckpointY", destination.position.y);
        }
        else
        {
            PlayerPrefs.SetFloat("CheckpointX", transform.position.x);
            PlayerPrefs.SetFloat("CheckpointY", transform.position.y);
        }

        PlayerPrefs.SetInt("TemCheckpoint", 1);
        PlayerPrefs.SetInt("UltimaAreaSalva", numeroDaAreaDestino);
        PlayerPrefs.Save();

        Debug.Log($"[CHECKPOINT HATCH] Salvo na Área: {numeroDaAreaDestino}");
    }

    public void IniciarTeleporteExterno()
    {
        if (!isTeleporting) StartCoroutine(TeleportRoutine());
    }

    private IEnumerator TeleportRoutine()
    {
        if (player == null || destination == null || FadeController.Instance == null) yield break;

        isTeleporting = true;
        yield return StartCoroutine(FadeController.Instance.FadeOut(0.3f));

        if (cameraBrain != null) cameraBrain.enabled = false;
        if (areaDestino != null) areaDestino.SetActive(true);

        if (cameraAtual != null && cameraDestino != null && cameraAtual != cameraDestino)
        {
            cameraAtual.Priority = 0;
            cameraDestino.Priority = 10;
        }

        player.position = destination.position;

        if (Camera.main != null)
        {
            Camera.main.transform.position = new Vector3(
                destination.position.x,
                destination.position.y,
                Camera.main.transform.position.z
            );
        }

        yield return new WaitForEndOfFrame();
        if (cameraBrain != null) cameraBrain.enabled = true;
        yield return StartCoroutine(FadeController.Instance.FadeIn(0.3f));

        if (areaAtual != null && areaAtual != areaDestino) areaAtual.SetActive(false);

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
        if (other.CompareTag("Player")) playerInside = false;
    }
}