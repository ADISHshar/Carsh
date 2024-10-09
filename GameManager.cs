using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Button playAgainButton;
    public Button backButton;
    public Button quitButton;
    public GameObject OptionPanel;

    void Start()
    {
        OptionPanel.SetActive(false);
        // Assign the button listeners
        playAgainButton.onClick.AddListener(PlayAgain);
        backButton.onClick.AddListener(Back);
        quitButton.onClick.AddListener(Quit);
    }

    public void OpenOptions()
    {
        OptionPanel.SetActive(true);
    }
    public void CloseOptions()
    {
        OptionPanel.SetActive(false);
    }
    void PlayAgain()
    {
        // Reload the current scene
        SceneManager.LoadScene("Offroad");
    }

    void Back()
    {
        // Load the previous scene or main menu
        // Assuming "MainMenu" is the name of your main menu scene
        SceneManager.LoadScene("menu");
    }

    void Quit()
    {
        // Quit the application
        Application.Quit();
        #if UNITY_EDITOR
        // If in the Unity editor, stop playing the scene
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
