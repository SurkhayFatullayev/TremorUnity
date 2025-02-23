using UnityEngine;
// IMPORTANT: Add this for NavMeshAgent
using UnityEngine.AI;

public class EnemyFollow : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;
    public float health;

    // Patroling
    public Vector3 walkPoint; // Missing space fixed: "Vector3walkPoint" → "Vector3 walkPoint"
    bool walkPointSet;
    public float walkPointRange;
    public float desiredShootingDistance = 5f; // The distance you want the enemy to maintain

    // Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    public GameObject projectile;

    // States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    private void Awake()
    {
        // Find player by name or drag into Inspector
        player = GameObject.Find("Player").transform;

        // Get reference to the NavMeshAgent on this same GameObject
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        // Check if the player is within sight or attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange) Patroling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInSightRange && playerInAttackRange) AttackPlayer();
    }

    private void Patroling()
    {
        // If we don’t have a valid walk point, find one
        if (!walkPointSet) SearchWalkPoint();

        // If we have a walk point, move there
        if (walkPointSet)
            agent.SetDestination(walkPoint);

        // Check if the enemy reached the walk point
        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        // Generate a random point within the walkPointRange
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(
            transform.position.x + randomX,
            transform.position.y,
            transform.position.z + randomZ
        );

        // Check if this random point is on the ground
        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

private void ChasePlayer()
{
    float distanceToPlayer = Vector3.Distance(transform.position, player.position);

    // If we are farther than our desired shooting distance, move closer
    if (distanceToPlayer > desiredShootingDistance)
    {
        agent.SetDestination(player.position);
    }
    else
    {
        // Stop if within shooting distance
        agent.SetDestination(transform.position);
    }
}

    private void AttackPlayer()
    {
        // Stop moving while attacking
        agent.SetDestination(transform.position);

        // Face the player
        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            // Attack code here
            // Corrected “Rigitbody” to “Rigidbody”
            Rigidbody rb = Instantiate(projectile, transform.position, Quaternion.identity)
                           .GetComponent<Rigidbody>();

            // Add forward and upward force
            rb.AddForce(transform.forward * 15f, ForceMode.Impulse);
            rb.AddForce(transform.up * 8f, ForceMode.Impulse);
            // Destroy the projectile after 1 seconds 
            Destroy(rb.gameObject, 1f);
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);



        }
    }





    private void ResetAttack()
    {
        // Allows another attack
        alreadyAttacked = false;
    }

    // public void TakeDamage(int damage)
    // {
    //     health -= damage;
    //     if (health <= 0)
    //         // Missing semicolon fixed
    //         Invoke(nameof(DestroyEnemy), 0.5f);
    // }

    // private void DestroyEnemy()
    // {
    //     // Destroy THIS enemy
    //     // “Destroy(GameObject)” changed to “Destroy(gameObject)”
    //     Destroy(gameObject);
    // }

    private void OnDrawGizmosSelected()
    {
        // Draw red sphere for attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Draw yellow sphere for sight range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
