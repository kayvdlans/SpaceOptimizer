using System;
using UnityEngine;

[Serializable]
public class BuildModule
{
    [SerializeField] private string _Name;
    [SerializeField] private int _Amount;
    [SerializeField] private bool _ConnectionNeeded;
    [SerializeField] private Vector2Int _Size;

    public string Name { get { return _Name; } }
    public int Amount { get { return _Amount; } }
    public bool ConnectionNeeded { get { return _ConnectionNeeded; } }
    public Vector2Int Size { get { return _Size; } }
  
    public int AmountPlaced { get; set; }

    public bool IsInsideFieldBounds(Vector2Int position, Vector2Int fieldMax)
    {
        return position.x + Size.x <= fieldMax.x && position.y + Size.y <= fieldMax.y;
    }

    public bool HasOverlap(Vector2Int position, FieldTile[,] fieldTiles)
    {
        for (int x = position.x; x < position.x + Size.x; x++)
        {
            for (int y = position.y; y < position.y + Size.y; y++)
            {
                if (!fieldTiles[x, y].Free)
                {
                    return true;
                }
            }
        }

        return false;
    }
}
