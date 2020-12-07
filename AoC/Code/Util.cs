using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
using System.Linq;

namespace AoC
{
    class Util
    {
        static public IEnumerable<string> ConvertInputToList(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return new List<string>();
            }
            return input.Split('\n').Select(str => str.Trim('\r'));
        }
    }

    class MinMax
    {
        public int Min { get; set; }
        public int Max { get; set; }
        public bool GTE_LTE(int val) { return Min <= val && val <= Max; }
        public bool GT_LTE(int val) { return Min < val && val <= Max; }
        public bool GTE_LT(int val) { return Min <= val && val < Max; }
        public bool GT_LT(int val) { return Min < val && val < Max; }
    };
}