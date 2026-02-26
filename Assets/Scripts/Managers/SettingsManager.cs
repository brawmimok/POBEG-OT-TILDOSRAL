using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("Audio Mixer")]
    public AudioMixer audioMixer;

    [Header("Sliders")]
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider uiSlider;

    private void Start()
    {
        LoadVolume(masterSlider, "MasterVolume");
        LoadVolume(musicSlider, "MusicVolume");
        LoadVolume(sfxSlider, "SFXVolume");
        LoadVolume(uiSlider, "UIVolume");

        masterSlider.onValueChanged.AddListener(value => SetVolume(value, "MasterVolume"));
        musicSlider.onValueChanged.AddListener(value => SetVolume(value, "MusicVolume"));
        sfxSlider.onValueChanged.AddListener(value => SetVolume(value, "SFXVolume"));
        uiSlider.onValueChanged.AddListener(value => SetVolume(value, "UIVolume"));
    }

    private void SetVolume(float value, string parameter)
    {
        if (value <= 0.0001f)
            value = 0.0001f;

        float volumeInDb = Mathf.Log10(value) * 20;
        audioMixer.SetFloat(parameter, volumeInDb);

        PlayerPrefs.SetFloat(parameter, value);
    }

    private void LoadVolume(Slider slider, string parameter)
    {
        float value = PlayerPrefs.GetFloat(parameter, 1f);
        slider.value = value;
        SetVolume(value, parameter);
    }
}
