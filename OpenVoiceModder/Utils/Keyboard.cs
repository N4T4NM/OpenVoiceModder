using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OpenVoiceModder.Utils
{
    public static class Keyboard
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_SYSKEYDOWN = 0x0104;

        private static LowLevelKeyboardProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;

        public delegate void GlobalKeyEvent(Key key, bool invokeAlternate);
        public static event GlobalKeyEvent? GlobalKey;

        public static void HookKeyboard() => _hookID = SetHook(_proc);
        public static void RemoveHook() => UnhookWindowsHookEx(_hookID);

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule!)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                    GetModuleHandle(curModule!.ModuleName!), 0);
            }
        }

        private delegate IntPtr LowLevelKeyboardProc(
            int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr HookCallback(
            int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN))
            {
                int vkCode = Marshal.ReadInt32(lParam);
                Key key = KeyInterop.KeyFromVirtualKey(vkCode);
                GlobalKey?.Invoke(key, IsKeyDown(Key.LeftAlt));
            }

            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook,
            LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
            IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("USER32.dll")]
        static extern short GetKeyState(byte nVirtKey);

        [DllImport("user32.dll", SetLastError = true)]
        static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        const int KEY_DOWN_EVENT = 0x0001; //Key down flag
        const int KEY_UP_EVENT = 0x0002; //Key up flag

        readonly static Dictionary<byte, bool> _states = new();

        public static bool IsKeyDown(Key key)
        {
            byte vk = (byte)KeyInterop.VirtualKeyFromKey(key);
            return (GetKeyState(vk) & 0x8000) != 0;
        }

        public static void HoldKey(Key key)
        {
            byte vk = (byte)KeyInterop.VirtualKeyFromKey(key);

            if (!_states.ContainsKey(vk))
                _states.Add(vk, false);

            new Task(() =>
            {
                _states[vk] = true;
                while (_states[vk])
                {
                    if (!IsKeyDown(key))
                        keybd_event(vk, vk, KEY_DOWN_EVENT, 0);
                }
            }).Start();
        }
        public static void ReleaseKey(Key key)
        {
            byte vk = (byte)KeyInterop.VirtualKeyFromKey(key);

            _states[vk] = false;
            keybd_event(vk, vk, KEY_UP_EVENT, 0);
        }
    }
}
