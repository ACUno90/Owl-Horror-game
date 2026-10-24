using UnityEngine;

public class HealthPickUp : MonoBehaviour
{
    [SerializeField] Transform HealthPickUpModel;

    [Header("audio")]
    public AudioClip AUDHealSound;
    public float AUDHealVol;
    public AudioSource HealAUD;

    private void OnTriggerEnter(Collider other)
    {
        // Try to find a Player component on the collider itself or on parent objects.
        Player player = other.GetComponent<Player>() ?? other.GetComponentInParent<Player>();
        HealAUD.PlayOneShot(AUDHealSound, AUDHealVol);
        if (player != null)
        {
           

            player.TakeHealth(10); // Heal 10 health points for the player
       
            Destroy(HealthPickUpModel.gameObject);
            return;
        }
     
    }
}
