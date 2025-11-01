using Godot;
using System;

public partial class Player : CharacterBody2D {
	
	public const float Speed = 300.0f;
	public const float JumpVelocity = -400.0f;
	
	public override void _Ready() {
		var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		animatedSprite2D.Play();
	}

	public override void _PhysicsProcess(double delta) {
		Vector2 velocity = Velocity;
		var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

		// Add the gravity.
		if (!IsOnFloor()) {
			velocity += GetGravity() * (float)delta;
		}

		// Handle Jump.
		if (Input.IsActionJustPressed("jump") && IsOnFloor()) {
			velocity.Y = JumpVelocity;
		}

		// Get the input direction and handle the movement/deceleration.
		var direction = Vector2.Zero;
		if (Input.IsActionPressed("right")) {
			direction.X += 1;
		} if (Input.IsActionPressed("left")) {
			direction.X -= 1;
		}
		
		// Handle attack
		if (Input.IsActionPressed("attack")) {
			
		}
		
		// Handle use
		if (Input.IsActionPressed("use")) {
			
		}
		
		if (direction != Vector2.Zero) {
			velocity.X = direction.X * Speed;
			animatedSprite2D.Animation = "walk";
			animatedSprite2D.FlipH = velocity.X < 0;
		} else {
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			animatedSprite2D.Animation = "idle";
		}
		

		Velocity = velocity;
		MoveAndSlide();
	}
}
