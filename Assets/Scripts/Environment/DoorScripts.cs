using UnityEngine;

public class DoorScripts : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        Debug.Log($"{gameObject.tag} interacted with!");

        // Add specific behavior here based on tag or logic
        if (CompareTag("KeyRed"))
        {
          
        }
        else if (CompareTag("KeyGreen"))
        {
            // play metal clank sound or unlock with key
        }
        else if (CompareTag("KeyRed"))
        {
            // play metal clank sound or unlock with key
        }
        else if (CompareTag("KeyBlack"))
        {
            // play metal clank sound or unlock with key
        }
    }
}
