using System.Collections;
using UnityEngine;

/// <summary>
/// Controls runtime-only behavior for a single cloud instance:
/// - Fade in by modifying _BaseColor.a (only),
/// - Self-despawn when z + buffer < plane.z,
/// - Robust null-checks to avoid MissingReferenceException.
/// </summary>
[DisallowMultipleComponent]
public class CloudBehavior : MonoBehaviour
{
    [Header("Runtime Links (set by spawner)")]
    [Tooltip("Assigned by CloudSpawner at runtime. CloudBehavior will not touch the prefab.")]
    public Transform plane;

    [Header("Fade & Despawn")]
    [Tooltip("Duration (seconds) to fade from fully transparent to fully opaque (alpha controlled via _BaseColor.a).")]
    public float fadeDuration = 2f;
    [Tooltip("Buffer used in the despawn check: destroy when transform.position.z + despawnBuffer < plane.position.z")]
    public float despawnBuffer = 50f;

    // internal arrays for renderer materials set on the instance
    Renderer[] renderers;
    Material[] instanceMaterials; // matched 1-to-1 with renderers[] to allow alpha control

    bool isFading = false;

    void Awake()
    {
        // Gather renderers in this instance and create material instances (renderer.material clones)
        renderers = GetComponentsInChildren<Renderer>(includeInactive: true);
        instanceMaterials = new Material[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            // Accessing renderer.material produces an instance at runtime, safe for NOT modifying project prefab.
            if (renderers[i] == null) continue;
            instanceMaterials[i] = renderers[i].material;
            // Try to set initial alpha to zero on _BaseColor if it exists
            if (instanceMaterials[i].HasProperty("_BaseColor"))
            {
                Color c = instanceMaterials[i].GetColor("_BaseColor");
                c.a = 0f;
                instanceMaterials[i].SetColor("_BaseColor", c);
            }
            else
            {
                // If material has no _BaseColor, do nothing (but we keep behavior safe)
            }
        }
    }

    void Start()
    {
        // Start fade-in coroutine
        StartCoroutine(FadeInCoroutine());

        // Safety: if plane is null at start, don't throw; keep running but do not attempt despawn until assigned.
    }

    IEnumerator FadeInCoroutine()
    {
        if (isFading) yield break;
        isFading = true;

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            SetAlphaForAll(t);
            yield return null;
        }

        SetAlphaForAll(1f);
        isFading = false;
    }

    void SetAlphaForAll(float alpha)
    {
        for (int i = 0; i < instanceMaterials.Length; i++)
        {
            Material mat = instanceMaterials[i];
            if (mat == null) continue;
            if (!mat.HasProperty("_BaseColor")) continue;
            Color c = mat.GetColor("_BaseColor");
            c.a = Mathf.Clamp01(alpha);
            mat.SetColor("_BaseColor", c);
        }
    }

    void Update()
    {
        // Despawn rule: only if we have a valid plane reference
        if (plane != null)
        {
            // protect against destroyed transforms
            if (plane.Equals(null))
            {
                // plane destroyed - stop checking
                plane = null;
                return;
            }

            float cloudZ = transform.position.z;
            if (cloudZ + despawnBuffer < plane.position.z)
            {
                // Optional: stop any coroutines to avoid them touching destroyed materials (safe)
                StopAllCoroutines();
                Destroy(gameObject);
                return;
            }
        }
    }

    void OnDestroy()
    {
        // Cleanup created material instances to avoid leaking memory in editor play mode
        if (instanceMaterials != null)
        {
            for (int i = 0; i < instanceMaterials.Length; i++)
            {
                if (instanceMaterials[i] != null)
                {
#if UNITY_EDITOR
                    // In the editor destroyImmediate the created material instances
                    DestroyImmediate(instanceMaterials[i]);
#else
                    Destroy(instanceMaterials[i]);
#endif
                    instanceMaterials[i] = null;
                }
            }
        }
    }
}
