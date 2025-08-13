using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // This might not be strictly needed in DoorTriggerArea, but kept if used elsewhere.

// CORRECTED: Changed class name from 'DoorTrigger' to 'DoorTriggerArea'
public class DoorTriggerArea : MonoBehaviour
{
    // Reference to the DoorController this trigger area is associated with.
    public DoorController associatedDoor;
    // This is the position where the gun should appear when this door is active.
    public Transform gunSpawnPoint;
    // This is the position where the spotlight should appear when this door is active.
    public Transform spotlightSpawnPoint;

    private bool hasGunBeenSpawned = false; // To ensure gun only spawns once per activation

    void Start()
    {
        if (associatedDoor == null)
            Debug.LogError($"{name}: Associated Door is not assigned!");

        if (gunSpawnPoint == null)
            Debug.LogError($"{name}: Gun Spawn Point is not assigned!");

        if (spotlightSpawnPoint == null)
            Debug.LogError($"{name}: Spotlight Spawn Point is not assigned!");
    }


    private void OnTriggerEnter(Collider other)
    {
        // Assuming your player has the tag "Player" and a Rigidbody/CharacterController.
        if (other.CompareTag("Player") && !hasGunBeenSpawned)
        {
            // Check if this DoorTriggerArea's associated door is the currently active one.
            if (GameFlowManager.Instance != null && GameFlowManager.Instance.GetActiveDoorTriggerArea() == this)
            {
                // Tell the GameFlowManager to spawn the single gun at this location.
                GameFlowManager.Instance.SpawnGunAtActiveDoor(gunSpawnPoint.position, spotlightSpawnPoint.position);
                hasGunBeenSpawned = true; // Prevent re-spawning the gun for this activation
            }
            else
            {
                Debug.Log("This door is not currently active. Press a button to activate it.");
                // Optionally, display a UI message: "This door is not active yet!"
            }
        }
    }

    // Reset this trigger area so the gun can spawn again if the door becomes active later
    public void ResetTriggerArea()
    {
        hasGunBeenSpawned = false;
    }
}
