# Global Morse Code Touchpad Typer

This application enables you to type morse code using your precision touchpad globally across any text field in Windows.

## How It Works

1. **Two Base Fingers**: Place two fingers on your touchpad and keep them there
2. **Third Finger Tapping**: Use a third finger to tap morse code:
   - **Short tap** (< 200ms) = **Dot** (.)
   - **Long tap** (200-500ms) = **Dash** (-)
3. **Character Output**: After 1.5 seconds of no tapping, the morse sequence is converted to a character and typed into the active text field

## Usage Instructions

1. **Start the Application**: Run `RawInput.Touchpad.exe`
2. **Enable Morse Typer**: Check "Enable Global Morse Code Typer" or use the system tray menu
3. **Minimize to Tray**: The app works globally when minimized to the system tray
4. **Type Anywhere**: Open any text field (Notepad, browser, etc.) and start morse typing

## Morse Code Reference

### Letters
- A: .-    B: -...  C: -.-.  D: -..   E: .
- F: ..-. G: --.   H: ....  I: ..    J: .---
- K: -.-   L: .-..  M: --    N: -.    O: ---
- P: .--.  Q: --.-  R: .-.   S: ...   T: -
- U: ..-   V: ...-  W: .--   X: -..-  Y: -.--
- Z: --..

### Numbers
- 1: .----  2: ..---  3: ...--  4: ....-  5: .....
- 6: -....  7: --...  8: ---..  9: ----.  0: -----

### Common Punctuation
- Period: .-.-.-    Comma: --..--    Question: ..--..
- Exclamation: -.-.--    Space: (pause between words)

## System Tray Features

- **Right-click tray icon** for menu options
- **Double-click tray icon** to show/hide window
- **Enable/Disable** morse typer from tray menu
- **Exit** application from tray menu

## Technical Details

- Works with Windows Precision Touchpads
- Uses global input injection to type in any active text field
- Runs in background with minimal resource usage
- No internet connection required

## Tips

- Practice the timing: short taps for dots, slightly longer for dashes
- Keep the two base fingers steady on the touchpad
- Wait for the character to appear before starting the next one
- Use the main window to see your current morse sequence in real-time

## Troubleshooting

- Ensure you have a Windows Precision Touchpad
- Run as administrator if typing doesn't work in some applications
- Check that "Enable Global Morse Code Typer" is checked
- Make sure the app is running (check system tray)