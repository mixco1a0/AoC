using System.Collections.Generic;
using System.Linq;

namespace _2020
{
    class Day03 : Day
    {
        public Day03() : base() {}

        protected override string GetDay() { return nameof(Day03).ToLower(); }

        protected override void RunPart1Solution(List<string> inputs)
        {
            LogAnswer($"{SlopeCheck(3, 1, inputs)} trees");
        }

        protected override void RunPart2Solution(List<string> inputs)
        {
            /*
            Right 1, down 1.
            Right 3, down 1. (This is the slope you already checked.)
            Right 5, down 1.
            Right 7, down 1.
            Right 1, down 2.
            */

            long a = SlopeCheck(1, 1, inputs);
            Debug($"Saw {a} trees.");

            long b = SlopeCheck(3, 1, inputs);
            Debug($"Saw {b} trees.");

            long c = SlopeCheck(5, 1, inputs);
            Debug($"Saw {c} trees.");

            long d = SlopeCheck(7, 1, inputs);
            Debug($"Saw {d} trees.");

            long e = SlopeCheck(1, 2, inputs);
            Debug($"Saw {e} trees.");

            LogAnswer($"{a*b*c*d*e} trees");
        }

        private int SlopeCheck(int rightStep, int downStep, List<string> inputs)
        {
            int curIdx = 0;
            int treeCount = 0;
            for (int i = downStep; i < inputs.Count; i += downStep)
            {
                curIdx = (curIdx + rightStep) % inputs[i].Length;
                if (inputs[i].ElementAt(curIdx) == '#')
                {
                    ++treeCount;
                }
            }
            return treeCount;
        }
    }
}