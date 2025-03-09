using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerCores : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerInventory playerInventory = other.GetComponent<PlayerInventory>();
        if (playerInventory != null)
        {
            playerInventory.CoreCollected();
            gameObject.SetActive(false);
        }
    }
}

