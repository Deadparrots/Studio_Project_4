using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomRewrite : MonoBehaviour
{
    public int SizeX, SizeY; // Might change to a vector 2 for allowing weirder rooms.
    public enum Rotation
    {
        Up = 0,
        Right,
        Down,
        Left
    }
    [System.Serializable]
    public class Connector
    {
       public bool processed; // should always start as false
       public Vector2Int[] rotatedpositions; //Relative to Set Room, Added to deal with rooms with sizes that are not 1X1
       public Rotation[] Exit; // the way the connector exits the room
    }

    private static Vector2Int[] exits = { new Vector2Int(0, 1), new Vector2Int(1, 0), new Vector2Int(0, -1), new Vector2Int(-1, 0) };

    public Vector2Int position;
    public Rotation rotation = Rotation.Up;
    public List<Connector> Connectors;

    public Vector2Int GetRotatedVectorByIndex(int index)
    {
        return Connectors[index].rotatedpositions[(int)rotation];
    }

    public Vector2Int GetConnectorMapPosition(int index) // Returns 
    {
        return position + Connectors[index].rotatedpositions[(int)rotation];
    }

    public Vector2Int GetConnectorMapExitPosition(int index)
    {
        return position + Connectors[index].rotatedpositions[(int)rotation] + exits[(int)Connectors[index].Exit[(int)rotation]];
    }

    public Vector2Int GetConnectorExitPosition(int index)
    {
        return Connectors[index].rotatedpositions[(int)rotation] + exits[(int)Connectors[index].Exit[(int)rotation]];
    }
    
    public bool SameSize(RoomRewrite other)
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
