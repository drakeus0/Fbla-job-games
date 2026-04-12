using UnityEngine;

public class ShowEndScreen : MonoBehaviour
{
    [SerializeField] private GameObject background;
    bool hasShown = false;
    void Start()
    {
        if (MainGameData.PilotCompleted && MainGameData.DeliveryCompleted && MainGameData.ChefCompleted)
        {
            if (!hasShown)
            {
                FreezeUnfreeze(true);
                hasShown = true;
                background.SetActive(true);
            }
        }
    }
    private void FreezeUnfreeze(bool freeze)
    {
        Time.timeScale = freeze ? 0f : 1f;
    }

    public void KeepPlaying()
    {
        background.SetActive(false);
        FreezeUnfreeze(false);
    }
}
