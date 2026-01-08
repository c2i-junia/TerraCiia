using Godot;

public partial class Player : CharacterBody2D
{
	// ----- Signal ----- //

	[Signal] public delegate void BreakBlockEventHandler(Vector2I coords, int sourceId);
	[Signal] public delegate void PlaceBlockEventHandler(Vector2I coords, int sourceId);


	// ----- Attributs ----- //

	private World _world;
	private AnimatedSprite2D _legsSprite;
	private AnimatedSprite2D _bodySprite;
	private Inventory _inventory;

	[Export] private float _speed = 300.0f;
	[Export] private float _jumpVelocity = -400.0f;

	[Export] private float _clickCooldown = 0.05f;
	private double _lastClickTime = 0;
	[Export] private float _interactionRange = 10000f;

	private Vector2 _direction = Vector2.Zero;
	private bool _isUsing = false;


	// ----- Getters ----- //


	// ----- Setters ----- //

	public void SetWorld(World world) { _world = world; }


	// ----- Override Godot Methods ----- //

	public override void _Ready()
	{
		_legsSprite = GetNode<AnimatedSprite2D>("LegsSprite");
		_bodySprite = GetNode<AnimatedSprite2D>("BodySprite");
		_bodySprite.Play();
		_legsSprite.Play();
		_inventory = GetNode<InventoryUi>("InventoryUI").GetInventory();

		_bodySprite.AnimationFinished += () =>
		{
			if (_bodySprite.Animation == "use")
				_isUsing = false;
		};
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
		_direction = Vector2.Zero;
		if (Input.IsActionPressed("right"))
		{
			_direction.X += 1;
		}
		if (Input.IsActionPressed("left"))
		{
			_direction.X -= 1;
		}

		if (_direction != Vector2.Zero)
		{
			velocity.X = _direction.X * _speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, _speed);
		}


		Velocity = velocity;
		ProcessAnimation();
		MoveAndSlide();
	}


	public override void _Process(double delta)
	{
		// Handle use
		if (Input.IsActionPressed("use"))
		{
			if (!_isUsing)
			{
				_isUsing = true;
				_bodySprite.Play("use");
			}

			if (Time.GetTicksMsec() - _lastClickTime < _clickCooldown * 1000) return;
			_lastClickTime = Time.GetTicksMsec();

			Vector2 mouseWorldPos = GetGlobalMousePosition();
			float dist = GlobalPosition.DistanceTo(mouseWorldPos);

			// limit range
			if (dist > _interactionRange)
				return;

			Vector2I tilePos = _world.GetTileMap().LocalToMap(_world.GetTileMap().ToLocal(mouseWorldPos));
			int tileId = _world.GetTileMap().GetCellSourceId(tilePos);

			if (tileId != -1 && _inventory.GetSelectedSlot().Item != null && _inventory.GetSelectedSlot().Item.Type == "tool")
			{ // break
				EmitSignal(SignalName.BreakBlock, tilePos, tileId);
			}
			else if (tileId == -1 && _inventory.GetSelectedSlot().Item != null && _inventory.GetSelectedSlot().Item.Type == "placeable" && _inventory.GetSelectedSlot().Amount != 0)
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

	public void ProcessAnimation()
	{
		// jump
		if (!IsOnFloor())
		{
			if (!_isUsing)
			{
				_bodySprite.Animation = "jump";
			}
			_legsSprite.Animation = "jump";
		}
		// walk
		else if (_direction != Vector2.Zero)
		{
			if (!_isUsing)
			{
				_bodySprite.Animation = "walk";
			}
			_legsSprite.Animation = "walk";
		}
		// idle
		else
		{
			if (!_isUsing)
			{
				_bodySprite.Animation = "idle";
			}
			_legsSprite.Animation = "idle";
		}

		// flip direction
		if (_direction != Vector2.Zero)
		{
			_bodySprite.FlipH = _direction.X < 0;
			_legsSprite.FlipH = _direction.X < 0;
		}
	}


	public void Collect(Item item)
	{
		_inventory.Insert(item);
	}
}
