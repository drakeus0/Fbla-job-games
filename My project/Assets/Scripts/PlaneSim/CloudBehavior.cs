using System.Collections;
using UnityEngine;

public class CloudBehavior : MonoBehaviour
{
    public Transform plane; // assign in Inspector

    [Header("Fade Settings")]
    public float fadeDuration = 2f;

    private Renderer[] renderers;
    private float spawnTime;

    void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();
        spawnTime = Time.time;

        // Set initial alpha to 0
        foreach (var r in renderers)
        {
            foreach (var mat in r.materials)
            {
                if (mat.HasProperty("_BaseColor"))
                {
                    Color c = mat.color;
                    c.a = 0f;
                    mat.color = c;
                }
            }
        }
    }

    void Update()
    {
        // Fade in
        float elapsed = Time.time - spawnTime;
        float alpha = Mathf.Clamp01(elapsed / fadeDuration);

        foreach (var r in renderers)
        {
            foreach (var mat in r.materials)
            {
                if (mat.HasProperty("_BaseColor"))
                {
                    Color c = mat.color;
                    c.a = alpha;
                    mat.color = c;
                }
            }
        }

        // Despawn if behind plane (with null check)
        if (plane != null && transform.position.z + 50f < plane.position.z)
        {
            Destroy(gameObject);
        }
    }
}
