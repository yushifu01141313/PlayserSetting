using UnityEngine;

public static class PlayerPrefsUtility
{
    public static bool GetBool(string key)
    {
        if (PlayerPrefs.HasKey(key))
        {
            return true;
        }
        return false;
    }

    public static void SetBool(string key, bool b)
    {
        if (b)
        {
            PlayerPrefs.SetInt(key, 1);
        }
        else
        {
            PlayerPrefs.DeleteKey(key);
        }
    }

    public static int GetInt(string key)
    {
        if (PlayerPrefs.HasKey(key))
        {
            return PlayerPrefs.GetInt(key);
        }
        return 0;
    }

    public static void SetInt(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
    }

    public static float GetFloat(string key)
    {
        return PlayerPrefs.GetFloat(key, 0);
    }

    public static void SetFloat(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);
    }

    public static string GetString(string key)
    {
        return PlayerPrefs.GetString(key, "");
    }

    public static void SetString(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
    }
}
