using UnityEngine;

public class RingArrowUI : MonoBehaviour
{
    public RectTransform arrow;
    public Transform plane;
    public Camera cam;

    public float screenEdgePadding = 80f;

    void Update()
    {
        if (RingManager.Instance == null || RingManager.Instance.currentRing == null)
        {
            arrow.gameObject.SetActive(false);
            return;
        }

        if(RingManager.Instance.currentRing == null)
            Debug.Log("No ring registered yet!");
        else
            Debug.Log("Arrow should point to ring: " + RingManager.Instance.currentRing.name);

        arrow.gameObject.SetActive(true);

        Vector3 screenPos = cam.WorldToScreenPoint(RingManager.Instance.currentRing.position);

        Vector3 center = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
        Vector3 dir = (screenPos - center).normalized;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        arrow.rotation = Quaternion.Euler(0, 0, angle - 90f);

        Vector3 arrowPos = center + dir * (Mathf.Min(Screen.width, Screen.height) / 2f - screenEdgePadding);
        arrow.position = arrowPos;
    }
}
