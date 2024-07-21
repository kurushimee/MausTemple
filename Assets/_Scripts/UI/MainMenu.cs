using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles UI elements of the main menu.
/// </summary>
public class MainMenu : MonoBehaviour {
    /// <summary>
    /// Loads the "Main" scene on pressing the play button.
    /// </summary>
    public void OnPlay() {
        SceneManager.LoadScene("Main");
    }

    /// <summary>
    /// Shuts down the game on pressing the exit button.
    /// </summary>
    public void OnLeave() {
        Application.Quit();
    }
}
