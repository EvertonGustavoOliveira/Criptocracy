using UnityEngine;
using UnityEngine.SceneManagement; 

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private string sceneToLoad; 

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            LoadNextScene();
        }
    }

    public void LoadNextScene()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}