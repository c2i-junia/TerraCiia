using Godot;

public partial class Player : CharacterBody2D
{
	// ----- Signal ----- //

	[Signal] public delegate void BreakBlockEventHandler(Vector2I coords, int sourceId);
	[Signal] public delegate void PlaceBlockEventHandler(Vector2I coords, int sourceId);


	// ----- Attributs ----- //

	private World _world;
	private AnimatedSprite2D _animatedSprite2D;
	private Inventory _inventory;

	[Export] private float _speed = 300.0f;
	[Export] private float _jumpVelocity = -400.0f;

	[Export] private float _clickCooldown = 0.05f;
	private double _lastClickTime = 0;
	[Export] private float _interactionRange = 10000f;


	// ----- Getters ----- //


	// ----- Setters ----- //

	public void SetWorld(World world) { _world = world; }


	// ----- Override Godot Methods ----- //

	public override void _Ready()
	{
		_animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_animatedSprite2D.Play();
		_inventory = GetNode<InventoryUi>("InventoryUI").GetInventory();
	}


	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		// Handle Jump.
		if (Input.IsActionJustPressed("jump") && IsOnFloor())
		{
			velocity.Y = _jumpVelocity;
		}

		// Get the input direction and handle the movement/deceleration.
		var direction = Vector2.Zero;
		if (Input.IsActionPressed("right"))
		{
			direction.X += 1;
		}
		if (Input.IsActionPressed("left"))
		{
			direction.X -= 1;
		}

		if (direction != Vector2.Zero)
		{
			velocity.X = direction.X * _speed;
			_animatedSprite2D.Animation = "walk";
			_animatedSprite2D.FlipH = velocity.X < 0;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, _speed);
			_animatedSprite2D.Animation = "idle";
		}


		Velocity = velocity;
		MoveAndSlide();
	}


	public override void _Process(double delta)
	{
		// Handle use
		if (Input.IsActionPressed("use"))
		{
			if (Time.GetTicksMsec() - _lastClickTime < _clickCooldown * 1000) return;
			_lastClickTime = Time.GetTicksMsec();

			Vector2 mouseWorldPos = GetGlobalMousePosition();
			float dist = GlobalPosition.DistanceTo(mouseWorldPos);

			// limit range
			if (dist > _interactionRange)
				return;

			Vector2I tilePos = _world.GetTileMap().LocalToMap(_world.GetTileMap().ToLocal(mouseWorldPos));
			int tileId = _world.GetTileMap().GetCellSourceId(tilePos);

			if (tileId != -1 && _inventory.GetSelectedSlot().Item == null) // TODO: replace by a tool later
			{ // break
				EmitSignal(SignalName.BreakBlock, tilePos, tileId);
			}
			else if (tileId == -1 && _inventory.GetSelectedSlot().Item != null && _inventory.GetSelectedSlot().Amount != 0)
			{ // place
				EmitSignal(SignalName.PlaceBlock, tilePos, _inventory.GetSelectedSlot().Item.Id);
				_inventory.Remove(_inventory.GetSelectedSlot().Item);
			}
		}

		// Handle interact
		if (Input.IsActionPressed("interact"))
		{

		}
	}


	// ----- Other methods ----- //

	public void Collect(Item item)
	{
		_inventory.Insert(item);
	}
}
