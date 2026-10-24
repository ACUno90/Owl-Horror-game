using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using JetBrains.Annotations;
using System.Data;
using UnityEngine.Rendering;
using Unity.VisualScripting;
using UnityEngine.EventSystems;

public class GameManger : MonoBehaviour
{
    [Header("Menues")]
    [SerializeField] GameObject Menu_Active;
    [SerializeField] GameObject Menu_Win;
    [SerializeField] GameObject Menu_Pause;
    [SerializeField] GameObject Menu_Lose;

    [SerializeField] GameObject Menu_Inventory;

    [Header("Player stuff")]
    public static GameManger Instance;
    public GameObject player;
    public Player PlayerScript;
    public Image PlayerHPBar;
    public Image PlayerHealthColor;
    public Slider HealthSlider;
    public Gradient healthGradient;

    [Header("Menues")]
    [SerializeField] private GameObject FirstSelectMenu;
    [SerializeField] private GameObject FirstLosingMenu;

    [SerializeField] private GameObject FirstWiningMenu;
    
    
    //other stuff
    float timeScale_OG;
    public bool Ispaused;
    public bool IsInventoryOpen;

    public int WinConditionCount;

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
        if(Input.GetButtonDown("Cancel"))
        {
            if (Menu_Active == null)
            {
                StartPause();
                Menu_Active = Menu_Pause;
                Menu_Pause.SetActive(Ispaused);
            }
            else if (Menu_Active == Menu_Pause)
            {
                StartUnPause();
            }
        }

        if (Input.GetButtonDown("Inventory"))
        {
            if (Menu_Active == null)
            {
                //StartPause();
                InventoryMenu();
                Menu_Active = Menu_Inventory;
               Menu_Inventory.SetActive(Ispaused);
            }
            else if (Menu_Active == Menu_Inventory)
            {
              // CloseInventoryMenu();
              StartUnPause();
                //Menu_Active = null;
            }
        }


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

    public void StartPause()
    {
        Ispaused = !Ispaused;
        Time.timeScale = 0f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        PlayerScript.enabled = false;
        if (player != null)
        {
            PlayerCamera camera = player.GetComponentInChildren<PlayerCamera>();
            if (camera != null)
            {
                camera.enabled = false;
            }
        }
        HealthSlider.enabled = false;
        EventSystem.current.SetSelectedGameObject(FirstSelectMenu);
    }

    public void StartUnPause()
    {
        Ispaused = !Ispaused;
        Time.timeScale = timeScale_OG;  
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;   
        Menu_Active.SetActive(Ispaused);
        Menu_Active = null;
        PlayerScript.enabled = true;
        if (player != null)
        {
            PlayerCamera camera = player.GetComponentInChildren<PlayerCamera>();
            if (camera != null)
            {
                camera.enabled = true;
            }
        }
        HealthSlider.enabled = true;
        EventSystem.current.SetSelectedGameObject(null);    
    }

    public void StartLose() 
    {
        // StartPause();
        Time.timeScale = 0f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Menu_Active = Menu_Lose;
        Menu_Lose.SetActive(true);
        PlayerScript.enabled = false;
        HealthSlider.enabled = false;
        EventSystem.current.SetSelectedGameObject(FirstLosingMenu);
    }

    public void UpdateGameGoal(int Goal)
    {
        WinConditionCount += Goal;
        if (WinConditionCount <=0) {
            Time.timeScale = 0f;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            Menu_Active = Menu_Win;
            Menu_Win.SetActive(true);
            PlayerScript.enabled = false;
            HealthSlider.enabled = false;
            EventSystem.current.SetSelectedGameObject(FirstWiningMenu);

        }
    }

    public void InventoryMenu()
    {
        Ispaused = !Ispaused;
        //  StartPause();
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        PlayerScript.enabled = false;
        if (player != null)
        {
            PlayerCamera camera = player.GetComponentInChildren<PlayerCamera>();
            if (camera != null)
            {
                camera.enabled = false;
           
            }
        }
        HealthSlider.enabled = false;
    }
    
    public void CloseInventoryMenu()
    {
        Ispaused = !Ispaused;
        //  StartUnPause();
        Time.timeScale = timeScale_OG;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        PlayerScript.enabled = true;
        if (player != null)
        {
            PlayerCamera camera = player.GetComponentInChildren<PlayerCamera>();
            if (camera != null)
            {
                camera.enabled = true;
            }
        }
        HealthSlider.enabled = true;
    }

}
