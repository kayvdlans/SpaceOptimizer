using System.Collections.Generic;
using UnityEngine;

public class ConnectionModule : Module
{
    public List<ConnectionModule> Neighbours { get; private set; }
    public bool Connected { get; private set; }

    public ConnectionModule(Vector2Int position, List<ConnectionModule> connectionModules) : base("Connection", Vector2Int.one, position)
    {
        Connected = false;
        FindNeighbours(connectionModules);
    }
    
    private void FindNeighbours(List<ConnectionModule> connectionModules)
    {
        Neighbours = new List<ConnectionModule>();

        foreach (ConnectionModule cm in connectionModules)
        {
            if (cm != this)
            {
                if (Vector2Int.Distance(Position, cm.Position) == 1)
                {
                    Neighbours.Add(cm);
                    cm.Neighbours.Add(this);
                }
            }
        }
    }

    public bool IsConnectedToMainModule(Bounds b, List<ConnectionModule> connectionsUsed)
    {
        if (!IsConnected(b) && !connectionsUsed.Contains(this))
        {
            connectionsUsed.Add(this);

            foreach (ConnectionModule connection in Neighbours)
            {
                if (IsConnectedToMainModule(b, connectionsUsed))
                {
                    Connected = true;
                    return true;
                }
            }

            return false;
        }
        
        return true;
    }

    public bool IsConnected (Bounds b)
    {
        return Position.x < b.Min.x ? (b.Min.x - Position.x == 1 && Position.y >= b.Min.y && Bounds.Max.y <= b.Max.y ? true : false) 
            : Position.x > b.Max.x ? (Position.x - b.Max.x == 1 && Position.y >= b.Min.y && Bounds.Max.y <= b.Max.y ? true : false) 
            : Position.y < b.Min.y ? (b.Min.y - Position.y == 1 && Position.x >= b.Min.x && Bounds.Max.x <= b.Max.x ? true : false)
            : Position.y - b.Max.y == 1 && Position.x >= b.Min.x && Bounds.Max.x <= b.Max.x ? true : false;
    }
}
