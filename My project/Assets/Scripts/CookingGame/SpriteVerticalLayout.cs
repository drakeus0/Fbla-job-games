using UnityEngine;

public class VerticalSpriteLayout : MonoBehaviour
{
    [Header("Extra spacing between ingredients")]
    public float spacing = 0f; // world units

    public void RefreshStack()
    {
        // Step 1: Calculate total height of the stack
        float totalHeight = 0f;
        foreach (Transform child in transform)
        {
            SpriteRenderer sr = child.GetComponent<SpriteRenderer>();
            if (sr == null || sr.sprite == null) continue;

            float height = sr.sprite.bounds.size.y * child.localScale.y;
            totalHeight += height;
        }

        int ingredientCount = transform.childCount;
        totalHeight += spacing * Mathf.Max(ingredientCount - 1, 0);

        // Step 2: Start at the BOTTOM of the stack
        float yOffset = -totalHeight / 2f;

        // Step 3: Position each child bottom → top
        foreach (Transform child in transform)
        {
            SpriteRenderer sr = child.GetComponent<SpriteRenderer>();
            if (sr == null || sr.sprite == null) continue;

            float height = sr.sprite.bounds.size.y * child.localScale.y;

            // Place child centered above current offset
            child.localPosition = new Vector3(0f, yOffset + height / 2f, 0f);

            // Move yOffset UP
            yOffset += height + spacing;
        }
    }
}
