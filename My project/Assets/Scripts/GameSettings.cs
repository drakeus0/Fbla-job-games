using UnityEngine;

public static class GameSettings
{
    public static bool musicEnabled = true;
    public static bool presentMode = false;
}

public enum SettingOption
{
    musicEnabled,
    presentMode,
}