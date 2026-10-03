using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public class interactables : MonoBehaviour
{
    Outline outline;
    public string Message;
    public UnityEvent onInteraction;

    void Start()
    {
        outline = GetComponent<Outline>();
        DisableOutline();
    }

    // Update is called once per frame
    public void Interact()
    {
        onInteraction.Invoke();
    }

    public void DisableOutline()
    {
        outline.enabled = false;    
    }

    public void EnableOutline()
    {
        outline.enabled = true;
    }   
}
