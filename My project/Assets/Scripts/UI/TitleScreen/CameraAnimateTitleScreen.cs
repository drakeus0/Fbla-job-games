using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Unity.VisualScripting;

public class CameraAnimateTitleScreen : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] Image blackCover; // UI Image for fading

    [Header("Camera Pan")]
    [SerializeField] Transform Points;
    [SerializeField] float panDuration = 5f;
    [SerializeField] float pauseBetweenPans = 0.5f;

    [Header("FOV Zoom")]
    [SerializeField] float minFOV = 60f;
    [SerializeField] float maxFOV = 65f;

    Vector3 rotationOne = new Vector3(18.677f, 0, 0);
    Vector3 rotationTwo = new Vector3(18.677f, 180, 0);

    Transform pointA;
    Transform pointB;

    Camera cam;

    // When to start fading in black (as a fraction of pan)
    [SerializeField, Range(0f, 1f)] float fadeStartFraction = 0.85f;
    [SerializeField] float fadeDuration = 0.5f;

    void Start()
    {
        pointA = Points.GetChild(0);
        pointB = Points.GetChild(1);

        cam = GetComponent<Camera>();
        cam.fieldOfView = minFOV;

        blackCover.gameObject.SetActive(true);
        blackCover.color = new Color(0, 0, 0, 0);

        StartCoroutine(PanLoop());
    }

    IEnumerator PanLoop()
    {
        while (true)
        {
            yield return StartCoroutine(PanWithFade(pointA.position, pointB.position, rotationOne));
            yield return new WaitForSeconds(pauseBetweenPans);

            transform.position = pointA.position;
            transform.rotation = Quaternion.Euler(rotationTwo);

            yield return StartCoroutine(PanWithFade(pointA.position, pointB.position, rotationTwo));
            yield return new WaitForSeconds(pauseBetweenPans);

            transform.position = pointA.position;
            transform.rotation = Quaternion.Euler(rotationOne);
        }
    }

    IEnumerator PanWithFade(Vector3 startPos, Vector3 endPos, Vector3 rotation)
    {
        float t = 0f;
        transform.position = startPos;
        transform.rotation = Quaternion.Euler(rotation);

        bool fadeStarted = false;

        while (t < 1f)
        {
            t += Time.deltaTime / panDuration;
            transform.position = Vector3.Lerp(startPos, endPos, t);

            // Slight FOV zoom for parallax
            cam.fieldOfView = Mathf.Lerp(minFOV, maxFOV, Mathf.Sin(t * Mathf.PI));

            // Start fading in black slightly before the end
            if (!fadeStarted && t >= fadeStartFraction)
            {
                fadeStarted = true;
                StartCoroutine(FadeBlack(1f, fadeDuration)); // fade in
            }

            yield return null;
        }

        // Reset FOV
        cam.fieldOfView = minFOV;

        // Fade black out while next pan starts
        StartCoroutine(FadeBlack(0f, fadeDuration));
    }

    IEnumerator FadeBlack(float targetAlpha, float duration)
    {
        float startAlpha = blackCover.color.a;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            blackCover.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        blackCover.color = new Color(0, 0, 0, targetAlpha);
    }
}
