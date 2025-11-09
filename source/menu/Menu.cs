using Godot;

public partial class Menu : Node
{
	[Signal]
	public delegate void StartGameEventHandler();

	private void OnStartButtonPressed()
	{
		GetNode<Label>("Title").Hide();
		GetNode<Button>("StartButton").Hide();
		EmitSignal(SignalName.StartGame);
	}
}
