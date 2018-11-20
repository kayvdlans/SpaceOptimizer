using System;
using UnityEngine;

namespace SpaceOptimizer.Modules
{
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

        /// <summary>
        /// Check if the module would be inside the field's bounds if it would be 
        /// placed at the given position.
        /// </summary>
        /// <param name="position">Position of the tile to check from</param>
        /// <param name="fieldMax">Max bounds of the field</param>
        public bool IsInsideFieldBounds(Vector2Int position, Vector2Int fieldMax)
        {
            return position.x >= 0
                && position.y >= 0
                && position.x + Size.x <= fieldMax.x
                && position.y + Size.y <= fieldMax.y;
        }

        /// <summary>
        /// Checks whether the tiles, which the module would occupy after being 
        /// placed, are empty.
        /// </summary>
        /// <param name="position">Position of the tile to check from</param>
        /// <param name="fieldTiles">2D array representation of the field</param>
        public bool HasOverlap(Vector2Int position, FieldTile[,] fieldTiles)
        {
            for (int x = position.x; x < position.x + Size.x; x++)
                for (int y = position.y; y < position.y + Size.y; y++)
                    if (fieldTiles[x, y].TileType != FieldTile.eTileType.Empty)
                        return true;

            return false;
        }
    }
}