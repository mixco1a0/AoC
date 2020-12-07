using System.Collections.Generic;
using System.Linq;

namespace AoC._2020
{
    class Day03 : Day
    {
        public Day03() : base("2020") { }

        protected override string GetDay() { return nameof(Day03).ToLower(); }

        protected override List<TestDatum> GetTestData()
        {
            List<TestDatum> testData = new List<TestDatum>();
            testData.Add(new TestDatum
            {
                TestPart = TestPart.One,
                Output = "7",
                RawInput =
@"..##.......
#...#...#..
.#....#..#.
..#.#...#.#
.#...##..#.
..#.##.....
.#.#.#....#
.#........#
#.##...#...
#...##....#
.#..#...#.#"
            });
            testData.Add(new TestDatum
            {
                TestPart = TestPart.Two,
                Output = "336",
                RawInput =
@"..##.......
#...#...#..
.#....#..#.
..#.#...#.#
.#...##..#.
..#.##.....
.#.#.#....#
.#........#
#.##...#...
#...##....#
.#..#...#.#"
            });
            return testData;
        }

        protected override string RunPart1Solution(List<string> inputs)
        {
            return $"{SlopeCheck(3, 1, inputs)}";
        }

        protected override string RunPart2Solution(List<string> inputs)
        {
            /*
            Right 1, down 1.
            Right 3, down 1. (This is the slope you already checked.)
            Right 5, down 1.
            Right 7, down 1.
            Right 1, down 2.
            */

            long a = SlopeCheck(1, 1, inputs);
            //Debug($"Saw {a} trees.");

            long b = SlopeCheck(3, 1, inputs);
            //Debug($"Saw {b} trees.");

            long c = SlopeCheck(5, 1, inputs);
            //Debug($"Saw {c} trees.");

            long d = SlopeCheck(7, 1, inputs);
            //Debug($"Saw {d} trees.");

            long e = SlopeCheck(1, 2, inputs);
            //Debug($"Saw {e} trees.");

            return $"{a * b * c * d * e}";
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