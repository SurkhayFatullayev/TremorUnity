using UnityEngine;
using UnityEngine.AI;

public class EnemyStopDistance : MonoBehaviour
{
    public NavMeshAgent agent;       // Assign in Inspector
    public Transform player;         // Assign in Inspector
    public float desiredDistance = 5f; // Distance to keep from player

    private void Update()
    {
        if (player == null || agent == null)
            return;

        // Calculate how far the enemy is from the player
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // If enemy is farther than desiredDistance, move closer
        if (distanceToPlayer > desiredDistance)
        {
            agent.SetDestination(player.position);
        }
        else
        {
            // Otherwise, stop moving
            agent.SetDestination(transform.position);
        }
    }
}
