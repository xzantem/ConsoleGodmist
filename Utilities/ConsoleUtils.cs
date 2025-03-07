using System;
using System.Runtime.InteropServices;

static class DisableConsoleQuickEdit {

    const uint ENABLE_QUICK_EDIT = 0x0040;

    // STD_INPUT_HANDLE (DWORD): -10 is the standard input device.
    const int STD_INPUT_HANDLE = -10;

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern IntPtr GetStdHandle(int nStdHandle);

    [DllImport("kernel32.dll")]
    static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

    [DllImport("kernel32.dll")]
    static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

    internal static bool Go() {

        IntPtr consoleHandle = GetStdHandle(STD_INPUT_HANDLE);

        // get current console mode
        uint consoleMode;
        if (!GetConsoleMode(consoleHandle, out consoleMode)) {
            // ERROR: Unable to get console mode.
            return false;
        }

        // Clear the quick edit bit in the mode flags
        consoleMode &= ~ENABLE_QUICK_EDIT;

        // set the new mode
        if (!SetConsoleMode(consoleHandle, consoleMode)) {
            // ERROR: Unable to set console mode
            return false;
        }

        return true;
    }
}

static class SetFullScreenConsole
{
    [DllImport("kernel32.dll", ExactSpelling = true)]
    private static extern IntPtr GetConsoleWindow();

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    
    [DllImport("user32.dll", SetLastError=true)]
    static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);
    
    [StructLayout(LayoutKind.Sequential)]
    public struct COORD
    {
        public short X;
        public short Y;

        public COORD(short X, short Y)
        {
            this.X = X;
            this.Y = Y;
        }
    };
    
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;        // x position of upper-left corner
        public int Top;         // y position of upper-left corner
        public int Right;       // x position of lower-right corner
        public int Bottom;      // y position of lower-right corner
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct CONSOLE_FONT_INFO_EX
    {
        public uint cbSize;
        public uint nFont;
        public COORD dwFontSize;
        public int FontFamily;
        public int FontWeight;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)] // Edit sizeconst if the font name is too big
        public string FaceName;
    } 
    
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern Int32 SetCurrentConsoleFontEx(IntPtr ConsoleOutput, bool MaximumWindow, ref CONSOLE_FONT_INFO_EX ConsoleCurrentFontEx);
    
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    extern static bool GetCurrentConsoleFontEx(IntPtr hConsoleOutput, bool bMaximumWindow, ref CONSOLE_FONT_INFO_EX lpConsoleCurrentFont);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr GetStdHandle(int nStdHandle);

    private const int SW_MAXIMIZE = 3;
    private const int STD_OUTPUT_HANDLE = -11;
    

    internal static void Go()
    {
        var consoleWindowHandle = GetConsoleWindow();
        var consoleOutputHandle = GetStdHandle(STD_OUTPUT_HANDLE);

        var consoleFontInfo = new CONSOLE_FONT_INFO_EX();
        consoleFontInfo.cbSize = (uint)Marshal.SizeOf(consoleFontInfo);

        GetCurrentConsoleFontEx(consoleOutputHandle, false, ref consoleFontInfo);
        var consoleWindowRect = new RECT();
        ShowWindow(consoleWindowHandle, SW_MAXIMIZE);
        GetWindowRect(consoleWindowHandle, out consoleWindowRect);
        var windowHeight = consoleWindowRect.Top + consoleWindowRect.Bottom;
        consoleFontInfo.dwFontSize.Y = (short)(windowHeight / 40);
        consoleFontInfo.dwFontSize.X = (short)(windowHeight / 80 - 1);

        SetCurrentConsoleFontEx(consoleOutputHandle, false, ref consoleFontInfo);
    }
}   