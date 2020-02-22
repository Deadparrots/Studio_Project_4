using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public int SizeX, SizeY;
    public List<Vector2Int> Connectors;

    public enum Rotation
    {
        Up,
        Down,
        Left,
        Right
    }
    public Rotation rotation;

    public List<Vector2Int> GetRotatedVectors()
    {
        List<Vector2Int> ret = new List<Vector2Int>();
        for (int i = 0; Connectors.Count > i;i++)
        {
            switch (rotation)
            {
                case Rotation.Up:
                    ret.Add(Connectors[i]);
                    break;
                case Rotation.Down:
                    ret.Add(Connectors[i] * -1);
                    break;
                case Rotation.Left:
                    ret.Add(new Vector2Int((Connectors[i].y > 0 ? -Connectors[i].y : Connectors[i].y), (Connectors[i].x > 0 ? Connectors[i].x : -Connectors[i].x)));
                    break;
                case Rotation.Right:
                    ret.Add(new Vector2Int((Connectors[i].y > 0 ? Connectors[i].y : -Connectors[i].y), (Connectors[i].x > 0 ? -Connectors[i].x : Connectors[i].x)));
                    // -1.x -> +1.y
                    // -1.y -> -1.x
                    // +1.x -> -1.y
                    // 1.y -> 1.x

                    break;
            }
        }
        return ret;
    }
}