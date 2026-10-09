using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    public GameObject doorCollider;


    void Start()
    {
        doorCollider.SetActive(true);
    }

    // Update is called once per frame
    //public void OnTriggerEnter(Collider other)
    //{
        

    //    //Player player = other.GetComponent<Player>() ?? other.GetComponentInParent<Player>();
    //    //if (player != null)
    //    //{
    //    //    doorCollider.SetActive(true);
    //    //    Destroy(gameObject);
    //    //}

    //    if(other.gameObject.tag == "Player")
    //    {
    //        doorCollider.SetActive(false);
    //        Destroy(gameObject);
    //    }
        
          


    //  //Debug.LogWarning("KeyPlayer component not found on the collided object.");
    //}




    public void OpenDoor()
    {

      ///  if (Input.GetKeyDown(KeyCode.E))
        //{
            doorCollider.SetActive(true);
            Destroy(gameObject);
           // Debug.LogWarning("Key: dor is gone!!.");

       // }
        Debug.LogWarning("Key: OpenDoor called.");

    }
}
