using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour //m'he mort fent aquest script :), sorry carlos pero estic molt cansat
{
    public void StartGame()
   {
        UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
        GameManager.Instance.StartNewGame();
   }
    public void QuitGame()
    {
         Application.Quit();
    }
}
