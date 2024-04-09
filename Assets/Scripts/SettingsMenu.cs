using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SettingsMenu : MonoBehaviour
{
    public AudioMixer MusicSettings;

    public void SetVolume(float volume)
    {
        MusicSettings.SetFloat("volume", volume);
    }

    //public void SetQuality(int qualityIndex)
    //{
    //    QualitySettings.SetQualityLevel(qualityIndex);
    //}

    public void LOW()
    {
        QualitySettings.SetQualityLevel(0);
    }

    public void MEDIME()
    {
        QualitySettings.SetQualityLevel(1);
    }

    public void HIGH()
    {
        QualitySettings.SetQualityLevel(2);
    }

    public void ULTRA()
    {
        QualitySettings.SetQualityLevel(3);
    }
}