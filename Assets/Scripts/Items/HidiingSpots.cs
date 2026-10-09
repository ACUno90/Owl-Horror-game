using UnityEngine;

public class HidiingSpots : MonoBehaviour, IDamage
{
    public GameObject HidingSpot;
    public Collider HidingSpotCollider;
    public float health;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HidingSpotGone()
    {
        Destroy(gameObject);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
       // PlayerAUD.PlayOneShot(AUDPlayerHurt[Random.Range(0, AUDPlayerHurt.Length)], AUDPlayerVol);
        if (health > 0)
        {
           Destroy(gameObject);
        }
    }
}
