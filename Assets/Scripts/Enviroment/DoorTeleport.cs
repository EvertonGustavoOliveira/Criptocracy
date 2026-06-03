using UnityEngine;
using System.Collections;
using Unity.Cinemachine;

public class DoorTeleport : MonoBehaviour
{
    [Header("Identificação do Checkpoint")]
    [Tooltip("Número da área para onde este teleporte leva")]
    [SerializeField] private int numeroDaAreaDestino;

    public int NumeroDaAreaDestino => numeroDaAreaDestino;

    [Header("Configurações de Teleporte")]
    [SerializeField] private Transform destination;

    [Header("Gerenciamento de Áreas")]
    [SerializeField] private GameObject areaAtual;
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
            cameraBrain = Camera.main.GetComponent<CinemachineBrain>();
    }

    // CORREÇÃO PRINCIPAL: Recebe o player como parâmetro e usa um MonoBehaviour
    // ativo (o próprio GameManager via FindAnyObjectByType) para rodar a coroutine,
    // já que este GameObject pode estar inativo quando chamado no respawn.
    public void ConfigurarCenarioNoNascimento(GameObject playerObj)
    {
        // 1. Ativa a área de destino imediatamente (síncrono, funciona mesmo inativo)
        if (areaDestino != null) areaDestino.SetActive(true);
        if (areaAtual != null && areaAtual != areaDestino) areaAtual.SetActive(false);

        // 2. Configura as prioridades de câmera imediatamente
        if (cameraDestino != null) cameraDestino.Priority = 30;
        if (cameraAtual != null && cameraAtual != cameraDestino) cameraAtual.Priority = 0;

        // 3. Delega a coroutine de câmera e reativação do player para um objeto ativo
        GameManager gm = FindAnyObjectByType<GameManager>();
        if (gm != null)
        {
            gm.StartCoroutine(RotinaRespawnCamera(playerObj));
        }
        else
        {
            // Fallback caso o GameManager não seja encontrado
            if (playerObj != null) playerObj.SetActive(true);
        }
    }

    private IEnumerator RotinaRespawnCamera(GameObject playerObj)
    {
        // Busca o CinemachineBrain aqui pois o Start pode não ter rodado ainda
        if (cameraBrain == null && Camera.main != null)
            cameraBrain = Camera.main.GetComponent<CinemachineBrain>();

        // Desativa o brain para o corte seco não ser interpolado
        if (cameraBrain != null) cameraBrain.enabled = false;

        yield return new WaitForEndOfFrame();

        // Corte seco: move a câmera física para o destino
        if (Camera.main != null && destination != null)
        {
            Camera.main.transform.position = new Vector3(
                destination.position.x,
                destination.position.y,
                Camera.main.transform.position.z
            );
        }

        // Reativa o brain — agora ele vai seguir o cameraDestino sem interpolar do ponto antigo
        if (cameraBrain != null) cameraBrain.enabled = true;

        yield return new WaitForEndOfFrame();

        // Só agora reativa o player, com o chão já existindo e a câmera no lugar certo
        if (playerObj != null) playerObj.SetActive(true);

        Debug.Log("[DOOR] Respawn configurado com sucesso.");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isTeleporting)
        {
            player = other.transform;
            SalvarEsteCheckpoint();
            StartCoroutine(TeleportRoutine());
        }
    }

    private void SalvarEsteCheckpoint()
    {
        PlayerPrefs.SetFloat("CheckpointX", transform.position.x);
        PlayerPrefs.SetFloat("CheckpointY", transform.position.y);
        PlayerPrefs.SetInt("TemCheckpoint", 1);
        PlayerPrefs.SetInt("UltimaAreaSalva", numeroDaAreaDestino);
        PlayerPrefs.Save();
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

        yield return new WaitForSeconds(0.2f);
        isTeleporting = false;
    }
}