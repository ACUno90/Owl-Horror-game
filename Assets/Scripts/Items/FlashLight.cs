using UnityEngine;
using UnityEngine.InputSystem;

public class FlashLight : MonoBehaviour
{
    [SerializeField] private Light flashlight;
    [SerializeField] private InputActionReference toggleFlashlightAction;


    private void OnEnable()
    {
        toggleFlashlightAction.action.performed += onToggleFlashlight;

    }

    private void OnDisable()
    {
        toggleFlashlightAction.action.performed -= onToggleFlashlight;

    }


    private void onToggleFlashlight(InputAction.CallbackContext ctx)
    {
         flashlight.enabled = !flashlight.enabled;  
    }
}
