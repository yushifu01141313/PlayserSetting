public static class DisplayOption
{
    public static bool OpenShadowsOption
    {
        get => PlayerPrefsUtility.GetBool(PlayerPrefsKey.ShadowsOption);
        set => PlayerPrefsUtility.SetBool(PlayerPrefsKey.ShadowsOption, value);
    }

    public static bool AntialiasingOption
    {
        get => PlayerPrefsUtility.GetBool(PlayerPrefsKey.AntialiasingOption);
        set => PlayerPrefsUtility.SetBool(PlayerPrefsKey.AntialiasingOption, value);
    }

    public static ResolutionRatioOptions ResolutionRatio
    {
        get => (ResolutionRatioOptions)PlayerPrefsUtility.GetInt(PlayerPrefsKey.ResolutionRatioOptionsOption);
        set => PlayerPrefsUtility.SetInt(PlayerPrefsKey.ResolutionRatioOptionsOption, (int)value);
    }

    public static GraphicsOptions ImageQuality
    {
        get => (GraphicsOptions)PlayerPrefsUtility.GetInt(PlayerPrefsKey.GraphicsOptions);
        set => PlayerPrefsUtility.SetInt(PlayerPrefsKey.GraphicsOptions, (int)value);
    }
}
public enum ResolutionRatioOptions
{
    Level1,
    Level2,
    Level3,
    Level4,
    Level5,
}

public enum GraphicsOptions
{
    Low,
    Medium,
    Hight,
}
