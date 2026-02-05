using UnityEngine;

public class UIAirspeedGauge : MonoBehaviour
{
    [Header("Needle")]
    [SerializeField] private RectTransform needle;

    [Header("Speed Range (knots)")]
    [SerializeField] private float minKnots = 0f;
    [SerializeField] private float maxKnots = 200f;

    [Header("Needle Angles (degrees)")]
    [SerializeField] private float minAngle = -130f; // needle at minKnots
    [SerializeField] private float maxAngle = 130f;  // needle at maxKnots

    [Header("Smoothing")]
    [SerializeField] private float smoothSpeed = 10f; // higher = snappier

    private float targetKnots;
    private float currentKnots;

    void Awake()
    {
        targetKnots = minKnots;
        currentKnots = minKnots;
        ApplyNeedle(currentKnots);
    }

    void Update()
    {
        // Smoothly approach target speed
        currentKnots = Mathf.Lerp(currentKnots, targetKnots, 1f - Mathf.Exp(-smoothSpeed * Time.deltaTime));
        ApplyNeedle(currentKnots);
    }

    public void SetSpeedKnots(float knots)
    {
        targetKnots = Mathf.Clamp(knots, minKnots, maxKnots);
    }

    public void SetSpeedNormalized(float t)
    {
        // t in [0..1]
        t = Mathf.Clamp01(t);
        targetKnots = Mathf.Lerp(minKnots, maxKnots, t);
    }

    private void ApplyNeedle(float knots)
    {
        float t = Mathf.InverseLerp(minKnots, maxKnots, knots);
        float angle = Mathf.Lerp(minAngle, maxAngle, t);
        needle.localRotation = Quaternion.Euler(0f, 0f, angle);
    }
}
