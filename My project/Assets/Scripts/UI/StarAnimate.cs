using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class StarAnimate : MonoBehaviour
{
    [SerializeField] Transform panel;
    [SerializeField] Transform star1;
    [SerializeField] Transform star2;
    [SerializeField] Transform star3;
    [SerializeField] Transform returnButton; // keep as Transform

    private float popDuration = 0.5f;
    private float starDelay = 0.2f;

    private Vector3 star1Original;
    private Vector3 star2Original;
    private Vector3 star3Original;

    private Image returnButtonImage;
    private TMP_Text returnButtonText;

    private void Awake()
    {
        star1Original = star1.localScale;
        star2Original = star2.localScale;
        star3Original = star3.localScale;

        panel.localScale = Vector3.zero;
        panel.gameObject.SetActive(false);

        star1.localScale = Vector3.zero;
        star2.localScale = Vector3.zero;
        star3.localScale = Vector3.zero;

        // Get components for fading
        returnButtonImage = returnButton.GetComponent<Image>();
        returnButtonText = returnButton.GetComponent<TMP_Text>();

        // Make button invisible
        if (returnButtonImage) returnButtonImage.color = new Color(returnButtonImage.color.r, returnButtonImage.color.g, returnButtonImage.color.b, 0f);
        if (returnButtonText) returnButtonText.color = new Color(returnButtonText.color.r, returnButtonText.color.g, returnButtonText.color.b, 0f);
        returnButton.gameObject.SetActive(false);
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
        if (starsEarned >= 1)
        {
            star1.DOScale(star1Original, popDuration).SetEase(Ease.OutBack);
            yield return new WaitForSeconds(starDelay);
        }

        if (starsEarned >= 2)
        {
            star2.DOScale(star2Original, popDuration).SetEase(Ease.OutBack);
            yield return new WaitForSeconds(starDelay);
        }

        if (starsEarned >= 3)
        {
            star3.DOScale(star3Original, popDuration).SetEase(Ease.OutBack);
            yield return new WaitForSeconds(starDelay);
        }

        // Fade in return button
        returnButton.gameObject.SetActive(true);
        if (returnButtonImage)
            returnButtonImage.DOFade(1f, popDuration);
        if (returnButtonText)
            returnButtonText.DOFade(1f, popDuration);
    }
}
