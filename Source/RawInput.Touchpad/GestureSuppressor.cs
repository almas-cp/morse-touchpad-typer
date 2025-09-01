using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace RawInput.Touchpad
{
    public class GestureSuppressor
    {
        private LowLevelMouseProc _proc = HookCallback;
        private IntPtr _hookID = IntPtr.Zero;
        private static bool _isEnabled = false;
        private static DateTime _lastClickTime = DateTime.MinValue;
        private static int _clickCount = 0;
        private static IntPtr _staticHookID = IntPtr.Zero;
        private static readonly TimeSpan DOUBLE_CLICK_TIME = TimeSpan.FromMilliseconds(500);

        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
                if (value)
                    StartHook();
                else
                    StopHook();
            }
        }

        public GestureSuppressor()
        {
            _proc = HookCallback;
        }

        private void StartHook()
        {
            if (_staticHookID == IntPtr.Zero)
            {
                _staticHookID = SetHook(_proc);
                _hookID = _staticHookID;
            }
        }

        private void StopHook()
        {
            if (_staticHookID != IntPtr.Zero)
            {
                UnhookWindowsHookEx(_staticHookID);
                _staticHookID = IntPtr.Zero;
                _hookID = IntPtr.Zero;
            }
        }

        private static IntPtr SetHook(LowLevelMouseProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_MOUSE_LL, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && _isEnabled)
            {
                // Check for left mouse button down (which includes touchpad taps)
                if (wParam == (IntPtr)WM_LBUTTONDOWN)
                {
                    DateTime currentTime = DateTime.Now;
                    
                    // Check if this is a potential double/triple click
                    if (currentTime - _lastClickTime <= DOUBLE_CLICK_TIME)
                    {
                        _clickCount++;
                        
                        // Block double clicks and beyond
                        if (_clickCount >= 2)
                        {
                            // Suppress this click event
                            return (IntPtr)1; // Non-zero return value suppresses the event
                        }
                    }
                    else
                    {
                        _clickCount = 1;
                    }
                    
                    _lastClickTime = currentTime;
                }
                // Also suppress double-click messages directly
                else if (wParam == (IntPtr)WM_LBUTTONDBLCLK)
                {
                    return (IntPtr)1; // Suppress double-click
                }
            }

            return CallNextHookEx(_staticHookID, nCode, wParam, lParam);
        }

        public void Dispose()
        {
            StopHook();
        }

        #region Win32 API

        private const int WH_MOUSE_LL = 14;
        private const int WM_LBUTTONDOWN = 0x0201;
        private const int WM_LBUTTONDBLCLK = 0x0203;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook,
            LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
            IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        #endregion
    }
}