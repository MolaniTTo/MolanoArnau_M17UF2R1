using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour //m'he mort fent aquest script :), sorry carlos pero estic molt cansat
{
    public Animator animator;

   public void StartGame()
   {
        animator = GetComponentInParent<Animator>();
        animator.SetTrigger("GoUp");
        
   }
    public void QuitGame()
    {
         Application.Quit();
    }

    public void LoadScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
        GameManager.Instance.StartNewGame();
    }

    public void GoBackToMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    public void GoToSettings()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Controles");
    }
}
