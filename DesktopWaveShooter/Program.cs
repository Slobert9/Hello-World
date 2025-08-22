using System;
using System.Windows.Forms;

namespace DesktopWaveShooter
{
	internal static class Program
	{
		[STAThread]
		private static void Main()
		{
			ApplicationConfiguration.Initialize();
			using var overlay = new UI.OverlayForm();
			Application.Run(overlay);
		}
	}
}