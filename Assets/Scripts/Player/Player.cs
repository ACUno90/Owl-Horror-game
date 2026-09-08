using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
public class Player : MonoBehaviour, IDamage
{
    [Header("Player Stats")]
    [SerializeField] public CharacterController controller;
    [SerializeField]  float speed;
   public float health;
   public float MaxHealth;
    [SerializeField] int CameraShotDistance;
    [SerializeField] int gravity;
    public Vector3 PlayerMoveDirc;

    [Header("Other stuff")]
   public Vector2 turn;
    public float CameraSensitivity = .5f;
    Vector3 PlayerVal;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;

    }
      
    // Update is called once per frame
    void Update()
    {
        //to rotate the camera and player with the mouse
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * CameraShotDistance, Color.green);
        turn.x += Input.GetAxis("Mouse X") * CameraSensitivity;
        turn.y += Input.GetAxis("Mouse Y") * CameraSensitivity;
        transform.rotation = Quaternion.Euler(-turn.y, turn.x, 0);
       
        Movement(); 
    }



    public void Movement()
    {
        if(controller.isGrounded)
        {
           PlayerVal = Vector3.zero;
            PlayerMoveDirc.y = 0f;
        }
        // get the input from the player and calculate the movement direction
        PlayerMoveDirc = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;
        // apply gravity to the player
        PlayerMoveDirc.y = -gravity * Time.deltaTime;
        // move the player based on the input and speed
        controller.Move(PlayerMoveDirc * speed * Time.deltaTime);
        controller.Move(PlayerVal * Time.deltaTime);
    }

    public void TakeDamage(int damage)
    {
        health -=damage;

        if (health > 0) { 
          //show losing screen 
            
        }
    }
}
