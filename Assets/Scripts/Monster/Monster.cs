using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.AI;
public class Monster : MonoBehaviour, IDamage
{
    [Header("Monster Stats")]
    [SerializeField] Renderer Model;
    [SerializeField] int HP;
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask Ground, WherePlayer,WhereHidingSpots;

    [Header("Patroling")]
    public Vector3 WalkPoint;
    bool IsWalking;
    bool IsSearchingHidingPlaces;
    [SerializeField] float walkpointRange;
    [SerializeField] float SightRange;
    bool isinSight;

    HidiingSpots hidingSpot;

    // Hiding spot targeting
    private HidiingSpots targetHidingSpotComp;
    private Vector3 targetApproachPoint;
    [Header("Hiding Spot Settings")]
    [SerializeField] private float hidingSampleRadius = 2f; // radius to sample navmesh around spot
    [SerializeField] private float hidingDestroyDistance = 1f; // how close to be to destroy the spot
    [SerializeField] private float hidingSearchInterval = 1f; // seconds between searches
    private float hidingSearchTimer = 0f;


    [Header("audio")]
    public AudioClip[] AUDMonsterDeath;
    public float AUDMonsterVol;
    public AudioSource MonsterAUD;
     public static Monster Instance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Instance = this;    
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        // check if the player is in sight range        
        isinSight = Physics.CheckSphere(transform.position, SightRange, WherePlayer | WhereHidingSpots);
        //// If we are already targeting a hiding spot, keep approaching it
        if (targetHidingSpotComp != null)
        {
            // keep moving toward the sampled approach point
            if (agent.destination != targetApproachPoint)
                agent.SetDestination(targetApproachPoint);

            // when close enough, destroy the hiding spot and clear target
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + hidingDestroyDistance)
            {
                if (targetHidingSpotComp != null)
                {
                    // destroy the referenced hiding object and clear the target
                    if (targetHidingSpotComp.HidingSpot != null)
                        Destroy(targetHidingSpotComp.HidingSpotCollider);
                    else
                        Destroy(targetHidingSpotComp.HidingSpotCollider);

                    targetHidingSpotComp = null;
                    agent.ResetPath();
                }
            }

