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
    public float attackStopDistance = 7f; // Default value, adjust in Inspector
    public float patrolSpeed = 3f;

    // States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    // Sound
    public AudioSource audioSource;  // Audio component
    public AudioClip shootingSound;  // Sound file to play

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
    // Calculate distance to player using a Raycast
    float distanceToPlayer = GetDistanceToPlayer();

    // Set stopping distance in NavMeshAgent (prevents constant recalculations)
    agent.stoppingDistance = attackStopDistance;

    // If farther than stopping distance, move toward the player
    if (distanceToPlayer > attackStopDistance)
    {
        agent.isStopped = false; // Ensure agent moves
        agent.SetDestination(player.position);
    }
    else
    {
        // Stop movement once within desired range
        agent.isStopped = true;
    }
}

private void AttackPlayer()
{
    // Stop moving while attacking
    agent.isStopped = true;

    // Ensure the player reference exists
    if (player == null)
    {
        Debug.LogError("Player is missing!");
        return;
    }

    // 🔥 **Check if there is an obstacle blocking the shot**
    if (IsObstacleBlockingShot())
    {
        Debug.Log("Enemy can't shoot! Moving to find a better angle...");
        MoveToBetterPosition();
        return; // Wait until position is adjusted
    }

    // 🔥 Fix: Face the player completely (including up/down angles)
    Vector3 direction = (player.position - transform.position).normalized;
    Quaternion lookRotation = Quaternion.LookRotation(direction);
    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);

    if (!alreadyAttacked)
    {
        // 🔥 Fix: Fire projectile at exact player position
        Vector3 shootTarget = player.position + Vector3.up * 1.2f; // Adjust height slightly for better targeting
        GameObject projectileInstance = Instantiate(projectile, transform.position + Vector3.up * 1.5f, Quaternion.identity);
        Rigidbody rb = projectileInstance.GetComponent<Rigidbody>();

        // 🔥 Fix: Calculate direction precisely towards the player
        Vector3 shootDirection = (shootTarget - projectileInstance.transform.position).normalized;

        // Apply force directly towards the player (no unnecessary upward force)
        rb.AddForce(shootDirection * 25f, ForceMode.Impulse);

        // Destroy projectile after 2 seconds
        Destroy(rb.gameObject, 2f);

            // 🔊 Play shooting sound when projectile is fired
            if (audioSource != null && shootingSound != null)
            {
                audioSource.PlayOneShot(shootingSound);
            }

        alreadyAttacked = true;
        Invoke(nameof(ResetAttack), timeBetweenAttacks);
    }
}


private bool IsObstacleBlockingShot()
{
    RaycastHit hit;
    Vector3 origin = transform.position + Vector3.up * 1.5f; // Adjusted shooting position
    Vector3 direction = (player.position - origin).normalized;

    if (Physics.Raycast(origin, direction, out hit, attackRange))
    {
        if (hit.collider.gameObject.CompareTag("Obstacle")) // Ensure obstacles have the "Obstacle" tag
        {
            return true; // Something is blocking the shot
        }
    }
    return false; // No obstacle, clear line of sight
}

private void MoveToBetterPosition()
{
    // Pick a random direction (left or right) to move sideways
    float strafeDirection = Random.Range(0, 2) == 0 ? -1 : 1; // -1 for left, 1 for right

    // Calculate new position
    Vector3 moveDirection = transform.right * strafeDirection * 3f; // Move 3 units sideways
    Vector3 newPosition = transform.position + moveDirection;

    // Ensure the new position is valid on the NavMesh
    NavMeshHit hit;
    if (NavMesh.SamplePosition(newPosition, out hit, 2f, NavMesh.AllAreas))
    {
        agent.isStopped = false; // Allow movement
        agent.SetDestination(hit.position);
    }
}



private float GetDistanceToPlayer()
{
    // Use Raycast for more accurate distance measurement
    RaycastHit hit;
    if (Physics.Raycast(transform.position, (player.position - transform.position).normalized, out hit))
    {
        return hit.distance;
    }
    return Vector3.Distance(transform.position, player.position);
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
