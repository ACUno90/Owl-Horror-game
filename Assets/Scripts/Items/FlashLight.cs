using UnityEngine;
using UnityEngine.InputSystem;

public class FlashLight : MonoBehaviour
{
    [SerializeField] private Light flashlight;
    [SerializeField] private InputActionReference toggleFlashlightAction;

    public AudioClip[] AUDFlashlightClick;
    public float AUDclickFlashVolume;
    public AudioSource FlashlughtAUD;

    private void OnEnable()
    {
        FlashlughtAUD.PlayOneShot(AUDFlashlightClick[Random.Range(0, AUDFlashlightClick.Length)], AUDclickFlashVolume);
        toggleFlashlightAction.action.performed += onToggleFlashlight;
   
    }
    

    private void OnDisable()
    {
        FlashlughtAUD.PlayOneShot(AUDFlashlightClick[Random.Range(0, AUDFlashlightClick.Length)], AUDclickFlashVolume);
        toggleFlashlightAction.action.performed -= onToggleFlashlight;
      
    }


    private void onToggleFlashlight(InputAction.CallbackContext ctx)
    {
       
        flashlight.enabled = !flashlight.enabled;  
    }
}
