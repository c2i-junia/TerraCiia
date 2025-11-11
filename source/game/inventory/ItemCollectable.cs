using Godot;

public partial class ItemCollectable : RigidBody2D
{
	// ----- Attributs ----- //

	[Export] private Item _item;
	private Player player = null;

	// ----- Getters ----- //

	public Item GetItem() { return _item; }


	// ----- Setters ----- //

	public void SetItem(Item item) { _item = item; }


	// ----- On Signal ----- //

	private async void OnArea2dBodyEntered(Node2D body)
	{
		if (body is Player playerBody)
		{
			player = playerBody;
			player.Collect(_item);
			await ToSignal(GetTree().CreateTimer(0.1), "timeout");
			QueueFree();
		}
	}
}
