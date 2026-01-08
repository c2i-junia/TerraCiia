using Godot;
using System.Linq;

public partial class Inventory : Resource
{
    // ----- Signal ----- //

    [Signal] public delegate void UpdateEventHandler();


    // ----- Attributs ----- //

    [Export] public Godot.Collections.Array<InventorySlot> Slots;


    // ----- Constructor ----- //

    public Inventory(int size)
    {
        Slots ??= [];

        if (size < 1) size = 1;

        for (int i = 0; i < size; i++)
        {
            Slots.Add(new InventorySlot());
        }
        Slots[0].Selected = true;

        Item item = ResourceLoader.Load<Item>("res://source/game/inventory/items/item_" + 100 + ".tres");
        Insert(item);
        item = ResourceLoader.Load<Item>("res://source/game/inventory/items/item_" + 101 + ".tres");
        Insert(item);
    }


    // ----- Getters ----- //

    public int GetSelectedSlotIndex()
    {
        int index = 0;
        for (int i = 0; i < Slots.Count; i++)
        {
            if (Slots[i].Selected) index = i;
        }
        return index;
    }


    public InventorySlot GetSelectedSlot()
    {
        return Slots[GetSelectedSlotIndex()];
    }


    // ----- Other methods ----- //

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


    public void Remove(Item item, int quantity = 1)
    {
        var itemSlots = new Godot.Collections.Array<InventorySlot>(
            Slots.Where(slot => slot.Item == item)
        );

        if (itemSlots.Count != 0)
        {
            itemSlots[0].Amount -= quantity;

            if (itemSlots[0].Amount <= 0)
                itemSlots[0].Item = null;
        }
        EmitSignal(SignalName.Update);
    }
}