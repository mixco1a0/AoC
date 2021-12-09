using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2021
{
    class Day09 : Day
    {
        public Day09() { }

        public override string GetSolutionVersion(Part part)
        {
            switch (part)
            {
                // case Part.One:
                //     return "v1";
                // case Part.Two:
                //     return "v1";
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
                Output = "",
                RawInput =
@""
            });
            return testData;
        }

        private List<Coords> Movements = new List<Coords>
        {
            new Coords(-1, 0),
            new Coords(1, 0),
            new Coords(0, 1),
            new Coords(0, -1),
        };

        private bool IsLowPoint(int[,] grid, int maxX, int maxY, int x, int y)
        {
            Coords cur = new Coords(x, y);
            foreach (Coords movement in Movements)
            {
                Coords neighbor = cur + movement;
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

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables)
        {
            int maxX = inputs.First().Count();
            int maxY = inputs.Count();
            int[,] grid = new int[maxX,maxY];
            for (int y = 0; y < maxY; ++y)
            {
                for (int x = 0; x < maxX; ++x)
                {
                    grid[x,y] = inputs[y][x] - '0';
                }
            }
            List<int> lowPoints = new List<int>();
            for (int y = 0; y < maxY; ++y)
            {
                for (int x = 0; x < maxX; ++x)
                {
                    if (IsLowPoint(grid, maxX, maxY, x, y))
                    {
                        lowPoints.Add(grid[x,y] + 1);
                    }
                }
            }
            return lowPoints.Sum().ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);
    }
}