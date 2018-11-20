using SpaceOptimizer.Modules;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SpaceOptimizer
{
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

        public FieldTile(Vector2Int position)
        {
            Position = position;
            TileType = eTileType.Empty;
        }

        /// <summary>
        /// Get a list of tiles based on the given <see cref="eTileType"/>s by 4-Way
        /// checking the tiles surrounding this one.
        /// </summary>
        /// <param name="field">2D array representation of the field</param>
        /// <param name="types"><see cref="eTileType"/>s to check</param>
        public List<FieldTile> GetAdjacentTilesOfType(FieldTile[,] field,
            params eTileType[] types)
        {
            List<FieldTile> adjacent = new List<FieldTile>();

            //Ugly as heck, but it works :).
            foreach (eTileType t in types)
            {
                if (Position.x - 1 >= 0
                    && field[Position.x - 1, Position.y].TileType == t)
                    adjacent.Add(field[Position.x - 1, Position.y]);

                if (Position.x + 1 < field.GetLength(0)
                    && field[Position.x + 1, Position.y].TileType == t)
                    adjacent.Add(field[Position.x + 1, Position.y]);

                if (Position.y - 1 >= 0
                    && field[Position.x, Position.y - 1].TileType == t)
                    adjacent.Add(field[Position.x, Position.y - 1]);

                if (Position.y + 1 < field.GetLength(1)
                    && field[Position.x, Position.y + 1].TileType == t)
                    adjacent.Add(field[Position.x, Position.y + 1]);
            }

            return adjacent;
        }

        /// <summary>
        /// Get the distance from the center of the given <paramref name="module"/>.
        /// </summary>
        /// <param name="module">Module to find the position of</param>
        public float GetDistanceToCenterOfModule(Module module)
        {
            return Vector2.Distance(Position, module.Center);
        }

        /// <summary>
        /// Check for each connection if the position of the connection is the 
        /// same as the position of the tile. If possible, I'll want to recreate
        /// this method to not loop through every connection. Might store a 
        /// reference to the connection in the tile itself.
        /// </summary>
        /// <param name="connections">List of connections to check from</param>
        public ConnectionModule GetConnectionModule(List<ConnectionModule> connections)
        {
            if (TileType != eTileType.Connection)
                return null;

            foreach (ConnectionModule connection in connections)
                if (connection.Position.Equals(Position))
                    return connection;

            Debug.Log("Realistically, this should never happen. However since we "
                + "got here anyways, something should be terribly wrong.");
            return null;
        }

        /// <summary>
        /// Temporary method to draw the tile in the inspector.
        /// </summary>
        public void Draw()
        {
            Handles.DrawSolidRectangleWithOutline(
                new Rect(Position.x, Position.y, 1, 1),
                Color.clear,
                Color.white);
        }
    }
}