using Godot;

public partial class World : Node2D
{
    // ----- Attributs ----- //

    private TileMapLayer _tileMap;
    private Marker2D _spawnPoint;


    // ----- Getters ----- //

    public TileMapLayer GetTileMap() { return _tileMap; }
    public Marker2D GetSpawnPoint() { return _spawnPoint; }


    // ----- Override Godot Methods ----- //

    public override void _Ready()
    {
        _tileMap = GetNode<TileMapLayer>("TileMapLayer");
        _spawnPoint = GetNode<Marker2D>("SpawnPoint");
        FixTerrainConnections();
    }


    // ----- On Signal ----- //

    private void OnPlayerBreakBlock(Vector2I coords, int sourceId)
	{
		GetTileMap().EraseCell(coords);
		UpdateNeighborCells(coords);

        if (sourceId == 1) sourceId = 0; // grass drop dirt
        
        PackedScene itemCollectableScene = ResourceLoader.Load<PackedScene>("res://source/game/inventory/item_collectable.tscn");
		Item itemCollectable = ResourceLoader.Load<Item>("res://source/game/inventory/items/item_" + sourceId + ".tres");

		// instantiate the collectable scene and place it at the broken tile position
        if (itemCollectableScene != null)
        {
            if (itemCollectableScene.Instantiate() is ItemCollectable instance)
            {
                // place at tile center (convert map coords -> tilemap local -> global)
                instance.GlobalPosition = GetTileMap().ToGlobal(GetTileMap().MapToLocal(coords));
                instance.GetNode<Sprite2D>("Sprite2D").Texture = itemCollectable.Texture;
				instance.SetItem(itemCollectable);
                AddChild(instance);
            }
        }
	}


	private void OnPlayerPlaceBlock(Vector2I coords, int sourceId)
	{
		GetTileMap().SetCell(coords, sourceId, Vector2I.Zero);
		UpdateNeighborCells(coords);
	}


    // ----- Other methods ----- //

    private void FixTerrainConnections()
    {
        var usedCells = _tileMap.GetUsedCells();
        if (usedCells.Count == 0)
            return;

        foreach (var cell in usedCells)
        {
            var sourceId = _tileMap.GetCellSourceId(cell);

            if (sourceId == -1) continue;

            _tileMap.SetCellsTerrainConnect([cell], 0, sourceId, false);
        }
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
            var sourceId = _tileMap.GetCellSourceId(n);
            if (sourceId == -1)
                continue;

            _tileMap.SetCellsTerrainConnect([n], 0, sourceId, false);
        }
    }
}
