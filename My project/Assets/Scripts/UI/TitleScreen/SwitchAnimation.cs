using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

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

    private bool isOn = false;

    void Start()
    {
        // Initialize UI
        UpdateSwitchUI(true, instant: false);
    }

    public void ToggleSwitch()
    {
        isOn = !isOn;
        UpdateSwitchUI(isOn);
    }

    private void UpdateSwitchUI(bool turnOn, bool instant = false)
    {
        if (instant)
            switchBackground.color = turnOn ? onColor : offColor;
        else
            switchBackground.DOColor(turnOn ? onColor : offColor, animationDuration);

        if (instant)
            switchThumb.anchoredPosition = turnOn ? thumbOnPosition : thumbOffPosition;
        else
            switchThumb.DOAnchorPos(turnOn ? thumbOnPosition : thumbOffPosition, animationDuration).SetEase(Ease.InOutQuad);

        switchText.text = turnOn ? "ON" : "OFF";
    }
}
