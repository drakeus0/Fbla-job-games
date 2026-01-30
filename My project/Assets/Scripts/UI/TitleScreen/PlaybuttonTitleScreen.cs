using UnityEngine;
using UnityEngine.UI;

public class TitleScreenManagerr : MonoBehaviour
{
    [SerializeField] GameObject titleScreenCanvas;
    [SerializeField] GameObject creditsPopup;

    [SerializeField] Camera titleScreenCam;
    [SerializeField] Camera PlayerCam;

    [SerializeField] GameObject Player;

    private void Start()
    {
        Player.SetActive(false);
    }
    public void playClicked()
    {
        //dot tween to make ui fade
        titleScreenCam.gameObject.SetActive(false);
        PlayerCam.gameObject.SetActive(true);
        Player.SetActive(true);
        titleScreenCanvas.SetActive(false);
    }

    public void CloseCredits()
    {
        creditsPopup.SetActive(false);
    }
    public void OpenCredits()
    {
        creditsPopup.SetActive(true);
    }
}
