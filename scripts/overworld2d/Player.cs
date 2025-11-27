using Godot;
using System;

namespace Overworld2d
{

    public partial class Player : CharacterBody2D
    {
        [Export]
        public float Speed = 200f; // Movement speed in pixels/sec

        public override void _PhysicsProcess(double delta)
        {
            Vector2 direction = Vector2.Zero;

            if (Input.IsActionPressed("MoveUp"))
                direction.Y -= 1;
            if (Input.IsActionPressed("MoveDown"))
                direction.Y += 1;
            if (Input.IsActionPressed("MoveLeft"))
                direction.X -= 1;
            if (Input.IsActionPressed("MoveRight"))
                direction.X += 1;

            // Normalize to prevent diagonal speed boost
            if (direction != Vector2.Zero)
                direction = direction.Normalized();

            Velocity = direction * Speed;
            MoveAndSlide();
        }
    }
}