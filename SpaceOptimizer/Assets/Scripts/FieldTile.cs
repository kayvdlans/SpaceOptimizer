using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class FieldTile
{
    public enum eTileType
    {
        Empty,
        Module,
        Connection
    }

    public Vector2Int Position { get; private set; }
    public eTileType TileType { get; set; }
    public float DistanceToMainModule { get; private set; }

    public FieldTile(Vector2Int position)
    {
        Position = position;
        TileType = eTileType.Empty;
        DistanceToMainModule = Vector2Int.Distance(Vector2Int.zero, position);
    }

    public List<FieldTile> GetAdjacentTilesOfType(FieldTile[,] field, params eTileType[] types)
    {
        List<FieldTile> adjacent = new List<FieldTile>();

        foreach (eTileType t in types)
        {
            if (Position.x - 1 >= 0 && field[Position.x - 1, Position.y].TileType == t)
                adjacent.Add(field[Position.x - 1, Position.y]);

            if (Position.x + 1 < field.GetLength(0) &&  field[Position.x + 1, Position.y].TileType == t)
                adjacent.Add(field[Position.x + 1, Position.y]);

            if (Position.y - 1 >= 0 && field[Position.x, Position.y - 1].TileType == t)
                adjacent.Add(field[Position.x, Position.y - 1]);

            if (Position.y < field.GetLength(1) && field[Position.x, Position.y + 1].TileType == t)
                adjacent.Add(field[Position.x, Position.y + 1]);
        }

        return adjacent;
    }
    
    public float GetDistanceToCenterOfModule(Vector2 center)
    {
        return Vector2.Distance(Position, center);
    }

    public void Draw()
    {
        Handles.DrawSolidRectangleWithOutline(
            new Rect(Position.x, Position.y, 1, 1),
            Color.clear,
            Color.white);
    }
}