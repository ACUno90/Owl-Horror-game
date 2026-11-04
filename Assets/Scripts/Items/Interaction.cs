using System.Diagnostics.Contracts;
using UnityEngine;


public interface IInteractable
{
    void Interact();
}


public class Interaction : MonoBehaviour
{
    public float interactionDistance = 3f;
    public GameObject InteractUI;

    private IInteractable Currentinteractable;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InteractUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        DectectInteractable();

        if(Currentinteractable != null && Input.GetKeyDown(KeyCode.E))
        {
            Currentinteractable.Interact();
        }
    }

    void DectectInteractable()
    {
        Ray hit = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if (Physics.Raycast(hit, out RaycastHit ray, interactionDistance))
        {
            IInteractable interactable = ray.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                Currentinteractable = interactable;
                InteractUI.SetActive(true);
                return;
            }
          
        }
        Currentinteractable = null;
        InteractUI.SetActive(false);

    }
}
