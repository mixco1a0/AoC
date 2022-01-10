using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AoC._2021
{
    class Day11 : Day
    {
        public Day11() { }

        public override string GetSolutionVersion(Part part)
        {
            switch (part)
            {
                case Part.One:
                    return "v1";
                case Part.Two:
                    return "v1";
                default:
                    return base.GetSolutionVersion(part);
            }
        }

        protected override List<TestDatum> GetTestData()
        {
            List<TestDatum> testData = new List<TestDatum>();
            testData.Add(new TestDatum
            {
                TestPart = Part.One,
                Variables = new Dictionary<string, string>() { { "steps", "10" } },
                Output = "204",
                RawInput =
@"5483143223
2745854711
5264556173
6141336146
6357385478
4167524645
2176841721
6882881134
4846848554
5283751526"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.One,
                Output = "1656",
                RawInput =
@"5483143223
2745854711
5264556173
6141336146
6357385478
4167524645
2176841721
6882881134
4846848554
5283751526"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.Two,
                Output = "195",
                RawInput =
@"5483143223
2745854711
5264556173
6141336146
6357385478
4167524645
2176841721
6882881134
4846848554
5283751526"
            });
            return testData;
        }

        private List<Core.Point> Surrounding = new List<Core.Point>
        {
            new Core.Point(-1, -1),
            new Core.Point(0, -1),
            new Core.Point(1, -1),
            new Core.Point(-1, 0),
            new Core.Point(1, 0),
            new Core.Point(-1, 1),
            new Core.Point(0, 1),
            new Core.Point(1, 1),
        };

        private void PrintGrid(int[,] grid, int maxX, int maxY)
        {
            for (int y = 0; y < maxY; ++y)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("{0,3} | ", y);
                for (int x = 0; x < maxX; ++x)
                {
                    sb.Append(grid[x, y]);
                }
                DebugWriteLine(sb.ToString());
            }
            DebugWriteLine(string.Empty);
        }

        private int Step(ref int[,] grid, int maxX, int maxY)
        {
            HashSet<Core.Point> history = new HashSet<Core.Point>();
            Queue<Core.Point> flash = new Queue<Core.Point>();
            for (int x = 0; x < maxX; ++x)
            {
                for (int y = 0; y < maxY; ++y)
                {
                    ++grid[x, y];
                    if (grid[x, y] > 9)
                    {
                        flash.Enqueue(new Core.Point(x, y));
                    }
                }
            }

            while (flash.Count > 0)
            {
                Core.Point cur = flash.Dequeue();
                if (cur.X < 0 || cur.X >= maxX || cur.Y < 0 || cur.Y >= maxY)
                {
                    continue;
                }
                if (history.Contains(cur))
                {
                    continue;
                }
                if (++grid[cur.X, cur.Y] > 9)
                {
                    history.Add(cur);
                    foreach (Core.Point next in Surrounding)
                    {
                        flash.Enqueue(cur + next);
                    }
                }

            }

            int flashCount = 0;
            foreach (Core.Point cur in history)
            {
                if (grid[cur.X, cur.Y] > 9)
                {
                    grid[cur.X, cur.Y] = 0;
                    ++flashCount;
                }
            }
            return flashCount;
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool findSync)
        {
            int steps;
            Util.GetVariable(nameof(steps), 100, variables, out steps);
            if (findSync)
            {
                steps = int.MaxValue;
            }

            int maxX = inputs.First().Length;
            int maxY = inputs.Count;
            int[,] grid = new int[maxX, maxY];
            int y = 0;
            foreach (string input in inputs)
            {
                int x = 0;
                foreach (char i in input)
                {
                    grid[x++, y] = (int)(i - '0');
                }
                ++y;
            }

            int flashCount = 0;
            for (int i = 1; i <= steps; ++i)
            {
                int curFlashCount = Step(ref grid, maxX, maxY);
                if (findSync && curFlashCount == maxX * maxY)
                {
                    return i.ToString();
                }
                flashCount += curFlashCount;
            }
            return flashCount.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);
    }
}