using System;

namespace AoC.Util
{
    public static class Log
    {
        public static Action<string> WriteLine { get; set; }
        public static Action<string> WriteFile { get; set; }
        static Log()
        {
            Reset();
        }
        public static void Reset()
        {
            WriteLine = (_) => { };
            WriteFile = (_) => { };
        }
    }
}