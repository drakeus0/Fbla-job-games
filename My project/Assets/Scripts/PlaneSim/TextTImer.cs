using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class HideUIAfterTime : MonoBehaviour
{
    public float delaySeconds = 10f;
    public float shrinkDuration = 0.5f; // time to shrink

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI text1;

    [Header("Background Images")]
    [SerializeField] private Image background1;

    private void Start()
    {
        StartCoroutine(HideRoutine());
    }

    private IEnumerator HideRoutine()
    {
        yield return new WaitForSeconds(delaySeconds);

        if (text1)
            ShrinkAndDisable(text1.gameObject);

        if (background1)
            ShrinkAndDisable(background1.gameObject);
    }

    private void ShrinkAndDisable(GameObject obj)
    {
        obj.transform.DOScale(Vector3.zero, shrinkDuration).SetEase(Ease.InBack)
            .OnComplete(() => obj.SetActive(false));
    }
}
