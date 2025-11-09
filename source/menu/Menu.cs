using Godot;

public partial class Menu : Node
{
	// ----- Signal ----- //

	[Signal] public delegate void StartGameEventHandler();
	[Signal] public delegate void QuitMenuEventHandler();


	// ----- Attributs ----- //

	private Label _title;
    private Button _startButton;
    private Button _quitButton;


    // ----- Override Godot Methods ----- //

    public override void _Ready()
    {
        _title = GetNode<Label>("Title");
        _startButton = GetNode<Button>("StartButton");
        _quitButton = GetNode<Button>("QuitButton");
    }

	// ----- On Signal ----- //

	private void OnStartButtonPressed()
	{
		EmitSignal(SignalName.StartGame);
	}


	private void OnQuitButtonPressed()
	{
		EmitSignal(SignalName.QuitMenu);
	}


	// ----- Other methods ----- //

	public void Hide()
	{
		_title.Hide();
		_startButton.Hide();
		_quitButton.Hide();
	}


	public void Show()
	{
		_title.Show();
		_startButton.Show();
		_quitButton.Show();
	}
}
