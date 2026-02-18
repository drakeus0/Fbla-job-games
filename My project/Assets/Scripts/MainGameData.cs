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
}
