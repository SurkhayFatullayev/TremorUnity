using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    private TextMeshProUGUI powercoreText;

    void Start()
    {
        powercoreText = GetComponent<TextMeshProUGUI>();
    }

    public void UpdatePowerCoreText(PlayerInventory playerInventory)
    {
        powercoreText.text = playerInventory.NumberOfCores.ToString();
    }
}
