using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    private void Start()
    {
        if (PlayerPrefs.HasKey("TemCheckpoint") && PlayerPrefs.GetInt("TemCheckpoint") == 1)
        {
            float x = PlayerPrefs.GetFloat("CheckpointX");
            float y = PlayerPrefs.GetFloat("CheckpointY");
            int areaSalva = PlayerPrefs.GetInt("UltimaAreaSalva");

            // CORREÇÃO: O player é desativado aqui para não cair no vazio enquanto
            // a área de destino ainda não foi ativada. Ele será reativado pelo
            // ConfigurarCenarioNoNascimento() após o cenário estar pronto.
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                player.SetActive(false);
                player.transform.position = new Vector2(x, y);
            }

            // Procura TODAS as portas, incluindo as que estão em áreas desativadas
            DoorTeleport[] todasAsPortas = FindObjectsByType<DoorTeleport>(
                FindObjectsInactive.Include, FindObjectsSortMode.None);

            foreach (DoorTeleport door in todasAsPortas)
            {
                if (door.NumeroDaAreaDestino == areaSalva)
                {
                    door.ConfigurarCenarioNoNascimento(player);
                    Debug.Log($"[GAMEMANAGER] Porta da Área {areaSalva} encontrada e configurada.");
                    return;
                }
            }

            // Procura TODOS os hatches, incluindo os que estão em áreas desativadas
            HatchTeleport[] todosOsHatches = FindObjectsByType<HatchTeleport>(
                FindObjectsInactive.Include, FindObjectsSortMode.None);

            foreach (HatchTeleport hatch in todosOsHatches)
            {
                if (hatch.NumeroDaAreaDestino == areaSalva)
                {
                    hatch.ConfigurarCenarioNoNascimento(player);
                    Debug.Log($"[GAMEMANAGER] Hatch da Área {areaSalva} encontrado e configurado.");
                    return;
                }
            }

            // Fallback: se não achou nenhum teleporte, ao menos reativa o player
            if (player != null) player.SetActive(true);
            Debug.LogError($"[GAMEMANAGER] ERRO: Nenhum teleporte encontrado com Área {areaSalva}!");
        }
    }
}