using Godot;

public partial class Main : Node
{
    // ----- Attributs ----- //
    
    private Menu _menu;
    private Game _game;


    // ----- Override Godot Methods ----- //

    public override void _Ready()
    {
        _menu = GetNode<Menu>("Menu");
        _game = GetNode<Game>("Game");
    }


    // ----- On Signal ----- //

    private void OnMenuStartGame()
    {
        _menu.Hide();
        _game.StartGame();
    }

    private void OnMenuQuitMenu()
    {
        GetTree().Quit();
    }
}
