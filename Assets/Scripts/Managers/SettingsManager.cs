using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanelObject;

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

    void Awake()
    {
        /*create an array to hold the resolutions
        Resolution[] resolutions = Screen.resolutions;
        //create a list of strings to hold the resolution options, without the hz
        List<string> resolutionOptions = new List<string>();
        foreach (Resolution r in resolutions)
        {
            resolutionOptions.Add(r.width + " x " + r.height);
        }
        resolutionNavButton.SetOptionsList(resolutionOptions);*/
    }

    void OnEnable()
    {
        //resolutionNavButton.SetOption(PlayerPrefs.GetInt("Resolution", resolutions.Length - 1));

        qualityNavButton.SetOption(PlayerPrefs.GetInt("Quality", 3));

        fullscreenNavButton.SetOption(PlayerPrefs.GetInt("Fullscreen", 1));

        masterSlider.value = PlayerPrefs.GetFloat("MasterVolume", 100);
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 100);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 100);
    }

    void Start()
    {

    }

    void Update()
    {
        
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution[] resolutions = Screen.resolutions;
        Screen.SetResolution(resolutions[resolutionIndex].width, resolutions[resolutionIndex].height, Screen.fullScreen);
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

    public void Apply()
    {
        //apply quality settings
        SetQuality(qualityNavButton.CurrentOptionIndex);
        PlayerPrefs.SetInt("Quality", qualityNavButton.CurrentOptionIndex);
        Debug.Log("Quality: " + qualityNavButton.CurrentOptionIndex);

        //apply fullscreen settings
        SetFullscreen(fullscreenNavButton.CurrentOptionIndex == 1);
        PlayerPrefs.SetInt("Fullscreen", fullscreenNavButton.CurrentOptionIndex);
        Debug.Log("Fullscreen: " + fullscreenNavButton.CurrentOptionIndex);

        //apply resolution settings
        SetResolution(resolutionNavButton.CurrentOptionIndex);
        PlayerPrefs.SetInt("Resolution", resolutionNavButton.CurrentOptionIndex);
        Debug.Log("Resolution: " + resolutionNavButton.CurrentOptionIndex);

        //apply volume settings
        
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
