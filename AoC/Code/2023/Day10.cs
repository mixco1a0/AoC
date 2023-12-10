using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2023
{
    class Day10 : Core.Day
    {
        public Day10() { }

        public override string GetSolutionVersion(Core.Part part)
        {
            switch (part)
            {
                // case Core.Part.One:
                //     return "v1";
                // case Core.Part.Two:
                //     return "v1";
                default:
                    return base.GetSolutionVersion(part);
            }
        }

        public override bool SkipTestData => false;

        protected override List<Core.TestDatum> GetTestData()
        {
            List<Core.TestDatum> testData = new List<Core.TestDatum>();
            //             testData.Add(new Core.TestDatum
            //             {
            //                 TestPart = Core.Part.One,
            //                 Output = "4",
            //                 RawInput =
            // @"-L|F7
            // 7S-7|
            // L|7||
            // -L-J|
            // L|-JF"
            //             });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.One,
                Output = "8",
                RawInput =
@"..F7.
.FJ|.
SJ.L7
|F--J
LJ..."
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
                Output = "",
                RawInput =
@""
            });
            return testData;
        }

        public Dictionary<char, List<Base.Pos2>> NextDirections = new Dictionary<char, List<Base.Pos2>>()
        {
            {'|', new List<Base.Pos2>() {new Base.Pos2(0, -1), new Base.Pos2(0, 1)}},
            {'-', new List<Base.Pos2>() {new Base.Pos2(-1, 0), new Base.Pos2(1, 0)}},
            {'L', new List<Base.Pos2>() {new Base.Pos2(0, -1), new Base.Pos2(1, 0)}},
            {'J', new List<Base.Pos2>() {new Base.Pos2(-1, 0), new Base.Pos2(0, -1)}},
            {'7', new List<Base.Pos2>() {new Base.Pos2(-1, 0), new Base.Pos2(0, 1)}},
            {'F', new List<Base.Pos2>() {new Base.Pos2(0, 1), new Base.Pos2(1, 0)}},
            {'.', new List<Base.Pos2>() {}},
        };

        public char GetStartingPipe(char[][] grid, int startX, int startY)
        {
            bool hasN = false, hasS = false, hasE = false, hasW = false;
            if (startY > 0)
            {
                char n = grid[startY - 1][startX];
                hasN = NextDirections[n].Contains(new Base.Pos2(0, -1));
            }
            if (startY < grid.Length - 1)
            {
                char s = grid[startY + 1][startX];
                hasS = NextDirections[s].Contains(new Base.Pos2(0, 1));
            }
            if (startY < grid[startY].Length - 1)
            {
                char e = grid[startY][startX + 1];
                hasE = NextDirections[e].Contains(new Base.Pos2(-1, 0));
            }
            if (startX > 0)
            {
                char w = grid[startY][startX - 1];
                hasW = NextDirections[w].Contains(new Base.Pos2(1, 0));
            }
            if (hasN)
            {
                if (hasE)
                {
                    return 'L';
                }
                else if (hasW)
                {
                    return 'J';
                }
                else
                {
                    return '|';
                }
            }
            else if (hasS)
            {
                if (hasE)
                {
                    return 'F';
                }
                else if (hasW)
                {
                    return '7';
                }
            }
            return '-';
        }

        private record Step(Base.Pos2 Pos, int Steps);

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables)
        {
            char[][] grid = inputs.Select(i => i.ToCharArray()).ToArray();
            int startX = 0, startY = 0;
            for (int y = 0; y < grid.Length; ++y)
            {
                for (int x = 0; x < grid[y].Length; ++x)
                {
                    if (grid[y][x] == 'S')
                    {
                        startX = x;
                        startY = y;
                        break;
                    }
                }
            }
            // Util.Grid.PrintGrid(Core.Log.ELevel.Debug, grid);
            grid[startY][startX] = GetStartingPipe(grid, startX, startY);
            // Util.Grid.PrintGrid(Core.Log.ELevel.Debug, grid);
            PriorityQueue<Step, int> queue = new PriorityQueue<Step, int>();
            Dictionary<Base.Pos2, int> visited = new Dictionary<Base.Pos2, int>();
            queue.Enqueue(new Step(new Base.Pos2(startX, startY), 0), 0);
            int maxSteps = 0;
            while (queue.Count > 0)
            {
                Step cur = queue.Dequeue();
                if (visited.ContainsKey(cur.Pos))
                {
                    continue;
                }

                maxSteps = Math.Max(maxSteps, cur.Steps);
                visited[cur.Pos] = cur.Steps;
                foreach (Base.Pos2 pos2 in NextDirections[grid[cur.Pos.Y][cur.Pos.X]])
                {
                    Base.Pos2 next = cur.Pos + pos2;
                    if (next.X >= 0 && next.Y >= 0 && next.X < grid[0].Length && next.Y < grid.Length)
                    {
                        queue.Enqueue(new Step(next, cur.Steps + 1), cur.Steps + 1);
                    }
                }
                // grid[cur.Pos.Y][cur.Pos.X] = cur.Steps.ToString().First();
                // Util.Grid.PrintGrid(Core.Log.ELevel.Debug, grid);
            }
            return maxSteps.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);
    }
}