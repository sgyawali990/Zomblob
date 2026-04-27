using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string firstLevelName = "Level1";
    public string OptionsMenu = "options here";
    public string customizeMenu = "customize here";

    public GameObject Titlepage;
    public GameObject customizePage;

    void Start()
    {
        Titlepage.SetActive(true);
        customizePage.SetActive(false);
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
        Titlepage.SetActive(false);
        customizePage.SetActive(true);

    }
    public void backToMenu()
    {
        FindObjectOfType<CameraSweep>().SweepToA();
        Titlepage.SetActive(true);
        customizePage.SetActive(false);
    }
    public void quitGame()
    {
        Application.Quit();
    }
}
