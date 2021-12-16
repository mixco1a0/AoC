using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2021
{
    class Day15 : Day
    {
        public Day15() { }

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
                Output = "7",
                RawInput =
@"116
138
213"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.One,
                Output = "40",
                RawInput =
@"1163751742
1381373672
2136511328
3694931569
7463417111
1319128137
1359912421
3125421639
1293138521
2311944581"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.Two,
                Output = "315",
                RawInput =
@"1163751742
1381373672
2136511328
3694931569
7463417111
1319128137
1359912421
3125421639
1293138521
2311944581"
            });
            return testData;
        }

        static readonly Core.Point[] GridMoves = new Core.Point[] { new Core.Point(0, 1), new Core.Point(1, 0), new Core.Point(-1, 0), new Core.Point(0, -1) };

        private class Node
        {
            public long Weight { get; set; }
            public Core.Point Prev { get; set; }
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

        private void Populate(ref Node[,] nodes, Core.Point start, Core.Point end)
        {

        }

        private Node[,] GetNodes(List<string> inputs, bool enlargen, out int maxX, out int maxY)
        {
            maxX = inputs.First().Length;
            maxY = inputs.Count;
            Node[,] nodes = new Node[maxX, maxY];
            for (int x = 0; x < maxX; ++x)
            {
                for (int y = 0; y < maxY; ++y)
                {
                    nodes[x, y] = new Node(inputs[y][x] - '0');
                }
            }

            if (enlargen)
            {
                Node[,] source = nodes;
                int sourceMaxX = maxX;
                int sourceMaxY = maxY;
                maxX *= 5;
                maxY *= 5;
                nodes = new Node[maxX, maxY];

                Func<Node[,], int, int, Node> GetAdjustedNode = (source, curX, curY) =>
                {
                    int adjustment = curX / sourceMaxX + curY / sourceMaxX;
                    long weight = (source[curX % sourceMaxX, curY % sourceMaxY].Weight - 1 + adjustment) % 9 + 1;
                    return new Node(weight);
                };

                for (int x = 0; x < maxX; ++x)
                {
                    for (int y = 0; y < maxY; ++y)
                    {
                        nodes[x, y] = GetAdjustedNode(source, x, y);
                    }
                }
            }

            // reset first state
            nodes[0, 0].Path = 0;
            nodes[0, 0].Weight = 0;
            nodes[0, 0].Prev = new Core.Point(0, 0);
            return nodes;
        }

        private void PrintNodes(Node[,] nodes, int maxX, int maxY)
        {
            for (int y = 0; y < maxY; ++y)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append($"{y,3} | ");
                for (int x = 0; x < maxX; ++x)
                {
                    sb.Append($"{nodes[x, y].Weight,1}");
                }
                DebugWriteLine(sb.ToString());
            }
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool enlargen)
        {
            Node[,] nodes = GetNodes(inputs, enlargen, out int maxX, out int maxY);
            Core.Point end = new Core.Point(maxX - 1, maxY - 1);

            PriorityQueue<Core.Point, long> gridWalker = new PriorityQueue<Core.Point, long>();
            gridWalker.Enqueue(new Core.Point(0, 0), 0);
            while (gridWalker.Count > 0)
            {
                Core.Point curPos = gridWalker.Dequeue();

                Node curNode = nodes[curPos.X, curPos.Y];
                if (curNode.Done)
                {
                    continue;
                }
                curNode.Done = true;

                Node prevNode = nodes[curNode.Prev.X, curNode.Prev.Y];
                curNode.Path = prevNode.Path + curNode.Weight;

                if (curPos.Equals(end))
                {
                    break;
                }

                foreach (Core.Point gridMove in GridMoves)
                {
                    Core.Point nextMove = curPos + gridMove;
                    if (nextMove.X >= 0 && nextMove.X < maxX && nextMove.Y >= 0 && nextMove.Y < maxY)
                    {
                        Node nextNode = nodes[nextMove.X, nextMove.Y];
                        if (!nextNode.Done)
                        {
                            if (nextNode.Prev != null)
                            {
                                Node existing = nodes[nextNode.Prev.X, nextNode.Prev.Y];
                                if (curNode.Path < existing.Path)
                                {
                                    nextNode.Prev = curPos;
                                }
                            }
                            else
                            {
                                nextNode.Prev = curPos;
                            }
                            gridWalker.Enqueue(nextMove, curNode.Path + nextNode.Weight);
                        }
                    }
                }
            }

            return nodes[maxX - 1, maxY - 1].Path.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);
    }
}