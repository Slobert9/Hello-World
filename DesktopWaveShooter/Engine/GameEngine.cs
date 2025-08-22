using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DesktopWaveShooter.Entities;
using DesktopWaveShooter.Systems;

namespace DesktopWaveShooter.Engine
{
	public sealed class GameEngine
	{
		private readonly Control _hostControl;
		private readonly List<Enemy> _enemies = new();
		private readonly WaveManager _waveManager;
		private readonly HudRenderer _hudRenderer;
		private readonly Random _random = new();

		public Input.InputState InputState { get; } = new();
		public Player Player { get; }
		public int CurrentWave => _waveManager.CurrentWaveIndex;

		public RectangleF WorldBounds { get; }
		public float GroundY { get; }

		public GameEngine(Control hostControl)
		{
			_hostControl = hostControl;
			WorldBounds = new RectangleF(0, 0, hostControl.Width, hostControl.Height);
			GroundY = hostControl.Bottom - 80f;
			Player = new Player(new PointF(hostControl.Width * 0.5f, GroundY - 8f));
			_waveManager = new WaveManager(_enemies, WorldBounds, GroundY);
			_hudRenderer = new HudRenderer(this);
		}

		public void Update(double deltaSeconds)
		{
			HandlePlayerInput(deltaSeconds);
			Player.Update(deltaSeconds, WorldBounds, GroundY);

			_waveManager.Update(deltaSeconds, Player.Position);
			foreach (var enemy in _enemies)
				enemy.Update(deltaSeconds, WorldBounds, GroundY, Player.Position);
		}

		private void HandlePlayerInput(double dt)
		{
			float move = 0f;
			if (InputState.LeftHeld) move -= 1f;
			if (InputState.RightHeld) move += 1f;
			Player.Move(move, dt);
			if (InputState.JumpPressed)
			{
				Player.TryJump();
				InputState.JumpPressed = false;
			}
		}

		public void Render(Graphics g)
		{
			// Clear with fully transparent color key background
			using var clearBrush = new SolidBrush(Color.FromArgb(0, 0, 0, 0));
			g.FillRectangle(clearBrush, WorldBounds);

			// Draw ground hint line (subtle)
			using (var pen = new Pen(Color.FromArgb(80, 255, 255, 255), 1))
				g.DrawLine(pen, 0, GroundY, WorldBounds.Width, GroundY);

			Player.Draw(g);
			foreach (var enemy in _enemies)
				enemy.Draw(g);

			_hudRenderer.Draw(g, _hostControl.ClientRectangle);
		}
	}
}