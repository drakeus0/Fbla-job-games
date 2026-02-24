using UnityEngine;

public class ShowHighScore : MonoBehaviour
{
    [SerializeField] GameObject star1;
    [SerializeField] GameObject star2;
    [SerializeField] GameObject star3;

    [SerializeField] string Job;

    private int highScore;
    void Start()
    {
        if (Job == "DeliveryDriver") highScore = MainGameData.DeliveryHighScore;
        else if (Job == "Pilot") highScore = MainGameData.PilotHighScore;
        else if (Job == "Chef") highScore = MainGameData.ChefHighScore;

        // Activate stars based on highScore
        if (highScore >= 1) star1.SetActive(true);
        if (highScore >= 2) star2.SetActive(true);
        if (highScore >= 3) star3.SetActive(true);
    }
}
