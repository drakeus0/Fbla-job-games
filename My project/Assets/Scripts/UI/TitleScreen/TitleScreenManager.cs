using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TitleScreenManager : MonoBehaviour
{
    [Header("UI Game Objects")]

    [SerializeField] GameObject creditsPopup;
    [SerializeField] GameObject titleScreenCanvas;
    [SerializeField] GameObject Buttons;

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

    CanvasGroup[] canvasGroups = Buttons.GetComponentsInChildren<CanvasGroup>(true);

    foreach (Transform child in Buttons.transform)
    {
        CanvasGroup cg = child.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = child.gameObject.AddComponent<CanvasGroup>();

        cg.DOFade(0f, fadeDuration);
    }

        titleScreenCam.gameObject.SetActive(false);
        PlayerCam.gameObject.SetActive(true);
        Player.SetActive(true);

    DOVirtual.DelayedCall(fadeDuration, () =>
    {
        titleScreenCanvas.SetActive(false);
    });
}


    public void QuitGame()
    {
        Application.Quit();
    }
}
