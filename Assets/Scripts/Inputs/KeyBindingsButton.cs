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
    private Event currEvent = new Event();
    private KeyCode[] mouseKeys;
    // Start is called before the first frame update
    void Start()
    {
        SettingAction = false;
        text.text = PlayerPrefs.GetString(actionSet);
        mouseKeys = new KeyCode[] { KeyCode.Mouse0, KeyCode.Mouse1, KeyCode.Mouse2, KeyCode.Mouse3, KeyCode.Mouse4, KeyCode.Mouse5, KeyCode.Mouse6 };
    }

    public void SetAction(bool _SettingAction)
    {
        SettingAction = _SettingAction;
        InputManager.inputManager.OnLastMenuLayer = !SettingAction; // temporary
    }

    // Update is called once per frame
    void Update() //TODO: Figure out how to do scroll wheel
    {
        string input = null;
        if (SettingAction)
        {
            Event.PopEvent(currEvent);
            input = null;

            if (currEvent.type == EventType.KeyDown)
            {
                if (currEvent.keyCode == KeyCode.None)
                    return;
                input = currEvent.keyCode.ToString();
            }
            else
            {
                for (int i = 0; mouseKeys.Length > i;i++) // For the 7 or so mousekeys since Events doesnt detect from mouse3 onwards
                {
                    if (Input.GetKeyDown(mouseKeys[i]))
                    {
                        input = mouseKeys[i].ToString();
                        break;
                    }
                }
            }

            if (!string.IsNullOrEmpty(input))
            {
                if (input == "Escape")
                {
                    toggle.isOn = false;
                    return;
                }
                InputManager.inputManager.SetKeyCode(actionSet, input);
                text.text = input;
                toggle.isOn = false;
            }
        }
    }
}
