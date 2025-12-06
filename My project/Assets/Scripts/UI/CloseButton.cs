using UnityEngine;
using DG.Tweening;

public class CloseButton : MonoBehaviour
{
    public float shrinkDuration = 0.5f; // seconds

    // Call this from the button's OnClick
    public void CloseUI()
    {
        if (transform.parent != null)
        {
            Transform panel = transform.parent;

            // Shrink the panel to zero scale, then disable it
            panel.DOScale(Vector3.zero, shrinkDuration)
                 .SetEase(Ease.InBack) // smooth shrinking effect
                 .OnComplete(() =>
                 {
                     panel.gameObject.SetActive(false);
                     panel.localScale = Vector3.one; // reset for next time
                 });
        }
    }
}
