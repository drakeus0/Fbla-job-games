using UnityEngine;

public class HideAllTerrainsAtHeight : MonoBehaviour
{
    public Transform plane;

    public float vanishAtY = 100f;

    [Header("Optional fog blending")]
    public bool useFog = true;
    public float fadeStartY = 60f;
    public Color fogColor = new Color(0.6f, 0.75f, 0.95f, 1f);
    public float lowFogStart = 1200f, lowFogEnd = 4000f;
    public float highFogStart = 50f, highFogEnd = 400f;
    public float fogClearOutTime = 1.2f;

    bool vanished = false;
    float clearOutTimer = 0f;

    Terrain[] terrains;

    void Start()
    {
        terrains = FindObjectsByType<Terrain>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        if (useFog)
        {
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.Linear;
            RenderSettings.fogColor = fogColor;
            RenderSettings.fogStartDistance = lowFogStart;
            RenderSettings.fogEndDistance = lowFogEnd;
        }
    }

    void Update()
    {
        if (!plane) return;

        float y = plane.position.y;

        if (!vanished)
        {
            if (useFog)
            {
                float t = Smooth01(Mathf.InverseLerp(fadeStartY, vanishAtY, y));
                RenderSettings.fogStartDistance = Mathf.Lerp(lowFogStart, highFogStart, t);
                RenderSettings.fogEndDistance   = Mathf.Lerp(lowFogEnd,   highFogEnd,   t);
            }

            if (y >= vanishAtY)
            {
                vanished = true;
                

                foreach (var t in terrains)
                {
                    if (!t) continue;
                    // Disable the component AND the GameObject (belt + suspenders)
                    t.enabled = false;
                    t.gameObject.SetActive(false);
                }

                clearOutTimer = 0f;
            }

            return;
        }

        if (!useFog) return;

        // ease fog out after terrain is gone
        clearOutTimer += Time.deltaTime;
        float k = Smooth01(Mathf.Clamp01(clearOutTimer / Mathf.Max(0.01f, fogClearOutTime)));
        RenderSettings.fogStartDistance = Mathf.Lerp(highFogStart, lowFogStart, k);
        RenderSettings.fogEndDistance   = Mathf.Lerp(highFogEnd,   lowFogEnd,   k);

        if (k >= 0.999f) RenderSettings.fog = false;
    }

    static float Smooth01(float x) => x * x * (3f - 2f * x);
}
