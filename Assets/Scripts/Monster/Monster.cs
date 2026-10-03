using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Monster : MonoBehaviour, IDamage
{
    [Header("Monster Stats")]
    [SerializeField] Renderer Model;
    [SerializeField] int HP;
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask Ground, WherePlayer;

    [Header("Patroling")]
    public Vector3 WalkPoint;
    bool IsWalking;
    [SerializeField] float walkpointRange;
    [SerializeField] float SightRange;
    bool isinSight;


    [Header("audio")]
    public AudioClip[] AUDMonsterDeath;
    public float AUDMonsterVol;
    public AudioSource MonsterAUD;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        // check if the player is in sight range
        isinSight = Physics.CheckSphere(transform.position, SightRange, WherePlayer);
        if (!isinSight)
        {
            Patroling();
        }
        if (isinSight)
        {
            Chase();
        }
    }

    public void Patroling()
    {
        // if the monster is not walking, then search for a walk point
        if (!IsWalking)
        {
            SearchWalkpath();
        }
        // if the monster is walking, then set the destination to the walk point
        if (IsWalking)
        {
            agent.SetDestination(WalkPoint);
        }
        // calculate the distance between the monster and the walk point
        Vector3 DistanceWalking = transform.position - WalkPoint;
        // if the distance between the monster and the walk point is less than 1, then the monster is not walking anymore
        if (DistanceWalking.magnitude < 1f)
        {
            IsWalking = false;
        }

    }

    private void SearchWalkpath()
    {
        // generate a random walk point within the walkpoint range
        float randomZ = Random.Range(-walkpointRange, walkpointRange);
        float randomX = Random.Range(-walkpointRange, walkpointRange);
        // set the walk point to the random position
        WalkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
        // check if the walk point is on the ground
        if (Physics.Raycast(WalkPoint, -transform.up, 2f, Ground))
        {
            IsWalking = true;
        }
    }

    public void Chase()
    {
        // set the destination of the monster to the player's position
        agent.SetDestination(player.position);
    }   

    public void TakeDamage(int damage)
    {
        HP -= damage;
        if (HP <= 0)
        {
            MonsterAUD.PlayOneShot(AUDMonsterDeath[Random.Range(0, AUDMonsterDeath.Length)], AUDMonsterVol);
            Destroy(gameObject);
        }
    }

    public void OnMeleeHit(Collider other)
    {
        // Try to find a Player component on the collider itself or on parent objects.
        Player player = other.GetComponent<Player>() ?? other.GetComponentInParent<Player>();
        if (player != null)
        {
            player.TakeDamage(10); // Deal 10 damage to the player
            return;
        }

        Debug.LogWarning("Monster: Player component not found on the collided object.");
    }

    private void OnTriggerEnter(Collider other)
    {
        OnMeleeHit(other);
    }

  
}
