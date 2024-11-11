using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Application = System.Windows.Forms.Application;

namespace WindowFocusTerminal
{
    internal class Program
    {
        // Import user32.dll methods for handling windows
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll")]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll")]
        private static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        [DllImport("kernel32.dll")]
        private static extern uint GetCurrentThreadId();

        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;

        private static LowLevelKeyboardProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;

        private const Keys targetKey = Keys.LShiftKey;
        private static readonly string targetProcessName = "WindowsTerminal";
        private static readonly int doubleTapInterval = 300;
        private static Stopwatch timer = new Stopwatch();
        private static bool firstTap = false;

        public static void Main()
        {
            _hookID = SetHook(_proc);
            Application.Run();
            UnhookWindowsHookEx(_hookID);
        }

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                if ((Keys)vkCode == targetKey)
                {
                    OnTargetKeyPressed();
                }
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        private static void OnTargetKeyPressed()
        {
            if (firstTap && timer.ElapsedMilliseconds < doubleTapInterval)
            {
                FocusOnTerminalProcess();
                firstTap = false;
                timer.Reset();
            }
            else
            {
                firstTap = true;
                timer.Restart();
            }
        }

        private static void FocusOnTerminalProcess()
        {
            Process[] processes = Process.GetProcessesByName(targetProcessName);
            if (processes.Length > 0)
            {
                IntPtr hWnd = processes[0].MainWindowHandle;
                if (hWnd != IntPtr.Zero)
                {
                    uint processId;
                    uint targetThreadId = GetWindowThreadProcessId(hWnd, out processId);
                    uint currentThreadId = GetCurrentThreadId();

                    AttachThreadInput(currentThreadId, targetThreadId, true);

                    // Attempt to bring the window to the foreground with retry
                    bool success = SetForegroundWindow(hWnd);
                    if (!success)
                    {
                        Console.WriteLine("Initial focus attempt failed, retrying...");
                        Thread.Sleep(100); // Wait a bit and retry
                        success = SetForegroundWindow(hWnd);
                    }

                    if (!success)
                    {
                        Console.WriteLine("Failed to focus Windows Terminal after retries.");
                    }
                    else
                    {
                        Console.WriteLine("Windows Terminal focused successfully.");
                    }

                    AttachThreadInput(currentThreadId, targetThreadId, false);
                }
                else
                {
                    Console.WriteLine("Windows Terminal window not found.");
                }
            }
            else
            {
                Console.WriteLine("Windows Terminal is not running.");
            }
        }
    }
}
