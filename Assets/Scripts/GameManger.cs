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

    [Header("Player stuff")]
    public static GameManger Instance;
    public GameObject player;
    public Player PlayerScript;
    public Image PlayerHPBar;
    public Image PlayerHealthColor;
    public Slider HealthSlider;
    public Gradient healthGradient;

    [Header("audio")]
    public AudioClip[] AUDclick;


    float timeScale_OG;


    void Awake()
    {
        Instance = this;
        timeScale_OG = Time.timeScale;
        // Try to resolve player and player script here, but be defensive in case they are not present yet.
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerScript = player.GetComponent<Player>();
            if (PlayerScript == null)
                Debug.LogError("GameManger: GameObject with tag 'Player' does not have a Player component attached.");
        }
        else
        {
            Debug.LogWarning("GameManger: No GameObject found with tag 'Player' during Awake.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHealthBar();
    }

    public void UpdateHealthBar()
    {
        // Ensure required references exist before attempting to update UI.
        if (PlayerScript == null)
        {
            // try to find player at runtime in case it wasn't available in Awake
            player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                PlayerScript = player.GetComponent<Player>();
            }
        }

        if (PlayerScript == null)
        {
            // nothing to display yet
            return;
        }

        if (HealthSlider == null || PlayerHealthColor == null)
        {
            Debug.LogWarning("GameManger: HealthSlider or PlayerHealthColor is not assigned in the inspector.");
            return;
        }

        SetMaxHealth();
        SetHealth();

    }   
     
    public void SetHealth()
    {
        if (PlayerScript == null || HealthSlider == null || PlayerHealthColor == null)
            return;

        HealthSlider.value = PlayerScript.health;
        PlayerHealthColor.color = healthGradient.Evaluate(HealthSlider.normalizedValue);
    }   

    public void SetMaxHealth()
    {
        if (PlayerScript == null || HealthSlider == null || PlayerHealthColor == null)
            return;

        HealthSlider.maxValue = PlayerScript.MaxHealth;
        HealthSlider.value = PlayerScript.health;
        PlayerHealthColor.color = healthGradient.Evaluate(1f);
    }

}
