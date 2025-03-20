using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    public GameObject weapon1; // Assign first weapon in Inspector
    public GameObject weapon2; // Assign second weapon in Inspector

    private int currentWeapon = 1; // Start with Weapon 1

    void Start()
    {
        // Ensure only the first weapon is enabled at start
        EquipWeapon(1);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            EquipWeapon(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            EquipWeapon(2);
        }
    }

    void EquipWeapon(int weaponNumber)
    {
        currentWeapon = weaponNumber;

        // Enable/Disable weapons based on selection
        if (weapon1 != null) weapon1.SetActive(weaponNumber == 1);
        if (weapon2 != null) weapon2.SetActive(weaponNumber == 2);
    }
}
