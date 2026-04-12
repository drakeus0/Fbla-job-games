using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using NUnit.Framework;

public class SwitchAnimation : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] Image switchBackground;   // The background of the switch
    [SerializeField] RectTransform switchThumb; // The thumb that slides
    [SerializeField] TMP_Text switchText;          // The text that says ON/OFF

    [Header("Colors & Positions")]
    [SerializeField] Color onColor = Color.green;
    [SerializeField] Color offColor = Color.red;
    [SerializeField] Vector2 thumbOnPosition;
    [SerializeField] Vector2 thumbOffPosition;
    [SerializeField] float animationDuration = 0.25f;

    [SerializeField] GameObject presentationModeTeleportButtons;

    [SerializeField] private SettingOption setting;

    private bool isOn;

    void Start()
    {

            switch (setting)
            {
                case SettingOption.musicEnabled:
                    isOn = GameSettings.musicEnabled;
                    break;
                case SettingOption.presentMode:
                    isOn = GameSettings.presentMode;
                    break;
                default:
                    isOn = true; 
                    break;
            }

            // Update the UI instantly so it matches the state
            UpdateSwitchUI(isOn, instant: true);
   
    }

    public void ToggleSwitch()
    {
        isOn = !isOn;
        UpdateSwitchUI(isOn);

        if (setting == SettingOption.presentMode)
        {
            GameSettings.presentMode = isOn;
            UpdatePresentationModeUI(isOn);
        }
        if (setting == SettingOption.musicEnabled) GameSettings.musicEnabled = isOn;
    }

    private void UpdateSwitchUI(bool turnOn, bool instant = false)
    {
        if (instant)
            switchBackground.color = turnOn ? onColor : offColor;
        else
            switchBackground.DOColor(turnOn ? onColor : offColor, animationDuration).SetUpdate(true);

        if (instant)
            switchThumb.anchoredPosition = turnOn ? thumbOnPosition : thumbOffPosition;
        else
            switchThumb.DOAnchorPos(turnOn ? thumbOnPosition : thumbOffPosition, animationDuration).SetEase(Ease.InOutQuad).SetUpdate(true);

        switchText.text = turnOn ? "ON" : "OFF";
    }

    private void UpdatePresentationModeUI(bool on)
    {
        presentationModeTeleportButtons.SetActive(on);
    }
}
