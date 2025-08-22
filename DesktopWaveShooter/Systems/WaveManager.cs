using System;
using System.Collections.Generic;
using System.Drawing;
using DesktopWaveShooter.Entities;

namespace DesktopWaveShooter.Systems
{
	public sealed class WaveManager
	{
		private readonly List<Enemy> _enemies;
		private readonly RectangleF _worldBounds;
		private readonly float _groundY;
		private readonly Random _random = new();
		private double _spawnCooldown;

		public int CurrentWaveIndex { get; private set; } = 1;

		public WaveManager(List<Enemy> enemies, RectangleF worldBounds, float groundY)
		{
			_enemies = enemies;
			_worldBounds = worldBounds;
			_groundY = groundY;
			QueueNextWave();
		}

		public void Update(double dt, PointF playerPosition)
		{
			_enemies.RemoveAll(e => e == null);
			if (_enemies.Count == 0)
			{
				CurrentWaveIndex++;
				QueueNextWave();
			}

			_spawnCooldown -= dt;
			if (_spawnCooldown <= 0)
			{
				SpawnOne();
				_spawnCooldown = 0.7 + Math.Max(0.0, 2.5 - CurrentWaveIndex * 0.2);
			}
		}

		private void QueueNextWave()
		{
			int baseCount = Math.Min(6 + CurrentWaveIndex, 20);
			bool boss = CurrentWaveIndex % 3 == 0;
			for (int i = 0; i < baseCount; i++)
			{
				SpawnOne(false);
			}
			if (boss)
			{
				SpawnOne(true);
			}
		}

		private void SpawnOne(bool boss = false)
		{
			bool fromLeft = _random.Next(2) == 0;
			float x = fromLeft ? _worldBounds.Left + 10f : _worldBounds.Right - 10f;
			float y = _groundY - (boss ? 16f : 8f);
			_enemies.Add(new Enemy(new PointF(x, y), boss));
		}
	}
}