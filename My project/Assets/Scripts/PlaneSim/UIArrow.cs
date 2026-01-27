using UnityEngine;

public class UIArrow : MonoBehaviour
{
    public Transform plane;
    public RectTransform arrow;
    public float radius = 120f;
    public Camera mainCamera;

    CanvasGroup canvasGroup;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        canvasGroup.alpha = 0f;

        if (!arrow.gameObject.activeSelf)
            arrow.gameObject.SetActive(true);

        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    void Update()
    {
        if (plane == null || RingManager.Instance == null || mainCamera == null)
        {
            canvasGroup.alpha = 0f;
            return;
        }

        Transform target = RingManager.Instance.currentTarget;
        if (target == null)
        {
            canvasGroup.alpha = 0f;
            return;
        }

        canvasGroup.alpha = 1f;

        // Project the target's world position to screen space
        Vector3 screenPos = mainCamera.WorldToScreenPoint(target.position);
        Vector3 screenPlanePos = mainCamera.WorldToScreenPoint(plane.position);

        Vector2 dir = new Vector2(
            screenPos.x - screenPlanePos.x,
            screenPos.y - screenPlanePos.y
        ).normalized;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        arrow.rotation = Quaternion.Euler(0, 0, angle - 90f);

        // place arrow on a circle around the plane
        arrow.anchoredPosition = dir * radius;
    }
}
