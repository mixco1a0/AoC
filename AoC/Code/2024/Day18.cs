using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2024
{
    class Day18 : Core.Day
    {
        public Day18() { }

        public override string GetSolutionVersion(Core.Part part)
        {
            return part switch
            {
                // Core.Part.One => "v1",
                // Core.Part.Two => "v1",
                _ => base.GetSolutionVersion(part),
            };
        }

        protected override List<Core.TestDatum> GetTestData()
        {
            List<Core.TestDatum> testData =
            [
                new Core.TestDatum
                {
                    TestPart = Core.Part.One,
                    Output = "22",
                    Variables = new Dictionary<string, string> { { nameof(_GridSize), "6" }, { nameof(_CorruptedCount), "12" } },
                    RawInput =
@"5,4
4,2
4,5
3,0
2,1
6,3
2,4
1,5
0,6
3,3
2,6
5,1
1,2
5,5
2,5
6,5
1,4
0,4
6,4
1,1
6,1
1,0
0,5
1,6
2,0"
                },
                new Core.TestDatum
                {
                    TestPart = Core.Part.Two,
                    Output = "",
                    RawInput =
@""
                },
            ];
            return testData;
        }


#pragma warning disable IDE1006 // Naming Styles
        private static int _GridSize { get; }
        private static int _CorruptedCount { get; }
#pragma warning restore IDE1006 // Naming Styles

        private record Vec2Score(Base.Vec2 Vec2, int Score);

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool _)
        {
            GetVariable(nameof(_GridSize), 70, variables, out int gridSize);
            GetVariable(nameof(_CorruptedCount), 1024, variables, out int corruptedCount);
            List<Base.Vec2> bytes = inputs.Select(i => Util.String.Split(i, ',').Select(int.Parse).ToArray()).Select(xy => new Base.Vec2(xy.First(), xy.Last())).ToList();
            HashSet<Base.Vec2> corruptedBytes = [];
            foreach (Base.Vec2 cb in bytes[..corruptedCount])
            {
                corruptedBytes.Add(cb);
            }
            Base.Grid2Int grid = new(gridSize + 1, gridSize + 1, int.MaxValue);
            PriorityQueue<Vec2Score, int> queue = new();
            queue.Enqueue(new(new(0, 0), 0), 0);
            while (queue.Count > 0)
            {
                Vec2Score v2s = queue.Dequeue();
                int score = grid[v2s.Vec2];
                if (score < v2s.Score)
                {
                    continue;
                }
                grid[v2s.Vec2] = v2s.Score;
                if (score != int.MaxValue)
                {
                    continue;
                }

                foreach (Util.Grid2.Dir dir in Util.Grid2.Iter.Cardinal)
                {
                    int nextScore = v2s.Score + 1;
                    Base.Vec2 next = v2s.Vec2 + Util.Grid2.Map.Neighbor[dir];
                    if (grid.Contains(next) && !corruptedBytes.Contains(next))
                    {
                        queue.Enqueue(new(next, nextScore), nextScore);
                    }
                }
            }
            return grid[gridSize, gridSize].ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);
    }
}