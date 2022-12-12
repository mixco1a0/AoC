using System;
using System.Collections.Generic;
using System.Text;

namespace AoC._2022
{
    class Day12 : Core.Day
    {
        public Day12() { }

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
                Output = "31",
                RawInput =
@"Sabqponm
abcryxxl
accszExk
acctuvwj
abdefghi"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
                Output = "29",
                RawInput =
@"Sabqponm
abcryxxl
accszExk
acctuvwj
abdefghi"
            });
            return testData;
        }

        static readonly Base.Point[] GridMoves = new Base.Point[] { new Base.Point(0, 1), new Base.Point(1, 0), new Base.Point(-1, 0), new Base.Point(0, -1) };

        private class Node
        {
            public long Height { get; set; }
            public Base.Point Prev { get; set; }
            public bool Done { get; set; }
            public long Path { get; set; }

            public Node(long height)
            {
                Height = height;
                Prev = null;
                Done = false;
                Path = long.MaxValue;
            }

            public override string ToString()
            {
                return Done ? $"{Path,4}" : $"?{Height,2}?";
            }
        }

        private Node[,] GetNodes(List<string> inputs, out Base.Point start, out Base.Point end, out int maxX, out int maxY, bool reverse)
        {
            start = new Base.Point(-1, -1);
            end = new Base.Point(-1, -1);
            maxX = inputs[0].Length;
            maxY = inputs.Count;
            Node[,] nodes = new Node[maxX, maxY];
            for (int x = 0; x < maxX; ++x)
            {
                for (int y = 0; y < maxY; ++y)
                {
                    switch (inputs[y][x])
                    {
                        case 'S':
                            if (reverse)
                            {
                                nodes[x, y] = new Node(0);
                            }
                            else
                            {
                                start = new Base.Point(x, y);
                                nodes[x, y] = new Node(0) { Prev = start, Path = 0 };
                            }
                            break;
                        case 'E':
                            if (reverse)
                            {
                                start = new Base.Point(x, y);
                                nodes[x, y] = new Node('z' - 'a') { Prev = start, Path = 0 };
                            }
                            else
                            {
                                end = new Base.Point(x, y);
                                nodes[x, y] = new Node('z' - 'a');
                            }
                            break;
                        default:
                            nodes[x, y] = new Node(inputs[y][x] - 'a');
                            break;
                    }
                }
            }
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
                    sb.Append($"{(char)(nodes[x, y].Height + 'a'),1}");
                }
                DebugWriteLine(sb.ToString());
            }
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool reverse)
        {
            Node[,] nodes = GetNodes(inputs, out Base.Point start, out Base.Point end, out int maxX, out int maxY, reverse);
            PriorityQueue<Base.Point, long> gridWalker = new PriorityQueue<Base.Point, long>();
            gridWalker.Enqueue(start, 0);
            while (gridWalker.Count > 0)
            {
                Base.Point curPos = gridWalker.Dequeue();

                Node curNode = nodes[curPos.X, curPos.Y];
                if (curNode.Done)
                {
                    continue;
                }
                curNode.Done = true;

                if (reverse)
                {
                    if (curNode.Height == 0)
                    {
                        return curNode.Path.ToString();
                    }
                }
                else
                {
                    if (curPos.CompareTo(end) == 0)
                    {
                        return curNode.Path.ToString();
                    }
                }

                foreach (Base.Point gridMove in GridMoves)
                {
                    Base.Point nextMove = curPos + gridMove;
                    if (nextMove.X >= 0 && nextMove.X < maxX && nextMove.Y >= 0 && nextMove.Y < maxY)
                    {
                        Node nextNode = nodes[nextMove.X, nextMove.Y];
                        Func<bool> canMoveTo = () =>
                        {
                            if (reverse)
                            {
                                return curNode.Height - 1 <= nextNode.Height;
                            }
                            else
                            {
                                return curNode.Height + 1 >= nextNode.Height;
                            }
                        };
                        if (canMoveTo())
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
                                nextNode.Path = curNode.Path + 1;
                            }
                            gridWalker.Enqueue(nextMove, nextNode.Path);
                        }
                    }
                }
            }
            return string.Empty;
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);
    }
}