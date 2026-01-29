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

        // Add spacing between each ingredient
        int ingredientCount = transform.childCount;
        totalHeight += spacing * Mathf.Max(ingredientCount - 1, 0);

        // Step 2: Determine starting Y position (top of stack)
        float yOffset = totalHeight / 2f; // start from half the stack above parent pivot

        // Step 3: Position each child
        foreach (Transform child in transform)
        {
            SpriteRenderer sr = child.GetComponent<SpriteRenderer>();
            if (sr == null || sr.sprite == null) continue;

            float height = sr.sprite.bounds.size.y * child.localScale.y;

            // Place the child so its center is offset from yOffset by half its height
            child.localPosition = new Vector3(0f, yOffset - height / 2f, 0f);

            // Move yOffset down by the child height + spacing
            yOffset -= height + spacing;
        }
    }
}
