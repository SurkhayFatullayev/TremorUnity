using UnityEngine;
using UnityEngine.SceneManagement;

public class KeyMenu : MonoBehaviour
{
    public string sceneName; // Assign your Main Menu scene name in the Inspector

    void Update()
    {
        // Check for "M" key press every frame
        if (Input.GetKeyDown(KeyCode.M))
        {
            ChangeScene(); // Call the same method used by the UI button
        }
    }

    // Method called by your UI button's OnClick event
    public void ChangeScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}
