using System.Collections.Generic;
using UnityEngine;

public class RandomizeNPCTexture : MonoBehaviour
{
    [SerializeField] private List<Texture> skins;
    [SerializeField] private Transform hairs;
    [SerializeField] private Renderer targetRenderer;

    private List<Color> hairColors = new List<Color>
    {
        new Color(0.65f, 0.42f, 0.20f),
        new Color(0.85f, 0.75f, 0.45f), // blonde
        Color.black
    };

    private void Start()
    {
        if (targetRenderer == null)
            targetRenderer = GetComponent<Renderer>();

        // Random skin
        if (skins.Count > 0)
        {
            targetRenderer.material.mainTexture = skins[Random.Range(0, skins.Count)];
        }

        // Random hair
        if (hairs.childCount > 0)
        {
            // deactivate all hair first
            foreach (Transform hair in hairs)
                hair.gameObject.SetActive(false);

            Transform randomHair = hairs.GetChild(Random.Range(0, hairs.childCount));
            Renderer rend = randomHair.GetComponent<Renderer>();
            if (rend != null)
            {
                rend.material.color = hairColors[Random.Range(0, hairColors.Count)];
            }
            randomHair.gameObject.SetActive(true);
        }
    }
}
