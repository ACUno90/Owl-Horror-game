using UnityEngine.Events;
using UnityEngine;


public class Gun : MonoBehaviour
{
    public UnityEvent OnGunShot;
    public float FireCooldown;

    public bool Automatic;
    private float CurrentCooldown;

    public AudioClip[] AUDGun;
    public float AUDGunVol;
    public AudioSource GunAUD;

    void Start()
    {
        CurrentCooldown = FireCooldown; 
    }


    void Update()
    {
        if (Automatic)
        {
            if(Input.GetButton("Fire1"))
            {
               if(CurrentCooldown <= 0f)
                {
                    OnGunShot.Invoke();
                    CurrentCooldown = FireCooldown;
                    GunAUD.PlayOneShot(AUDGun[Random.Range(0, AUDGun.Length)], AUDGunVol);
                }
            }
        }
        else
        {
            if (Input.GetButtonDown("Fire1") )
            {

                if (CurrentCooldown <= 0f)
                {
                    OnGunShot.Invoke();
                    CurrentCooldown = FireCooldown;
                    GunAUD.PlayOneShot(AUDGun[Random.Range(0, AUDGun.Length)], AUDGunVol);

                }
            }
        }
        CurrentCooldown -= Time.deltaTime;
    }
}
