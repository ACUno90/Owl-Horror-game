using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
public class Player : MonoBehaviour, IDamage, IHealth
{
    [Header("Player Stats")]
    [SerializeField] public CharacterController controller;
    [SerializeField] Transform PlayerModel;
    public GameObject PlayerGun;
    [SerializeField]  float speed;
   public float health;
    private bool IsEquipped = false;
    public float MaxHealth;
    [SerializeField] int CameraShotDistance;
    [SerializeField] int gravity;
    public Vector3 PlayerMoveDirc;
    [SerializeField] float CrouchHeight =1f;
    [SerializeField] float CrouchSpeed = 2f;
    private Vector3 InitialCameraPosition; //for crounching
    private Vector3 InitialCameraPositionHeight;



    public float Height
    {
        get => controller.height;   
        set => controller.height = value;
     
    }

    public float ModelHeight
    {
        get => PlayerModel.lossyScale.y;
        set => PlayerModel.localScale = new Vector3(PlayerModel.localScale.x, value, PlayerModel.localScale.z); 
    }

    [SerializeField] private Vector3 crouchCenter = new Vector3(0f, 0.5f, 0f);


    [Header("Other stuff")]
   public Vector2 turn;
    public float CameraSensitivity = .5f;
    Vector3 PlayerVal;

    [Header("audio")]
    public AudioClip[] AUDPlayerHurt;
    public AudioClip[] AUDPlayerHeal;
    public float AUDPlayerVol;

    public AudioClip[] AUDPlayerWalk;
    public float AUDPlayerVolWalk;
    public AudioSource PlayerAUD;


