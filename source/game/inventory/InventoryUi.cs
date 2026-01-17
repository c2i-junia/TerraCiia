using System;
using Godot;

public partial class InventoryUi : Control
{
	// ----- Attributs ----- //

	private Inventory _inventory;
	private Godot.Collections.Array<InventoryUISlot> _uiSlots;

	private bool _isOpen = false;

	private InventorySlot _heldSlot = new InventorySlot();
	private bool _hasHeld = false;

	Sprite2D _heldItemDisplay;
	Label _heldAmount;


	// ----- Getters ----- //

	public Inventory GetInventory() { return _inventory; }


	// ----- Override Godot Methods ----- //

	public override void _Ready()
	{
		_inventory = new Inventory(50);
		_inventory.Update += UpdateSlots;

		_uiSlots = [];

		foreach (var child in GetNode<GridContainer>("HotbarSlots").GetChildren())
		{
			if (child is InventoryUISlot slot)
				_uiSlots.Add(slot);
		}

		foreach (var child in GetNode<GridContainer>("OtherSlots").GetChildren())
		{
			if (child is InventoryUISlot slot)
				_uiSlots.Add(slot);
		}

		for (int i = 0; i < _uiSlots.Count; i++)
		{
			_uiSlots[i].SetIndex(i);
			_uiSlots[i].LeftClicked += OnSlotLeftClicked;
			_uiSlots[i].RightClicked += OnSlotRightClicked;
		}

		_heldItemDisplay = GetNode<Sprite2D>("HeldItemDisplay");
		_heldAmount = GetNode<Label>("HeldItemDisplay/Amount");

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

		if (Input.IsActionJustPressed("inventory_slot+"))
		{
			int current = _inventory.GetSelectedSlotIndex();
			int next = (current + 1) % 10;

			_inventory.Slots[current].Selected = false;
			_inventory.Slots[next].Selected = true;
			UpdateSlots();
		}

		if (Input.IsActionJustPressed("inventory_slot-"))
		{
			int current = _inventory.GetSelectedSlotIndex();
			int prev = (current - 1 + 10) % 10; // add slotCount to avoid negative remainder

			_inventory.Slots[current].Selected = false;
			_inventory.Slots[prev].Selected = true;
			UpdateSlots();
		}


		if (_heldSlot.Item != null)
		{
			_heldItemDisplay.Texture = _heldSlot.Item.Texture;

			Vector2 mousePos = GetLocalMousePosition();
			_heldItemDisplay.Position = mousePos;
			_heldAmount.Text = _heldSlot.Amount.ToString();
			if (_heldAmount.Text.ToInt() >1) _heldAmount.Visible = true;
		}
		else
		{
			_heldItemDisplay.Texture = null;
			_heldAmount.Visible = false;
		}
	}


	// ----- On Signal ----- //

	private void OnSlotLeftClicked(int index)
	{
		var slot = _inventory.Slots[index];

		if (!_hasHeld)
		{
			if (slot.Item == null) return;

			_heldSlot.Item = slot.Item;
			_heldSlot.Amount = slot.Amount;

			slot.Item = null;
			slot.Amount = 0;
			_hasHeld = true;
		}
		else
		{
			// swap
			if (slot.Item == null || slot.Item.Id != _heldSlot.Item.Id)
			{
				var tempItem = slot.Item;
				var tempAmount = slot.Amount;

				slot.Item = _heldSlot.Item;
				slot.Amount = _heldSlot.Amount;

				_heldSlot.Item = tempItem;
				_heldSlot.Amount = tempAmount;

				if (_heldSlot.Item == null)
					_hasHeld = false;
			}
			// fuse
			else
			{
				slot.Amount += _heldSlot.Amount;
				_heldSlot.Item = null;
				_heldSlot.Amount = 0;

				_hasHeld = false;
			}
		}

		UpdateSlots();
	}


	private void OnSlotRightClicked(int index)
	{
		var slot = _inventory.Slots[index];

		if (slot.Item == null) return;

		// take 1
		if (!_hasHeld)
		{
			_heldSlot.Item = (Item)slot.Item.Duplicate(true);
			_heldSlot.Amount = 1;

			slot.Amount -= 1;
			if (slot.Amount <= 0) slot.Item = null;
			_hasHeld = true;
		}
		// add 1
		else if (slot.Item.Id == _heldSlot.Item.Id)
		{
			_heldSlot.Amount += 1;

			slot.Amount -= 1;
			if (slot.Amount <= 0) slot.Item = null;
			_hasHeld = true;
		}

		UpdateSlots();
	}


	// ----- Other methods ----- //

	private void Open()
	{
		GetNode<GridContainer>("OtherSlots").Visible = true;
		_isOpen = true;
	}


	private void Close()
	{
		GetNode<GridContainer>("OtherSlots").Visible = false;
		_isOpen = false;
	}


	private void UpdateSlots()
	{
		for (int i = 0; i < Mathf.Min(_inventory.Slots.Count, _uiSlots.Count); i++)
		{
			_uiSlots[i].Update(_inventory.Slots[i]);
		}
	}
}
