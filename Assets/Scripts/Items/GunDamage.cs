using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunDamage : MonoBehaviour
{
    public int damage;
    public float BulletRange;
    private Transform PlayerCam;

   public Transform Barrel;

    void Start()
    {
        PlayerCam =Camera.main.transform;   
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Shoot()
    {
        Ray hit = new Ray(Barrel.position, Barrel.forward);
        if (Physics.Raycast(hit, out RaycastHit HitInfo, BulletRange))
        {
            IDamage damageable = HitInfo.transform.GetComponent<IDamage>();
            if (damageable != null && HitInfo.transform.CompareTag("Monster"))
            {
                damageable.TakeDamage(damage);
            }
        }
    }
}
