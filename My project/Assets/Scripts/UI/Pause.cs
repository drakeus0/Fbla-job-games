using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public class Pause : MonoBehaviour
{
    [SerializeField] private GameObject pauseBackground;

    [SerializeField] private GameObject titleScreen;

    [SerializeField] private GameObject quitJobButton;

    [SerializeField] GameObject settingsPopup;
    private RectTransform settingsRect;
    private Vector3 settingsNormalScale;

    [SerializeField] private bool isMiniGame;

    private void Start()
    {
        settingsRect = settingsPopup.GetComponent<RectTransform>();
        settingsNormalScale = settingsRect.localScale;

        settingsPopup.SetActive(false);
        settingsRect.localScale = settingsNormalScale * .6f;

        if (isMiniGame) {
            quitJobButton.SetActive(true);
        }
    }


    private void Update()
    {
        //if title screen, you cant pause
        if (titleScreen != null)
        {
            if (titleScreen.activeSelf) return;
        }

        //activate and deactivate with esc
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (!pauseBackground.activeSelf)
            {
                pauseBackground.SetActive(true);
                FreezeUnfreeze(true);
            }
            else
            {
                ResumeGame();
            }
        }
    }

    public void ResumeGame()
    {
        pauseBackground.SetActive(false);
        FreezeUnfreeze(false);
    }

    private void FreezeUnfreeze(bool freeze)
    {
        Time.timeScale = freeze ? 0f : 1f;
    }



    //animations
    public void OpenSettings()
    {
        settingsPopup.SetActive(true);

        settingsRect.localScale = settingsNormalScale * .6f;
        settingsRect
            .DOScale(settingsNormalScale, .25f)
            .SetEase(Ease.OutCubic)
            .SetUpdate(true);
    }

    public void CloseSettings()
    {
        settingsRect
            .DOScale(settingsNormalScale * .6f, .25f)
            .SetEase(Ease.InCubic)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                settingsPopup.SetActive(false);
            });
    }

    public void QuitJob()
    {
        FreezeUnfreeze(false);
         SceneManager.LoadScene("MainGame");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
