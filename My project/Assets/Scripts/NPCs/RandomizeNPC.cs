using System.Collections.Generic;
using UnityEngine;

public class RandomizeNPCTexture : MonoBehaviour
{
    [SerializeField] private List<Texture> skins; // assign your images here
    [SerializeField] private Renderer targetRenderer; // your character's renderer

    private void Start()
    {

        if (targetRenderer == null) targetRenderer = GetComponent<Renderer>();

        Texture randomTexture = skins[Random.Range(0, skins.Count)];

        targetRenderer.material.mainTexture = randomTexture;
    }
}
