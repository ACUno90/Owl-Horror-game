using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] int CSensitivity;
    [SerializeField] int lockverMin, lockVerMax;
    [SerializeField] bool inverY;
    float roll;
    float defaultLocalY;
    Player player;
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        player = GetComponent<Player>();
        defaultLocalY = transform.localPosition.y;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseY = Input.GetAxis("Mouse Y") * CSensitivity * Time.deltaTime;
        float mouseX = Input.GetAxis("Mouse X") * CSensitivity * Time.deltaTime;
        if (!inverY) { 
           roll += mouseY;
          
        }
        else
            roll += mouseX;
    }
}
