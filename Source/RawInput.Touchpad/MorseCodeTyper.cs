using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Threading;

namespace RawInput.Touchpad
{
    public class MorseCodeTyper
    {
        private readonly Dictionary<string, char> _morseToChar = new()
        {
            {".-", 'A'}, {"-...", 'B'}, {"-.-.", 'C'}, {"-..", 'D'}, {".", 'E'},
            {"..-.", 'F'}, {"--.", 'G'}, {"....", 'H'}, {"..", 'I'}, {".---", 'J'},
            {"-.-", 'K'}, {".-..", 'L'}, {"--", 'M'}, {"-.", 'N'}, {"---", 'O'},
            {".--.", 'P'}, {"--.-", 'Q'}, {".-.", 'R'}, {"...", 'S'}, {"-", 'T'},
            {"..-", 'U'}, {"...-", 'V'}, {".--", 'W'}, {"-..-", 'X'}, {"-.--", 'Y'},
            {"--..", 'Z'}, {".----", '1'}, {"..---", '2'}, {"...--", '3'}, {"....-", '4'},
            {".....", '5'}, {"-....", '6'}, {"--...", '7'}, {"---..", '8'}, {"----.", '9'},
            {"-----", '0'}, {"--..--", ','}, {".-.-.-", '.'}, {"..--..", '?'}, {"-.-.--", '!'},
            {"-..-.", '/'}, {"-....-", '-'}, {".-..-.", '"'}, {".--.-.", '@'}, {"---...", ':'},
            {"-.-.-.", ';'}, {"-...-", '='}, {".-.-.", '+'}, {"-.--.", '('}, {"-.--.-", ')'},
            {"..--.-", '_'}, {"...-..-", '$'}, {".-...", '&'}, {" ", ' '}
        };

        private readonly DispatcherTimer _tapTimer;
        private readonly DispatcherTimer _sequenceTimer;
        private readonly StringBuilder _currentSequence = new();
        private readonly HashSet<int> _baseContacts = new();
        private readonly Stopwatch _tapStopwatch = new();
        
        private bool _isTapping = false;
        private bool _isEnabled = false;
        private int _lastContactCount = 0;
        
        // Timing constants (in milliseconds)
        private const int DOT_THRESHOLD = 200;  // Tap shorter than this = dot
        private const int DASH_THRESHOLD = 500; // Tap longer than this = dash
        private const int SEQUENCE_TIMEOUT = 1500; // Time to wait before typing character
        
        public event Action<char> CharacterTyped;
        public event Action<string> MorseSequenceUpdated;
        
        public bool IsEnabled 
        { 
            get => _isEnabled; 
            set 
            { 
                _isEnabled = value;
                if (!value) Reset();
            } 
        }

        public MorseCodeTyper()
        { 
           _tapTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(50) };
            _tapTimer.Tick += OnTapTimer;
            
            _sequenceTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(SEQUENCE_TIMEOUT) };
            _sequenceTimer.Tick += OnSequenceTimeout;
        }

        public void ProcessContacts(TouchpadContact[] contacts)
        {
            if (!_isEnabled || contacts == null) return;

            int currentContactCount = contacts.Length;
            
            // Detect when third finger is added (morse tap starts)
            if (currentContactCount == 3 && _lastContactCount == 2 && !_isTapping)
            {
                StartTap();
            }
            // Detect when third finger is lifted (morse tap ends)
            else if (currentContactCount == 2 && _lastContactCount == 3 && _isTapping)
            {
                EndTap();
            }
            // Update base contacts when we have exactly 2 contacts
            else if (currentContactCount == 2 && !_isTapping)
            {
                UpdateBaseContacts(contacts);
            }
            // Reset if all fingers lifted
            else if (currentContactCount == 0)
            {
                Reset();
            }
            
            _lastContactCount = currentContactCount;
        }

        private void StartTap()
        {
            _isTapping = true;
            _tapStopwatch.Restart();
            _tapTimer.Start();
        }

        private void EndTap()
        {
            _isTapping = false;
            _tapTimer.Stop();
            
            long tapDuration = _tapStopwatch.ElapsedMilliseconds;
            
            if (tapDuration < DOT_THRESHOLD)
            {
                _currentSequence.Append('.');
            }
            else if (tapDuration < DASH_THRESHOLD)
            {
                _currentSequence.Append('-');
            }
            else
            {
                // Very long tap = dash
                _currentSequence.Append('-');
            }
            
            MorseSequenceUpdated?.Invoke(_currentSequence.ToString());
            
            // Start sequence timeout timer
            _sequenceTimer.Stop();
            _sequenceTimer.Start();
        }

        private void OnTapTimer(object sender, EventArgs e)
        {
            // This timer runs while tapping to provide real-time feedback
            // Could be used for visual feedback in the future
        }

        private void OnSequenceTimeout(object sender, EventArgs e)
        {
            _sequenceTimer.Stop();
            
            string sequence = _currentSequence.ToString();
            if (!string.IsNullOrEmpty(sequence) && _morseToChar.TryGetValue(sequence, out char character))
            {
                TypeCharacter(character);
                CharacterTyped?.Invoke(character);
            }
            
            _currentSequence.Clear();
            MorseSequenceUpdated?.Invoke("");
        }

        private void UpdateBaseContacts(TouchpadContact[] contacts)
        {
            _baseContacts.Clear();
            foreach (var contact in contacts)
            {
                _baseContacts.Add(contact.ContactId);
            }
        }

        private void Reset()
        {
            _isTapping = false;
            _tapTimer.Stop();
            _sequenceTimer.Stop();
            _currentSequence.Clear();
            _baseContacts.Clear();
            _lastContactCount = 0;
            MorseSequenceUpdated?.Invoke("");
        }

        private void TypeCharacter(char character)
        {
            // Send the character to the currently active window using Win32 API
            SendCharacterGlobally(character);
        }

        #region Win32 API for Global Input

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll")]
        private static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);

        [DllImport("kernel32.dll")]
        private static extern uint GetCurrentThreadId();

        [DllImport("user32.dll")]
        private static extern IntPtr GetFocus();

        [DllImport("user32.dll")]
        private static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern bool SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        private const uint WM_CHAR = 0x0102;
        private const uint WM_KEYDOWN = 0x0100;
        private const uint WM_KEYUP = 0x0101;

        private void SendCharacterGlobally(char character)
        {
            IntPtr foregroundWindow = GetForegroundWindow();
            if (foregroundWindow == IntPtr.Zero) return;

            uint foregroundThreadId = GetWindowThreadProcessId(foregroundWindow, out _);
            uint currentThreadId = GetCurrentThreadId();

            // Attach to the foreground window's thread to get the focused control
            bool attached = false;
            if (foregroundThreadId != currentThreadId)
            {
                attached = AttachThreadInput(currentThreadId, foregroundThreadId, true);
            }

            try
            {
                IntPtr focusedWindow = GetFocus();
                IntPtr targetWindow = focusedWindow != IntPtr.Zero ? focusedWindow : foregroundWindow;

                // Send the character as WM_CHAR message
                PostMessage(targetWindow, WM_CHAR, (IntPtr)character, IntPtr.Zero);
            }
            finally
            {
                if (attached)
                {
                    AttachThreadInput(currentThreadId, foregroundThreadId, false);
                }
            }
        }

        #endregion
    }
}