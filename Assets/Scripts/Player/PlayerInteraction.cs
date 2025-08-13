using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public interface IInteractable
{
    void Interact();
}

public class PlayerInteraction : MonoBehaviour
{
    public Transform InteractorSource;
    public float InteractRange;

    void Start()
    {
        if (InteractorSource == null)
        {
            Debug.LogError("PlayerInteraction: InteractorSource is not assigned! Please assign your camera or a suitable transform.");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Ray ray = new Ray(InteractorSource.position, InteractorSource.forward);
            RaycastHit hitInfo;

            // Debug: Log that a raycast is being performed
            Debug.Log("PlayerInteraction: E key pressed. Performing raycast.");

            if (Physics.Raycast(ray, out hitInfo, InteractRange))
            {
                GameObject hitObject = hitInfo.collider.gameObject;
                // Debug: Log what object was hit
                Debug.Log($"PlayerInteraction: Raycast hit object: {hitObject.name}");

                // Try to get the IInteractable component
                IInteractable interactObj;
                if (hitObject.TryGetComponent(out interactObj))
                {
                    // Debug: Log that an IInteractable was found
                    Debug.Log($"PlayerInteraction: Found IInteractable on {hitObject.name}. Calling Interact().");
                    interactObj.Interact();
                }
                else
                {
                    // Debug: Log that no IInteractable was found and list components
                    Debug.LogWarning($"PlayerInteraction: Hit {hitObject.name} but it is not interactable. Components on this GameObject:");
                    foreach (Component comp in hitObject.GetComponents<Component>())
                    {
                        Debug.LogWarning($"- {comp.GetType().Name}");
                    }
                }
            }
            else
            {
                // Debug: Log if nothing was hit by the raycast
                Debug.Log("PlayerInteraction: Raycast hit nothing.");
            }
        }
    }
}
