using SpaceOptimizer.Modules;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceOptimizer
{
    [RequireComponent(typeof(ModuleBuilder))]
    public class LayoutOptimizer : MonoBehaviour
    {
        [Header("Modules")]
        [SerializeField] private ModuleBuilder _ModuleBuilder;
        [SerializeField] private BuildModule _MainBuildModule;

        [Header("Field")]
        [SerializeField] private Vector2Int _FieldTileSize;
        [SerializeField] private Vector2Int _FieldSize;

        private List<Module> _Modules = new List<Module>();
        private List<ConnectionModule> _ConnectionModules = new List<ConnectionModule>();

        public Vector2Int FieldTileSize { get { return _FieldTileSize; } }
        public Vector2Int FieldSize { get { return _FieldSize; } }

        public List<Module> Modules { get { return _Modules; } }
        public List<ConnectionModule> ConnectionModules { get { return _ConnectionModules; } }

        public FieldTile[,] FieldTiles { get; private set; }

        private void InitializeFieldTiles()
        {
            FieldTiles = new FieldTile[_FieldSize.x * _FieldTileSize.x, _FieldSize.y * _FieldTileSize.y];
            for (int y = 0; y < FieldTiles.GetLength(1); y++)
            {
                for (int x = 0; x < FieldTiles.GetLength(0); x++)
                {
                    FieldTiles[x, y] = new FieldTile(new Vector2Int(x, y));
                }
            }
        }

        private ConnectionModule CreateConnectionModuleAtPosition(Vector2Int position)
        {
            ConnectionModule connectionModule = new ConnectionModule(position, _ConnectionModules);
            _ConnectionModules.Add(connectionModule);
            FieldTiles[position.x, position.y].TileType = FieldTile.eTileType.Connection;

            return connectionModule;
        }

        private Module CreateModuleFromBuildModuleAtPosition(BuildModule buildModule, Vector2Int position)
        {
            Module module = new Module(buildModule.Name, buildModule.Size, position);

            for (int x = position.x; x < position.x + buildModule.Size.x; x++)
                for (int y = position.y; y < position.y + buildModule.Size.y; y++)
                    FieldTiles[x, y].TileType = FieldTile.eTileType.Module;

            buildModule.AmountPlaced++;
            _Modules.Add(module);

            if (buildModule.ConnectionNeeded)
            {
                foreach (FieldTile tile in _Modules[0].GetSortedListByDistanceToModule(_Modules[0].GetAdjacentTilesOfType(FieldTiles, FieldTile.eTileType.Connection, FieldTile.eTileType.Empty), module))
                {
                    if (TryCreateConnectionToModule(module, tile, new List<FieldTile>()))
                        break;
                }
            }

            return module;
        }

        private bool TryCreateConnectionToModule(Module module, FieldTile tile, List<FieldTile> unavailable)
        {
            if (unavailable.Contains(tile))
                return false;
            else
                unavailable.Add(tile);

            ConnectionModule connection = tile.TileType == FieldTile.eTileType.Connection ? tile.GetConnectionModule(_ConnectionModules) : CreateConnectionModuleAtPosition(tile.Position);

            if (connection != null)
            {
                if (connection.IsConnected(module.Bounds))
                {
                    return true;
                }
                else
                {
                    List<FieldTile> tiles = tile.GetAdjacentTilesOfType(FieldTiles, FieldTile.eTileType.Empty, FieldTile.eTileType.Connection);

                    tiles.Sort((x, y) => x.GetDistanceToCenterOfModule(module).CompareTo(y.GetDistanceToCenterOfModule(module)));

                    foreach (FieldTile t in tiles)
                        if (TryCreateConnectionToModule(module, t, unavailable))
                            return true;
                }
            }

            return false;
        }

        private void TryCreateNewModule(Vector2Int position)
        {
            for (int i = 0; i < _ModuleBuilder.BuildModules.Count; i++)
            {
                if (_ModuleBuilder.BuildModules[i].AmountPlaced < _ModuleBuilder.BuildModules[i].Amount
                    && _ModuleBuilder.BuildModules[i].IsInsideFieldBounds(position, _FieldSize * _FieldTileSize)
                    && !_ModuleBuilder.BuildModules[i].HasOverlap(position, FieldTiles))
                {
                    CreateModuleFromBuildModuleAtPosition(_ModuleBuilder.BuildModules[i], position);
                    break;
                }
            }
        }

        private void OnDrawGizmos()
        {
            _Modules.Clear();
            _ConnectionModules.Clear();

            foreach (BuildModule bm in _ModuleBuilder.BuildModules)
                bm.AmountPlaced = 0;

            InitializeFieldTiles();

            CreateModuleFromBuildModuleAtPosition(_MainBuildModule, Vector2Int.zero);

            for (int y = 0; y < _FieldSize.y * _FieldTileSize.y; y++)
            {
                for (int x = 0; x < _FieldSize.x * _FieldTileSize.x; x++)
                {
                    TryCreateNewModule(new Vector2Int(x, y));
                }
            }
        }
    }
}