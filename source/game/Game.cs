using System;
using Godot;

public partial class Game : Node
{
	// ----- Attributs ----- //

	private Player _player;
	private World _world;


	// ----- Override Godot Methods ----- //

	public override void _Ready()
	{
		_player = GetNode<Player>("Player");
		_world = GetNode<World>("World");
		_player.SetWorld(_world);
		_player.SetInventory(GetNode<InventoryUi>("HUD/InventoryUI").GetInventory());
		_player.Position = _world.GetSpawnPoint().Position;
		_player.SetMovementLimits(_world.GetTileMap().GetUsedRect(), _world.GetTileMap().TileSet.TileSize);
	}


	// ----- Other methods ----- //

	public void StartGame()
	{
		// Load player and world here
	}
}
