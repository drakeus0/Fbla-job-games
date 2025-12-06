using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using DG.Tweening;
using UnityEngine.SceneManagement;
using static MainGameData;

public class ChangeScene : MonoBehaviour
{
    [SerializeField] string scene;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] GameObject player;
    [SerializeField] string jobName;

    [SerializeField] float triggerDistance = 4f;
    [SerializeField] float scaleDuration = 0.25f;

    bool isVisible = false;
    Vector3 originalScale;

    Transform uiParent; 

    private void Start()
    {
        uiParent = text.transform.parent;

        originalScale = uiParent.localScale;

        uiParent.localScale = Vector3.zero;
        uiParent.gameObject.SetActive(false);

        text.text = ""; 
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.transform.position);

        if (distance < triggerDistance)
        {
            if (!isVisible)
                ShowUI();

            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                MainGameData.playerReturnPos = player.transform.position;
                SceneManager.LoadScene(scene);
            }
        }
        else
        {
            if (isVisible)
                HideUI();
        }
    }

    void ShowUI()
    {
        isVisible = true;

        text.text = "Press E to work as a " + jobName;

        uiParent.gameObject.SetActive(true);
        uiParent.localScale = Vector3.zero;

        uiParent.DOKill();
        uiParent.DOScale(originalScale, scaleDuration).SetEase(Ease.OutBack);
    }

    void HideUI()
    {
        isVisible = false;

        uiParent.DOKill();
        uiParent.DOScale(Vector3.zero, scaleDuration).SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                if (!isVisible)
                    uiParent.gameObject.SetActive(false);
            });
    }
}
