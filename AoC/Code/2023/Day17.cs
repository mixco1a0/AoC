using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using AoC.Util;

namespace AoC._2023
{
    class Day17 : Core.Day
    {
        public Day17() { }

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
                Output = "71",
                RawInput =
@"111111111111
999999999991
999999999991
999999999991
999999999991"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
                Output = "94",
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
            return testData;
        }

        private enum Direction { First, East = First, South, West, North, Last }

        private static Dictionary<Direction, Base.Pos2> Next = new Dictionary<Direction, Base.Pos2>()
        {
            {Direction.North, new Base.Pos2(0, -1)},
            {Direction.South, new Base.Pos2(0, 1)},
            {Direction.East, new Base.Pos2(1, 0)},
            {Direction.West, new Base.Pos2(-1, 0)}
        };

        private static Dictionary<Direction, char> DirectionChar = new Dictionary<Direction, char>()
        {
            {Direction.North, '^'},
            {Direction.South, 'v'},
            {Direction.East, '>'},
            {Direction.West, '<'}
        };

        private static Dictionary<Direction, string> DirectionLetters = new Dictionary<Direction, string>()
        {
            {Direction.North, "Nn"},
            {Direction.South, "Ss"},
            {Direction.East, "Ee"},
            {Direction.West, "Ww"}
        };

        private static Dictionary<Direction, Direction> Backtrack = new Dictionary<Direction, Direction>()
        {
            {Direction.North, Direction.South},
            {Direction.South, Direction.North},
            {Direction.East, Direction.West},
            {Direction.West, Direction.East},
        };

        private record Movement(Base.Pos2 Pos, Direction Direction, int Steps, int HeatLoss, string History)
        {
            public override string ToString()
            {
                return $"@{Pos.ToString()}|{DirectionChar[Direction]}|{Steps}|{HeatLoss}|{History}";
            }
        }

        private class Node
        {
            public int Weight { get; set; }
            public int[] Steps { get; set; }
            public Base.Pos2[] Prev { get; set; }
            public bool[][] Done { get; set; }
            public long[] Path { get; set; }
            public string[] History { get; set; }

            public Node(int weight, int maxSteps)
            {
                int count = (int)Direction.Last;
                Weight = weight;
                Steps = new int[count];
                Prev = new Base.Pos2[count];
                Done = new bool[count][];
                Path = new long[count];
                History = new string[count];
                for (Direction dir = Direction.First; dir != Direction.Last; ++dir)
                {
                    Steps[(int)dir] = 0;
                    Prev[(int)dir] = null;
                    Done[(int)dir] = new bool[maxSteps + 1];
                    for (int i = 0; i <= maxSteps; ++i)
                    {
                        Done[(int)dir][i] = false;
                    }
                    Path[(int)dir] = long.MaxValue;
                    History[(int)dir] = "";
                }
            }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                for (Direction d = Direction.First; d != Direction.Last; ++d)
                {
                    sb.Append($"{d.ToString()[0]}:");

                    int i = (int)d;
                    if (Done[i].Any(a => a))
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
        }

        private void ParseInput(List<string> inputs, int maxSteps, out Node[,] nodes)
        {
            int xMax = inputs.First().Length;
            int yMax = inputs.Count;
            nodes = new Node[xMax, yMax];
            for (int x = 0; x < xMax; ++x)
            {
                for (int y = 0; y < yMax; ++y)
                {
                    nodes[x, y] = new Node(inputs[y][x] - '0', maxSteps);
                }
            }

            // reset first state
            for (Direction dir = Direction.First; dir != Direction.Last; ++dir)
            {
                nodes[0, 0].Path[(int)dir] = 0;
                nodes[0, 0].Path[(int)dir] = 0;
                nodes[0, 0].Prev[(int)dir] = Base.Pos2.Zero;
            }
        }

