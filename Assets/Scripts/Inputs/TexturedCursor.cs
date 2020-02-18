using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TexturedCursor : MonoBehaviour
{
    private Vector2 position;
    public Texture2D texture;

    // Start is called before the first frame update
    void Start()
    {
        //Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnGUI()
    {
        Vector2 mousepos = new Vector2(Input.mousePosition.x, Screen.height -  Input.mousePosition.y);
        position = new Vector2(Screen.width / 2 - Input.mousePosition.x, Screen.height / 2 - Input.mousePosition.y);
        float angle = -(Mathf.Atan2(position.y, position.x) * Mathf.Rad2Deg) - 90;
        Globals.MouseToCenterAngle = angle;
        //transform.rotation = Quaternion.Euler(0, 0, -angle);

        Matrix4x4 back = GUI.matrix;
        GUIUtility.RotateAroundPivot(angle, mousepos );
        Rect rect = new Rect(mousepos.x, mousepos.y, texture.width, texture.height);
        GUI.DrawTexture(rect, texture);
        GUI.matrix = back;
    }
}
