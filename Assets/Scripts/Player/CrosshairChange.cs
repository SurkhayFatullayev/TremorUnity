using UnityEngine;
using UnityEngine.UI;

public class CrosshairRecoil : MonoBehaviour
{
    public RectTransform crosshair; // Assign UI crosshair (Image or Panel)
    public float minCrosshairSize = 50f;
    public float maxCrosshairSize = 150f;
    public float recoilSpeed = 5f; // Speed at which recoil expands/contracts

    [Header("Player Movement Reference")]
    public FirstPersonController playerMovement; // Replace with your movement script

    private Vector3 lastPosition;
    private float playerSpeed;
    private float targetCrosshairSize;

    void Update()
    {
        UpdatePlayerSpeed();
        UpdateCrosshairSize();
    }

    void UpdatePlayerSpeed()
    {
        // If playerMovement has a velocity variable, use it
        if (playerMovement != null && playerMovement.velocity != null)
        {
            playerSpeed = playerMovement.velocity.magnitude;
        }
        else
        {
            // If no velocity is available, manually calculate speed
            playerSpeed = (transform.position - lastPosition).magnitude / Time.deltaTime;
            lastPosition = transform.position;
        }
    }

    void UpdateCrosshairSize()
    {
        targetCrosshairSize = Mathf.Lerp(minCrosshairSize, maxCrosshairSize, playerSpeed / 10f);

        float newSize = Mathf.Lerp(crosshair.sizeDelta.x, targetCrosshairSize, Time.deltaTime * recoilSpeed);
        crosshair.sizeDelta = new Vector2(newSize, newSize);
    }

    // Call this when shooting to simulate extra recoil
    public void AddRecoil()
    {
        targetCrosshairSize += 10f; // Small bump when shooting
    }
}
