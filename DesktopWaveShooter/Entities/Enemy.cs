using System;
using System.Drawing;

namespace DesktopWaveShooter.Entities
{
	public sealed class Enemy
	{
		public PointF Position { get; private set; }
		private PointF _velocity;
		private readonly bool _isBoss;

		public Enemy(PointF start, bool isBoss)
		{
			Position = start;
			_isBoss = isBoss;
		}

		public void Update(double dt, RectangleF worldBounds, float groundY, PointF playerPosition)
		{
			float targetDir = Math.Sign(playerPosition.X - Position.X);
			float speed = _isBoss ? 120f : 90f;
			_velocity.X = targetDir * speed;

			const float gravity = 980f;
			_velocity.Y += (float)(gravity * dt);

			Position = new PointF(
				Clamp(Position.X + _velocity.X * (float)dt, worldBounds.Left, worldBounds.Right),
				Position.Y + _velocity.Y * (float)dt
			);

			if (Position.Y >= groundY - (_isBoss ? 16f : 8f))
			{
				Position = new PointF(Position.X, groundY - (_isBoss ? 16f : 8f));
				_velocity.Y = 0f;
			}
		}

		public void Draw(Graphics g)
		{
			var size = _isBoss ? 24f : 12f;
			var rect = new RectangleF(Position.X - size / 2f, Position.Y - size / 2f, size, size);
			Color suit = _isBoss ? Color.FromArgb(255, 220, 60, 60) : Color.FromArgb(255, 50, 50, 50);
			using var brush = new SolidBrush(suit);
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