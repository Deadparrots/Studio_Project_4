using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public int SizeX, SizeY; // Might change to a vector 2 for allowing weirder rooms.
    public Vector2Int position;
    public List<Vector2Int> Connectors;
    public List<bool> ConnectorsProcessed;
    public List<Rotation> ConnectorDirection;
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
                    if (SizeX == 1 && SizeY == 1)
                    {
                        ret.Add(Connectors[i] * -1);
                    }
                    else
                    {
                        int x = Connectors[i].x;
                        int y = Connectors[i].y;

                        Vector2Int middle = new Vector2Int(SizeX / 2, SizeY / 2); // also offset from position

                        x -= middle.x;
                        y -= middle.y;
                        ret.Add(new Vector2Int(x * -1, y * -1));


                        //if (y == )

                    }
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

    public Vector2Int GetRotatedVectorByIndex(int index) // -1 should be the lowest possible value returned
    {
        switch (rotation)
        {
            case Rotation.Up:
                return Connectors[index];
            case Rotation.Down:
                {
                    if (SizeX == 1 && SizeY == 1)
                    {
                        return Connectors[index] * -1;
                    }
                    else
                    {
                        int x = Connectors[index].x;
                        int y = Connectors[index].y;

                        Vector2Int middle = new Vector2Int(SizeX / 2, SizeY / 2); // also offset from position

                        x -= middle.x;
                        y -= middle.y;
                        return new Vector2Int(x * -1,y * -1);


                        //if (y == )

                    }
                }
            case Rotation.Left:
                {
                    if (SizeX == 1 && SizeY == 1)
                        return (new Vector2Int((Connectors[index].y > 0 ? -Connectors[index].y : Connectors[index].y), (Connectors[index].x > 0 ? Connectors[index].x : -Connectors[index].x)));
                    else
                    {
                        float x = Connectors[index].x;
                        float y = Connectors[index].y;

                        Vector2Int middle = new Vector2Int(SizeX / 2, SizeY / 2); // also offset from position

                        x -= middle.x;
                        y -= middle.y;

                        x = x * Mathf.Cos(-270) - y * Mathf.Sin(-270);
                        y = y * Mathf.Cos(-270) + x * Mathf.Sin(-270);

                        x += middle.x;
                        y += middle.y;

                        if (x > SizeX)
                            x = SizeX;
                        if (y > SizeY)
                            y = SizeY;

                        return new Vector2Int(Mathf.RoundToInt(x), Mathf.RoundToInt(y));

                    }
                }
            case Rotation.Right:
                {
                    if (SizeX == 1 && SizeY == 1)
                        return new Vector2Int((Connectors[index].y > 0 ? Connectors[index].y : -Connectors[index].y), (Connectors[index].x > 0 ? -Connectors[index].x : Connectors[index].x));
                    else
                    {
                        float x = Connectors[index].x;
                        float y = Connectors[index].y;

                        Vector2Int middle = new Vector2Int(SizeX / 2, SizeY / 2); // also offset from position

                        x -= middle.x;
                        y -= middle.y;

                        if (x == 0)
                            x = 1;
                        if (y == 0)
                            y = 1;

                        x = x * Mathf.Cos(-90) - y * Mathf.Sin(-90);
                        y = y * Mathf.Cos(-90) + x * Mathf.Sin(-90);

                        x += middle.x;
                        y += middle.y;

                        if (x > SizeX)
                            x = SizeX;
                        if (y > SizeY)
                            y = SizeY;

                        return new Vector2Int(Mathf.RoundToInt(x), Mathf.RoundToInt(y));

                    }
                }
        }
        return new Vector2Int(0,0);
    }

    private void Awake()
    {
        ConnectorsProcessed.Clear();
        ConnectorsProcessed.AddRange(System.Linq.Enumerable.Repeat(false, Connectors.Count));
    }

    public bool SameSize( Room other)
    {
        if (SizeX == other.SizeX && SizeY == other.SizeY)
            return true;
        else
            return false;
    }

    public static bool Facing(Room first, Room second, int indexfirst, int indexsecond)
    {
        switch ((Rotation)((int)first.ConnectorDirection[indexfirst] + (int)first.rotation % 4))
        {
            case Rotation.Up:
                if ((Rotation)((int)second.ConnectorDirection[indexsecond] + (int)second.rotation % 4) == Rotation.Down)
                    return true;
                break;
            case Rotation.Down:

                if ((Rotation)((int)second.ConnectorDirection[indexsecond] + (int)second.rotation % 4) == Rotation.Up)
                    return true;
                break;
            case Rotation.Left:
                if ((Rotation)((int)second.ConnectorDirection[indexsecond] + (int)second.rotation % 4) == Rotation.Right)
                    return true;
                break;
            case Rotation.Right:
                if ((Rotation)((int)second.ConnectorDirection[indexsecond] + (int)second.rotation % 4) == Rotation.Left)
                    return true;
                break;
        }
        return false;
    }
}