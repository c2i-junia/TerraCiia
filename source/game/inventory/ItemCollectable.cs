using Godot;

public partial class ItemCollectable : RigidBody2D
{
	[Export] Item item;
	Player player = null;

	private async void OnArea2dBodyEntered(Node2D body)
    {
        if (body is Player playerBody)
		{
			player = playerBody;
			player.Collect(item);
			await ToSignal(GetTree().CreateTimer(0.1), "timeout");
			QueueFree();
        }
    }
}
