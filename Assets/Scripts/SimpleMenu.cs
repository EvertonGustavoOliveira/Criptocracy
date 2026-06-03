using UnityEngine;
using UnityEngine.SceneManagement; // Necessário para trocar de cena

public class SimpleMenu : MonoBehaviour
{
    [Header("Configuração")]
    [Tooltip("Digite o nome EXATO da cena do seu jogo aqui")]
    [SerializeField] private string nomeDaCenaJogo = "Fase 1";

    // Chamado pelo botão Iniciar / Jogar
    public void IniciarJogo()
    {
        // Carrega a cena do gameplay direto
        SceneManager.LoadScene(nomeDaCenaJogo);
    }

    // Chamado pelo botão Sair
    public void SairDoJogo()
    {
        Debug.Log("Jogador saiu do jogo.");
        Application.Quit(); // Fecha o executável buildado
    }
}