            return; // while handling hiding spot, skip normal behavior
        }

        // periodic search for hiding spots (only when not currently handling one)
        hidingSearchTimer -= Time.deltaTime;
        if (hidingSearchTimer <= 0f)
        {
            hidingSearchTimer = hidingSearchInterval;
            SearchHidingPlaces();
        }

        if (!isinSight)
        {
            Patroling();
        }
        else
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
            Thread.Sleep(2000);
            Destroy(gameObject);
        }
    }

    private void OnMeleeHit(Collider other)
    {
        // Try to find a Player component on the collider itself or on parent objects.
        Player player = other.GetComponent<Player>() ?? other.GetComponentInParent<Player>();
        if (player != null)
        {
            player.TakeDamage(10); // Deal 10 damage to the player
            return;
        }

        HidiingSpots hs = other.GetComponent<HidiingSpots>() ?? other.GetComponentInParent<HidiingSpots>();
        if (hs != null)
        {
            hs.HidingSpotGone(); // Call the HidingSpotGone method on the HidiingSpots component    
            return;
        }

        Debug.LogWarning("Monster: Player component not found on the collided object.");
    }

    

    //private void DestoryHidingSpot(Collider other)
    //{
    //    // Debug information to help locate why the component isn't found
    //    Debug.Log($"Monster: DestoryHidingSpot invoked for collider '{other.name}' tag='{other.tag}' layer={LayerMask.LayerToName(other.gameObject.layer)} path='{GetTransformPath(other.transform)}'");

    //    // Direct lookup on the collider's GameObject
    //    HidiingSpots hidingSpot = other.GetComponent<HidiingSpots>();
    //    if (hidingSpot == null)
    //    {
    //        // Try parent and children explicitly
    //        hidingSpot = other.GetComponentInParent<HidiingSpots>();
    //    }
    //    if (hidingSpot == null)
    //    {
    //        hidingSpot = other.GetComponentInChildren<HidiingSpots>();
    //    }

    //    if (hidingSpot != null)
    //    {
    //        if (hidingSpot.HidingSpot != null)
    //        {
    //            Debug.Log($"Monster: Destroying hiding spot object '{hidingSpot.HidingSpot.name}' found via collider '{other.name}'");
    //            Destroy(hidingSpot.HidingSpot);
    //        }
    //        else
    //        {
    //            Debug.Log($"Monster: Destroying hiding spot component's GameObject '{hidingSpot.gameObject.name}' found via collider '{other.name}'");
    //            Destroy(hidingSpot.gameObject);
    //        }
    //        return;
    //    }

    //    // Fallback: search for HidiingSpots near the collision point (covers nested colliders or mis-assigned components)
    //    var searchPos = other.ClosestPoint(transform.position);
    //    Collider[] nearby = Physics.OverlapSphere(searchPos, 1f, WhereHidingSpots);
    //    foreach (var col in nearby)
    //    {
    //        var spot = col.GetComponent<HidiingSpots>() ?? col.GetComponentInParent<HidiingSpots>() ?? col.GetComponentInChildren<HidiingSpots>();
    //        if (spot != null)
    //        {
    //            if (spot.HidingSpot != null)
    //            {
    //                Debug.Log($"Monster: Destroying nearby hiding spot '{spot.HidingSpot.name}' found via OverlapSphere from collider '{other.name}'");
    //                Destroy(spot.HidingSpot);
    //            }
    //            else
    //            {
    //                Debug.Log($"Monster: Destroying nearby hiding spot component's GameObject '{spot.gameObject.name}' found via OverlapSphere from collider '{other.name}'");
    //                Destroy(spot.gameObject);
    //            }
    //            return;
    //        }
    //    }

    //    Debug.LogWarning($"Monster: No HidiingSpots found to destroy for collider '{other.name}'. Ensure the HidiingSpots component exists on the collider or its ancestors/children, and that the collider is on the WhereHidingSpots layer.");
    //}

    private string GetTransformPath(Transform t)
    {
        var parts = new System.Collections.Generic.List<string>();
        while (t != null)
        {
            parts.Insert(0, t.name);
            t = t.parent;
        }
        return string.Join("/", parts);
    }

    private void OnTriggerEnter(Collider other)
    {
        //DestoryHidingSpot(other);
        OnMeleeHit(other);

    }

    private void OnCollisionEnter(Collision collision)
    {
        // handle physical collisions as well
      //  DestoryHidingSpot(collision.collider);
        OnMeleeHit(collision.collider);
    }



    public void SearchHidingPlaces()
    {
        // find all hiding spot colliders in range
        Collider[] hidingSpots = Physics.OverlapSphere(transform.position, SightRange, WhereHidingSpots);
        if (hidingSpots == null || hidingSpots.Length == 0)
        {
            return;
        }

        // Evaluate each spot and choose the best reachable approach point using NavMesh sampling
        float bestPathLength = float.MaxValue;
        HidiingSpots bestSpotComp = null;
        Vector3 bestApproach = Vector3.zero;

        foreach (Collider spotCollider in hidingSpots)
        {
            var spotComp = spotCollider.GetComponent<HidiingSpots>() ?? spotCollider.GetComponentInParent<HidiingSpots>();
            if (spotComp == null) continue;

            Vector3 targetPos = spotComp.HidingSpot != null ? spotComp.HidingSpot.transform.position : spotComp.transform.position;

            // sample navmesh near the hiding spot to find a reachable approach point
            NavMeshHit sampleHit;
            if (!NavMesh.SamplePosition(targetPos, out sampleHit, hidingSampleRadius, NavMesh.AllAreas))
                continue; // no navigable point near this hiding spot

            // compute a path to the sampled point and measure its length
            NavMeshPath path = new NavMeshPath();
            if (!agent.CalculatePath(sampleHit.position, path) || path.status != NavMeshPathStatus.PathComplete)
            {
                continue; // not reachable
            }

            float pathLen = 0f;
            Vector3 prev = transform.position;
            foreach (var corner in path.corners)
            {
                pathLen += Vector3.Distance(prev, corner);
                prev = corner;
            }

            if (pathLen < bestPathLength)
            {
                bestPathLength = pathLen;
                bestSpotComp = spotComp;
                bestApproach = sampleHit.position;
            }
        }

        if (bestSpotComp != null)
        {
            // target the best hiding spot
            targetHidingSpotComp = bestSpotComp;
            targetApproachPoint = bestApproach;
            agent.SetDestination(targetApproachPoint);
        }
    }

    bool TryTargetHidingSpot(Transform spot)
    {

        // find nearest walkable point near the spot
        const float sampleRadius = 2f;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(spot.position, out hit, sampleRadius, NavMesh.AllAreas))
        {
          //  targetHidingSpot = spot;
            agent.SetDestination(hit.position); // go to nearest reachable nav point
            return true;
        }
        return false;

    }


}
