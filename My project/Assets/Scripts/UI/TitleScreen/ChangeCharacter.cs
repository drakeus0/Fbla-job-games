using UnityEngine;

public class ChangeCharacter : MonoBehaviour
{
    [SerializeField] Renderer shirtRenderer;
    [SerializeField] Texture[] shirtTextures;
    private int currentShirt = 0;

    public void ChangeShirt(int direction)
    {
        currentShirt += direction;
        if (currentShirt < 0) currentShirt = shirtTextures.Length - 1;
        if (currentShirt >= shirtTextures.Length) currentShirt = 0;

        // Apply the new texture
        shirtRenderer.material.mainTexture = shirtTextures[currentShirt];
    }
}
