using UnityEngine;

public class PlayerCusomization : MonoBehaviour
{
    public static PlayerCusomization Instance;

    public int hairIndex = 0;
    public int hairColorIndex = 0;
    public int shirtIndex = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
