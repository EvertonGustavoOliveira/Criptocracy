using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class FadeController : MonoBehaviour
{
    public static FadeController Instance;
    [SerializeField] private Image fadeImage;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        Instance = this;
    }

    public IEnumerator FadeOut(float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            Color color = fadeImage.color;
            color.a = Mathf.Lerp(0f, 1f, timer / duration);
            fadeImage.color = color;

            yield return null;
        }
    }

    public IEnumerator FadeIn(float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            Color color = fadeImage.color;
            color.a = Mathf.Lerp(1f, 0f, timer / duration);
            fadeImage.color = color;

            yield return null;
        }
    }
}
