using UnityEngine;

public class HealthPickUp : MonoBehaviour
{
    [SerializeField] Transform HealthPickUpModel;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        // Try to find a Player component on the collider itself or on parent objects.
        Player player = other.GetComponent<Player>() ?? other.GetComponentInParent<Player>();
        if (player != null)
        {
            player.TakeHealth(10); // Heal 10 health points for the player
            return;
        }
      Destroy(HealthPickUpModel.gameObject);
    }
}
