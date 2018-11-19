using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public struct Bounds
{
    public Vector2Int Min;
    public Vector2Int Max;
}

public class Module
{
    public Module(string name, Vector2Int size, Vector2Int position)
    {
        Name = name;
        Size = size;
        Position = position;
    }

    public string Name { get; private set; }
    public Vector2Int Size { get; private set; }
    public Vector2Int Position { get; set; }

    public Bounds Bounds
    {
        get
        {
            return new Bounds { Min = Position, Max = Corners[2] };
        }
    }

    public Vector2Int[] Corners
    {
        get
        {
            Vector2Int[] corners = new Vector2Int[]
            {
                    Position,
                    new Vector2Int(Position.x, Position.y + Size.y - 1),
                    new Vector2Int(Position.x + Size.x - 1, Position.y + Size.y - 1),
                    new Vector2Int(Position.x + Size.x - 1, Position.y)
            };
            return corners;
        }
    }

    public void Draw(Color color, Color outline)
    {
        Handles.DrawSolidRectangleWithOutline(
                new Rect(Position.x, Position.y, Size.x, Size.y),
                color, outline);
    }

    public FieldTile GetClosestFreeTile(FieldTile[,] fieldTiles, List<ConnectionModule> connectionModules)
    {
        float distance = float.MaxValue;
        FieldTile closestTile = null;

        for (int y = Bounds.Min.y - 1; y <= Bounds.Max.y + 1; y++)
        {
            if (y < 0 || y >= fieldTiles.GetLength(1))
                continue;

            for (int x = Bounds.Min.x - 1; x <= Bounds.Max.x + 1; x++)
            {
                if (x < 0 || x >= fieldTiles.GetLength(0))
                    continue;

                if (!fieldTiles[x, y].Free)
                    continue;

                if (x == Bounds.Min.x - 1 && y == Bounds.Min.y - 1 || 
                    x == Bounds.Max.x + 1 && y == Bounds.Max.y + 1 ||
                    x == Bounds.Min.x - 1 && y == Bounds.Max.y + 1 ||
                    x == Bounds.Max.x + 1 && y == Bounds.Min.y - 1)
                    continue;


                float distanceToClosestConnection = float.MaxValue;
                foreach (ConnectionModule connection in connectionModules)
                {
                    float distanceToConnection = Vector2Int.Distance(connection.Position, fieldTiles[x, y].Position);
                    if (distanceToConnection < distanceToClosestConnection)
                        distanceToClosestConnection = distanceToConnection;
                }

                if (connectionModules.Count != 0 ? distanceToClosestConnection < distance : fieldTiles[x, y].DistanceToMainModule < distance)
                {
                    distance = connectionModules.Count != 0 ? distanceToClosestConnection : fieldTiles[x, y].DistanceToMainModule;
                    closestTile = fieldTiles[x, y];
                }
            }
        }

        return closestTile;
    }

    public bool HasConnection(List<ConnectionModule> connectionModules)
    {
        foreach (ConnectionModule cm in connectionModules)
            if (cm.IsConnected(Bounds))
                return true;

        return false;
    }

    public ConnectionModule GetConnection(List<ConnectionModule> connectionModules)
    {
        foreach (ConnectionModule cm in connectionModules)
            if (cm.IsConnected(Bounds))
                return cm;

        return null;
    }
}