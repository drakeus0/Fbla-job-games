using System.Collections.Generic;
using UnityEngine;

public class ChangeCharacter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform hairsParent;
    [SerializeField] private Renderer shirtRenderer;
    [SerializeField] private Texture[] shirtTextures;

    private List<Color> hairColors = new List<Color>
    {
        new Color(0.55f, 0.27f, 0.07f),
        new Color(0.85f, 0.75f, 0.45f), // blonde
        Color.black
    };

    private int currentHair;
    private int currentHairColor;
    private int currentShirt;

    private void Start()
    {
        // Load saved values from manager
        if (PlayerCusomization.Instance != null)
        {
            currentHair = PlayerCusomization.Instance.hairIndex;
            currentHairColor = PlayerCusomization.Instance.hairColorIndex;
            currentShirt = PlayerCusomization.Instance.shirtIndex;
        }

        ApplyHair(currentHair);
        ApplyHairColor(currentHairColor);
        ApplyShirt(currentShirt);
        Debug.Log("UpdatedSkin" + currentHair + currentHairColor + currentShirt);
    }

    // --- Hair ---
    public void ChangeHair(int direction)
    {
        currentHair += direction;
        if (currentHair < 0) currentHair = hairsParent.childCount - 1;
        if (currentHair >= hairsParent.childCount) currentHair = 0;

        ApplyHair(currentHair);

        if (PlayerCusomization.Instance != null)
            PlayerCusomization.Instance.hairIndex = currentHair;
        Debug.Log(PlayerCusomization.Instance.hairIndex);
    }

    private void ApplyHair(int index)
    {
        // Deactivate all hairs
        foreach (Transform child in hairsParent)
            child.gameObject.SetActive(false);

        // Activate the selected one
        hairsParent.GetChild(index).gameObject.SetActive(true);
        Debug.Log("changed hair");
    }

    // --- Hair Color ---
    public void ChangeHairColor(int index)
    {
        currentHairColor = index;
        ApplyHairColor(currentHairColor);

        if (PlayerCusomization.Instance != null)
            PlayerCusomization.Instance.hairColorIndex = currentHairColor;
    }

    private void ApplyHairColor(int index)
    {
        foreach (Transform child in hairsParent)
        {
            Renderer rend = child.GetComponent<Renderer>();
            if (rend != null)
                rend.material.color = hairColors[index];
        }
    }

    // --- Shirt ---
    public void ChangeShirt(int direction)
    {
        currentShirt += direction;
        if (currentShirt < 0) currentShirt = shirtTextures.Length - 1;
        if (currentShirt >= shirtTextures.Length) currentShirt = 0;

        ApplyShirt(currentShirt);

        if (PlayerCusomization.Instance != null)
            PlayerCusomization.Instance.shirtIndex = currentShirt;
    }

    private void ApplyShirt(int index)
    {
        if (shirtRenderer != null && shirtTextures.Length > 0)
            shirtRenderer.material.mainTexture = shirtTextures[index];
    }
}