    private bool IsPlayingStop;
    private bool IsCrouching = false;
     private float originalHeight;
    private float originalModelHeight;
    private Vector3 originalCenter;
    private float originalSpeed;
    private Coroutine stepCoroutine;
    [Header("Ground Check")]
    [SerializeField] private float groundCastRadius = 0.2f; //0.6 is the best one for this
    [SerializeField] private float groundCastDistance = 0.3f; //0.7 is the best one for this
    [SerializeField] private Vector3 groundCastOffset = new Vector3(0f, -0.1f, 0f); // Offset to start the spherecast slightly below the player's position, -0.1f is the best one for this  
    [SerializeField] private LayerMask groundLayer = ~0;
    private bool isGrounded;
    [Header("Debug")]
    [SerializeField] private bool debugFootsteps = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        IsPlayingStop = false;
  
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        InitialCameraPosition = Camera.main.transform.localPosition;
       // InitialCameraPositionHeight = Camera.main.transform.localPosition.y;
        originalHeight = controller.height;
        originalModelHeight = PlayerModel.localScale.y;
        originalCenter = controller.center;
        originalSpeed = speed;
    }
      
    // Update is called once per frame
    void Update()
    {
        //to rotate the camera and player with the mouse
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * CameraShotDistance, Color.green);
        turn.x += Input.GetAxis("Mouse X") * CameraSensitivity;
        turn.y += Input.GetAxis("Mouse Y") * CameraSensitivity;
        transform.rotation = Quaternion.Euler(-turn.y, turn.x, 0);

        if(Input.GetKeyDown(KeyCode.LeftControl))
        {
            ToggleCrounch();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ToggleWeapon();
        }

        Movement();
    
    }



    public void Movement()
    {
        // update grounded state using a SphereCast from a small offset under the player
        var origin = transform.position + groundCastOffset;
        isGrounded = Physics.SphereCast(origin, groundCastRadius, Vector3.down, out RaycastHit hitInfo, groundCastDistance, groundLayer);

        if (isGrounded)
        {
            PlayerVal = Vector3.zero;
            PlayerMoveDirc.y = 0f;

            if (!IsPlayingStop && PlayerMoveDirc != Vector3.zero)
            {
                StartCoroutine(PlaySteps());
            }
        }

        // get the input from the player and calculate the movement direction
        PlayerMoveDirc = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;

        // horizontal movement only
        Vector3 horiz = new Vector3(PlayerMoveDirc.x, 0f, PlayerMoveDirc.z);
        bool isMoving = horiz.magnitude > 0.01f;
        if (debugFootsteps) Debug.Log($"Movement debug: isGrounded={isGrounded} horizMag={horiz.magnitude} isMoving={isMoving}");


        // apply gravity to the player
        PlayerMoveDirc.y = -gravity * Time.deltaTime;
        // move the player based on the input and speed
        controller.Move(PlayerMoveDirc * speed * Time.deltaTime);
        controller.Move(PlayerVal * Time.deltaTime);

    }

    private void OnDrawGizmosSelected()
    {
        // visualize spherecast origin and distance
        var origin = transform.position + groundCastOffset;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(origin, groundCastRadius);
        Gizmos.DrawLine(origin, origin + Vector3.down * groundCastDistance);
    }

    public void TakeDamage(int damage)
    {
        health -=damage;
        PlayerAUD.PlayOneShot(AUDPlayerHurt[Random.Range(0, AUDPlayerHurt.Length)], AUDPlayerVol);  
        if (health > 0) { 

          //show losing screen 
            GameManger.Instance.StartLose();    
        }
    }

    IEnumerator PlaySteps()
    {
       IsPlayingStop = true;
      
       // Play footstep sound
            PlayerAUD.PlayOneShot(AUDPlayerWalk[Random.Range(0, AUDPlayerWalk.Length)], AUDPlayerVolWalk);
        if (speed < 6)
        {
            yield return new WaitForSeconds(0.5f);
        }
        else
        {
            yield return new WaitForSeconds(0.3f);
        }
        IsPlayingStop = false;
    }

      public void TakeHealth(int healAmount)
    {
        health += healAmount;
        //PlayerAUD.PlayOneShot(AUDPlayerHeal[Random.Range(0, AUDPlayerHeal.Length)], AUDPlayerVol);
    }

    public void Crounch()
    {
        float standingHeight = Height; // The height of the character when standing
        float PlayerModelHeight = ModelHeight; // The height of the character model
        float crouchingHeight = standingHeight / 2; // The height of the character when crouching
        float crouchHeightPlayerModel = PlayerModelHeight / 2; // The difference in height between standing and crouching
           //  IsCrouching = false;
    
            float halfHeightDifference = new Vector3(0, (standingHeight - CrouchHeight) / 2, 0).y;
           // float halfHeightDifferenceModel = new Vector3(0, (PlayerModelHeight - crouchHeightPlayerModel) / 2, 0).y;
            var newCameraPosition = InitialCameraPosition - new Vector3(0, CrouchHeight, 0);
            Height = CrouchHeight;
            ModelHeight = CrouchHeight;
      //  controller.center = crouchCenter;   
        PlayerModel.localPosition = new Vector3(PlayerModel.localPosition.x, crouchCenter.y, PlayerModel.localPosition.z);  
      
        PlayerCamera.Instance.transform.localPosition = newCameraPosition;
            speed = CrouchSpeed;
             IsCrouching = true;
    }

    public void UnCrounch()
    {
   
            Height = originalHeight;
            ModelHeight = originalModelHeight;
       // controller.center = originalCenter;
        PlayerCamera.Instance.transform.localPosition = InitialCameraPosition;
            speed = originalSpeed;
            IsCrouching = false;
   
    }

    private void ToggleCrounch()
    {
        if(IsCrouching == true)
        {
            UnCrounch();
        }
        else
        {
            Crounch();
        }
    }


    private void ToggleWeapon()
    {
        if (IsEquipped == true)
        {
            HideWeapon();
        }
        else
        {
            ShowWeapon();
        }
    }

    public void HideWeapon()
    {
        PlayerGun.SetActive(false);
        IsEquipped = false;
    }

    public void ShowWeapon()
    {
        PlayerGun.SetActive(true);
        IsEquipped = true;
    }
}
