using UnityEngine;

public class HealthPickUp : MonoBehaviour
{
    public int Heal = 20;

    [Header("audio")]
    public AudioClip AUDHealSound;
    public float AUDHealVol;
    public AudioSource HealAUD;


    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Inventory.Instance.ConsumeItem(1);
            Player.Instance.health += Heal;
            Destroy(gameObject);
        }
    }

}
