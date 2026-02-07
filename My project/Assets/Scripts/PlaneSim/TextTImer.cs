using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class HideUIAfterTime : MonoBehaviour
{
    public float delaySeconds = 10f;

    [Header("Text")]
    public TextMeshProUGUI text1;
    public TextMeshProUGUI text2;

    [Header("Background Images")]
    public Image background1;
    public Image background2;

    void Start()
    {
        StartCoroutine(HideRoutine());
    }

    IEnumerator HideRoutine()
    {
        yield return new WaitForSeconds(delaySeconds);

        if (text1) text1.gameObject.SetActive(false);
        if (text2) text2.gameObject.SetActive(false);

        if (background1) background1.gameObject.SetActive(false);
        if (background2) background2.gameObject.SetActive(false);
    }
}
