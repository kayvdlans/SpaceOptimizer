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

    public Vector2 Center
    {
        get
        {
            return new Vector2(Position.x + Size.x / 2, Position.y + Size.y / 2);
        }
    }

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

    public List<FieldTile> GetAdjacentTilesOfType(FieldTile[,] field, params FieldTile.eTileType[] types)
    {
        List<FieldTile> adjacent = new List<FieldTile>();

        for (int y = Bounds.Min.y; y <= Bounds.Max.y; y++)
        {
            for (int x = Bounds.Min.x; x <= Bounds.Max.x; x++)
            {
                adjacent.AddRange(field[x, y].GetAdjacentTilesOfType(field, types));
            }
        }

        return adjacent;
    }

    public List<FieldTile> GetSortedListByDistanceToModule(List<FieldTile> tiles, Module module)
    {
        tiles.Sort((x, y) => x.GetDistanceToCenterOfModule(module.Center).CompareTo(y.GetDistanceToCenterOfModule(module.Center)));
        return tiles;
    }

    public FieldTile GetClosestTileToModule(List<FieldTile> tiles)
    {
        float distance = float.MaxValue;
        FieldTile best = null;

        foreach (FieldTile tile in tiles)
        {
            if (tile.GetDistanceToCenterOfModule(Center) < distance)
            {
                distance = tile.GetDistanceToCenterOfModule(Center);
                best = tile;
            }
        }

        return best;
    }

    public bool HasConnection(List<ConnectionModule> connectionModules)
    {
        foreach (ConnectionModule cm in connectionModules)
            if (cm.IsConnected(Bounds))
                return true;

        return false;
    }

    public ConnectionModule GetBestConnection(Module module, List<ConnectionModule> connectionModules, FieldTile[,] field)
    {
        FieldTile tile = GetClosestTileToModule(GetAdjacentTilesOfType(field, FieldTile.eTileType.Connection));

        foreach (ConnectionModule cm in connectionModules)
            if (cm.Position.Equals(tile.Position))
                return cm;

        return null;
    }
}