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

        // private record Movement(Base.Pos2 Pos, Direction Direction, int Steps, int HeatLoss, Base.Pos2 Prev, Direction PrevDirection);
        private record Movement(Base.Pos2 Pos, Direction Direction, int Steps, int HeatLoss, string History)
        {
            public override string ToString()
            {
                return $"@{Pos.ToString()}|{DirectionChar[Direction]}|{Steps}|{HeatLoss}|{History}";
            }
        }

        private class Node
        {
            public long Weight { get; set; }
            public int[] Steps { get; set; }
            public Base.Pos2[] Prev { get; set; }
            public bool[][] Done { get; set; }
            public long[] Path { get; set; }
            public string[] History { get; set; }

            public Node(long weight, int maxSteps)
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

            public Base.Pos2 BestPrev()
            {
                return null;
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
            bool print = false;
            ParseInput(inputs, out int[,] grid);
            ParseInput(inputs, maxSteps, out Node[,] nodes);
            int xMax = nodes.GetLength(0);
            int yMax = nodes.GetLength(1);
            PriorityQueue<Movement, int> toCheck = new PriorityQueue<Movement, int>();
            toCheck.Enqueue(new Movement(Base.Pos2.Zero, Direction.Last, 0, 0, ""), 0);
            int minHeatLoss = int.MaxValue;
            while (toCheck.Count != 0)
            {
                Movement cur = toCheck.Dequeue();
                // if (print) Log(Core.Log.ELevel.Debug, $"HeatLoss={cur.HeatLoss}");
                // if (print) PrintNodes(nodes, cur.History);

                if (cur.Direction != Direction.Last)
                {
                    int curDir = (int)cur.Direction;
                    Node curNode = nodes[cur.Pos.X, cur.Pos.Y];
                    if (curNode.Done[curDir][cur.Steps])
                    {
                        int steps = curNode.Steps[curDir];
                        if (cur.HeatLoss <= curNode.Path[curDir])
                        {
                            curNode.Path[curDir] = cur.HeatLoss;
                            curNode.Steps[curDir] = cur.Steps;

                            // if (print) Log(Core.Log.ELevel.Debug, "Before");
                            // if (print) PrintNodes(nodes, curNode.History[curDir]);
                            // curNode.History[curDir] = cur.History;
                            // if (print) Log(Core.Log.ELevel.Debug, "After");
                            // if (print) PrintNodes(nodes, curNode.History[curDir]);
                        }

                        if (cur.Steps >= steps && steps >= minSteps)
                        {
                            continue;
                        }
                    }

                    curNode.Path[curDir] = cur.HeatLoss;
                    curNode.Steps[curDir] = cur.Steps;
                    curNode.Done[curDir][cur.Steps] = true;

                    if (cur.Pos.X == grid.GetLength(0) - 1 && cur.Pos.Y == grid.GetLength(1) - 1)
                    {
                        if (cur.Steps < minSteps)
                        {
                            continue;
                        }

                        // if (print) PrintNodes(nodes, curNode.History[(int)cur.Direction]);
                        minHeatLoss = Math.Min(minHeatLoss, cur.HeatLoss);
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
                    if (next.X < 0 || next.X >= grid.GetLength(0) || next.Y < 0 || next.Y >= grid.GetLength(1))
                    {
                        continue;
                    }

                    // make sure not to check extra solutions
                    int heatLoss = cur.HeatLoss + grid[next.X, next.Y];
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


// using System.Text;
// using System;
// using System.Collections.Generic;
// using System.Linq;

// namespace AoC._2023
// {
//     class Day17 : Core.Day
//     {
//         public Day17() { }

//         public override string GetSolutionVersion(Core.Part part)
//         {
//             switch (part)
//             {
//                 // case Core.Part.One:
//                 //     return "v1";
//                 // case Core.Part.Two:
//                 //     return "v1";
//                 default:
//                     return base.GetSolutionVersion(part);
//             }
//         }

//         public override bool SkipTestData => false;

//         protected override List<Core.TestDatum> GetTestData()
//         {
//             List<Core.TestDatum> testData = new List<Core.TestDatum>();
//             testData.Add(new Core.TestDatum
//             {
//                 TestPart = Core.Part.One,
//                 Output = "11",
//                 RawInput =
// @"9111111
// 2211251
// 3344552"
//             });
//             testData.Add(new Core.TestDatum
//             {
//                 TestPart = Core.Part.One,
//                 Output = "102",
//                 RawInput =
// @"2413432311323
// 3215453535623
// 3255245654254
// 3446585845452
// 4546657867536
// 1438598798454
// 4457876987766
// 3637877979653
// 4654967986887
// 4564679986453
// 1224686865563
// 2546548887735
// 4322674655533"
//             });
//             testData.Add(new Core.TestDatum
//             {
//                 TestPart = Core.Part.Two,
//                 Output = "71",
//                 RawInput =
// @"111111111111
// 999999999991
// 999999999991
// 999999999991
// 999999999991"
//             });
//             testData.Add(new Core.TestDatum
//             {
//                 TestPart = Core.Part.Two,
//                 Output = "94",
//                 RawInput =
// @"2413432311323
// 3215453535623
// 3255245654254
// 3446585845452
// 4546657867536
// 1438598798454
// 4457876987766
// 3637877979653
// 4654967986887
// 4564679986453
// 1224686865563
// 2546548887735
// 4322674655533"
//             });
//             return testData;
//         }

//         private enum Direction { North, South, East, West, None }
//         private static Dictionary<Direction, Base.Pos2> Next = new Dictionary<Direction, Base.Pos2>()
//         {
//             {Direction.North, new Base.Pos2(0, -1)},
//             {Direction.South, new Base.Pos2(0, 1)},
//             {Direction.East, new Base.Pos2(1, 0)},
//             {Direction.West, new Base.Pos2(-1, 0)}
//         };

//         private static Dictionary<Direction, Direction> Backtrack = new Dictionary<Direction, Direction>()
//         {
//             {Direction.North, Direction.South},
//             {Direction.South, Direction.North},
//             {Direction.East, Direction.West},
//             {Direction.West, Direction.East},
//         };

//         private void ParseInput(List<string> inputs, out int[,] grid)
//         {
//             grid = new int[inputs[0].Length, inputs.Count()];
//             for (int x = 0; x < inputs[0].Length; ++x)
//             {
//                 for (int y = 0; y < inputs.Count; ++y)
//                 {
//                     grid[x, y] = inputs[y][x] - '0';
//                 }
//             }
//         }

//         private void PrintGrid(int[,] grid)
//         {
//             StringBuilder stringBuilder = new StringBuilder();
//             Core.Log.WriteLine(Core.Log.ELevel.Debug, $"Printing grid {grid.GetLength(0)}x{grid.GetLength(1)}:");
//             for (int y = 0; y < grid.GetLength(1); ++y)
//             {
//                 stringBuilder.Clear();
//                 stringBuilder.Append($"{y,4}| ");
//                 stringBuilder.Append(string.Join(string.Empty, Enumerable.Range(0, grid.GetLength(0)).Select(x => (char)(grid[x, y] + '0'))));
//                 Core.Log.WriteLine(Core.Log.ELevel.Debug, stringBuilder.ToString());
//             }
//         }

//         private record Movement(Base.Pos2 Pos, Direction Direction, int Steps, int HeatLoss, Base.Pos2 Prev);

//         private class NodeState
//         {
//             public int Steps { get; set; }
//             public Base.Pos2 Prev { get; set; }
//             public long Path { get; set; }
//         }

//         private class Node
//         {
//             public long Weight { get; set; }
//             public Dictionary<int, NodeState> States { get; set; }
//             public string History { get; set; }

//             public Node(long weight)
//             {
//                 Weight = weight;
//                 States = new Dictionary<int, NodeState>();
//                 History = string.Empty;
//             }

//             public void Add(int steps, Direction direction, long path, Base.Pos2 pos2)
//             {
//                 int hash = Hash(steps, direction);
//                 States[hash] = new NodeState { Steps = steps, Prev = pos2, Path = path };
//             }

//             public static int Hash(int steps, Direction direction)
//             {
//                 return HashCode.Combine(steps, direction);
//             }

//             public override string ToString()
//             {
//                 return "";
//             }
//         }

//         private void ParseInput(List<string> inputs, int maxSteps, out Node[,] nodes)
//         {
//             int xMax = inputs.First().Length;
//             int yMax = inputs.Count;
//             nodes = new Node[xMax, yMax];
//             for (int x = 0; x < xMax; ++x)
//             {
//                 for (int y = 0; y < yMax; ++y)
//                 {
//                     nodes[x, y] = new Node(inputs[y][x] - '0');
//                 }
//             }

//             // reset first state
//             for (Direction dir = Direction.North; dir != Direction.None; ++dir)
//             {
//                 nodes[0, 0].Add(0, dir, 0, Base.Pos2.Zero);
//             }
//         }

//         private void PrintNodes(Node[,] nodes)
//         {

//         }

//         private void PrintSolution(Node[,] nodes)
//         {

//         }

//         private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, int minSteps, int maxSteps)
//         {
//             ParseInput(inputs, out int[,] grid);
//             // PrintGrid(grid);
//             ParseInput(inputs, maxSteps, out Node[,] nodes);
//             PriorityQueue<Movement, int> toCheck = new PriorityQueue<Movement, int>();
//             toCheck.Enqueue(new Movement(Base.Pos2.Zero, Direction.East, 0, 0, Base.Pos2.Zero), 0);
//             toCheck.Enqueue(new Movement(Base.Pos2.Zero, Direction.South, 0, 0, Base.Pos2.Zero), 0);
//             while (toCheck.Count != 0)
//             {
//                 Movement cur = toCheck.Dequeue();
//                 int curDir = (int)cur.Direction;
//                 Node curNode = nodes[cur.Pos.X, cur.Pos.Y];
//                 int curHash = Node.Hash(cur.Steps, cur.Direction);
//                 if (curNode.States.ContainsKey(curHash))
//                 {
//                     NodeState state = curNode.States[curHash];
//                     int steps = state.Steps;
//                     if (cur.HeatLoss <= state.Path)
//                     {
//                         state.Path = cur.HeatLoss;
//                         state.Prev = cur.Prev;
//                         state.Steps = cur.Steps;
//                     }

//                     if (cur.Steps >= steps && steps >= minSteps)
//                     {
//                         continue;
//                     }
//                 }
//                 else
//                 {
//                     curNode.Add(cur.Steps, cur.Direction, cur.HeatLoss, cur.Prev);
//                 }


//                 if (maxSteps > 5)
//                 {
//                     // PrintNodes(nodes);
//                 }

//                 if (cur.Pos.X == grid.GetLength(0) - 1 && cur.Pos.Y == grid.GetLength(1) - 1)
//                 {
//                     if (cur.Steps < minSteps)
//                     {
//                         continue;
//                     }

//                     if (maxSteps > 5)
//                     {
//                         // PrintNodes(nodes);
//                         // PrintSolution(nodes);
//                     }
//                     return cur.HeatLoss.ToString();
//                 }

//                 for (Direction d = Direction.North; d != Direction.None; ++d)
//                 {
//                     // no backtracking
//                     if (d == Backtrack[cur.Direction])
//                     {
//                         continue;
//                     }

//                     // make sure if minimum steps haven't been reached, the only valid direction is the same direction
//                     bool minStepsNotReached = (cur.Steps + 1) < minSteps;
//                     if (minStepsNotReached && d != cur.Direction)
//                     {
//                         continue;
//                     }

//                     // make sure max steps aren't reached
//                     int totalSteps = (d == cur.Direction) ? cur.Steps + 1 : 1;
//                     if (d == cur.Direction && totalSteps > maxSteps)
//                     {
//                         continue;
//                     }

//                     // make sure position is valid
//                     Base.Pos2 next = cur.Pos + Next[d];
//                     if (next.X < 0 || next.X >= grid.GetLength(0) || next.Y < 0 || next.Y >= grid.GetLength(1))
//                     {
//                         continue;
//                     }

//                     // add heat loss and move on
//                     int heatLoss = cur.HeatLoss + grid[next.X, next.Y];
//                     toCheck.Enqueue(new Movement(next, d, totalSteps, heatLoss, cur.Pos), minStepsNotReached ? -1 : heatLoss);
//                 }
//             }
//             return string.Empty;
//         }

//         protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
//             => SharedSolution(inputs, variables, 1, 3);

//         protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
//             => SharedSolution(inputs, variables, 4, 10);
//     }
// }