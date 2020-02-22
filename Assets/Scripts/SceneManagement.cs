using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //die
    }
    public void ToMainmenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void ToSplashscreen()
    {
        SceneManager.LoadScene("SplashScreen");
    }
    public void ToConnectScreen()
    {
        SceneManager.LoadScene("ConnectScreen");
    }
    public void ToGame()
    {
        SceneManager.LoadScene("Gameplay");
    }
    public void ToEndScreen()
    {
        SceneManager.LoadScene("EndScreen");
    }
}
