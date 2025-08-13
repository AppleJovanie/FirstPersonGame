using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFlowManager : MonoBehaviour
{
    // Singleton instance
    public static GameFlowManager Instance { get; private set; }

    // Reference to the single gun and spotlight in the scene.
    public GunPickup gameGun;
    public Light gameSpotlight;

    private DoorTriggerArea activeDoorTriggerArea = null;

    private void Awake()
    {
        // Implement singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep manager across scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
    }

    void Start()
    {
        if (gameGun == null)
        {
            Debug.LogError("GameFlowManager: Game Gun is not assigned! Please assign the single Gun GameObject.");
        }
        if (gameSpotlight == null)
        {
            Debug.LogError("GameFlowManager: Game Spotlight is not assigned! Please assign the single Spotlight GameObject.");
        }
        else
        {
            gameSpotlight.enabled = false;
        }
        
    }

    // Sets the currently active DoorTriggerArea and resets the gun spawn state.
    public void SetActiveDoorTriggerArea(DoorTriggerArea triggerArea)
    {
        // If there was a previously active trigger area, reset its gun spawn state.
        if (activeDoorTriggerArea != null)
        {
            activeDoorTriggerArea.ResetTriggerArea();
        }

        activeDoorTriggerArea = triggerArea;
        Debug.Log($"Active door trigger area set to: {triggerArea.name}");

        // Hide the gun and spotlight immediately when a new door is chosen.
        if (gameGun != null) gameGun.gameObject.SetActive(false);
        if (gameSpotlight != null) gameSpotlight.enabled = false;
    }

    // Gets the currently active DoorTriggerArea.
    public DoorTriggerArea GetActiveDoorTriggerArea()
    {
        return activeDoorTriggerArea;
    }

    // Spawns the single gun and spotlight at the given positions.
    public void SpawnGunAtActiveDoor(Vector3 gunPos, Vector3 spotlightPos)
    {
        if (gameGun != null && gameSpotlight != null)
        {
            gameGun.transform.position = gunPos;
            gameGun.gameObject.SetActive(true); // Make gun GameObject active
            gameGun.ResetGunForSpawn();         // Call gun's reset method

            gameSpotlight.transform.position = spotlightPos;
            gameSpotlight.enabled = true; // Turn on spotlight
            Debug.Log("Gun and spotlight spawned at active door.");
        }
        else
        {
            Debug.LogError("Cannot spawn gun/spotlight: references are null in GameFlowManager.");
        }
    }

    // Unlocks the currently active door after the gun is picked up.
    public void UnlockActiveDoor()
    {
        if (activeDoorTriggerArea != null && activeDoorTriggerArea.associatedDoor != null)
        {
            activeDoorTriggerArea.associatedDoor.UnlockDoor();
            Debug.Log($"Unlocked door: {activeDoorTriggerArea.associatedDoor.name}");
        }
        else
        {
            Debug.LogWarning("No active door to unlock or associated door is null.");
        }

        // Hide the gun and spotlight after pickup.
        if (gameGun != null) gameGun.gameObject.SetActive(false);
        if (gameSpotlight != null) gameSpotlight.enabled = false;
    }
}
