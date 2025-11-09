using Godot;

public partial class Player : CharacterBody2D
{
	public const float Speed = 300.0f;
	public const float JumpVelocity = -400.0f;

	[Export] private float clickCooldown = 0.1f;

	private double _lastClickTime = 0;

	[Export] private NodePath _worldPath;
	[Export] private float _interactionRange = 10000f;

	private World _world;

	public override void _Ready()
	{
		var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		animatedSprite2D.Play();
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;
		var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		// Handle Jump.
		if (Input.IsActionJustPressed("jump") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
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
			velocity.X = direction.X * Speed;
			animatedSprite2D.Animation = "walk";
			animatedSprite2D.FlipH = velocity.X < 0;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			animatedSprite2D.Animation = "idle";
		}


		Velocity = velocity;
		MoveAndSlide();
	}

	public override void _Process(double delta)
	{
		// Handle use
		if (Input.IsActionPressed("use"))
		{
			if (Time.GetTicksMsec() - _lastClickTime < clickCooldown * 1000) return;
			_lastClickTime = Time.GetTicksMsec();

			Vector2 mouseWorldPos = GetGlobalMousePosition();
			float dist = GlobalPosition.DistanceTo(mouseWorldPos);

			// limit range
			if (dist > _interactionRange)
				return;

			Vector2I tilePos = _world.GetTileMap().LocalToMap(_world.GetTileMap().ToLocal(mouseWorldPos));
			int current = _world.GetTileMap().GetCellSourceId(tilePos);

			if (current != -1)
			{ // break
				_world.GetTileMap().EraseCell(tilePos);
				// var cells = _world.GetNeighborCells(tilePos);
				// _world.GetTileMap().SetCellsTerrainConnect(cells, 0, 1, false);
				_world.UpdateNeighborCells(tilePos);
			}
			else if (current == -1)
			{ // place
				_world.GetTileMap().SetCell(tilePos, 1, Vector2I.Zero);
				//var cells = new Godot.Collections.Array<Vector2I> { tilePos };
				// var cells = _world.GetNeighborCells(tilePos);
				// cells.Add(tilePos);
				// _world.GetTileMap().SetCellsTerrainConnect(cells, 0, 1, false);
				_world.UpdateNeighborCells(tilePos);
			}
		}

		// Handle interact
		if (Input.IsActionPressed("interact"))
		{

		}
	}

	public void SetWorld(World world)
	{
		_world = world;
	}

}
