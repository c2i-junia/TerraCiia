using Godot;

public partial class InventoryUi : Control
{
	// ----- Attributs ----- //

	private Inventory _inventory;
	private Godot.Collections.Array<InventoryUISlot> _slots;

	private bool _isOpen = false;


	// ----- Getters ----- //

	public Inventory GetInventory() { return _inventory; }


	// ----- Override Godot Methods ----- //

	public override void _Ready()
	{
		_inventory = new Inventory(8);
		_inventory.Update += UpdateSlots;

		_slots = [];
		foreach (var child in GetNode<GridContainer>("NinePatchRect/GridContainer").GetChildren())
		{
			if (child is InventoryUISlot slot)
				_slots.Add(slot);
		}

		UpdateSlots();
		Open();
	}


	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("inventory"))
		{
			if (_isOpen) Close();
			else Open();
		}

		if (Input.IsActionJustPressed("inventory_slot+"))
		{
			int current = _inventory.GetSelectedSlotIndex();
            int next = (current + 1) % _inventory.Slots.Count;

            _inventory.Slots[current].Selected = false;
            _inventory.Slots[next].Selected = true;
            UpdateSlots();
		}

		if (Input.IsActionJustPressed("inventory_slot-"))
		{
			int current = _inventory.GetSelectedSlotIndex();
            int prev = (current - 1 + _inventory.Slots.Count) % _inventory.Slots.Count; // add slotCount to avoid negative remainder

            _inventory.Slots[current].Selected = false;
            _inventory.Slots[prev].Selected = true;
            UpdateSlots();
		}
	}


	// ----- Other methods ----- //

	private void Open()
	{
		Visible = true;
		_isOpen = true;
	}


	private void Close()
	{
		Visible = false;
		_isOpen = false;
	}


	private void UpdateSlots()
	{
		for (int i = 0; i < Mathf.Min(_inventory.Slots.Count, _slots.Count); i++)
		{
			_slots[i].Update(_inventory.Slots[i]);
		}
	}
}
