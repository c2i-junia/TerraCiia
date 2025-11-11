using Godot;

public partial class InventoryUi : Control
{
	private Inventory _inventory;
	private Godot.Collections.Array<InventoryUISlot> _slots;

	private bool _isOpen = false;

	public Inventory GetInventory() { return _inventory; }


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
		Close();
	}


	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("inventory"))
		{
			if (_isOpen) Close();
			else Open();
		}
	}


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
