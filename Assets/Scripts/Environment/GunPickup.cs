using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPickup : MonoBehaviour
{
    private bool isPickedUp = false;
    // Store references to components to avoid repeated GetComponent calls
    private MeshRenderer gunMeshRenderer;
    private BoxCollider gunBoxCollider;

    void Awake() // Awake is called before Start, good for getting component references
    {
        // MODIFIED: Use GetComponentInChildren to find the MeshRenderer on this or any child.
        gunMeshRenderer = GetComponentInChildren<MeshRenderer>();
        // MODIFIED: Use GetComponentInChildren to find the BoxCollider on this or any child.
        gunBoxCollider = GetComponentInChildren<BoxCollider>();

        if (gunMeshRenderer == null)
        {
            Debug.LogError("GunPickup: MeshRenderer component not found on " + gameObject.name + " or its children in Awake!");
        }
        if (gunBoxCollider == null)
        {
            Debug.LogError("GunPickup: BoxCollider component not found on " + gameObject.name + " or its children in Awake!");
        }
    }

    void Start()
    {
        // Ensure the gun's mesh renderer and collider are off initially.
        if (gunMeshRenderer != null)
        {
            gunMeshRenderer.enabled = false;
        }
        if (gunBoxCollider != null)
        {
            gunBoxCollider.enabled = false;
        }

        Debug.Log("GunPickup: Initialized. MeshRenderer and BoxCollider should be off.");
    }

    public void ResetGunForSpawn()
    {
        isPickedUp = false;

        if (gunMeshRenderer != null)
        {
            gunMeshRenderer.enabled = true; // Make gun visible
            Debug.Log("GunPickup: MeshRenderer enabled!");
        }
        else
        {
            Debug.LogError("GunPickup: MeshRenderer is NULL in ResetGunForSpawn! Cannot enable it.");
        }

        if (gunBoxCollider != null)
        {
            gunBoxCollider.enabled = true;     // Make it interactable
            Debug.Log("GunPickup: BoxCollider enabled!");
        }
        else
        {
            Debug.LogError("GunPickup: BoxCollider is NULL in ResetGunForSpawn! Cannot enable it.");
        }

        Debug.Log("Gun is ready for pickup.");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isPickedUp)
        {
            Debug.Log("Player entered gun pickup area. Press E to pick up.");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && !isPickedUp && Input.GetKeyDown(KeyCode.E))
        {
            PickUpGun();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !isPickedUp)
        {
            Debug.Log("Player left gun pickup area.");
        }
    }

    private void PickUpGun()
    {
        isPickedUp = true;
        Debug.Log("Gun picked up!");

        if (GameFlowManager.Instance != null)
        {
            GameFlowManager.Instance.UnlockActiveDoor();
        }
        else
        {
            Debug.LogError("GameFlowManager.Instance is null! Cannot unlock door.");
        }

        if (gunMeshRenderer != null) gunMeshRenderer.enabled = false;
        if (gunBoxCollider != null) gunBoxCollider.enabled = false;

        Debug.Log("GunPickup: Gun hidden and collider disabled after pickup.");
    }
}
