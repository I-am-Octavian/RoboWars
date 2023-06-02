using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MAINMENU : MonoBehaviour
{
    // Start is called before the first frame update
    
    public void Play() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void quitGame()
    {
        Debug.Log("QUIT!");
        Application.Quit();
    }
    public void LoadMenu()

    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MENU");
    }
}