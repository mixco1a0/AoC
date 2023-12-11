using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
                Output = "2",
                RawInput =
@".S--7.
F|..|.
.|F-J.
.||.F7
.|L-J|
.L---J"
            });
            //             testData.Add(new Core.TestDatum
            //             {
            //                 TestPart = Core.Part.Two,
            //                 Output = "8",
            //                 RawInput =
            // @".F----7F7F7F7F-7....
            // .|F--7||||||||FJ....
            // .||.FJ||||||||L7....
            // FJL7L7LJLJ||LJ.L-7..
            // L--J.L7...LJS7F-7L7.
            // ....F-J..F7FJ|L7L7L7
            // ....L7.F7||L7|.L7L7|
            // .....|FJLJ|FJ|F7|.LJ
            // ....FJL-7.||.||||...
            // ....L---J.LJ.LJLJ..."
            //             });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
                Output = "10",
                RawInput =
@"FF7FSF7F7F7F7F7F---7
L|LJ||||||||||||F--J
FL-7LJLJ||||||LJL-77
F--JF--7||LJLJ7F7FJ-
L---JF-JLJ.||-FJLJJ7
|F|F-JF---7F7-L7L|7|
|FFJF7L7F-JF7|JL---7
7-L-JL7||F7|L7F-7F7|
L.L7LFJ|||||FJL7||LJ
L7JLJL-JLJLJL--JLJ.L"
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


        private List<Base.Pos2> Neighbors = new List<Base.Pos2>
        {
            new Base.Pos2(-1, -1),
            new Base.Pos2(-1, 0),
            new Base.Pos2(-1, 1),
            new Base.Pos2(1, 0),
            new Base.Pos2(0, 1),
            new Base.Pos2(-1, -1),
            new Base.Pos2(0, -1),
            new Base.Pos2(1, -1),
        };

        public Dictionary<char, List<string>> Expanded = new Dictionary<char, List<string>>()
        {
            {'|', new List<string>() {".|.", ".|.", ".|."}},
            {'-', new List<string>() {"...", "---", "..."}},
            {'L', new List<string>() {".|.", ".L-", "..."}},
            {'J', new List<string>() {".|.", "-J.", "..."}},
            {'7', new List<string>() {"...", "-7.", ".|."}},
            {'F', new List<string>() {"...", ".F-", ".|."}},
            {'.', new List<string>() {"...", "...", "..."}},
        };

        public char GetStartingPipe(char[][] grid, int startX, int startY)
        {
            bool hasN = false, hasS = false, hasE = false, hasW = false;
            if (startY > 0)
            {
                char n = grid[startY - 1][startX];
                hasN = NextDirections[n].Contains(new Base.Pos2(0, 1));
            }
            if (startY < grid.Length - 1)
            {
                char s = grid[startY + 1][startX];
                hasS = NextDirections[s].Contains(new Base.Pos2(0, -1));
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

        private char[][] Expand(char[][] source)
        {
            List<string> expanded = new List<string>();
            for (int y = 0; y < source.Length; ++y)
            {
                StringBuilder row1 = new StringBuilder(), row2 = new StringBuilder(), row3 = new StringBuilder();
                for (int x = 0; x < source[y].Length; ++x)
                {
                    row1.Append(Expanded[source[y][x]][0]);
                    row2.Append(Expanded[source[y][x]][1]);
                    row3.Append(Expanded[source[y][x]][2]);
                }
                expanded.Add(row1.ToString());
                expanded.Add(row2.ToString());
                expanded.Add(row3.ToString());
            }
            return expanded.Select(s => s.ToCharArray()).ToArray();
        }

        private char[][] Shrink(char[][] source)
        {
            List<string> source2 = source.Select(s => string.Join("", s)).ToList();
            List<string> shrunk = new List<string>();
            for (int i = 0; i < source2.Count; ++i)
            {
                if (i % 3 == 1)
                {
                    shrunk.Add(string.Join("", source2[i].Select((c, index) => new { c, index }).Where(obj => obj.index % 3 == 1).Select(obj => obj.c)));
                }
            }
            return shrunk.Select(s => s.ToCharArray()).ToArray();
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool findEnd)
        {
            char[][] grid = inputs.Select(i => i.ToCharArray()).ToArray();
            char[][] grid2 = inputs.Select(i => i.ToCharArray()).ToArray();
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

            grid[startY][startX] = GetStartingPipe(grid, startX, startY);
            grid2[startY][startX] = grid[startY][startX];

            PriorityQueue<Step, int> priorityQueue = new PriorityQueue<Step, int>();
            Dictionary<Base.Pos2, int> visited = new Dictionary<Base.Pos2, int>();
            priorityQueue.Enqueue(new Step(new Base.Pos2(startX, startY), 0), 0);
            int maxSteps = 0;
            char[][] visual = inputs.Select(i => i.ToCharArray()).ToArray();
            while (priorityQueue.Count > 0)
            {
                Step cur = priorityQueue.Dequeue();
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
                        priorityQueue.Enqueue(new Step(next, cur.Steps + 1), cur.Steps + 1);
                    }
                }
                visual[cur.Pos.Y][cur.Pos.X] = '#';
                grid2[cur.Pos.Y][cur.Pos.X] = '#';
            }

            if (findEnd)
            {
                return maxSteps.ToString();
            }

            // Util.Grid.PrintGrid(Core.Log.ELevel.Debug, grid);
            // char[][] visual = Expand(grid);
            // Util.Grid.PrintGrid(Core.Log.ELevel.Debug, grid);
            // visual[startY][startX] = grid[startY][startX];
            Util.Grid.PrintGrid(Core.Log.ELevel.Debug, visual);

            Queue<Base.Pos2> queue = new Queue<Base.Pos2>();
            for (int x = 0; x < visual[0].Length; ++x)
            {
                if (visual[0][x] == '.')
                {
                    queue.Enqueue(new Base.Pos2(x, 0));
                }
                if (visual[visual.Length - 1][x] == '.')
                {
                    queue.Enqueue(new Base.Pos2(x, visual.Length - 1));
                }
            }
            for (int y = 0; y < visual.Length; ++y)
            {
                if (visual[y][0] == '.')
                {
                    queue.Enqueue(new Base.Pos2(0, y));
                }
                if (visual[y][visual[y].Length - 1] == '.')
                {
                    queue.Enqueue(new Base.Pos2(visual[y].Length - 1, y));
                }
            }

            //Neighbors
            while (queue.Count > 0)
            {
                // Util.Grid.PrintGrid(Core.Log.ELevel.Debug, visual);
                Base.Pos2 cur = queue.Dequeue();
                if (visual[cur.Y][cur.X] == '#')
                {
                    continue;
                }

                visual[cur.Y][cur.X] = '#';
                foreach (Base.Pos2 neighbor in Neighbors)
                {
                    Base.Pos2 next = neighbor + cur;
                    if (next.X >= 0 && next.Y >= 0 && next.X < visual[0].Length && next.Y < visual.Length && visual[next.Y][next.X] != '#')
                    {
                        queue.Enqueue(next);
                    }
                }
            }

            return string.Join("", visual.SelectMany(s => s)).Where(c => c != '#').Count().ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
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

            grid[startY][startX] = GetStartingPipe(grid, startX, startY);

            PriorityQueue<Step, int> priorityQueue = new PriorityQueue<Step, int>();
            Dictionary<Base.Pos2, int> visited = new Dictionary<Base.Pos2, int>();
            priorityQueue.Enqueue(new Step(new Base.Pos2(startX, startY), 0), 0);
            int maxSteps = 0;
            while (priorityQueue.Count > 0)
            {
                Step cur = priorityQueue.Dequeue();
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
                        priorityQueue.Enqueue(new Step(next, cur.Steps + 1), cur.Steps + 1);
                    }
                }
            }
            return maxSteps.ToString();
        }

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            char[][] grid = inputs.Select(i => i.ToCharArray()).ToArray();
            // Util.Grid.PrintGrid(Core.Log.ELevel.Debug, grid);
            char[][] grid2 = inputs.Select(i => i.ToCharArray()).ToArray();
            char[][] grid3 = inputs.Select(i => i.ToCharArray()).ToArray();
            {
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

                grid[startY][startX] = GetStartingPipe(grid, startX, startY);
                grid3[startY][startX] = grid[startY][startX];

                PriorityQueue<Step, int> priorityQueue = new PriorityQueue<Step, int>();
                Dictionary<Base.Pos2, int> visited = new Dictionary<Base.Pos2, int>();
                priorityQueue.Enqueue(new Step(new Base.Pos2(startX, startY), 0), 0);
                int maxSteps = 0;
                while (priorityQueue.Count > 0)
                {
                    Step cur = priorityQueue.Dequeue();
                    if (visited.ContainsKey(cur.Pos))
                    {
                        continue;
                    }

                    grid3[cur.Pos.Y][cur.Pos.X] = '#';
                    maxSteps = Math.Max(maxSteps, cur.Steps);
                    visited[cur.Pos] = cur.Steps;
                    foreach (Base.Pos2 pos2 in NextDirections[grid[cur.Pos.Y][cur.Pos.X]])
                    {
                        Base.Pos2 next = cur.Pos + pos2;
                        if (next.X >= 0 && next.Y >= 0 && next.X < grid[0].Length && next.Y < grid.Length)
                        {
                            priorityQueue.Enqueue(new Step(next, cur.Steps + 1), cur.Steps + 1);
                        }
                    }
                }
            }
            // Util.Grid.PrintGrid(Core.Log.ELevel.Debug, grid3);


            for (int y = 0; y < grid.Length; ++y)
            {
                for (int x = 0; x < grid[y].Length; ++x)
                {
                    if (grid3[y][x] != '#')
                    {
                        grid[y][x] = '.';
                    }
                }
            }
            // Util.Grid.PrintGrid(Core.Log.ELevel.Debug, grid);

            grid2 = Expand(grid);
            // Util.Grid.PrintGrid(Core.Log.ELevel.Debug, grid2);

            Queue<Base.Pos2> queue = new Queue<Base.Pos2>();
            Func<char, bool> check = (c) => c != '#';
            for (int x = 0; x < grid2[0].Length; ++x)
            {
                if (check(grid2[0][x]))
                {
                    queue.Enqueue(new Base.Pos2(x, 0));
                }
                if (check(grid2[grid2.Length - 1][x]))
                {
                    queue.Enqueue(new Base.Pos2(x, grid2.Length - 1));
                }
            }
            for (int y = 0; y < grid2.Length; ++y)
            {
                if (check(grid2[y][0]))
                {
                    queue.Enqueue(new Base.Pos2(0, y));
                }
                if (check(grid2[y][grid2[y].Length - 1]))
                {
                    queue.Enqueue(new Base.Pos2(grid2[y].Length - 1, y));
                }
            }

            //Neighbors
            while (queue.Count > 0)
            {
                Base.Pos2 cur = queue.Dequeue();
                if (grid2[cur.Y][cur.X] == '#')
                {
                    continue;
                }

                grid2[cur.Y][cur.X] = '#';
                // Util.Grid.PrintGrid(Core.Log.ELevel.Debug, grid2);
                // Log("");
                foreach (Base.Pos2 neighbor in Neighbors)
                {
                    Base.Pos2 next = neighbor + cur;
                    if (next.X >= 0 && next.Y >= 0 && next.X < grid2[0].Length && next.Y < grid2.Length && grid2[next.Y][next.X] == '.')
                    {
                        queue.Enqueue(next);
                    }
                }
            }
            // Util.Grid.PrintGrid(Core.Log.ELevel.Debug, grid2);
            // Util.Grid.PrintGrid(Core.Log.ELevel.Debug, grid);
            grid = Shrink(grid2);
            for (int y = 0; y < grid.Length; ++y)
            {
                for (int x = 0; x < grid[y].Length; ++x)
                {
                    if (grid3[y][x] == '#')
                    {
                        grid[y][x] = '#';
                    }
                }
            }
            // Util.Grid.PrintGrid(Core.Log.ELevel.Debug, grid);
            // Util.Grid.PrintGrid(Core.Log.ELevel.Debug, grid);

            return string.Join("", grid.SelectMany(s => s)).Where(c => c != '#').Count().ToString();
        }
        // 5283 too high
        // 789 too high
        // 774 too high
    }
}