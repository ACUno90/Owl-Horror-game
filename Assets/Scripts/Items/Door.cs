using UnityEngine;
public class Door : MonoBehaviour
{
    [SerializeField] private float interactionDistance = 3f;


    void Update()
    {
        if (!Input.GetMouseButtonDown(0) || Inventory.Instance == null || !Inventory.Instance.IsHoldingKey())
        {
            return;
        }

        PlayerCamera camera = PlayerCamera.Instance;
        if (camera == null)
        {
            return;
        }

        Ray ray = new Ray(camera.transform.position, camera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance))
        {
            Door hitDoor = hit.collider.GetComponentInParent<Door>();
            if (hitDoor == this)
            {

                Inventory.Instance.ConsumeItem(1);
                Destroy(gameObject);
            }
        }
    }
}
