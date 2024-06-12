using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("Quality")]
    [SerializeField] private UINavButton qualityNavButton;
    [SerializeField] private UINavButton fullscreenNavButton;
    [SerializeField] private UINavButton resolutionNavButton;

    [Header("Volume")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Button masterPlay;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Button musicPlay;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Button sfxPlay;

    [Header("Settings")]
    Resolution[] resolutions;
    int resolutionIndex;
    int qualityIndex;
    int fullscreenIndex;

    void Awake()
    {
        resolutions = Screen.resolutions;
        List<string> resolutionOptions = new List<string>();

        for (int i = 0; i < resolutions.Length; i++)
        {
            Resolution r = resolutions[i];
            resolutionOptions.Add(r.width + "x" + r.height);
            if (r.width == Screen.currentResolution.width && r.height == Screen.currentResolution.height)
            {
                resolutionIndex = i;
            }
        }
        resolutionNavButton.SetOptionsList(resolutionOptions);
        resolutionNavButton.SetOption(resolutionIndex, Color.white);
    }

    void OnEnable()
    {
        resolutionIndex = PlayerPrefs.GetInt("Resolution", 6);
        qualityIndex = PlayerPrefs.GetInt("Quality", 3);
        fullscreenIndex = PlayerPrefs.GetInt("Fullscreen", 1);

        resolutionNavButton.SetOption(resolutionIndex, Color.white);
        qualityNavButton.SetOption(qualityIndex, Color.white);
        fullscreenNavButton.SetOption(fullscreenIndex, Color.white);

        float master = PlayerPrefs.GetFloat("MasterVolume", 100);
        masterSlider.value = master;
        
        float music = PlayerPrefs.GetFloat("MusicVolume", 100);
        musicSlider.value = music;

        float sfx = PlayerPrefs.GetFloat("SFXVolume", 100);
        sfxSlider.value = sfx;
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetResolutionAndFullscreen(int resolutionIndex, int fullscreenIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, fullscreenIndex == 1 ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed);
    }

    public void SetMasterVolume(float volume)
    {
        volume = Map(volume, 0, 100, 0.0001f, 1);
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20f);
    }

    public void SetBGMVolume(float volume)
    {
        volume = Map(volume, 0, 100, 0.0001f, 1);
        audioMixer.SetFloat("BGMVolume", Mathf.Log10(volume) * 20f);
    }

    public void SetSFXVolume(float volume)
    {
        volume = Map(volume, 0, 100, 0.0001f, 1);
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20f);
    }

    public void Cancel()
    {
        SetQuality(PlayerPrefs.GetInt("Quality", 3));
        SetResolutionAndFullscreen(PlayerPrefs.GetInt("Resolution", 6), PlayerPrefs.GetInt("Fullscreen", 1));

        SetMasterVolume(PlayerPrefs.GetFloat("MasterVolume", 1));
        SetBGMVolume(PlayerPrefs.GetFloat("MusicVolume", 1));
        SetSFXVolume(PlayerPrefs.GetFloat("SFXVolume", 1));
    }

    public void Apply()
    {           
        PlayerPrefs.SetInt("Quality", qualityNavButton.CurrentOptionIndex);
        PlayerPrefs.SetInt("Resolution", resolutionNavButton.CurrentOptionIndex);
        PlayerPrefs.SetInt("Fullscreen", fullscreenNavButton.CurrentOptionIndex);

        SetQuality(qualityNavButton.CurrentOptionIndex);
        SetResolutionAndFullscreen(resolutionNavButton.CurrentOptionIndex, fullscreenNavButton.CurrentOptionIndex);

        PlayerPrefs.SetFloat("MasterVolume", masterSlider.value);
        PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
        PlayerPrefs.SetFloat("SFXVolume", sfxSlider.value);

        PlayerPrefs.Save();
    }

    public float Map (float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
}
