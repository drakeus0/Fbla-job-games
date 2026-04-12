using UnityEngine;

public static class MainGameData
{
    public static Vector3 playerReturnPos;

    private static bool initialized = false;

    public static void Initialize(Transform playerTransform)
    {
        if (!initialized)
        {
            Debug.Log("ran");
            playerReturnPos = playerTransform.position;
            initialized = true;
        }
        if (initialized)
        {
            playerTransform.position = playerReturnPos;
        }
    }

    public static int PilotHighScore = 0;
    public static int ChefHighScore = 0;
    public static int DeliveryHighScore = 0;

    public static bool PilotCompleted = false;
    public static bool ChefCompleted = false;
    public static bool DeliveryCompleted = false;
}
