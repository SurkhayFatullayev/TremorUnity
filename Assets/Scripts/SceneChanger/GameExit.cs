using UnityEngine;
using UnityEngine.SceneManagement;

public class GameExit : MonoBehaviour
{
    public string sceneName;  // Optional: for changing scenes if needed

    // Method to exit the game
    public void ExitGame()
    {
        Debug.Log("Exiting Game...");

        // If the game is running in the editor, stop play mode
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // If the game is built and running, quit the application
        Application.Quit();
#endif
    }

//     // Optional: Method to change scenes if needed
//     public void ChangeScene()
//     {
//         SceneManager.LoadScene(sceneName);
//     }
 }
