using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    [Header("Audio")]
    public AudioMixer mainMixer; // Assign your main Audio Mixer asset
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;

    [Header("Controls")]
    public TextMeshProUGUI controlsText; // Assign a text element to display controls

    void Start()
    {
        // Set up listeners for when the slider values change
        if (masterVolumeSlider != null) masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        if (musicVolumeSlider != null) musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        if (sfxVolumeSlider != null) sfxVolumeSlider.onValueChanged.AddListener(SetSfxVolume);

        // Display the game controls
        DisplayControls();
    }

    // --- Audio Functions ---

    public void SetMasterVolume(float volume)
    {
        // The name "MasterVolume" must match the parameter you create in the Audio Mixer
        if (mainMixer != null) mainMixer.SetFloat("MasterVolume", ConvertToDecibels(volume));
    }

    public void SetMusicVolume(float volume)
    {
        if (mainMixer != null) mainMixer.SetFloat("MusicVolume", ConvertToDecibels(volume));
    }

    public void SetSfxVolume(float volume)
    {
        if (mainMixer != null) mainMixer.SetFloat("SfxVolume", ConvertToDecibels(volume));
    }

    // Helper function to convert linear slider value (0-1) to decibels (-80 to 0)
    private float ConvertToDecibels(float linearValue)
    {
        // Mathf.Log10(0) is undefined, so clamp the value to avoid errors
        return Mathf.Log10(Mathf.Max(linearValue, 0.0001f)) * 20f;
    }

    // --- Controls Function ---

    void DisplayControls()
    {
        if (controlsText != null)
        {
            // You can customize this text to list your game's controls
            controlsText.text = "<b><u>Game Controls</u></b>\n\n" +
                                "<b>Move:</b> W, A, S, D\n" +
                                "<b>Interact / Pick Up:</b> E\n" +
                                "<b>Shoot:</b> Left Mouse\n" +
                                "<b>Reload:</b> Right Mouse\n" +
                                "<b>Open Inventory:</b> I / Tab";
        }
    }
}