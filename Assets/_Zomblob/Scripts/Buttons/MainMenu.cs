using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string firstLevelName = "Level1";
    public string OptionsMenu = "options here";
    public string customizeMenu = "customize here";

    public GameObject Titlepage;
    public GameObject customizePage;
    public GameObject OptionsPage;
    public GameObject[] characters;

    void Start()
    {
        Titlepage.SetActive(true);
        customizePage.SetActive(false);
        OptionsPage.SetActive(false);
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
        FindObjectOfType<CameraSweep>().SweepToC();
        Titlepage.SetActive(false);
        OptionsPage.SetActive(true);
    }
    public void closeOptions()
    {
        FindObjectOfType<CameraSweep>().SweepToA();
        OptionsPage.SetActive(false);
        Titlepage.SetActive(true);
    }
    public void Customize()
    {
        FindObjectOfType<CameraSweep>().SweepToB();
        OptionsPage.SetActive(false);
        Titlepage.SetActive(false);
        customizePage.SetActive(true);

    }
    public void backToMenu()
    {
        FindObjectOfType<CameraSweep>().SweepToA();
        OptionsPage.SetActive(false);
        customizePage.SetActive(false);
        Titlepage.SetActive(true);
    }
    public void quitGame()
    {
        Application.Quit();
    }
    public void characterSelectLeft(){
        for (int i=0; i<6; i++){
            if (characters[i].activeInHierarchy){
                characters[i].SetActive(false);
                if (i==0){
                    characters[5].SetActive(true);
                }
                else{
                    characters[i-1].SetActive(true);
                }
                break;
            }
        }
    }
    public void characterSelectRight(){
        for (int i=0; i<6; i++){
            if (characters[i].activeInHierarchy){
                characters[i].SetActive(false);
                if (i==5){
                    characters[0].SetActive(true);
                }
                else{
                    characters[i+1].SetActive(true);
                }
                break;
            }
        }
    }
}
