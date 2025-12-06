using UnityEngine;
using System.Collections;

public class FadeToggle : MonoBehaviour
{
    public float fadeDuration = 0.5f;
    public float visibleAlpha = 94f / 255f; 

    private Renderer rend;
    private Material instancedMat;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        if (rend != null)
        {
            // Create a unique material instance so we can modify alpha
            instancedMat = new Material(rend.material);
            rend.material = instancedMat;
        }
    }

    public void FadeIn()
    {
        gameObject.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(Fade(0f, visibleAlpha));
    }

    public void FadeOut()
    {
        StopAllCoroutines();
        StartCoroutine(Fade(visibleAlpha, 0f, disableAfter: true));
    }

    IEnumerator Fade(float start, float end, bool disableAfter = false)
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(start, end, t / fadeDuration);

            SetAlpha(a);
            yield return null;
        }

        SetAlpha(end);

        if (disableAfter)
            gameObject.SetActive(false);
    }

    void SetAlpha(float a)
    {
        if (instancedMat != null)
        {
            Color c = instancedMat.color;
            c.a = a;
            instancedMat.color = c;
        }
    }
}
