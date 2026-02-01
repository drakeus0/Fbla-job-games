using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using NUnit.Framework.Interfaces;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] List<GameObject> slides = new List<GameObject>();
    [SerializeField] GameObject nextSlideButton;

    int currentSlide = 0;

    float slideDuration = 0.45f;
    float offscreenX;

    void Start()
    {
        RectTransform canvasRT = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        RectTransform slideRT = slides[0].GetComponent<RectTransform>();

        offscreenX = canvasRT.rect.width + slideRT.rect.width * 15;

        // Init positions
        for (int i = 0; i < slides.Count; i++)
        {
            slides[i].SetActive(true);

            RectTransform rt = slides[i].GetComponent<RectTransform>();

            if (i == 0)
                rt.localPosition = Vector3.zero;
            else
                rt.localPosition = new Vector3(offscreenX, 0, 0);
        }
    }

    public void NextSlide()
    {
        if (currentSlide >= slides.Count - 2)
        {
            nextSlideButton.SetActive(false);

        }
        else if (currentSlide >= slides.Count - 1)
        {
            return;
        }

        RectTransform currentRT = slides[currentSlide].GetComponent<RectTransform>();
        RectTransform nextRT = slides[currentSlide + 1].GetComponent<RectTransform>();

        currentRT.DOKill();
        nextRT.DOKill();

        // Move current fully off-screen
        currentRT.DOLocalMoveX(offscreenX, slideDuration)
                 .SetEase(Ease.InQuad);

        // Move next in with bounce
        nextRT.localPosition = new Vector3(offscreenX, 0, 0);
        nextRT.DOLocalMoveX(0, slideDuration)
              .SetEase(Ease.OutBack, 1.15f)
              .SetDelay(slideDuration * 0.9f);

        currentSlide++;
    }
}
