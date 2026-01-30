using UnityEngine;
using TMPro;

public class RunStatsUI : MonoBehaviour
{
    [Header("References")]
    public StarAnimate starAnimate;   // drag StarAnimate here
    public TMP_Text timerText;        // top timer text

    [Header("Star thresholds (seconds)")]
    public float oneStarMax = 10f;   // 0–10 => 1 star
    public float twoStarMax = 20f;   // 10–20 => 2 stars
    // 20+ => 3 stars

    float timeAlive = 0f;
    bool running = false;
    bool ended = false;

    void Start()
    {
        SetTimerUI(0f);
    }

    void Update()
    {
        if (!running || ended) return;

        timeAlive += Time.deltaTime;
        SetTimerUI(timeAlive);
    }

    void SetTimerUI(float t)
    {
        if (timerText)
            timerText.text = t.ToString("0.0");
    }

    // Called when plane starts flying
    public void StartRun()
    {
        Debug.Log("running?");
        ended = false;
        running = true;
        timeAlive = 0f;
        SetTimerUI(0f);
    }

    // Called when plane crashes
    public void EndRun()
    {
        if (ended) return;

        ended = true;
        running = false;

        int stars = CalculateStars(timeAlive);

        if (starAnimate)
            starAnimate.ShowUI(stars);
    }

    int CalculateStars(float t)
    {
        if (t < oneStarMax) return 1;
        if (t < twoStarMax) return 2;
        return 3;
    }
}
