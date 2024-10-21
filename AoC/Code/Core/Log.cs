using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace AoC.Core
{

    public static class TempLog
    {
        public static Action<string> WriteLine { get; set; }
        public static Action<string> WriteFile { get; set; }
        static TempLog()
        {
            Reset();
        }
        public static void Reset()
        {
            WriteLine = (_) => { };
            WriteFile = (_) => { };
        }
    }

    public static class Log
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

        public enum ELevel
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
        public static ELevel LogLevel { get; set; }
        public static bool Enabled { get; set; }
        public static char ColorMarker => '^';
        private static string ColorRegex => string.Concat(@"(\", ColorMarker, @"[^\", ColorMarker, @"]*\", ColorMarker, ")");

        public static Color Positive => Color.MediumSeaGreen;
        public static Color Neutral => Color.Khaki;
        public static Color Negative => Color.Salmon;

        static Log()
        {
            StdHandle = GetStdHandle(STD_OUTPUT_HANDLE);
            GetConsoleMode(StdHandle, out int mode);
            SetConsoleMode(StdHandle, mode | ENABLE_VIRTUAL_TERMINAL_PROCESSING);

            IncludeTimeStamp = true;
            IncludeLogLevel = true;
            if (System.Diagnostics.Debugger.IsAttached)
            {
                LogLevel = ELevel.Debug;
            }
            else
            {
                LogLevel = ELevel.Info;
            }
            Enabled = true;
        }

        public static void Write(ELevel level, string message)
        {
            InternalLog(Console.Write, level, message);
        }

        public static void Write(ELevel level, string format, params object[] args)
        {
            string message = string.Format(format, args);
            InternalLog(Console.Write, level, message);
        }

        public static void Write(ELevel level, string message, List<Color> colors)
        {
            InternalLogColorized(Console.Write, level, message, colors);
        }

        public static void WriteAppend(ELevel level, string message)
        {
            bool useLogLevel = IncludeLogLevel;
            IncludeLogLevel = false;
            bool useTimeStamp = IncludeTimeStamp;
            IncludeTimeStamp = false;
            InternalLog(Console.Write, level, message);
            IncludeLogLevel = useLogLevel;
            IncludeTimeStamp = useTimeStamp;
        }

        public static void WriteAppend(ELevel level, string message, List<Color> colors)
        {
            bool useLogLevel = IncludeLogLevel;
            IncludeLogLevel = false;
            bool useTimeStamp = IncludeTimeStamp;
            IncludeTimeStamp = false;
            InternalLogColorized(Console.Write, level, message, colors);
            IncludeLogLevel = useLogLevel;
            IncludeTimeStamp = useTimeStamp;
        }

        public static void WriteAppendEnd(ELevel level)
        {
            bool useLogLevel = IncludeLogLevel;
            IncludeLogLevel = false;
            bool useTimeStamp = IncludeTimeStamp;
            IncludeTimeStamp = false;
            InternalLogSingleMessage(Console.Write, level, Environment.NewLine, false);
            IncludeLogLevel = useLogLevel;
            IncludeTimeStamp = useTimeStamp;
        }

        public static void WriteSameLine(ELevel level, string message)
        {
            InternalLogSingleMessage(Console.Write, level, message, true);
        }

        public static void WriteLine(ELevel level, string message)
        {
            InternalLog(Console.WriteLine, level, message);
        }

        public static void WriteLine(ELevel level, string format, params object[] args)
        {
            string message = string.Format(format, args);
            InternalLog(Console.WriteLine, level, message);
        }

        public static void WriteLine(ELevel level, string message, List<Color> colors)
        {
            InternalLogColorized(Console.WriteLine, level, message, colors);
        }

        public static void WriteException(Exception e)
        {
            List<Color> exceptionColor = new List<Color>() { Negative };
            WriteLine(ELevel.Fatal, $"{Core.Log.ColorMarker}{e.Message}{Core.Log.ColorMarker}", exceptionColor);
            foreach (string st in e.StackTrace.Split("\n"))
            {
                WriteLine(ELevel.Fatal, $"{Core.Log.ColorMarker}{st}{Core.Log.ColorMarker}", exceptionColor);
            }
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

        private static void InternalLogColorized(Action<string> logFunc, ELevel level, string message, List<Color> colors)
        {
            string[] split = Regex.Split(message, ColorRegex);
            int colorIndex = 0;
            StringBuilder colorizedMessage = new StringBuilder();
            for (int i = 0; i < split.Length; ++i)
            {
                if (!string.IsNullOrWhiteSpace(split[i]) && split[i][0] == ColorMarker)
                {
                    string format = GetColorFormat(EPlane.Foreground, colors[colorIndex++]);
                    colorizedMessage.AppendFormat(string.Format("{0}{1}", format, split[i].Substring(1, split[i].Length - 2)).Replace('{', '[').Replace('}', ']'));
                }
                else
                {
                    colorizedMessage.AppendFormat("\x1b[0m{0}", split[i].Replace('{', '[').Replace('}', ']'));
                }
            }
            InternalLog(logFunc, level, colorizedMessage.ToString());
        }

        private static void InternalLog(Action<string> logFunc, ELevel level, string message)
        {
            foreach (string subMessage in message.Split("\n"))
            {
                InternalLogSingleMessage(logFunc, level, subMessage, false);
            }
        }

        private static void InternalLogSingleMessage(Action<string> logFunc, ELevel level, string message, bool sameLine)
        {
            if (!Enabled)
            {
                return;
            }

            if (level >= LogLevel)
            {
                StringBuilder sb = new StringBuilder();
                if (sameLine)
                {
                    sb.Append('\r');
                }
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