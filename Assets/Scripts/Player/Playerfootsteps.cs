using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerFootsteps : MonoBehaviour
{
    [Header("Footstep Settings")]
    [Tooltip("The audio clips for footstep sounds. One will be chosen randomly.")]
    public AudioClip[] footstepClips;
    [Tooltip("The time in seconds between each footstep sound.")]
    public float timeBetweenSteps = 0.5f;
    [Tooltip("The volume of the footstep sounds.")]
    [Range(0.0f, 1.0f)]
    public float footstepVolume = 0.7f;

    [Header("Pitch Variation")]
    [Tooltip("The minimum pitch for the footstep sound.")]
    [Range(0.5f, 1.0f)]
    public float minPitch = 0.9f;
    [Tooltip("The maximum pitch for the footstep sound.")]
    [Range(1.0f, 1.5f)]
    public float maxPitch = 1.1f;

    private AudioSource audioSource;
    private float stepTimer;
    private CharacterController characterController;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        characterController = GetComponent<CharacterController>();

        // Configure the AudioSource
        audioSource.volume = footstepVolume;
    }

    void Update()
    {
        // Get player movement input
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Check if the player is on the ground and moving
        if (characterController.isGrounded && (horizontalInput != 0 || verticalInput != 0))
        {
            // Player is moving, so handle playing the sound
            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0)
            {
                PlayFootstepSound();
                // Reset the timer
                stepTimer = timeBetweenSteps;
            }
        }
        else
        {
            // --- NEW LOGIC ---
            // Player is not moving or is in the air, so stop any playing sounds.
            // This will immediately cut off the footstep audio clip.
            audioSource.Stop();
        }
    }

    private void PlayFootstepSound()
    {
        if (footstepClips.Length == 0) return;

        int randomIndex = Random.Range(0, footstepClips.Length);
        AudioClip clipToPlay = footstepClips[randomIndex];

        audioSource.pitch = Random.Range(minPitch, maxPitch);

        // Play the sound. audioSource.Stop() in Update() will be able to cancel this.
        audioSource.PlayOneShot(clipToPlay);
    }
}

