using System;

namespace AoC.Util
{
    public static class String
    {
        public static string[] Split(string input, string seperator)
        {
            return input.Split(seperator.ToCharArray(), StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        }

        public static string[] Split(string input, char seperator)
        {
            return Split(input, seperator.ToString());
        }
    }
}