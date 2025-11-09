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
		_player.Position = _world.GetSpawnPoint().Position;
	}


	// ----- Other methods ----- //

	public void StartGame()
	{
		// Load player and world here
	}
}
