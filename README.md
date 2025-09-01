# 🔤 Morse Code Touchpad Typer

A Windows application that enables global morse code input using your precision touchpad. Type morse code anywhere in the system using a simple three-finger technique.

![License](https://img.shields.io/badge/license-MIT-blue.svg)
![Platform](https://img.shields.io/badge/platform-Windows-lightgrey.svg)
![.NET](https://img.shields.io/badge/.NET-5.0--windows-purple.svg)

## ✨ Features

- **🌐 Global Operation**: Works in any text field across all Windows applications
- **👆 Intuitive Input**: Simple three-finger morse code technique
- **🎯 Smart Gesture Management**: Automatically suppresses interfering touchpad gestures
- **🔄 System Tray Integration**: Runs quietly in the background
- **⚡ Real-time Feedback**: Live morse sequence display and character preview
- **🛡️ Reliable Input**: Multiple input methods ensure compatibility across applications

## 🚀 How It Works

### Input Method
1. **Place two fingers** on your touchpad as "anchor" points
2. **Tap with a third finger** to create morse code:
   - **Short tap** (< 200ms) = **Dot** (.)
   - **Long tap** (200-500ms) = **Dash** (-)
3. **Wait 1.5 seconds** after completing a sequence
4. **Character appears** automatically in the active text field

### Example
- **A**: Tap short, then long (. -)
- **Q**: Tap long, long, short, long (- - . -)
- **SOS**: Short-short-short, long-long-long, short-short-short (... --- ...)

## 📋 Requirements

- **Windows 10/11** with Precision Touchpad
- **.NET 5.0 Runtime** or later
- **Administrator privileges** (recommended for maximum compatibility)

## 🔧 Installation

### Option 1: Download Release
1. Download the latest release from [Releases](../../releases)
2. Extract the ZIP file
3. Run `RawInput.Touchpad.exe`

### Option 2: Build from Source
```bash
git clone https://github.com/yourusername/morse-touchpad-typer.git
cd morse-touchpad-typer
dotnet build Source/RawInput.Touchpad/RawInput.Touchpad.csproj
```

## 🎮 Usage

### Getting Started
1. **Launch the application** - it starts minimized in the system tray
2. **Enable morse typer** via the checkbox or system tray menu
3. **Open any text application** (Notepad, browser, Word, etc.)
4. **Start typing morse code** using the three-finger technique

### System Tray Controls
- **Right-click tray icon** for quick menu
- **Double-click tray icon** to show/hide main window
- **Toggle morse typer** on/off
- **Exit application** cleanly

### Visual Feedback
- **Current sequence** shows your morse input in real-time
- **Typed character** displays what was just entered
- **Status messages** keep you informed of the app state

## 📖 Morse Code Reference

### Letters A-Z
```
A .-    B -...  C -.-.  D -..   E .     F ..-. 
G --.   H ....  I ..    J .---  K -.-   L .-.. 
M --    N -.    O ---   P .--.  Q --.-  R .-. 
S ...   T -     U ..-   V ...-  W .--   X -..- 
Y -.--  Z --..
```

### Numbers 0-9
```
0 -----  1 .----  2 ..---  3 ...--  4 ....- 
5 .....  6 -....  7 --...  8 ---..  9 ----.
```

### Common Punctuation
```
. .-.-.-    , --..--    ? ..--..    ! -.-.-- 
/ -..-      - -....-    " .-..-.    @ .--.-. 
: ---...    ; -.-.-.    = -...-     + .-.-. 
( -.--.     ) -.--.-    _ ..--.-    $ ...-..- 
& .-...
```

## ⚠️ Important Notes

### Gesture Suppression
When the morse typer is enabled, **double-click and triple-click text selection gestures are automatically disabled** to prevent interference with morse input. This is restored when you disable the morse typer.

### Compatibility
- Works with **all Windows applications** that accept text input
- Tested with: Notepad, Word, browsers, IDEs, chat applications
- May require **administrator privileges** for some protected applications

## 🛠️ Technical Details

### Architecture
- **Raw Input API**: Direct touchpad contact detection
- **Global Input Injection**: Multiple methods for reliable character typing
- **Low-level Hooks**: Smart gesture suppression without breaking other functionality
- **System Tray Integration**: Background operation with minimal resource usage

### Input Methods (Fallback Chain)
1. **Virtual Key Simulation**: Direct key press simulation for letters
2. **Unicode SendInput**: For special characters and symbols  
3. **PostMessage**: Legacy compatibility fallback

## 🐛 Troubleshooting

### Common Issues
- **Characters not appearing**: Try running as administrator
- **Touchpad not detected**: Ensure you have a Windows Precision Touchpad
- **Text selection still active**: Disable and re-enable the morse typer
- **App not responding**: Check system tray - app may be minimized

### Debug Steps
1. Verify precision touchpad exists (shown in main window)
2. Check that morse typer is enabled
3. Test with simple applications like Notepad first
4. Ensure no other touchpad software is interfering

## 🤝 Contributing

Contributions are welcome! Please feel free to submit issues, feature requests, or pull requests.

### Development Setup
```bash
git clone https://github.com/yourusername/morse-touchpad-typer.git
cd morse-touchpad-typer
dotnet restore
dotnet build
```

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- Built on the Windows Raw Input API for touchpad detection
- Inspired by the need for accessible alternative input methods
- Thanks to the morse code community for keeping this communication method alive

## 📞 Support

- **Issues**: [GitHub Issues](../../issues)
- **Discussions**: [GitHub Discussions](../../discussions)
- **Email**: [your-email@example.com]

---

**Made with ❤️ for the accessibility and morse code communities**