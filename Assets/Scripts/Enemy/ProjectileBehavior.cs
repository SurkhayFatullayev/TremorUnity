using UnityEngine;

public class ProjectileBehavior : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        // Optionally deal damage to collision.gameObject here
        //Destroy(gameObject); // Destroys the projectile
    }
}
