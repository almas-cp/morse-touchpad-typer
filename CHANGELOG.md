# Changelog

All notable changes to the Morse Code Touchpad Typer project will be documented in this file.

## [1.0.0] - 2025-01-09

### Added
- **Global morse code input** using precision touchpad
- **Three-finger input method**: Two anchors + one tapping finger
- **Complete morse code support**: All letters, numbers, and common punctuation
- **System tray integration** for background operation
- **Smart gesture suppression** to prevent text selection interference
- **Real-time feedback** showing current morse sequence
- **Multiple input methods** for maximum compatibility:
  - Virtual key simulation for letters
  - Unicode SendInput for special characters
  - PostMessage fallback for legacy applications
- **Automatic timing detection**:
  - Short taps (< 200ms) = dots
  - Long taps (200-500ms) = dashes
  - 1.5 second timeout for character completion
- **Global operation** across all Windows applications
- **Clean UI** with status indicators and instructions

### Technical Features
- Raw Input API integration for direct touchpad access
- Low-level mouse hooks for gesture suppression
- Global input injection with fallback methods
- Background processing with minimal resource usage
- Proper cleanup and disposal of system resources

### Fixed
- Character typing reliability across different applications
- Gesture interference with morse code input
- System tray functionality and window management
- Memory leaks and proper resource disposal

## [0.1.0] - Initial Development

### Added
- Basic touchpad contact detection
- Morse code sequence recognition
- Simple character mapping
- Initial UI implementation