using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour
{
    // Start is called before the first frame update
    string currentScene;
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
        ChangeScene("MainMenu");
    }
    public void ToSplashscreen()
    {
        ChangeScene("SplashScreen");
    }
    public void ToConnectScreen()
    {
        ChangeScene("ConnectScreen");
    }
    public void ToGame()
    {
        ChangeScene("Gameplay");
    }
    public void ToEndScreen()
    {
        ChangeScene("EndScreen");
    }

    public void Unload(string sceneName)
    {
        if (SceneManager.GetSceneByName(sceneName).isLoaded)
            SceneManager.UnloadSceneAsync(sceneName);
    }

    public void ChangeScene(string sceneName)
    {
        if (currentScene != "")
            Unload(currentScene);

        if (Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Scene nextScene = SceneManager.GetSceneByName(sceneName);
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
            currentScene = sceneName;
        }
    }
}
    