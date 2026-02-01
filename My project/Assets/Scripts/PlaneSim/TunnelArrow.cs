using UnityEngine;

public class UITunnelArrow : MonoBehaviour
{
    public Transform player;
    public Transform tunnelTarget;
    public RectTransform arrow;
    public float radius = 150f;
    public Camera worldCamera;

    CanvasGroup canvasGroup;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (!canvasGroup)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        if (!worldCamera)
            worldCamera = Camera.main;

        canvasGroup.alpha = 1f;
    }

    void Update()
    {
        if (!player || !tunnelTarget || !arrow || !worldCamera)
        {
            canvasGroup.alpha = 0f;
            return;
        }

        Vector3 playerScreen = worldCamera.WorldToScreenPoint(player.position);
        Vector3 targetScreen = worldCamera.WorldToScreenPoint(tunnelTarget.position);

        if (targetScreen.z < 0)
        {
            canvasGroup.alpha = 0f;
            return;
        }

        Vector2 dir = new Vector2(
            targetScreen.x - playerScreen.x,
            targetScreen.y - playerScreen.y
        ).normalized;

        // ROTATION
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        arrow.localRotation = Quaternion.Euler(0, 0, angle - 90f);

        // ORBIT AROUND SCREEN CENTER
        arrow.anchoredPosition = dir * radius;

        canvasGroup.alpha = 1f;
    }
}
