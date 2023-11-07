using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AoC._2022
{
    class Day24 : Core.Day
    {
        public Day24() { }

        public override string GetSolutionVersion(Core.Part part)
        {
            switch (part)
            {
                case Core.Part.One:
                    return "v1";
                case Core.Part.Two:
                    return "v1";
                default:
                    return base.GetSolutionVersion(part);
            }
        }

        public override bool SkipTestData => true;

        protected override List<Core.TestDatum> GetTestData()
        {
            List<Core.TestDatum> testData = new List<Core.TestDatum>();
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.One,
                Output = "18",
                RawInput =
@"#.######
#>>.<^<#
#.<..<<#
#>v.><>#
#<^v^^>#
######.#"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
                Output = "54",
                RawInput =
@"#.######
#>>.<^<#
#.<..<<#
#>v.><>#
#<^v^^>#
######.#"
            });
            return testData;
        }

        private int MaxX { get; set; }
        private int MaxY { get; set; }
        private Pos2 Start { get; set; }
        private Pos2 End { get; set; }
        private List<HashSet<Pos2>> SafeCols { get; set; }
        private List<HashSet<Pos2>> SafeRows { get; set; }

        private record Pos2(int X, int Y)
        {
            public static Pos2 operator +(Pos2 a, Pos2 b)
            {
                return new Pos2(a.X + b.X, a.Y + b.Y);
            }

            public int Manhattan(Pos2 other)
            {
                return Math.Abs(X - other.X) + Math.Abs(Y - other.Y);
            }
        }

        private void Parse(List<string> inputs)
        {
            MaxX = inputs.First().Length - 2;
            MaxY = inputs.Count - 2;

            SafeCols = new List<HashSet<Pos2>>();
            for (int xCycle = 0; xCycle < MaxX; ++xCycle)
            {
                SafeCols.Add(new HashSet<Pos2>());
                for (int x = 0; x < MaxX; ++x)
                {
                    for (int y = 0; y < MaxY; ++y)
                    {
                        SafeCols[xCycle].Add(new Pos2(x, y));
                    }
                }
            }

            SafeRows = new List<HashSet<Pos2>>();
            for (int yCycle = 0; yCycle < MaxX; ++yCycle)
            {
                SafeRows.Add(new HashSet<Pos2>());
                for (int y = 0; y < MaxX; ++y)
                {
                    for (int x = 0; x < MaxX; ++x)
                    {
                        SafeRows[yCycle].Add(new Pos2(x, y));
                    }
                }
            }

            char[,] grid = new char[MaxX, MaxY];
            int gY = 0;
            foreach (string line in inputs.Skip(1).SkipLast(1))
            {
                int gX = 0;
                foreach (char l in line[1..^1])
                {
                    grid[gX, gY] = l;
                    ++gX;
                }
                ++gY;
            }

            RemoveUnsafePos(ref grid);
        }

        private void RemoveUnsafePos(ref char[,] grid)
        {
            for (int y = 0; y < MaxY; ++y)
            {
                for (int x = 0; x < MaxX; ++x)
                {
                    if (grid[x, y] == '>' || grid[x, y] == '<')
                    {
                        bool moveRight = grid[x, y] == '>';
                        for (int xCycle = 0; xCycle < MaxX; ++xCycle)
                        {
                            int xDiff = (x + (xCycle * (moveRight ? 1 : -1)) + MaxX) % MaxX;
                            Pos2 unsafePos = new Pos2(xDiff, y);
                            SafeRows[xCycle].Remove(unsafePos);
                        }
                    }
                }
            }

            for (int x = 0; x < MaxX; ++x)
            {
                for (int y = 0; y < MaxY; ++y)
                {
                    if (grid[x, y] == '^' || grid[x, y] == 'v')
                    {
                        bool moveDown = grid[x, y] == 'v';
                        for (int yCycle = 0; yCycle < MaxY; ++yCycle)
                        {
                            int yDiff = (y + (yCycle * (moveDown ? 1 : -1)) + MaxY) % MaxY;
                            Pos2 unsafePos = new Pos2(x, yDiff);
                            SafeCols[yCycle].Remove(unsafePos);
                        }
                    }
                }
            }
        }

        private void PrintCycle(int cycle, Pos2 cur)
        {
            char[,] grid = new char[MaxX, MaxY];
            for (int x = 0; x < MaxX; ++x)
            {
                for (int y = 0; y < MaxY; ++y)
                {
                    Pos2 pos = new Pos2(x, y);
                    int xCycle = cycle % MaxX;
                    int yCycle = cycle % MaxY;
                    if (!SafeRows[xCycle].Contains(pos) || !SafeCols[yCycle].Contains(pos))
                    {
                        grid[x, y] = 'X';
                    }
                    else if (cur.X == x && cur.Y == y)
                    {
                        grid[x, y] = '@';
                    }
                    else
                    {
                        grid[x, y] = '.';
                    }
                }
            }

            Util.Grid.PrintGrid(grid, Core.Log.ELevel.Debug);
        }

        private bool IsSpotSafe(int cycle, int x, int y)
        {
            Pos2 pos = new Pos2(x, y);
            int xCycle = cycle % MaxX;
            int yCycle = cycle % MaxY;
            if (!SafeRows[xCycle].Contains(pos) || !SafeCols[yCycle].Contains(pos))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private record CyclePos(int Cycle, Pos2 Pos)
        {
            public long Priority(Pos2 end)
            {
                return Cycle + Pos.Manhattan(end);
            }

            public override string ToString()
            {
                return $"@{Cycle} ({Pos.X},{Pos.Y})";
            }
        }

        static readonly Pos2[] MovePos = new Pos2[] { new Pos2(1, 0), new Pos2(0, 1), new Pos2(0, 0), new Pos2(0, -1), new Pos2(-1, 0) };

        private class WalkPath
        {
            public CyclePos Prev { get; set; }
            public bool Done { get; set; }
            public int Path { get; set; }

            public WalkPath()
            {
                Prev = new CyclePos(0, new Pos2(0, 0));
                Done = false;
                Path = int.MaxValue;
            }

            public override string ToString()
            {
                string done = Done ? "#|" : "?|";
                return $"{done} {Path} [{Prev.ToString()}]";
            }
        }

        private int Walk(int initialCycle)
        {
            int lower = Math.Min(MaxX, MaxY);
            int upper = Math.Max(MaxX, MaxY);
            int maxUniqueCycles = upper;
            while (maxUniqueCycles % lower != 0)
            {
                maxUniqueCycles += upper;
            }

            int minWalk = int.MaxValue;
            Dictionary<CyclePos, WalkPath> walkPaths = new Dictionary<CyclePos, WalkPath>();
            CyclePos cycleStart = new CyclePos(initialCycle, Start);
            walkPaths[cycleStart] = new WalkPath() { Prev = cycleStart, Path = -1 };
            Queue<CyclePos> queue = new Queue<CyclePos>();
            queue.Enqueue(cycleStart);
            while (queue.Count > 0)
            {
                CyclePos curCP = queue.Dequeue();
                WalkPath curWP = walkPaths[curCP];
                if (curWP.Done)
                {
                    continue;
                }
                curWP.Done = true;

                WalkPath prevWP = walkPaths[curWP.Prev];
                curWP.Path = prevWP.Path + 1;

                if (curWP.Path >= minWalk)
                {
                    continue;
                }

                foreach (Pos2 movePos in MovePos)
                {
                    CyclePos nextCP = new CyclePos(curCP.Cycle + 1, movePos + curCP.Pos);
                    if ((nextCP.Pos.X >= 0 && nextCP.Pos.X < MaxX && nextCP.Pos.Y >= 0 && nextCP.Pos.Y < MaxY) || (nextCP.Pos == Start && curCP.Pos == Start))
                    {
                        if (IsSpotSafe(nextCP.Cycle, nextCP.Pos.X, nextCP.Pos.Y) || (nextCP.Pos == Start))
                        {
                            if (!walkPaths.ContainsKey(nextCP))
                            {
                                walkPaths[nextCP] = new WalkPath() { Prev = curCP };
                            }
                            else
                            {
                                WalkPath existingWP = walkPaths[nextCP];
                                if (existingWP.Done)
                                {
                                    continue;
                                }

                                if (curWP.Path < existingWP.Path)
                                {
                                    existingWP.Prev = curCP;
                                }
                            }
                            queue.Enqueue(nextCP);
                        }
                    }
                    else if (nextCP.Pos == End)
                    {
                        minWalk = Math.Min(minWalk, curWP.Path + 1);
                    }
                }
            }

            return minWalk;
        }

        private void SwapStartEnd()
        {
            Pos2 tmp = Start;
            Start = End;
            End = tmp;
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool getSnack)
        {
            Parse(inputs);
            Start = new Pos2(0, -1);
            End = new Pos2(MaxX - 1, MaxY);
            int minWalk = Walk(0);
            if (getSnack)
            {
                SwapStartEnd();
                minWalk += Walk(minWalk);
                SwapStartEnd();
                minWalk += Walk(minWalk);
            }
            return minWalk.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);
    }
}