using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace DesktopWaveShooter.Input
{
	public static class RawInput
	{
		private const int RIDEV_INPUTSINK = 0x00000100;
		private const int RID_INPUT = 0x10000003;
		private const int WM_INPUT = 0x00FF;

		public static void RegisterKeyboard(nint hwnd)
		{
			RAWINPUTDEVICE[] devices = new RAWINPUTDEVICE[1];
			devices[0].usUsagePage = 0x01; // Generic desktop controls
			devices[0].usUsage = 0x06;     // Keyboard
			devices[0].dwFlags = RIDEV_INPUTSINK;
			devices[0].hwndTarget = hwnd;
			if (!RegisterRawInputDevices(devices, (uint)devices.Length, (uint)Marshal.SizeOf<RAWINPUTDEVICE>()))
			{
				throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
			}
		}

		public static void ProcessMessage(Message m, InputState state)
		{
			if (m.Msg != WM_INPUT) return;

			uint dwSize = 0;
			GetRawInputData(m.LParam, RID_INPUT, IntPtr.Zero, ref dwSize, (uint)Marshal.SizeOf<RAWINPUTHEADER>());
			if (dwSize == 0) return;
			IntPtr buffer = Marshal.AllocHGlobal((int)dwSize);
			try
			{
				if (GetRawInputData(m.LParam, RID_INPUT, buffer, ref dwSize, (uint)Marshal.SizeOf<RAWINPUTHEADER>()) != dwSize)
					return;
				RAWINPUT raw = Marshal.PtrToStructure<RAWINPUT>(buffer);
				if (raw.header.dwType != 1) return; // RIM_TYPEKEYBOARD
				HandleKeyboard(raw.data.keyboard, state);
			}
			finally
			{
				Marshal.FreeHGlobal(buffer);
			}
		}

		private static void HandleKeyboard(RAWKEYBOARD kb, InputState state)
		{
			bool isKeyUp = (kb.Flags & 0x01) != 0; // RI_KEY_BREAK
			int vkey = kb.VKey;

			switch (vkey)
			{
				case 0x41: // A
				case 0x25: // Left
					state.LeftHeld = !isKeyUp;
					break;
				case 0x44: // D
				case 0x27: // Right
					state.RightHeld = !isKeyUp;
					break;
				case 0x53: // S
				case 0x28: // Down
					state.DownHeld = !isKeyUp;
					break;
				case 0x20: // Space
					if (!isKeyUp) state.JumpPressed = true;
					break;
				case 0x10: // Shift
					state.ThrowHeld = !isKeyUp;
					break;
				case 0x11: // Ctrl
					state.ShootHeld = !isKeyUp;
					break;
			}
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct RAWINPUTDEVICE
		{
			public ushort usUsagePage;
			public ushort usUsage;
			public int dwFlags;
			public nint hwndTarget;
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct RAWINPUT
		{
			public RAWINPUTHEADER header;
			public RAWINPUTUNION data;
		}

		[StructLayout(LayoutKind.Explicit)]
		private struct RAWINPUTUNION
		{
			[FieldOffset(0)] public RAWMOUSE mouse;
			[FieldOffset(0)] public RAWKEYBOARD keyboard;
			[FieldOffset(0)] public RAWHID hid;
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct RAWINPUTHEADER
		{
			public int dwType;
			public int dwSize;
			public nint hDevice;
			public nint wParam;
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct RAWKEYBOARD
		{
			public ushort MakeCode;
			public ushort Flags;
			public ushort Reserved;
			public ushort VKey;
			public uint Message;
			public uint ExtraInformation;
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct RAWMOUSE
		{
			public ushort usFlags;
			public uint ulButtons;
			public ushort usButtonFlags;
			public ushort usButtonData;
			public uint ulRawButtons;
			public int lLastX;
			public int lLastY;
			public uint ulExtraInformation;
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct RAWHID
		{
			public int dwSizeHid;
			public int dwCount;
			public byte bRawData;
		}

		[DllImport("user32.dll", SetLastError = true)]
		private static extern bool RegisterRawInputDevices(RAWINPUTDEVICE[] pRawInputDevices, uint uiNumDevices, uint cbSize);

		[DllImport("user32.dll", SetLastError = true)]
		private static extern uint GetRawInputData(nint hRawInput, uint uiCommand, IntPtr pData, ref uint pcbSize, uint cbSizeHeader);
	}
}