using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeManager : MonoBehaviour
{
    public Image fadeImage;
    public float fadeSpeed = 2f;

    private void Awake()
    {
        if (fadeImage != null)
            fadeImage.color = new Color(0, 0, 0, 0);
    }

    public void FadeOutInCinematic(System.Action midAction)
    {
        StartCoroutine(FadeRoutine(midAction));
    }

    private IEnumerator FadeRoutine(System.Action midAction)
    {
        float alpha = 0f;
        while (alpha < 1f)
        {
            alpha += Time.deltaTime * fadeSpeed;
            fadeImage.color = new Color(0, 0, 0, Mathf.Clamp01(alpha));
            yield return null;
        }

        midAction?.Invoke();

        while (alpha > 0f)
        {
            alpha -= Time.deltaTime * fadeSpeed;
            fadeImage.color = new Color(0, 0, 0, Mathf.Clamp01(alpha));
            yield return null;
        }
    }
}