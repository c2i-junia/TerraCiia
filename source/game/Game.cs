using Godot;

public partial class Game : Node
{
	public override void _Ready()
	{
		var player = GetNode<Player>("Player");
		var world = GetNode<World>("World");
		player.SetWorld(world);
	}
	
	void OnStartGame()
	{
		GD.Print("Hello world");
	}
}
