using Godot;

public partial class Item: Resource
{
    // ----- Attributs ----- //

    [Export] public int Id;
    [Export] public string Name;
    [Export] public Texture2D Texture;
    [Export] public string Description;
    [Export] public string Type;
}