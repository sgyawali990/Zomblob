using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string firstLevelName = "Level1";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void startGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(firstLevelName);
    }
    public void openOptions()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Options");
    }
    public void closeOptions()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
    public void quitGame()
    {
        Application.Quit();
    }
}
