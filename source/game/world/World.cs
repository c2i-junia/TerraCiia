using System;
using Godot;

public partial class World : Node2D
{
    private static readonly StringName _dropItemPathKey = new("DropItemPath");

    // ----- Attributs ----- //

    private TileMapLayer _tileMap;
    private Marker2D _spawnPoint;

    private RandomNumberGenerator _rng;

    [Export] private int _seed = 0; // TO choose/randomize later in menu
    [Export] private int _worldWidth = 500;  // World width
    [Export] private int _bottomLevel = 200; // Bottom of the world
    [Export] private int _groundLevel = -50;  // Average Y position for the surface
    [Export] private int _amplitude = 20;    // Max height of hills
    [Export] private float _caveThreshold = -0.4f; // Value to set cave size ?

    [Export] private int _stoneStartingLevel = 0; // Starting height for stone
    [Export] private int _transitionSize = 30; // transition zone between dirt and stone

    [Export] private FastNoiseLite _heightNoise;
    [Export] private FastNoiseLite _caveNoise;

    // ----- Getters ----- //

    public TileMapLayer GetTileMap() { return _tileMap; }
    public Marker2D GetSpawnPoint() { return _spawnPoint; }
    public int GetWorldWidth() { return _worldWidth; }


    // ----- Override Godot Methods ----- //

    public override void _Ready()
    {
        _tileMap = GetNode<TileMapLayer>("TileMapLayer");
        _spawnPoint = GetNode<Marker2D>("SpawnPoint");

        GenerateWorld(_seed);
    }


    // ----- On Signal ----- //

    private void OnPlayerBreakBlock(Vector2I coords)
    {
        // Read tile metadata before erasing the cell.
        TileData brokenTileData = GetTileMap().GetCellTileData(coords);
        Item dropItem = TryGetDropItem(brokenTileData);

        GetTileMap().EraseCell(coords);
        UpdateNeighborCells(coords);

        PackedScene itemCollectableScene = ResourceLoader.Load<PackedScene>("res://source/game/inventory/item_collectable.tscn");

        // instantiate the collectable scene and place it at the broken tile position
        if (itemCollectableScene != null && dropItem != null)
        {
            if (itemCollectableScene.Instantiate() is ItemCollectable instance)
            {
                // place at tile center (convert map coords -> tilemap local -> global)
                instance.GlobalPosition = GetTileMap().ToGlobal(GetTileMap().MapToLocal(coords));
                instance.GetNode<Sprite2D>("Sprite2D").Texture = dropItem.Texture;
                instance.SetItem(dropItem);
                AddChild(instance);
            }
        }
    }


    private void OnPlayerPlaceBlock(Vector2I coords, int sourceId)
    {
        GetTileMap().SetCell(coords, sourceId, Vector2I.Zero);
        UpdateNeighborCells(coords);
    }


    // ----- Procedural Generation ----- //

    public void GenerateWorld(int seed)
    {
        _rng = new RandomNumberGenerator { Seed = (ulong)seed };

        _heightNoise.Seed = seed; // The other params are chosen in the Godot inspector
        _caveNoise.Seed = seed + 111;

        // link terrain id to coords
        var terrainGroups = new System.Collections.Generic.Dictionary<int, Godot.Collections.Array<Vector2I>>();

        // build world reliefs
        _tileMap.Clear();
        for (int x = -_worldWidth / 2; x < _worldWidth / 2; x++)
        {
            // Retrieve a value between -1.0 and 1.0
            float heightValue = _heightNoise.GetNoise1D(x);
            // Convert in Y coords
            int surfaceY = _groundLevel + Mathf.RoundToInt(heightValue * _amplitude);

            // Fill from bottom to surface
            for (int y = surfaceY; y < _bottomLevel; y++)
            {
                float caveValue = _caveNoise.GetNoise2D(x, y);

                if (caveValue > _caveThreshold)
                {
                    // do nothing to add caves.
                    continue;
                }

                Vector2I coords = new(x, y);
                int sourceId;
                if (y == surfaceY) sourceId = 1; // grass
                else
                {
                    // Calulate stone probability (0.0 to 1.0)
                    float stoneChance = Mathf.Clamp((float)(y - _stoneStartingLevel) / _transitionSize, 0, 1);

                    // Add a 2D noise to add random spot
                    float stoneNoise = _heightNoise.GetNoise2D(x * 2.0f, y * 2.0f);
                    if (stoneNoise + stoneChance > 0.6f)
                        sourceId = 2; // Stone
                    else
                        sourceId = 0; // Dirt
                }

                _tileMap.SetCell(coords, sourceId, Vector2I.Zero);
                if (TryGetCellTerrain(coords, out int terrainSet, out int terrainId))
                {
                    // add to dictionary for connexion
                    if (!terrainGroups.ContainsKey(terrainId))
                        terrainGroups[terrainId] = new Godot.Collections.Array<Vector2I>();
                    terrainGroups[terrainId].Add(coords);
                }
            }
        }

        // Apply connexion between tiles
        foreach (var (terrainId, cells) in terrainGroups)
        {
            _tileMap.SetCellsTerrainConnect(cells, 0, terrainId, false);
        }

        // Place Tree
        PlaceRandomTreesOnSurface();
    }


