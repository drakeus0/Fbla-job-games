using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using UnityEngine.SceneManagement;

public class TitleScreenManager : MonoBehaviour
{
    private static bool titleScreenOpened = false;

    [Header("UI Game Objects")]

    [SerializeField] GameObject creditsPopup;
    [SerializeField] GameObject titleScreenCanvas;
    [SerializeField] GameObject Buttons;
    [SerializeField] Image blackCover; 

    [Header("Game Setup")]
    [SerializeField] Camera titleScreenCam;
    [SerializeField] Camera PlayerCam;
    [SerializeField] GameObject Player;

    [Header("Credits Animation")]
    [SerializeField] float closedScale = 0.6f;
    [SerializeField] float popDuration = 0.25f;
    [SerializeField] Ease openEase = Ease.OutCubic;
    [SerializeField] Ease closeEase = Ease.InCubic;

    private RectTransform creditsRect;
    private Vector3 normalScale;

    void Start()
    {
        Player.SetActive(false);

        creditsRect = creditsPopup.GetComponent<RectTransform>();
        normalScale = creditsRect.localScale;

        creditsPopup.SetActive(false);
        creditsRect.localScale = normalScale * closedScale;

        blackCover.color = new Color(0, 0, 0, 0);
        Canvas canvas = blackCover.transform.parent.gameObject.GetComponent<Canvas>();
        canvas.overrideSorting = true;
        canvas.sortingOrder = 999;

        if (titleScreenOpened)
        {
            titleScreenCam.gameObject.SetActive(false);
            PlayerCam.gameObject.SetActive(true);
            Player.SetActive(true);
            titleScreenCanvas.SetActive(false);
            this.enabled = false;
        }
    }

    public void OpenCredits()
    {
        creditsPopup.SetActive(true);

        creditsRect.localScale = normalScale * closedScale;
        creditsRect
            .DOScale(normalScale, popDuration)
            .SetEase(openEase);
    }

    public void CloseCredits()
    {
        creditsRect
            .DOScale(normalScale * closedScale, popDuration)
            .SetEase(closeEase)
            .OnComplete(() =>
            {
                creditsPopup.SetActive(false);
            });
    }

    public void playClicked()
    {
        float fadeDuration = 0.5f;

        titleScreenOpened = true;

        StartCoroutine(FadeBlack(1f, fadeDuration));
}

    IEnumerator FadeBlack(float targetAlpha, float duration)
    {
        blackCover.gameObject.SetActive(true);
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
        SceneManager.LoadScene("intro");

    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
