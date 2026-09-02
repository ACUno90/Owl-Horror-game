using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using JetBrains.Annotations;
using System.Data;
using UnityEngine.Rendering;
using Unity.VisualScripting;

public class GameManger : MonoBehaviour
{
    [Header("Menues")]
    [SerializeField] GameObject Menu_Active;

    [Header("other")]
    public static GameManger Instance;
    public GameObject player;
    public Player PlayerScript;

    [Header("audio")]
    public AudioClip[] AUDclick;





    void Awake()
    {
        Instance = this;
        player = GameObject.FindGameObjectWithTag("Player");
        PlayerScript = player.GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
