using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace DesktopWaveShooter.UI
{
	public sealed class OverlayForm : Form
	{
		private readonly Engine.GameEngine _gameEngine;
		private long _lastTimestamp;

		public OverlayForm()
		{
			Text = "DesktopWaveShooter";
			FormBorderStyle = FormBorderStyle.None;
			ShowInTaskbar = false;
			TopMost = true;
			DoubleBuffered = true;
			SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);

			BackColor = Color.Lime;
			TransparencyKey = Color.Lime;

			Bounds = SystemInformation.VirtualScreen;
			StartPosition = FormStartPosition.Manual;
			Location = new Point(SystemInformation.VirtualScreen.Left, SystemInformation.VirtualScreen.Top);
			Size = new Size(SystemInformation.VirtualScreen.Width, SystemInformation.VirtualScreen.Height);

			_gameEngine = new Engine.GameEngine(this);
		}

		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e);
			ApplyExtendedStyles();
			Input.RawInput.RegisterKeyboard(Handle);
		}

		protected override void OnShown(EventArgs e)
		{
			base.OnShown(e);
			_lastTimestamp = Stopwatch.GetTimestamp();
			Application.Idle += OnApplicationIdle;
		}

		protected override void OnFormClosed(FormClosedEventArgs e)
		{
			Application.Idle -= OnApplicationIdle;
			base.OnFormClosed(e);
		}

		private void OnApplicationIdle(object? sender, EventArgs e)
		{
			while (IsApplicationIdle())
			{
				long now = Stopwatch.GetTimestamp();
				double deltaSeconds = (now - _lastTimestamp) / (double)Stopwatch.Frequency;
				_lastTimestamp = now;
				_gameEngine.Update(deltaSeconds);
				Invalidate();
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			e.Graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
			e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
			e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
			e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
			_gameEngine.Render(e.Graphics);
		}

		protected override void WndProc(ref Message m)
		{
			if (m.Msg == 0x00FF) // WM_INPUT
			{
				Input.RawInput.ProcessMessage(m, _gameEngine.InputState);
			}
			base.WndProc(ref m);
		}

		private void ApplyExtendedStyles()
		{
			int exStyle = GetWindowLong(Handle, GwlExstyle);
			exStyle |= WsExLayered | WsExTransparent | WsExToolwindow;
			SetWindowLong(Handle, GwlExstyle, exStyle);
			// Make black fully transparent using color key to preserve desktop visibility
			SetLayeredWindowAttributes(Handle, 0, 255, 0x02); // LWA_ALPHA
		}

		private static bool IsApplicationIdle()
		{
			NativeMessage msg;
			return !PeekMessage(out msg, IntPtr.Zero, 0, 0, 0);
		}

		private const int GwlExstyle = -20;
		private const int WsExTransparent = 0x00000020;
		private const int WsExLayered = 0x00080000;
		private const int WsExToolwindow = 0x00000080;

		[StructLayout(LayoutKind.Sequential)]
		private struct NativeMessage
		{
			public IntPtr handle;
			public uint msg;
			public IntPtr wParam;
			public IntPtr lParam;
			public uint time;
			public Point p;
		}

		[DllImport("user32.dll")]
		private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

		[DllImport("user32.dll")]
		private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

		[DllImport("user32.dll")]
		private static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

		[DllImport("user32.dll")]
		private static extern bool PeekMessage(out NativeMessage lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax, uint wRemoveMsg);
	}
}