using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica se foi o jogador que entrou no Checkpoint
        if (other.CompareTag("Player"))
        {
            // Salva as coordenadas X e Y deste objeto na memória do jogo
            PlayerPrefs.SetFloat("CheckpointX", transform.position.x);
            PlayerPrefs.SetFloat("CheckpointY", transform.position.y);
            
            // Cria uma chave dizendo que o jogador já ativou pelo menos um checkpoint
            PlayerPrefs.SetInt("TemCheckpoint", 1);
            PlayerPrefs.Save();

            Debug.Log($"[CHECKPOINT] Ponto salvo com sucesso em: {transform.position}");
            
            // DICA: Se quiser rodar uma animação de bandeira subindo ou luz acendendo, chame aqui!
        }
    }
}