using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
  //  public GameObject doorCollider;


    void Start()
    {
       // doorCollider.SetActive(true);
    }

    private void Update()
    {


    }


    public void OpenDoor()
    {


        if (Input.GetMouseButtonDown(1))
        {

            Inventory.Instance.ConsumeItem(1);
            Destroy(gameObject);
          
        }
        Debug.LogWarning("Key: OpenDoor called.");

    }
}
