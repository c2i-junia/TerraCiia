using Godot;

public partial class InventorySlot: Resource
{
    // ----- Attributs ----- //

    [Export] public Item Item = null;
    [Export] public int Amount = 0;
    public bool Selected = false;
}