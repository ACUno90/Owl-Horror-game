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
