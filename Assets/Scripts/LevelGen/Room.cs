using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public int SizeX, SizeY; // Might change to a vector 2 for allowing weirder rooms.
    public Vector2Int position;
    public List<Vector2Int> Connectors;
    public List<bool> ConnectorsProcessed;

    public enum Rotation
    {
        Up = 0,
        Right,
        Down,
        Left
    }
    public Rotation rotation = Rotation.Up;

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

    public Vector2Int GetRotatedVectorByIndex(int index)
    {
        switch (rotation)
        {
            case Rotation.Up:
                return Connectors[index];
            case Rotation.Down:
                return Connectors[index] * -1;
            case Rotation.Left:
                return (new Vector2Int((Connectors[index].y > 0 ? -Connectors[index].y : Connectors[index].y), (Connectors[index].x > 0 ? Connectors[index].x : -Connectors[index].x)));
            case Rotation.Right:
                return new Vector2Int((Connectors[index].y > 0 ? Connectors[index].y : -Connectors[index].y), (Connectors[index].x > 0 ? -Connectors[index].x : Connectors[index].x));
        }
        return new Vector2Int(0,0);
    }

    private void Awake()
    {
        ConnectorsProcessed.Clear();
        ConnectorsProcessed.AddRange(System.Linq.Enumerable.Repeat(false, Connectors.Count));
    }
}