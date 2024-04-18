using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2023
{
    class Day17 : Core.Day
    {
        public Day17() { }

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
                Output = "11",
                RawInput =
@"9111111
2211251
3344552"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.One,
                Output = "102",
                RawInput =
@"2413432311323
3215453535623
3255245654254
3446585845452
4546657867536
1438598798454
4457876987766
3637877979653
4654967986887
4564679986453
1224686865563
2546548887735
4322674655533"
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

        private enum Direction { North, South, East, West, None }
        private static Dictionary<Direction, Base.Pos2> Next = new Dictionary<Direction, Base.Pos2>()
        {
            {Direction.North, new Base.Pos2(0, -1)},
            {Direction.South, new Base.Pos2(0, 1)},
            {Direction.East, new Base.Pos2(1, 0)},
            {Direction.West, new Base.Pos2(-1, 0)}
        };

        private static Dictionary<Direction, Direction> Backtrack = new Dictionary<Direction, Direction>()
        {
            {Direction.North, Direction.South},
            {Direction.South, Direction.North},
            {Direction.East, Direction.West},
            {Direction.West, Direction.East},
        };

        private void ParseInput(List<string> inputs, out int[,] grid)
        {
            grid = new int[inputs[0].Length, inputs.Count()];
            for (int x = 0; x < inputs[0].Length; ++x)
            {
                for (int y = 0; y < inputs.Count; ++y)
                {
                    grid[x, y] = inputs[y][x] - '0';
                }
            }
        }

        private void PrintGrid(int[,] grid)
        {
            StringBuilder stringBuilder = new StringBuilder();
            Core.Log.WriteLine(Core.Log.ELevel.Debug, $"Printing grid {grid.GetLength(0)}x{grid.GetLength(1)}:");
            for (int y = 0; y < grid.GetLength(1); ++y)
            {
                stringBuilder.Clear();
                stringBuilder.Append($"{y,4}| ");
                stringBuilder.Append(string.Join(string.Empty, Enumerable.Range(0, grid.GetLength(0)).Select(x => (char)(grid[x, y] + '0'))));
                Core.Log.WriteLine(Core.Log.ELevel.Debug, stringBuilder.ToString());
            }
        }

        private record Movement(Base.Pos2 Pos, Direction Direction, int Steps, int HeatLoss, Base.Pos2 Prev);

        private class Node
        {
            public long Weight { get; set; }
            public int[] Steps { get; set; }
            public Base.Pos2[] Prev { get; set; }
            public bool[] Done { get; set; }
            public long[] Path { get; set; }

            public Node(long weight)
            {
                int count = (int)Direction.None;
                Weight = weight;
                Steps = new int[count];
                Prev = new Base.Pos2[count];
                Done = new bool[count];
                Path = new long[count];
                for (Direction dir = Direction.North; dir != Direction.None; ++dir)
                {
                    Steps[(int)dir] = 0;
                    Prev[(int)dir] = null;
                    Done[(int)dir] = false;
                    Path[(int)dir] = long.MaxValue;
                }
            }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                for (Direction d = Direction.North; d != Direction.None; ++d)
                {
                    sb.Append($"{d.ToString()[0]}:");

                    int i = (int)d;
                    if (Done[i])
                    {
                        sb.Append($"{Path[i],3}");
                    }
                    else
                    {
                        sb.Append($"???");
                    }
                    sb.Append($"|");
                }
                return sb.ToString();
            }

            public Base.Pos2 BestPrev()
            {
                Base.Pos2 prev = null;
                long min = long.MaxValue;
                for (Direction d = Direction.North; d != Direction.None; ++d)
                {
                    int i = (int)d;
                    if (Done[i])
                    {
                        if (min > Path[i])
                        {
                            min = Path[i];
                            prev = Prev[i];
                        }
                    }
                }
                return prev;
            }
        }

        private void ParseInput(List<string> inputs, out Node[,] nodes)
        {
            int xMax = inputs.First().Length;
            int yMax = inputs.Count;
            nodes = new Node[xMax, yMax];
            for (int x = 0; x < xMax; ++x)
            {
                for (int y = 0; y < yMax; ++y)
                {
                    nodes[x, y] = new Node(inputs[y][x] - '0');
                }
            }

            // reset first state
            for (Direction dir = Direction.North; dir != Direction.None; ++dir)
            {
                nodes[0, 0].Path[(int)dir] = 0;
                nodes[0, 0].Path[(int)dir] = 0;
                nodes[0, 0].Prev[(int)dir] = Base.Pos2.Zero;
            }
        }

        private void PrintNodes(Node[,] nodes)
        {
            Log("-------------------------");
            for (int y = 0; y < nodes.GetLength(1); ++y)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append($"{y,3} | ");
                for (int x = 0; x < nodes.GetLength(0); ++x)
                {
                    if (x == 0 && y == 0)
                    {
                        sb.Append($"S");
                        continue;
                    }

                    long shortest = long.MaxValue;
                    Direction best = Direction.None;
                    for (Direction d = Direction.North; d != Direction.None; ++d)
                    {
                        if (nodes[x, y].Done[(int)d])
                        {
                            if (nodes[x, y].Path[(int)d] <= shortest)
                            {
                                shortest = nodes[x, y].Path[(int)d];
                                best = d;
                            }
                        }
                    }
                    char c = '?';
                    switch (best)
                    {
                        case Direction.North:
                            c = '^';
                            break;
                        case Direction.South:
                            c = 'v';
                            break;
                        case Direction.East:
                            c = '>';
                            break;
                        case Direction.West:
                            c = '<';
                            break;
                    }
                    sb.Append($"{c,1}");
                }
                Log(sb.ToString());
            }
        }

        private void PrintSolution(Node[,] nodes)
        {
            int xMax = nodes.GetLength(0);
            int yMax = nodes.GetLength(1);
            char[,] printout = new char[xMax, yMax];
            for (int y = 0; y < yMax; ++y)
            {
                for (int x = 0; x < xMax; ++x)
                {
                    printout[x, y] = (char)(nodes[x, y].Weight + '0');
                }
            }

            Base.Pos2 end = new Base.Pos2(xMax - 1, yMax - 1);
            Base.Pos2 cur = Base.Pos2.Zero;
            while (cur != end)
            {
                printout[cur.X, cur.Y] = '*';
                cur = nodes[cur.X, cur.Y].BestPrev();
            }

            Log("-------------------------");
            for (int y = 0; y < nodes.GetLength(1); ++y)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append($"{y,3} | ");
                for (int x = 0; x < nodes.GetLength(0); ++x)
                {
                    sb.Append(printout[x, y]);
                }
                Log(sb.ToString());
            }
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables)
        {
            ParseInput(inputs, out int[,] grid);
            // PrintGrid(grid);
            ParseInput(inputs, out Node[,] nodes);
            PriorityQueue<Movement, int> toCheck = new PriorityQueue<Movement, int>();
            toCheck.Enqueue(new Movement(Base.Pos2.Zero, Direction.East, 0, 0, Base.Pos2.Zero), 0);
            toCheck.Enqueue(new Movement(Base.Pos2.Zero, Direction.South, 0, 0, Base.Pos2.Zero), 0);
            while (toCheck.Count != 0)
            {
                Movement cur = toCheck.Dequeue();
                int curDir = (int)cur.Direction;
                Node curNode = nodes[cur.Pos.X, cur.Pos.Y];
                if (curNode.Done[curDir])
                {
                    int steps = curNode.Steps[curDir];
                    if (cur.HeatLoss <= curNode.Path[curDir])
                    {
                        curNode.Path[curDir] = cur.HeatLoss;
                        curNode.Prev[curDir] = cur.Prev;
                        curNode.Steps[curDir] = cur.Steps;
                    }

                    if (cur.Steps >= steps)
                    {
                        continue;
                    }
                }

                curNode.Path[curDir] = cur.HeatLoss;
                curNode.Prev[curDir] = cur.Prev;
                curNode.Steps[curDir] = cur.Steps;
                curNode.Done[curDir] = true;

                if (cur.Pos.X == grid.GetLength(0) - 1 && cur.Pos.Y == grid.GetLength(1) - 1)
                {
                    // PrintNodes(nodes);
                    // PrintSolution(nodes);
                    return cur.HeatLoss.ToString();
                }

                for (Direction d = Direction.North; d != Direction.None; ++d)
                {
                    if (d == Backtrack[cur.Direction])
                    {
                        continue;
                    }
                    else if (d == cur.Direction && cur.Steps >= 3)
                    {
                        continue;
                    }
                    Base.Pos2 next = cur.Pos + Next[d];
                    if (next.X < 0 || next.X >= grid.GetLength(0) || next.Y < 0 || next.Y >= grid.GetLength(1))
                    {
                        continue;
                    }
                    int steps = (d == cur.Direction) ? cur.Steps + 1 : 1;
                    int heatLoss = cur.HeatLoss + grid[next.X, next.Y];
                    toCheck.Enqueue(new Movement(next, d, steps, heatLoss, cur.Pos), heatLoss);
                }
            }
            return string.Empty;
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);
        // [too low] 846

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);
    }
}