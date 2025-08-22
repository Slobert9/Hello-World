# DesktopWaveShooter (Windows Overlay)

A minimal Windows overlay prototype using WinForms with a transparent, click-through window. Shows a pixel player, basic enemies, a wave counter, and a tiny HUD. Intended to run as a desktop overlay without blocking mouse/keyboard to other apps.

## Requirements
- Windows 10/11
- .NET 8 SDK (x64)

## Build
```powershell
# In PowerShell on Windows
$env:DOTNET_EnableWindowsTargeting=1

dotnet build .\DesktopWaveShooter.csproj -c Release
```

## Run
```powershell
# Launch the overlay
./bin/Release/net8.0-windows/DesktopWaveShooter.exe
```

If the window does not appear full-screen across monitors, press Alt+F4 to exit. The overlay uses:
- Transparent color key with click-through (`WS_EX_TRANSPARENT | WS_EX_LAYERED | WS_EX_TOOLWINDOW`)
- Raw Input keyboard sink so the game can read keys without focus

## Controls
- A / Left: Move left
- D / Right: Move right
- S / Down: Crouch (placeholder)
- Space: Jump / Double-jump
- Ctrl: Shoot (placeholder)
- Shift: Throw (placeholder)

## Notes
- This is a prototype using GDI+ for simplicity. For a production build, consider DirectX/Direct2D/MonoGame for performance and sprite batching.
- Audio is not integrated yet; FMOD/OpenAL/XAudio2 are recommended.
- Boss spawns every 3rd wave. Visuals are placeholder rectangles; replace with pixel art sprites.
- The overlay is click-through to preserve desktop interactivity.