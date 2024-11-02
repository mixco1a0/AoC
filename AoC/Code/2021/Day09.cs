using System.Collections.Generic;
using System.Linq;

using AoC.Core;

namespace AoC._2021
{
    class Day09 : Day
    {
        public Day09() { }

        public override string GetSolutionVersion(Part part)
        {
            switch (part)
            {
                case Part.One:
                    return "v2";
                case Part.Two:
                    return "v2";
                default:
                    return base.GetSolutionVersion(part);
            }
        }

        public override bool SkipTestData => true;

        protected override List<TestDatum> GetTestData()
        {
            List<TestDatum> testData = new List<TestDatum>();
            testData.Add(new TestDatum
            {
                TestPart = Part.One,
                Output = "15",
                RawInput =
@"2199943210
3987894921
9856789892
8767896789
9899965678"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.Two,
                Output = "1134",
                RawInput =
@"2199943210
3987894921
9856789892
8767896789
9899965678"
            });
            return testData;
        }

        private List<Base.Vec2> Movements = new List<Base.Vec2>
        {
            new Base.Vec2(-1, 0),
            new Base.Vec2(1, 0),
            new Base.Vec2(0, 1),
            new Base.Vec2(0, -1),
        };

        private bool IsLowPoint(int[,] grid, int maxX, int maxY, int x, int y)
        {
            Base.Vec2 cur = new Base.Vec2(x, y);
            foreach (Base.Vec2 movement in Movements)
            {
                Base.Vec2 neighbor = cur + movement;
                if (neighbor.X < 0 || neighbor.X >= maxX || neighbor.Y < 0 || neighbor.Y >= maxY)
                {
                    continue;
                }
                if (grid[cur.X, cur.Y] >= grid[neighbor.X, neighbor.Y])
                {
                    return false;
                }
            }
            return true;
        }

        private int FindBasin(int[,] grid, int maxX, int maxY, int x, int y)
        {
            int basinSize = 0;
            HashSet<Base.Vec2> history = new HashSet<Base.Vec2>();
            Queue<Base.Vec2> toCheck = new Queue<Base.Vec2>();
            toCheck.Enqueue(new Base.Vec2(x, y));
            while (toCheck.Count() > 0)
            {
                Base.Vec2 coords = toCheck.Dequeue();
                if (coords.X < 0 || coords.X >= maxX || coords.Y < 0 || coords.Y >= maxY)
                {
                    continue;
                }
                if (grid[coords.X, coords.Y] == 9)
                {
                    continue;
                }
                if (history.Contains(coords))
                {
                    continue;
                }
                ++basinSize;
                history.Add(coords);

                foreach (Base.Vec2 movement in Movements)
                {
                    toCheck.Enqueue(coords + movement);
                }
            }

            return basinSize;
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool checkBasin)
        {
            int maxX = inputs.First().Count();
            int maxY = inputs.Count();
            int[,] grid = new int[maxX, maxY];
            for (int y = 0; y < maxY; ++y)
            {
                for (int x = 0; x < maxX; ++x)
                {
                    grid[x, y] = inputs[y][x] - '0';
                }
            }
            List<int> lowPoints = new List<int>();
            List<int> basins = new List<int>();
            for (int y = 0; y < maxY; ++y)
            {
                for (int x = 0; x < maxX; ++x)
                {
                    if (IsLowPoint(grid, maxX, maxY, x, y))
                    {
                        if (checkBasin)
                        {
                            basins.Add(FindBasin(grid, maxX, maxY, x, y));
                        }
                        else
                        {
                            lowPoints.Add(grid[x, y] + 1);
                        }
                    }
                }
            }
            if (checkBasin)
            {
                int[] biggest = basins.OrderBy(b => b).TakeLast(3).ToArray();
                return (biggest[0] * biggest[1] * biggest[2]).ToString();
            }
            return lowPoints.Sum().ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);
    }
}