using Godot;

public enum ItemType
{
    Placeable,
    Tool,
    Weapon
}

[Tool]
[GlobalClass]
public partial class Item : Resource
{
    // ----- Attributs ----- //

    [Export] public int Id;
    [Export] public string Name = "";
    [Export] public Texture2D Texture;
    [Export(PropertyHint.MultilineText)] public string Description = "";

    private ItemType _type = ItemType.Placeable;

    [Export]
    public ItemType Type
    {
        get => _type;
        set
        {
            if (_type == value) return;
            _type = value;

            NotifyPropertyListChanged(); // Force the inspector to recalculate properties visibility
        }
    }

    [Export] public int TileId = -1; // Only for placeable


    // ----- Override Godot Methods ----- //

    public override void _ValidateProperty(Godot.Collections.Dictionary property)
    {
        string name = property["name"].AsString();

        if (name == nameof(TileId))
        {
            // Hide TileId if the item is not Placeable
            if (Type != ItemType.Placeable)
            {
                PropertyUsageFlags usage = (PropertyUsageFlags)(int)property["usage"];
                usage &= ~PropertyUsageFlags.Editor; // invert bits to hide in inspector
                property["usage"] = (int)usage;
            }
        }
    }
}