using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class StarAnimate : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] Transform panel;
    [SerializeField] Transform star1;
    [SerializeField] Transform star2;
    [SerializeField] Transform star3;
    [SerializeField] Transform funFact;
    [SerializeField] TMP_Text funFactText;
    [SerializeField] Transform returnButton;

    [Header("Settings")]
    private float popDuration = 0.5f;
    private float starDelay = 0.2f;
    private float funFactFadeDuration = 1f;

    private Vector3 star1Original;
    private Vector3 star2Original;
    private Vector3 star3Original;

    private Image returnButtonImage;
    private TMP_Text returnButtonText;

    private void Awake()
    {
        // Cache original star scales
        star1Original = star1.localScale;
        star2Original = star2.localScale;
        star3Original = star3.localScale;

        // Start panel hidden
        panel.localScale = Vector3.zero;
        panel.gameObject.SetActive(false);

        // Start stars hidden
        star1.localScale = Vector3.zero;
        star2.localScale = Vector3.zero;
        star3.localScale = Vector3.zero;

        // Return button fade components
        returnButtonImage = returnButton.GetComponent<Image>();
        returnButtonText = returnButton.GetComponent<TMP_Text>();

        // Return button invisible
        returnButton.gameObject.SetActive(false);
        if (returnButtonImage)
            returnButtonImage.color = new Color(returnButtonImage.color.r, returnButtonImage.color.g, returnButtonImage.color.b, 0f);
        if (returnButtonText)
            returnButtonText.color = new Color(returnButtonText.color.r, returnButtonText.color.g, returnButtonText.color.b, 0f);

        // Fun fact starts invisible
        if (funFactText)
            funFactText.alpha = 0f;

        funFact.gameObject.SetActive(false); // ensure hidden until animation
    }

    public void ShowUI(float starsEarned)
    {
        panel.gameObject.SetActive(true);
        panel.localScale = Vector3.zero;

        panel.DOScale(Vector3.one, popDuration).SetEase(Ease.OutBack)
             .OnComplete(() =>
             {
                 StartCoroutine(PopStars(starsEarned));
             });
    }

    private IEnumerator PopStars(float starsEarned)
    {
        // STAR 1
        if (starsEarned >= 1)
        {
            star1.DOScale(star1Original, popDuration).SetEase(Ease.OutBack);
            yield return new WaitForSeconds(starDelay);
        }

        // STAR 2
        if (starsEarned >= 2)
        {
            star2.DOScale(star2Original, popDuration).SetEase(Ease.OutBack);
            yield return new WaitForSeconds(starDelay);
        }

        // STAR 3
        if (starsEarned >= 3)
        {
            star3.DOScale(star3Original, popDuration).SetEase(Ease.OutBack);
            yield return new WaitForSeconds(starDelay);
        }

        // --- FUN FACT ANIMATION ---
        funFact.gameObject.SetActive(true);

        // Fade in
        if (funFactText)
            funFactText.DOFade(1f, funFactFadeDuration);

        // Soft tilt back and forth (like a gentle wobble)
        funFact.DOLocalRotate(new Vector3(0, 0, 5f), 1.5f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);

        // Smooth floating motion (up and down)
        funFact.DOLocalMoveY(funFact.localPosition.y + 8f, 1.4f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);


        // --- RETURN BUTTON ---
        returnButton.gameObject.SetActive(true);

        if (returnButtonImage)
            returnButtonImage.DOFade(1f, popDuration);
        if (returnButtonText)
            returnButtonText.DOFade(1f, popDuration);
    }
}