    public void SpawnTree(Vector2I coords)
    {
        if (_tileMap.GetCellSourceId(coords) != -1)
            return;

        int height = _rng.RandiRange(5, 9);
        Vector2I currentTile = coords;

        // on grass
        currentTile += new Vector2I(0, 1);
        _tileMap.SetCell(currentTile, 1, Vector2I.Zero);
        // trunk
        for (int i = 0; i < height; i++)
        {
            currentTile += new Vector2I(0, -1);
            _tileMap.SetCell(currentTile, 3, Vector2I.Zero);
        }
        // leaf
        currentTile += new Vector2I(0, -1);
        _tileMap.SetCell(currentTile, 4, Vector2I.Zero);
    }


    public void PlaceRandomTreesOnSurface(int minSpacingInTiles = 4)
    {
        Rect2I usedRect = _tileMap.GetUsedRect();
        if (usedRect.Size == Vector2I.Zero)
            return;

        // find empty cell above the first solid cell of each x row
        var surfaceSpots = new Godot.Collections.Array<Vector2I>();

        int minX = usedRect.Position.X;
        int maxX = usedRect.Position.X + usedRect.Size.X - 1;
        int minY = usedRect.Position.Y;
        int maxY = usedRect.Position.Y + usedRect.Size.Y - 1;

        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                Vector2I cell = new(x, y);
                if (_tileMap.GetCellSourceId(cell) == -1)
                    continue;

                Vector2I above = new(x, y - 1);
                if (_tileMap.GetCellSourceId(above) != -1)
                    continue;

                surfaceSpots.Add(above);
                break; // 1 surface per row
            }
        }

        if (surfaceSpots.Count == 0)
            return;

        // random possibility to place a tree, with a min space between each one
        var chosen = new Godot.Collections.Array<Vector2I>();

        for (int i = 0; i < surfaceSpots.Count; i++)
        {
            Vector2I candidate = surfaceSpots[i];

            int luck = _rng.RandiRange(0, 4);
            if (luck == 0) // 1 in 5
            {
                chosen.Add(candidate);
                i += minSpacingInTiles;
            }
        }

        // Spawn
        foreach (var spot in chosen)
            SpawnTree(spot);
    }


    // ----- Other methods ----- //

    private bool TryGetCellTerrain(Vector2I coords, out int terrainSet, out int terrain)
    {
        terrainSet = -1;
        terrain = -1;

        TileData tileData = _tileMap.GetCellTileData(coords);
        if (tileData == null)
            return false;

        terrainSet = tileData.TerrainSet;
        terrain = tileData.Terrain;
        return terrainSet >= 0 && terrain >= 0;
    }


    private Item TryGetDropItem(TileData tileData)
    {
        // explicit resource path stored in TileSet custom data.
        if (tileData != null)
        {
            Variant dropPathVariant = tileData.GetCustomData(_dropItemPathKey);
            if (dropPathVariant.VariantType == Variant.Type.String)
            {
                string dropPath = dropPathVariant.AsString();
                if (!string.IsNullOrWhiteSpace(dropPath) && ResourceLoader.Exists(dropPath))
                    return ResourceLoader.Load<Item>(dropPath);
            }
        }

        return null;
    }


    public Godot.Collections.Array<Vector2I> GetNeighborCells(Vector2I coords)
    {
        var neighbors = new Godot.Collections.Array<Vector2I>();

        var neighborTypes = new TileSet.CellNeighbor[] {
            TileSet.CellNeighbor.TopLeftCorner,
            TileSet.CellNeighbor.TopSide,
            TileSet.CellNeighbor.TopRightCorner,
            TileSet.CellNeighbor.RightSide,
            TileSet.CellNeighbor.LeftSide,
            TileSet.CellNeighbor.BottomLeftCorner,
            TileSet.CellNeighbor.BottomSide,
            TileSet.CellNeighbor.BottomRightCorner
        };

        foreach (var nt in neighborTypes)
        {
            var n = _tileMap.GetNeighborCell(coords, nt);
            if (_tileMap.GetCellSourceId(n) != -1)
            {
                neighbors.Add(n);
            }
        }

        return neighbors;
    }


    public void UpdateNeighborCells(Vector2I coords)
    {
        var neighbors = GetNeighborCells(coords);
        neighbors.Add(coords);
        if (neighbors.Count == 0)
            return;

        foreach (var n in neighbors)
        {
            if (TryGetCellTerrain(n, out int terrainSet, out int terrain))
            {
                _tileMap.SetCellsTerrainConnect([n], terrainSet, terrain, false);
            }
        }
    }
}