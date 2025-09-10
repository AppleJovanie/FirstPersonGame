using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour 

    //Settings Manager
{
    [Header("Audio Settings")]
    [Tooltip("Assign your main AudioMixer asset here.")]
    public AudioMixer mainMixer;
    [Tooltip("Assign the volume slider from your Settings Panel.")]
    public Slider masterVolumeSlider;

    // The name of the exposed parameter in your AudioMixer (e.g., "MasterVolume").
    private const string MIXER_MASTER_VOLUME = "MasterVolume";
    // The key used to save the volume setting in PlayerPrefs.
    private const string PREFS_MASTER_VOLUME = "MasterVolume";

    private void Awake()
    {
        // This makes the SettingsManager a persistent singleton,
        // ensuring it doesn't get destroyed when changing scenes.
        SettingsManager[] managers = FindObjectsOfType<SettingsManager>();
        if (managers.Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        // When the game starts, load the saved volume preference.
        // If no preference is saved, it defaults to 1 (full volume).
        float savedVolume = PlayerPrefs.GetFloat(PREFS_MASTER_VOLUME, 1f);

        // Update both the slider's visual position and the actual audio mixer volume.
        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.value = savedVolume;
        }
        SetMasterVolume(savedVolume);
    }

    /// <summary>
    /// This public function should be called by the OnValueChanged event of your volume Slider.
    /// It takes the slider's value (from 0.0001 to 1) and converts it to decibels for the mixer.
    /// </summary>
    /// <param name="volume">The slider value.</param>
    public void SetMasterVolume(float volume)
    {
        if (mainMixer != null)
        {
            // The AudioMixer uses a logarithmic scale (decibels), so we convert the linear slider value.
            // A value of 1 becomes 0 dB (full volume), and a value of 0.0001 becomes -80 dB (silent).
            mainMixer.SetFloat(MIXER_MASTER_VOLUME, Mathf.Log10(volume) * 20);

            // Save the player's preference so it's remembered next time they play.
            PlayerPrefs.SetFloat(PREFS_MASTER_VOLUME, volume);
        }
    }
}
