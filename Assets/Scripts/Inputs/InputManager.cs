using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

// This is supposed to be something to allow changing controls at runtime


public class InputManager : MonoBehaviour
{
    public static InputManager inputManager;

    public static KeyCode MoveForward    {get; set;} //Supposed to move according to cursor position
    public static KeyCode MoveUp    {get; set;}
    public static KeyCode MoveDown  {get; set;}
    public static KeyCode MoveLeft  {get; set;}
    public static KeyCode MoveRight {get; set;}

    void Awake()
    {
        //Singleton pattern
        if (inputManager == null)
        {
            DontDestroyOnLoad(gameObject);
            inputManager = this;
        }
        else if (inputManager != this)
        {
            Destroy(gameObject);
        }

        MoveForward = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("MoveForward", "Space")); // Will Be Changed to something else later on
        MoveUp = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("MoveUp", "W"));
        MoveDown  = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("MoveDown", "S"));
        MoveLeft  = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("MoveLeft", "A"));
        MoveRight = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("MoveRight", "D"));

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
