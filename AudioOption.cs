using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioOption
{
    public static float GlobalAudioVolume
    {
        get => PlayerPrefsUtility.GetFloat(PlayerPrefsKey.GlobalVolume);
        set => PlayerPrefsUtility.SetFloat(PlayerPrefsKey.GlobalVolume, value);
    }

    public static float MusicAudioVolume
    {
        get => PlayerPrefsUtility.GetFloat(PlayerPrefsKey.MusicVolume);
        set => PlayerPrefsUtility.SetFloat(PlayerPrefsKey.MusicVolume, value);
    }
    public static float EffectAudioVolume
    {
        get => PlayerPrefsUtility.GetFloat(PlayerPrefsKey.EffectAudioVolume);
        set => PlayerPrefsUtility.SetFloat(PlayerPrefsKey.EffectAudioVolume, value);
    }
}
