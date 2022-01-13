using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace AoC.Core
{
    public static class Logger
    {
        private static readonly int STD_OUTPUT_HANDLE = -11;
        private static readonly int ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;


        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetConsoleMode(IntPtr stdHandle, out int mode);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetConsoleMode(IntPtr stdHandle, int mode);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetStdHandle(int stdHande);

        public enum EPlane
        {
            Foreground = 38,
            Background = 48
        }

        public enum ELogLevel
        {
            All,
            Spam,
            Debug,
            Info,
            Vital,
            Warn,
            Error,
            Fatal,
            Off
        }

        private static IntPtr StdHandle { get; set; }
        public static bool IncludeTimeStamp { get; set; }
        public static bool IncludeLogLevel { get; set; }
        public static ELogLevel LogLevel { get; set; }
        public static bool Enabled { get; set; }
        public static char ColorMarker => '^';
        private static string ColorRegex => string.Concat(@"(\", ColorMarker, @"[^\", ColorMarker, @"]*\", ColorMarker, ")");

        static Logger()
        {
            StdHandle = GetStdHandle(STD_OUTPUT_HANDLE);
            GetConsoleMode(StdHandle, out int mode);
            SetConsoleMode(StdHandle, mode | ENABLE_VIRTUAL_TERMINAL_PROCESSING);

            IncludeTimeStamp = true;
            IncludeLogLevel = true;
            if (System.Diagnostics.Debugger.IsAttached)
            {
                LogLevel = ELogLevel.Debug;
            }
            else
            {
                LogLevel = ELogLevel.Info;
            }
            Enabled = true;
        }

        public static void Write(ELogLevel level, string message)
        {
            Log(Console.Write, level, message);
        }

        public static void Write(ELogLevel level, string format, params object[] args)
        {
            string message = string.Format(format, args);
            Log(Console.Write, level, message);
        }

        public static void Write(ELogLevel level, string message, List<Color> colors)
        {
            LogColorized(Console.Write, level, message, colors);
        }

        public static void WriteAppend(ELogLevel level, string message)
        {
            bool useTimeStamp = IncludeTimeStamp;
            IncludeTimeStamp = false;
            Log(Console.Write, level, message);
            IncludeTimeStamp = useTimeStamp;
        }

        public static void WriteSameLine(ELogLevel level, string message)
        {
            Log(Console.Write, level, $"\r{message}");
        }

        public static void WriteLine(ELogLevel level, string message)
        {
            Log(Console.WriteLine, level, message);
        }

        public static void WriteLine(ELogLevel level, string format, params object[] args)
        {
            string message = string.Format(format, args);
            Log(Console.WriteLine, level, message);
        }

        public static void WriteLine(ELogLevel level, string message, List<Color> colors)
        {
            LogColorized(Console.WriteLine, level, message, colors);
        }

        private static string GetColorFormat(EPlane plane, Color color)
        {
            return GetColorFormat(plane, color.R, color.G, color.B);
        }

        private static string GetColorFormat(EPlane plane, byte r, byte g, byte b)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("\x1b[{0};2;{1};{2};{3}m", (byte)plane, r, g, b);
            return sb.ToString();
        }

        private static void LogColorized(Action<string> logFunc, ELogLevel level, string message, List<Color> colors)
        {
            string[] split = Regex.Split(message, ColorRegex);
            int colorIndex = 0;
            StringBuilder colorizedMessage = new StringBuilder();
            for (int i = 0; i < split.Length; ++i)
            {
                if (split[i][0] == '^')
                {
                    string format = GetColorFormat(EPlane.Foreground, colors[colorIndex++]);
                    colorizedMessage.AppendFormat(string.Format("{0}{1}", format, split[i].Substring(1, split[i].Length - 2)));
                }
                else
                {
                    colorizedMessage.AppendFormat("\x1b[0m{0}", split[i]);
                }
            }
            Log(logFunc, level, colorizedMessage.ToString());
        }

        private static void Log(Action<string> logFunc, ELogLevel level, string message)
        {
            foreach (string subMessage in message.Split("\n"))
            {
                LogSingleMessage(logFunc, level, subMessage);
            }
        }

        private static void LogSingleMessage(Action<string> logFunc, ELogLevel level, string message)
        {
            if (!Enabled)
            {
                return;
            }

            if (level >= LogLevel)
            {
                StringBuilder sb = new StringBuilder();
                if (IncludeTimeStamp)
                {
                    sb.Append(GetLogTimeStamp());
                }
                if (IncludeLogLevel)
                {
                    sb.Append(string.Format("[{0,-5}] ", level.ToString().ToUpper()));
                }
                sb.Append(message);
                logFunc(sb.ToString());
            }
        }

        public static string GetLogTimeStamp()
        {
            return $"|{DateTime.Now.ToString("hh:mm:ss.fff")}| ";
        }
    }
}