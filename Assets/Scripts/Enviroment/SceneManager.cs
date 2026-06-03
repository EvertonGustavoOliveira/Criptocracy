using UnityEngine;
using UnityEngine.SceneManagement; // Obrigatório para trocar de cena

public class ChangeSceneOnCollide : MonoBehaviour
{
    [Tooltip("Digite o nome EXATO da cena do menu aqui")]
    [SerializeField] private string nomeDoMenu = "MenuFases";

    // Detecta quando o Player encosta no colisor (Precisa estar marcado como Is Trigger)
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica se quem bateu foi o Player pela Tag
        if (other.CompareTag("Player"))
        {
            // Carrega a cena do menu na hora
            SceneManager.LoadScene(nomeDoMenu);
        }
    }
}