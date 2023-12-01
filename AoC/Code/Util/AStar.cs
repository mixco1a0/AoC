using System.Collections.Generic;
using System.Text;

namespace AoC.Util
{
    public class AStarNode
    {
        public Base.Pos2 Prev { get; set; }
        public bool Processed { get; set; }
        public long Length { get; set; }

        public AStarNode()
        {
            Prev = null;
            Processed = false;
            Length = long.MaxValue;
        }

        public override string ToString()
        {
            return Processed ? $"{Length,4}" : $"????";
        }
    }

    public class AStar<TNode>
        where TNode : AStarNode
    {
        private bool Initialized { get; set; }
        public int MaxX { get; private set; }
        public int MaxY { get; private set; }
        public TNode[,] Nodes { get; private set; }
        public char[,] Input { get; private set; }
        public Base.Pos2 Start { get; private set; }
        public Base.Pos2 End { get; private set; }

        public AStar(int maxX, int maxY)
        {
            Initialized = false;
            Start = new Base.Pos2(-1, -1);
            End = new Base.Pos2(-1, -1);
            MaxX = maxX;
            MaxY = maxY;
            Nodes = new TNode[MaxX, MaxY];
            Input = new char[MaxX, MaxY];
        }

        public delegate TNode InitializeNode(int x, int y);
        public delegate bool IsNode(int x, int y);

        public void Initialize(List<string> inputs, InitializeNode initializeNode, IsNode isStartNode, IsNode isEndNode)
        {
            for (int x = 0; x < MaxX; ++x)
            {
                for (int y = 0; y < MaxY; ++y)
                {
                    Input[x, y] = inputs[y][x];
                    Nodes[x, y] = initializeNode(x, y);
                    if (isStartNode(x, y))
                    {
                        Start = new Base.Pos2(x, y);
                    }
                    if (isEndNode != null && isEndNode(x, y))
                    {
                        End = new Base.Pos2(x, y);
                    }
                }
            }

            Initialized = true;
        }

        public delegate TNode GetExpandedNode(TNode[,] originalNodes, int x, int y, int originalMaxX, int originalMaxY);
        public delegate char GetExpandedInput(char[,] originalInput, int x, int y);
        public void Expand(int multiplier, GetExpandedNode getExpandedNode, GetExpandedInput getExpandedRaw, IsNode isStartNode, IsNode isEndNode)
        {
            TNode[,] originalNodes = Nodes;
            char[,] originalInput = Input;
            int originalMaxX = MaxX;
            int originalMaxY = MaxY;

            MaxX *= multiplier;
            MaxY *= multiplier;
            Nodes = new TNode[MaxX, MaxY];
            Input = new char[MaxX, MaxY];

            for (int x = 0; x < MaxX; ++x)
            {
                for (int y = 0; y < MaxY; ++y)
                {
                    Nodes[x, y] = getExpandedNode(originalNodes, x, y, originalMaxX, originalMaxY);
                    Input[x, y] = getExpandedRaw(originalInput, x, y);
                    if (isStartNode(x, y))
                    {
                        Start = new Base.Pos2(x, y);
                    }
                    if (isEndNode != null && isEndNode(x, y))
                    {
                        End = new Base.Pos2(x, y);
                    }
                }
            }
        }

        static readonly Base.Pos2[] NeighborOffsets = new Base.Pos2[] { new Base.Pos2(0, 1), new Base.Pos2(1, 0), new Base.Pos2(-1, 0), new Base.Pos2(0, -1) };
        static readonly char[] NeighborDirection = new char[] { '↓', '→', '←', '↑' };

        public delegate bool CanUseNode(TNode curNode, TNode nextNode);
        public delegate bool IsEnd(Base.Pos2 curPos);
        public delegate long AdjustLength(TNode curNode, TNode prevNode);
        public delegate long GetPriority(TNode curNode, TNode nextNode);
        public void Process(CanUseNode canUseNode = null, IsEnd isEnd = null, AdjustLength adjustLength = null, GetPriority getPriority = null)
        {
            if (!Initialized)
            {
                return;
            }

            // by default, all nodes should be usable
            if (canUseNode == null)
            {
                canUseNode = (TNode curNode, TNode nextNode) => { return true; };
            }

            // by default, check for the end position
            if (isEnd == null)
            {
                isEnd = (Base.Pos2 curPos) => { return curPos.Equals(End); };
            }

            // by default, add 1 to the previous length
            if (adjustLength == null)
            {
                adjustLength = (TNode curNode, TNode prevNode) => { return prevNode.Length + 1; };
            }

            // by default, use the next node's length as the priority
            if (getPriority == null)
            {
                getPriority = (TNode curNode, TNode nextNode) => { return nextNode.Length; }; ;
            }

            // reset starting node information
            Nodes[Start.X, Start.Y].Prev = Start;
            Nodes[Start.X, Start.Y].Length = 0;

            // walk all the nodes
            PriorityQueue<Base.Pos2, long> priorityQueue = new PriorityQueue<Base.Pos2, long>();
            priorityQueue.Enqueue(Start, 0);
            while (priorityQueue.Count > 0)
            {
                Base.Pos2 curPos = priorityQueue.Dequeue();
                TNode curNode = Nodes[curPos.X, curPos.Y];

                // make sure nodes are only processed once
                if (curNode.Processed)
                {
                    continue;
                }
                curNode.Processed = true;

                // check for completion
                if (isEnd(curPos))
                {
                    // pathing is complete, set the end position
                    End = curPos;
                    break;
                }

                // check all neighbors
                TNode prevNode = Nodes[curNode.Prev.X, curNode.Prev.Y];
                foreach (Base.Pos2 neighborOffset in NeighborOffsets)
                {
                    Base.Pos2 nextPos = curPos + neighborOffset;
                    if (nextPos.X >= 0 && nextPos.X < MaxX && nextPos.Y >= 0 && nextPos.Y < MaxY)
                    {
                        TNode nextNode = Nodes[nextPos.X, nextPos.Y];
                        if (!nextNode.Processed && canUseNode(curNode, nextNode))
                        {
                            if (nextNode.Prev != null)
                            {
                                TNode nextPrevNode = Nodes[nextNode.Prev.X, nextNode.Prev.Y];
                                if (curNode.Length < nextPrevNode.Length)
                                {
                                    nextNode.Prev = curPos;
                                }
                            }
                            else
                            {
                                nextNode.Prev = curPos;
                                nextNode.Length = adjustLength(nextNode, curNode);
                            }
                            priorityQueue.Enqueue(nextPos, getPriority(curNode, nextNode));
                        }
                    }
                }
            }
        }

        public string GetOptimalPath()
        {
            return Nodes[End.X, End.Y].Length.ToString();
        }

        public delegate char PrintOverride(int x, int y);
        public void PrintNodes(Core.Log.ELevel level = Core.Log.ELevel.Debug, PrintOverride printOverride = null)
        {
            for (int y = 0; y < MaxY; ++y)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append($"{y,3} | ");
                for (int x = 0; x < MaxX; ++x)
                {
                    if (printOverride != null)
                    {
                        sb.Append($"{printOverride(x, y),1}");
                    }
                    else
                    {
                        sb.Append($"{Input[x, y],1}");
                    }
                }
                Core.Log.WriteLine(level, sb.ToString());
            }
            Core.Log.WriteLine(level, "");
        }

        public void PrintPath(Core.Log.ELevel level = Core.Log.ELevel.Debug)
        {
            char[,] path = Input;
            Base.Pos2 curPos = End;
            while (curPos != null)
            {
                TNode curNode = Nodes[curPos.X, curPos.Y];
                Base.Pos2 prevPos = curNode.Prev;

                if (prevPos.Equals(curPos))
                {
                    break;
                }

                Base.Pos2 direction = curPos - prevPos;
                for (int i = 0; i < NeighborOffsets.Length; ++i)
                {
                    if (NeighborOffsets[i].Equals(direction))
                    {
                        path[prevPos.X, prevPos.Y] = NeighborDirection[i];
                        break;
                    }
                }
                curPos = curNode.Prev;
            }

            path[Start.X, Start.Y] = 'S';
            path[End.X, End.Y] = 'E';
            PrintNodes(level, (int x, int y) => { return path[x, y]; });
            Core.Log.WriteLine(level, "");
        }
    }
}