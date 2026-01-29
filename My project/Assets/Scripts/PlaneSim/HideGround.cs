using UnityEngine;

public class CinematicGroundHide : MonoBehaviour
{
    public Transform plane;
    public GameObject groundRoot;

    [Header("Cameras")]
    public Camera gameplayCam;
    public Camera skyCam;

    [Header("Heights")]
    public float startCinematicY = 85f;
    public float hideGroundY = 100f;

    [Header("Timing")]
    public float blendToSkyTime = 0.8f;
    public float holdTime = 0.4f;
    public float blendBackTime = 0.8f;

    bool done = false;

    void Start()
    {
        if (skyCam) skyCam.gameObject.SetActive(false);
    }

    void Update()
    {
        if (done || !plane) return;

        if (plane.position.y >= startCinematicY)
        {
            done = true;
            StartCoroutine(Sequence());
        }
    }

    System.Collections.IEnumerator Sequence()
    {
        // Enable sky cam and match FOV for a clean blend
        skyCam.gameObject.SetActive(true);
        skyCam.fieldOfView = gameplayCam.fieldOfView;

        // Blend to sky cam (using a fake blend: cross-fade via depth + overlay)
        // Simple approach: switch cameras during a screen fade.
        yield return StartCoroutine(ScreenFade(0f, 1f, blendToSkyTime * 0.5f));

        gameplayCam.gameObject.SetActive(false);

        yield return StartCoroutine(ScreenFade(1f, 0f, blendToSkyTime * 0.5f));

        // Wait until we actually reach hide height, then hide ground
        while (plane.position.y < hideGroundY) yield return null;
        if (groundRoot) groundRoot.SetActive(false);

        yield return new WaitForSeconds(holdTime);

        // Fade back to gameplay cam
        yield return StartCoroutine(ScreenFade(0f, 1f, blendBackTime * 0.5f));

        gameplayCam.gameObject.SetActive(true);
        skyCam.gameObject.SetActive(false);

        yield return StartCoroutine(ScreenFade(1f, 0f, blendBackTime * 0.5f));
    }

    // Minimal screen fade without UI setup: uses OnGUI (fine for a quick effect)
    float fade = 0f;
    

    System.Collections.IEnumerator ScreenFade(float from, float to, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float a = Mathf.Clamp01(t / duration);
            fade = Mathf.Lerp(from, to, EaseInOut(a));
            yield return null;
        }
        fade = to;
    }

    float EaseInOut(float x) => x * x * (3f - 2f * x); // smoothstep

    void OnGUI()
    {
        if (fade <= 0f) return;
        GUI.color = new Color(0f, 0f, 0f, fade);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
        GUI.color = Color.white;
    }
}
