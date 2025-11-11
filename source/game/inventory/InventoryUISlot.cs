using System;
using Godot;

public partial class InventoryUISlot : Panel
{
	private Sprite2D _itemDisplay;
	private Label _amount;
	private RichTextLabel _description;


	public override void _Ready()
	{
		_itemDisplay = GetNode<Sprite2D>("CenterContainer/ItemDisplay");
		_amount = GetNode<Label>("Amount");
		_description = GetNode<RichTextLabel>("Description");
	}


	public void Update(InventorySlot slot)
	{
		if (slot.Item != null)
		{
			_itemDisplay.Visible = true;
			if (slot.Amount > 1) _amount.Visible = true;

			_itemDisplay.Texture = slot.Item.Texture;
			_amount.Text = slot.Amount.ToString();

			_description.Text = slot.Item.Name + "\n" + slot.Item.Description;
		}
		else
		{
			_itemDisplay.Visible = false;
			_amount.Visible = false;
			_description.Visible = false;
		}
	}
	

	private void OnMouseEntered()
    {
        if (!string.IsNullOrEmpty(_description.Text))
            _description.Visible = true;
    }


	private void OnMouseExited()
	{
		_description.Visible = false;
	}
}
