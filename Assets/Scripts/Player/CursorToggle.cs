using UnityEngine;

/// <summary>
/// Toggles the mouse cursor lock state when pressing ESC.
/// Useful for first-person games where you want the cursor hidden during play,
/// but visible when accessing UI or menus.
/// </summary>
public class CursorToggle : MonoBehaviour
{
    private bool isCursorLocked = true;

    void Start()
    {
        LockCursor(); // By default, lock the cursor at game start
    }

    void Update()
    {
        // Press ESC to toggle cursor lock/unlock
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isCursorLocked)
            {
                UnlockCursor();
            }
            else
            {
                LockCursor();
            }
        }
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;  // Locks cursor to screen center
        Cursor.visible = false;                    // Hides the cursor
        isCursorLocked = true;
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;    // Freely move cursor in the game window
        Cursor.visible = true;                     // Show the cursor
        isCursorLocked = false;
    }
}
