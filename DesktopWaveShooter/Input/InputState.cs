using System;

namespace DesktopWaveShooter.Input
{
	public sealed class InputState
	{
		public bool LeftHeld { get; set; }
		public bool RightHeld { get; set; }
		public bool DownHeld { get; set; }
		public bool ShootHeld { get; set; }
		public bool ThrowHeld { get; set; }

		public bool JumpPressed { get; set; }

		public void ResetTransient()
		{
			JumpPressed = false;
		}
	}
}