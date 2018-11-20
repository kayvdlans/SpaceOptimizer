using SpaceOptimizer.Modules;
using UnityEngine;
using UnityEditor;

namespace SpaceOptimizer
{
    [RequireComponent(typeof(LayoutOptimizer))]
    public class LayoutDrawer : MonoBehaviour
    {
        [SerializeField]
        private LayoutOptimizer _LayoutOptimizer;

        private void DrawField()
        {
            for (int y = 0; y < _LayoutOptimizer.FieldSize.y; y++)
                for (int x = 0; x < _LayoutOptimizer.FieldSize.x; x++)
                    Handles.DrawSolidRectangleWithOutline(
                        new Rect(x * _LayoutOptimizer.FieldTileSize.x, y * _LayoutOptimizer.FieldTileSize.y,
                            _LayoutOptimizer.FieldTileSize.x, _LayoutOptimizer.FieldTileSize.y),
                        Color.clear,
                        Color.black);
        }

        private void OnDrawGizmos()
        {
            if (_LayoutOptimizer.FieldTiles != null)
                foreach (FieldTile tile in _LayoutOptimizer.FieldTiles)
                    tile.Draw();

            DrawField();

            foreach (Module module in _LayoutOptimizer.Modules)
                module.Draw(Color.red, Color.cyan);

            foreach (ConnectionModule connection in _LayoutOptimizer.ConnectionModules)
                connection.Draw(Color.yellow, Color.cyan);
        }
    }
}
