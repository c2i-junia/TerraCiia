using Godot;
using System.Linq;

public partial class Inventory : Resource
{
    [Signal] public delegate void UpdateEventHandler();

    [Export] public Godot.Collections.Array<InventorySlot> Slots;

    public Inventory(int size)
    {
        Slots ??= [];

        for (int i = 0; i < size; i++)
        {
            Slots.Add(new InventorySlot());
        }
    }

    public void Insert(Item item)
    {
        var itemSlots = new Godot.Collections.Array<InventorySlot>(
            Slots.Where(slot => slot.Item == item)
        );

        if (itemSlots.Count != 0)
        {
            itemSlots[0].Amount += 1;
        }
        else
        {
            var emptySlots = new Godot.Collections.Array<InventorySlot>(
                Slots.Where(slot => slot.Item == null)
            );
            if (emptySlots.Count != 0)
            {
                emptySlots[0].Item = item;
                emptySlots[0].Amount = 1;
            }
        }
        EmitSignal(SignalName.Update);
    }
}