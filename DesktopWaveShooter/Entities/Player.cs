using System;
using System.Drawing;

namespace DesktopWaveShooter.Entities
{
	public sealed class Player
	{
		public PointF Position { get; private set; }
		private PointF _velocity;
		private bool _isOnGround;
		private bool _canDoubleJump;

		public int Health { get; private set; } = 100;
		public int Ammo { get; private set; } = 30;
		public int MaxAmmo { get; } = 30;

		private const float MoveSpeed = 220f;
		private const float JumpVelocity = -360f;
		private const float Gravity = 980f;
		private const float MaxFallSpeed = 800f;
		private const float Friction = 0.0001f;

		public Player(PointF start)
		{
			Position = start;
			_isOnGround = false;
			_canDoubleJump = true;
		}

		public void Move(float moveAxis, double dt)
		{
			_velocity.X = (float)(moveAxis * MoveSpeed);
		}

		public void TryJump()
		{
			if (_isOnGround)
			{
				_velocity.Y = JumpVelocity;
				_isOnGround = false;
				_canDoubleJump = true;
			}
			else if (_canDoubleJump)
			{
				_velocity.Y = JumpVelocity * 0.9f;
				_canDoubleJump = false;
			}
		}

		public void Update(double dt, RectangleF worldBounds, float groundY)
		{
			_velocity.Y += (float)(Gravity * dt);
			if (_velocity.Y > MaxFallSpeed) _velocity.Y = MaxFallSpeed;

			Position = new PointF(
				Clamp(Position.X + _velocity.X * (float)dt, worldBounds.Left, worldBounds.Right),
				Position.Y + _velocity.Y * (float)dt
			);

			if (Position.Y >= groundY - 8f)
			{
				Position = new PointF(Position.X, groundY - 8f);
				_velocity.Y = 0f;
				_isOnGround = true;
			}
			else
			{
				_isOnGround = false;
			}
		}

		public void Draw(Graphics g)
		{
			var rect = new RectangleF(Position.X - 6, Position.Y - 6, 12, 12);
			using var brush = new SolidBrush(Color.FromArgb(255, 80, 200, 255));
			g.FillRectangle(brush, rect);
		}

		private static float Clamp(float v, float min, float max)
		{
			if (v < min) return min;
			if (v > max) return max;
			return v;
		}
	}
}