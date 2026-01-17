using System;
using Godot;

public partial class InventoryUISlot : Panel
{
	// ----- Attributs ----- //

	private Sprite2D _itemDisplay;
	private Label _amount;
	private RichTextLabel _description;
	private TextureRect _selectedRect;

	private int _index;

	public event Action<int> LeftClicked;
	public event Action<int> RightClicked;


	// ----- Setters ----- //

	public void SetIndex(int index)
	{
		_index = index;
	}


	// ----- Override Godot Methods ----- //

	public override void _Ready()
	{
		_itemDisplay = GetNode<Sprite2D>("CenterContainer/ItemDisplay");
		_amount = GetNode<Label>("Amount");
		_description = GetNode<RichTextLabel>("Description");
		_selectedRect = GetNode<TextureRect>("SelectedRect");

		_description.Visible = false;
	}


	public override void _GuiInput(InputEvent @event)
	{
		if (@event is InputEventMouseButton mb)
		{
			if (mb.ButtonIndex == MouseButton.Left && mb.Pressed)
			{
				LeftClicked?.Invoke(_index);
			}
			if (mb.ButtonIndex == MouseButton.Right && mb.Pressed)
			{
				RightClicked?.Invoke(_index);
			}
			_description.Visible = false;
		}
	}


	// ----- On Signal ----- //

	private void OnMouseEntered()
	{
		if (!string.IsNullOrEmpty(_description.Text))
			_description.Visible = true;
	}


	private void OnMouseExited()
	{
		_description.Visible = false;
	}


	// ----- Other methods ----- //

	public void Update(InventorySlot slot)
	{
		if (slot.Item != null)
		{
			_itemDisplay.Visible = true;
			if (slot.Amount > 1) _amount.Visible = true;
			else _amount.Visible = false;

			_itemDisplay.Texture = slot.Item.Texture;
			_amount.Text = slot.Amount.ToString();

			var raw = slot.Item.Name + "\n" + slot.Item.Description;
			_description.Text = System.Text.RegularExpressions.Regex.Unescape(raw);
		}
		else
		{
			_itemDisplay.Visible = false;
			_amount.Visible = false;
			_description.Visible = false;

			_itemDisplay.Texture = null;
			_amount.Text = "0";
			_description.Text = "";
		}

		_selectedRect.Visible = slot.Selected;
	}
}
