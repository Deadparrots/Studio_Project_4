using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyBindingsButton : MonoBehaviour
{
    public bool isToggle;
    public string actionSet;
    public bool SettingAction;
    public Text text;
    public Toggle toggle; // Using this to untoggle itself
    private string originaltext; // Might be temporary.
    // Start is called before the first frame update
    void Start()
    {
        SettingAction = false;
        originaltext = text.text;
        text.text = originaltext + PlayerPrefs.GetString(actionSet);
    }

    public void SetAction(bool _SettingAction)
    {
        SettingAction = _SettingAction;
    }

    // Update is called once per frame
    void Update()
    {
 
    }

    private void OnGUI()
    {
        if (SettingAction)
        {
            Event e = Event.current;

            if (e.keyCode == KeyCode.None)
                return;

            string input = e.keyCode.ToString();

            if (!string.IsNullOrEmpty(input))
            {
                if (input == "Escape")
                {
                    toggle.isOn = false;
                    return;
                }
                InputManager.inputManager.SetKeyCode(actionSet, input);
                text.text = originaltext + input;
                toggle.isOn = false;
            }
        }
    }
}
