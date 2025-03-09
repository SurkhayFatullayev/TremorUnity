using UnityEngine;

public class bulletTrigger : MonoBehaviour
{
//--THIS SCRIPT FUNCTION IS GOING TO SEND THE TRIGGER EVENTS
    void OnTriggerEnter(Collider col)
    {
        //here we are adding points
        switch (col.tag)
        {

        }
    }

//--THIS SCRIPT FUNCTION IS GOING TO SEND THE COLLISION EVENTS
    void onCollisionEnter(Collision col)
    {
        switch (col.gameObject.tag)
        {

        }
        //here we are notifying that the player didnt receive the points
    }
}
