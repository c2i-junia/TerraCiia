using Godot;

public partial class InventorySlot: Resource
{
    [Export] public Item Item;
    [Export] public int Amount;
}