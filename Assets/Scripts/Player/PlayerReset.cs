using UnityEngine;

public class PlayerReset : MonoBehaviour
{
    // Uma variável estática não é destruída e nem resetada quando a cena recarrega!
    private static bool jaResetouNoPlay = false;

    void Awake()
    {
        // Se já resetou uma vez desde que o Play foi dado, não faz nada e para aqui
        if (jaResetouNoPlay) return;

        // Se for a primeira vez que o jogo liga (o primeiro frame do Play)
        PlayerPrefs.DeleteKey("TemCheckpoint");
        PlayerPrefs.DeleteKey("CheckpointX");
        PlayerPrefs.DeleteKey("CheckpointY");
        PlayerPrefs.Save();
        
        // Marca como verdadeiro para que o código nunca mais rode até você parar o jogo e dar Play de novo
        jaResetouNoPlay = true;

        Debug.Log("[DEV] Memória de Checkpoints resetada apenas para o início do Play.");
    }
}