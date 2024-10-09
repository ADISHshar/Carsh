using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public Button Play;
    public GameObject PanelMenu;

    private void Start()
    {
        PanelMenu.SetActive(false);
        Play.onClick.AddListener(LoadOffRoadScene);
    }
    public void OpenMenu()
    {
        PanelMenu.SetActive(true);
    }
    public void CloseMenu()
    {
        PanelMenu.SetActive(false);
    }

    private void LoadOffRoadScene()
    {
        SceneManager.LoadScene("OffRoad");
    }
}
