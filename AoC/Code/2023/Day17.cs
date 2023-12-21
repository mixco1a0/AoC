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

        private record Movement(Base.Pos2 Pos, Direction Direction, int Line, int HeatLoss, List<Base.Pos2> History);

        private class Node
        {
            public long Weight { get; set; }
            public Base.Pos2 Prev { get; set; }
            public bool Done { get; set; }
            public long Path { get; set; }

            public Node(long weight)
            {
                Weight = weight;
                Prev = null;
                Done = false;
                Path = long.MaxValue;
            }

            public override string ToString()
            {
                return Done ? $"{Path,4}" : $"?{Weight,2}?";
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
            nodes[0, 0].Path = 0;
            nodes[0, 0].Weight = 0;
            nodes[0, 0].Prev = new Base.Pos2();
        }

        private void PrintNodes(Node[,] nodes)
        {
            for (int y = 0; y < nodes.GetLength(1); ++y)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append($"{y,3} | ");
                for (int x = 0; x < nodes.GetLength(0); ++x)
                {
                    sb.Append($"{nodes[x, y].Weight,1}");
                }
                Log(sb.ToString());
            }
        }

        private void PrintHistory(int[,] grid, List<Base.Pos2> history)
        {
            for (int y = 0; y < grid.GetLength(1); ++y)
            {
                StringBuilder stringBuilder = new StringBuilder();
                for (int x = 0; x < grid.GetLength(0); ++x)
                {
                    Base.Pos2 pos = new Base.Pos2(x, y);
                    if (history.Contains(pos))
                    {
                        stringBuilder.Append('*');
                    }
                    else
                    {
                        stringBuilder.Append(grid[x, y]);
                    }
                }
                Core.Log.WriteLine(Core.Log.ELevel.Debug, stringBuilder.ToString());
            }
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables)
        {
            // ParseInput(inputs, out Node[,] nodes);
            // // PrintNodes(nodes);
            // int xMax = nodes.GetLength(0);
            // int yMax = nodes.GetLength(1);
            // Base.Pos2 end = new Base.Pos2(xMax - 1, yMax - 1);

            // PriorityQueue<Movement, long> gridWalker = new PriorityQueue<Movement, long>();
            // gridWalker.Enqueue(new Movement(new Base.Pos2(), Direction.None, 0, 0, new List<Base.Pos2>() { new Base.Pos2() }), 0);
            // while (gridWalker.Count > 0)
            // {
            //     Movement movement = gridWalker.Dequeue();
            //     Base.Pos2 curPos = movement.Pos;

            //     Node curNode = nodes[curPos.X, curPos.Y];
            //     if (curNode.Done)
            //     {
            //         continue;
            //     }
            //     curNode.Done = true;

            //     Node prevNode = nodes[curNode.Prev.X, curNode.Prev.Y];
            //     curNode.Path = prevNode.Path + curNode.Weight;

            //     if (curPos.Equals(end))
            //     {
            //         break;
            //     }

            //     foreach (var pair in Next)
            //     {
            //         bool sameDirection = pair.Key == movement.Direction;
            //         if (sameDirection && movement.Line == 3)
            //         {
            //             continue;
            //         }

            //         if (movement.Direction == Backtrack[pair.Key])
            //         {
            //             continue;
            //         }

            //         Base.Pos2 nextPos = curPos + pair.Value;
            //         if (nextPos.X >= 0 && nextPos.X < xMax && nextPos.Y >= 0 && nextPos.Y < yMax)
            //         {
            //             Node nextNode = nodes[nextPos.X, nextPos.Y];
            //             if (!nextNode.Done)
            //             {
            //                 if (nextNode.Prev != null)
            //                 {
            //                     Node existing = nodes[nextNode.Prev.X, nextNode.Prev.Y];
            //                     if (curNode.Path < existing.Path)
            //                     {
            //                         nextNode.Prev = curPos;
            //                     }
            //                 }
            //                 else
            //                 {
            //                     nextNode.Prev = curPos;
            //                 }

            //                 gridWalker.Enqueue(new Movement(nextPos, pair.Key, (sameDirection ? movement.Line + 1 : 0), 0, null), curNode.Path + nextNode.Weight);
            //             }
            //         }
            //     }
            // }

            // return nodes[xMax - 1, yMax - 1].Path.ToString();



            ParseInput(inputs, out int[,] grid);
            PrintGrid(grid);

            Base.Pos2 start = new Base.Pos2();
            Base.Pos2 end = new Base.Pos2(grid.GetLength(0) - 1, grid.GetLength(1) - 1);

            PriorityQueue<Movement, long> priorityQueue = new PriorityQueue<Movement, long>();
            priorityQueue.Enqueue(new Movement(new Base.Pos2(), Direction.None, 0, 0, new List<Base.Pos2>() { start }), 0);
            while (priorityQueue.Count > 0)
            {
                Movement movement = priorityQueue.Dequeue();
                if (movement.Pos == end)
                {
                    return movement.HeatLoss.ToString();
                }

                foreach (var pair in Next)
                {
                    bool sameDirection = pair.Key == movement.Direction;
                    if (sameDirection && movement.Line == 3)
                    {
                        continue;
                    }

                    if (movement.Direction == Backtrack[pair.Key])
                    {
                        continue;
                    }

                    Base.Pos2 nextPos = movement.Pos + pair.Value;
                    if (nextPos.X >= 0 && nextPos.X <= end.X && nextPos.Y >= 0 && nextPos.Y <= end.X && !movement.History.Contains(nextPos))
                    {
                        int nextHeatLoss = movement.HeatLoss + grid[nextPos.X, nextPos.Y];
                        List<Base.Pos2> posHistory = new List<Base.Pos2>(movement.History);
                        posHistory.Add(nextPos);
                        // double avg = 0;
                        // posHistory.ForEach((p) => { avg += p.Manhattan(end); });
                        // avg /= posHistory.Count;
                        // posHistory.ForEach((p) => {avg += grid[p.X, p.Y]; });
                        // double avg = (double)nextHeatLoss / (double)posHistory.Count;
                        priorityQueue.Enqueue(new Movement(nextPos, pair.Key, movement.Line + (sameDirection ? 1 : 0), nextHeatLoss, posHistory), nextHeatLoss);
                    }
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