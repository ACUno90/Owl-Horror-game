using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
public class Player : MonoBehaviour
{
    [Header("Player Stats")]
    [SerializeField] public CharacterController controller;
    [SerializeField]  float speed;
    [SerializeField]  float health;
    [SerializeField]  float MaxHealth;
    [SerializeField] int CameraShotDistance;

    [Header("Other stuff")]
   public Vector2 turn;
    public float CameraSensitivity = .5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;

    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * CameraShotDistance, Color.green);
        turn.x += Input.GetAxis("Mouse X") * CameraSensitivity;
        turn.y += Input.GetAxis("Mouse Y") * CameraSensitivity;
        transform.rotation = Quaternion.Euler(-turn.y, turn.x, 0);
    }
}
