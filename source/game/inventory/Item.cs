using Godot;

public partial class Item: Resource
{
    [Export] public string Name;
    [Export] public Texture2D Texture;
    [Export] public string Description;
}