        private void PrintNodes(Node[,] nodes, string history)
        {
            int xMax = nodes.GetLength(0);
            int yMax = nodes.GetLength(1);
            char[,] chars = new char[xMax, yMax];
            char[,] chars2 = new char[xMax, yMax];
            for (int x = 0; x < xMax; ++x)
            {
                for (int y = 0; y < yMax; ++y)
                {
                    chars[x, y] = '#';
                    chars2[x, y] = '?';
                }
            }

            Base.Pos2 cur = new Base.Pos2();
            foreach (char c in history)
            {
                Direction d = Direction.Last;
                switch (c)
                {
                    case 'N':
                    case 'n':
                        d = Direction.North;
                        break;
                    case 'E':
                    case 'e':
                        d = Direction.East;
                        break;
                    case 'S':
                    case 's':
                        d = Direction.South;
                        break;
                    case 'W':
                    case 'w':
                        d = Direction.West;
                        break;
                }
                cur += Next[d];
                chars[cur.X, cur.Y] = DirectionChar[d];
                chars2[cur.X, cur.Y] = (char)(nodes[cur.X, cur.Y].Weight + '0');
            }

            Grid.PrintGrid(chars, Core.Log.ELevel.Debug);
            Grid.PrintGrid(chars2, Core.Log.ELevel.Debug);
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, int minSteps, int maxSteps)
        {
            ParseInput(inputs, maxSteps, out Node[,] nodes);
            int xMax = nodes.GetLength(0);
            int yMax = nodes.GetLength(1);
            PriorityQueue<Movement, int> toCheck = new PriorityQueue<Movement, int>();
            toCheck.Enqueue(new Movement(Base.Pos2.Zero, Direction.Last, 0, 0, ""), 0);
            int minHeatLoss = int.MaxValue;
            while (toCheck.Count != 0)
            {
                Movement cur = toCheck.Dequeue();

                if (cur.Direction != Direction.Last)
                {
                    int curDir = (int)cur.Direction;
                    Node curNode = nodes[cur.Pos.X, cur.Pos.Y];
                    if (curNode.Done[curDir][cur.Steps])
                    {
                        continue;
                    }

                    curNode.Path[curDir] = cur.HeatLoss;
                    curNode.Steps[curDir] = cur.Steps;
                    curNode.Done[curDir][cur.Steps] = true;

                    if (cur.Pos.X == xMax - 1 && cur.Pos.Y == yMax - 1)
                    {
                        if (cur.Steps < minSteps)
                        {
                            continue;
                        }

                        minHeatLoss = Math.Min(minHeatLoss, cur.HeatLoss);
                        return minHeatLoss.ToString();
                    }
                }

                for (Direction d = Direction.First; d != Direction.Last; ++d)
                {
                    bool minStepsNotReached = cur.Steps < minSteps;
                    int totalSteps = (d == cur.Direction) ? cur.Steps + 1 : 1;
                    if (cur.Direction != Direction.Last)
                    {
                        // no backtracking
                        if (d == Backtrack[cur.Direction])
                        {
                            continue;
                        }

                        // make sure max steps aren't reached
                        if (d == cur.Direction)
                        {
                            if (totalSteps > maxSteps)
                            {
                                continue;
                            }
                        }
                        // make sure if minimum steps haven't been reached, the only valid direction is the same direction
                        else if (minStepsNotReached)
                        {
                            continue;
                        }
                    }

                    // make sure position is valid
                    Base.Pos2 next = cur.Pos + Next[d];
                    if (next.X < 0 || next.X >= xMax || next.Y < 0 || next.Y >= yMax)
                    {
                        continue;
                    }

                    // make sure not to check extra solutions
                    int heatLoss = cur.HeatLoss + nodes[next.X, next.Y].Weight;
                    if (heatLoss > minHeatLoss)
                    {
                        continue;
                    }
                    toCheck.Enqueue(new Movement(next, d, totalSteps, heatLoss, ""), minStepsNotReached ? -1 : heatLoss);

                    // StringBuilder newHistory = new StringBuilder();
                    // newHistory.Append(cur.History);
                    // if (minStepsNotReached || cur.Steps == 1)
                    // {
                    //     newHistory.Append(DirectionLetters[d][1]);
                    // }
                    // else
                    // {
                    //     newHistory.Append(DirectionLetters[d][0]);
                    // }
                    // nodes[next.X, next.Y].History[(int)d] = newHistory.ToString();

                    // // add heat loss and move on
                    // toCheck.Enqueue(new Movement(next, d, totalSteps, heatLoss, newHistory.ToString()), minStepsNotReached ? -1 : heatLoss);
                }
            }
            return minHeatLoss.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, 1, 3);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, 4, 10);
    }
}