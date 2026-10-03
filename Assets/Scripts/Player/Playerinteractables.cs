using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Playerinteractables : MonoBehaviour
{
    public float playerReach = 3f;
    interactables currentInteractables;
    // Update is called once per frame
    void Update()
    {
        CheckInteraction(); 
        if (Input.GetKeyDown(KeyCode.E) && currentInteractables != null) {
            currentInteractables.Interact();
        }   
    }

    void CheckInteraction()
    {
        RaycastHit hit;
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if (Physics.Raycast(ray, out hit, playerReach))
        {
            if (hit.collider.tag == "Interactable")
            {
                interactables newInteractable = hit.collider.GetComponent<interactables>(); 
                if(currentInteractables && newInteractable != currentInteractables) {// if there is a currentInteractable and it is not the newInteractable
                   currentInteractables.DisableOutline();
                }
                if (newInteractable.enabled) {
                    SetNewCurrentInteractable(newInteractable);
                }
                else // if new interactable is disabled
                {
                    DisableCurrentInteractable();
                }
            }
            else // if not an interactable object, disable the current interactable
            {
                DisableCurrentInteractable();   
            }
        }
    }

    void SetNewCurrentInteractable(interactables NewInter)
    {
       
        currentInteractables = NewInter;
        currentInteractables.EnableOutline();
        HUDController.Instance.EnableInteractionText(currentInteractables.Message); 
    }

    void DisableCurrentInteractable()
    {
        HUDController.Instance.DisableInteractionText();    
        if (currentInteractables)
        {
            currentInteractables.DisableOutline();
            currentInteractables = null;
        }

    }
}
