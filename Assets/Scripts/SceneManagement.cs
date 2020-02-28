using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour
{
    // Start is called before the first frame update
    private string currentScene = "";

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

    private void Unload(string sceneName)
    {
        if (SceneManager.GetSceneByName(sceneName).isLoaded)
            SceneManager.UnloadSceneAsync(sceneName);
    }

    private void ChangeScene(string sceneName)
    {
        if (currentScene != "")
            Unload(currentScene);

        if (Application.CanStreamedLevelBeLoaded(sceneName))
        {
            if(!SceneManager.GetSceneByName(sceneName).isLoaded)
            {
                SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
                currentScene = sceneName;
            }
        }
    }
}
    