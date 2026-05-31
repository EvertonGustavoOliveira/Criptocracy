using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class HatchTeleport : MonoBehaviour
{
    [Header("Teleport")]
    [SerializeField] private Transform destination;

    private Transform player;
    private bool playerInside;
    private bool isTeleporting;

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

    private IEnumerator TeleportRoutine()
    {
        if (player == null)
        {
            Debug.LogError("Player não encontrado.");
            yield break;
        }

        if (destination == null)
        {
            Debug.LogError("Destination não configurado.");
            yield break;
        }

        if (FadeController.Instance == null)
        {
            Debug.LogError("FadeController não encontrado.");
            yield break;
        }

        isTeleporting = true;

        yield return StartCoroutine(
            FadeController.Instance.FadeOut(0.3f)
        );

        // Teleporta o jogador
        player.position = destination.position;

        yield return new WaitForSeconds(0.1f);

        yield return StartCoroutine(
            FadeController.Instance.FadeIn(0.3f)
        );

        // Pequeno cooldown para evitar reativação imediata
        yield return new WaitForSeconds(0.5f);

        isTeleporting = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            player = other.transform;

            Debug.Log("Pressione E para entrar");
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