using System.Drawing;
using DesktopWaveShooter.Engine;

namespace DesktopWaveShooter.Systems
{
	public sealed class HudRenderer
	{
		private readonly GameEngine _engine;

		public HudRenderer(GameEngine engine)
		{
			_engine = engine;
		}

		public void Draw(Graphics g, Rectangle clientRect)
		{
			using var textBrush = new SolidBrush(Color.FromArgb(220, 255, 255, 255));
			using var bgBrush = new SolidBrush(Color.FromArgb(60, 0, 0, 0));
			var font = SystemFonts.MessageBoxFont;

			string leftText = $"HP {_engine.Player.Health}  Ammo {_engine.Player.Ammo}/{_engine.Player.MaxAmmo}";
			var leftSize = g.MeasureString(leftText, font);
			var leftRect = new RectangleF(8, clientRect.Height - leftSize.Height - 8, leftSize.Width + 8, leftSize.Height + 4);
			g.FillRectangle(bgBrush, leftRect);
			g.DrawString(leftText, font, textBrush, new PointF(12, clientRect.Height - leftSize.Height - 6));

			string rightText = $"Wave {_engine.CurrentWave}";
			var rightSize = g.MeasureString(rightText, font);
			var rightRect = new RectangleF(clientRect.Width - rightSize.Width - 16, 8, rightSize.Width + 8, rightSize.Height + 4);
			g.FillRectangle(bgBrush, rightRect);
			g.DrawString(rightText, font, textBrush, new PointF(clientRect.Width - rightSize.Width - 12, 10));
		}
	}
}