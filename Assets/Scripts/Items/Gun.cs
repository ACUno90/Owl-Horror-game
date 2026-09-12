using UnityEngine.Events;
using UnityEngine;


public class Gun : MonoBehaviour
{
    public UnityEvent OnGunShot;
    public float FireCooldown;

    public bool Automatic;
    private float CurrentCooldown;

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

                }
            }
        }
        CurrentCooldown -= Time.deltaTime;
    }
}
