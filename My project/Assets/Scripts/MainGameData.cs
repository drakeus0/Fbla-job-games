using UnityEngine;

public static class MainGameData
{
    public static Vector3 playerReturnPos;

    private static bool initialized = false;

    public static void Initialize(Transform playerTransform)
    {
        if (!initialized)
        {
            playerReturnPos = playerTransform.position;
            initialized = true;
        }
    }
}
