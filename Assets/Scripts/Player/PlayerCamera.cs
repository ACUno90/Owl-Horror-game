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

    public static PlayerCamera Instance;
    void Start()
    {
        Instance = this;
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
        //clamp the roll value to prevent the camera from rotating too far up or down
        roll = Mathf.Clamp(roll, lockverMin, lockVerMax);
        // apply the roll value to the camera's local rotation
        transform.localPosition = Quaternion.Euler(roll, 0, 0) * transform.localPosition;
        // rotate the parent object (the player) around the y-axis based on the mouseX value
        transform.parent.Rotate(Vector3.up * mouseX);
    }
}
