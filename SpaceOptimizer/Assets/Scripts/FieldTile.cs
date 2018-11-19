using UnityEngine;
using UnityEditor;

public class FieldTile
{
    public Vector2Int Position { get; private set; }
    public bool Free { get; set; }
    public float DistanceToMainModule { get; private set; }

    public FieldTile(Vector2Int position)
    {
        Position = position;
        Free = true;
        DistanceToMainModule = Vector2Int.Distance(Vector2Int.zero, position);
    }

    public void Draw()
    {
        Handles.DrawSolidRectangleWithOutline(
            new Rect(Position.x, Position.y, 1, 1),
            Color.clear,
            Color.white);
    }
}