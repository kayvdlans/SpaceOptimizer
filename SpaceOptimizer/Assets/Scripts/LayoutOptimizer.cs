﻿using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ModuleBuilder))]
public class LayoutOptimizer :  MonoBehaviour
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
        FieldTiles[position.x, position.y].Free = false;

        return connectionModule;
    }

    private Module CreateModuleFromBuildModuleAtPosition(BuildModule buildModule, Vector2Int position)
    {
        Module module = new Module(buildModule.Name, buildModule.Size, position);

        for (int x = position.x; x < position.x + buildModule.Size.x; x++)
            for (int y = position.y; y < position.y + buildModule.Size.y; y++)
                FieldTiles[x, y].Free = false;

        buildModule.AmountPlaced++;
        _Modules.Add(module);

        if (buildModule.ConnectionNeeded)
        {
            ConnectionModule connection = module.GetConnection(_ConnectionModules);
            if (connection == null || !connection.IsConnectedToMainModule(_Modules[0].Bounds, new List<ConnectionModule>()))
            {
                ConnectionModule cm = CreateConnectionModuleAtPosition(module.GetClosestFreeTile(FieldTiles, _ConnectionModules).Position);
                if (!cm.IsConnectedToMainModule(_Modules[0].Bounds, new List<ConnectionModule>()))
                {
                    while (true)
                    {
                        FieldTile newTile = cm.GetClosestFreeTile(FieldTiles, _ConnectionModules);
                            cm = CreateConnectionModuleAtPosition(newTile.Position);

                        if (cm.IsConnectedToMainModule(_Modules[0].Bounds, new List<ConnectionModule>()))
                        {
                            break;
                        }
                    }
                }
            }
        }

        return module;
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