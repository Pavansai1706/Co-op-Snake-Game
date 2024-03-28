using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManagerScript : MonoBehaviour
{
    private const string MAINMENU = "MainMenu";

    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void mainMenu()
    {
        SceneManager.LoadScene(MAINMENU);
    }
   public void playGame()
    {
        SceneManager.LoadSceneAsync(2);
    }
    public void loadGame()
    {
        SceneManager.LoadSceneAsync(1);
    }
}
