using Godot;

public partial class Player : CharacterBody2D
{
	// ----- Signal ----- //

	[Signal] public delegate void BreakBlockEventHandler(Vector2I coords);
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

	private float _limitLeft = float.MinValue;
	private float _limitRight = float.MaxValue;
	private float _limitTop = float.MinValue;
	private float _limitBottom = float.MaxValue;


	// ----- Getters ----- //


	// ----- Setters ----- //

	public void SetWorld(World world) { _world = world; }

	public void SetInventory(Inventory inventory) { _inventory = inventory; }

	public void SetMovementLimits(Rect2I usedRect, Vector2I tileSize)
	{
		// convert tilemap coord in pixels
		_limitLeft = usedRect.Position.X * tileSize.X;
		_limitRight = (usedRect.Position.X + usedRect.Size.X) * tileSize.X;
		_limitTop = usedRect.Position.Y * tileSize.Y;
		_limitBottom = (usedRect.Position.Y + usedRect.Size.Y) * tileSize.Y;

		// also set the cam limits
		Camera2D cam = GetNode<Camera2D>("Camera2D");
		cam.LimitLeft = (int)_limitLeft;
		cam.LimitRight = (int)_limitRight;
		cam.LimitTop = (int)_limitTop;
		cam.LimitBottom = (int)_limitBottom;
	}


	// ----- Override Godot Methods ----- //

	public override void _Ready()
	{
		_legsSprite = GetNode<AnimatedSprite2D>("LegsSprite");
		_bodySprite = GetNode<AnimatedSprite2D>("BodySprite");
		_bodySprite.Play();
		_legsSprite.Play();

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

		// don't go outside
		Vector2 pos = GlobalPosition;
		pos.X = Mathf.Clamp(pos.X, _limitLeft, _limitRight);
		pos.Y = Mathf.Clamp(pos.Y, _limitTop, _limitBottom);
		GlobalPosition = pos;
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

			if (tileId != -1 && _inventory.GetSelectedSlot().Item != null && _inventory.GetSelectedSlot().Item.Type == ItemType.Tool)
			{ // break
				EmitSignal(SignalName.BreakBlock, tilePos);
			}
			else if (tileId == -1 && _inventory.GetSelectedSlot().Item != null && _inventory.GetSelectedSlot().Item.Type == ItemType.Placeable && _inventory.GetSelectedSlot().Amount != 0)
			{ // place
				EmitSignal(SignalName.PlaceBlock, tilePos, _inventory.GetSelectedSlot().Item.TileId);
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
