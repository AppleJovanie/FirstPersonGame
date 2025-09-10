using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]

public class InteractionSoundPlayer : MonoBehaviour
{
    public static InteractionSoundPlayer Instance { get; private set; }

    private AudioSource audioSource;

    private void Awake()
    {
        // Create a static instance so any script can easily access this player.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
