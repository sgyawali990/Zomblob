using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string firstLevelName = "Level1";
    public string OptionsMenu = "options here";
    public string customizeMenu = "customize here";

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
        UnityEngine.SceneManagement.SceneManager.LoadScene(OptionsMenu);
    }
    public void closeOptions()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
    public void Customize()
    {
        FindObjectOfType<CameraSweep>().SweepToB();
    }
    public void quitGame()
    {
        Application.Quit();
    }
}
