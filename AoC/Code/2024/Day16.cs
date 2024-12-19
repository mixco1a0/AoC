using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2024
{
    class Day16 : Core.Day
    {
        public Day16() { }

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
                    Output = "7036",
                    RawInput =
@"###############
#.......#....E#
#.#.###.#.###.#
#.....#.#...#.#
#.###.#####.#.#
#.#.#.......#.#
#.#.#####.###.#
#...........#.#
###.#.#####.#.#
#...#.....#.#.#
#.#.#.###.#.#.#
#.....#...#.#.#
#.###.#.#.#.#.#
#S..#.....#...#
###############"
                },
                new Core.TestDatum
                {
                    TestPart = Core.Part.One,
                    Output = "11048",
                    RawInput =
@"#################
#...#...#...#..E#
#.#.#.#.#.#.#.#.#
#.#.#.#...#...#.#
#.#.#.#.###.#.#.#
#...#.#.#.....#.#
#.#.#.#.#.#####.#
#.#...#.#.#.....#
#.#.#####.#.###.#
#.#.#.......#...#
#.#.###.#####.###
#.#.#...#.....#.#
#.#.#.#####.###.#
#.#.#.........#.#
#.#.#.#########.#
#S#.............#
#################"
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

        private const char Start = 'S';
        private const char End = 'E';
        private const char Wall = '#';
        private const char Path = '.';

        private record DirectedVec2(Util.Grid2.Dir Dir, Base.Vec2 Vec2);

        private record ScoredDV2(DirectedVec2 DV2, int Score);

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool _)
        {
            Base.Grid2Char grid = new(inputs);
            HashSet<Base.Vec2> paths = [];
            Base.Vec2 start = new();
            Base.Vec2 end = new();
            foreach (Base.Vec2 vec2 in grid)
            {
                switch (grid[vec2])
                {
                    case Start:
                        start = vec2;
                        break;
                    case End:
                        end = vec2;
                        paths.Add(vec2);
                        break;
                    case Path:
                        paths.Add(vec2);
                        break;
                }
            }

            HashSet<DirectedVec2> visited = [];
            PriorityQueue<ScoredDV2, int> path = new();
            path.Enqueue(new ScoredDV2(new DirectedVec2(Util.Grid2.Dir.East, start), 0), 0);
            while (path.Count > 0)
            {
                ScoredDV2 sdv2 = path.Dequeue();
                if (visited.Contains(sdv2.DV2))
                {
                    continue;
                }
                visited.Add(sdv2.DV2);

                // Log($"{sdv2}");
                // grid.PrintNextArrow(Core.Log.ELevel.Spam, sdv2.DV2.Vec2, sdv2.DV2.Dir);

                // win condition
                if (sdv2.DV2.Vec2.Equals(end))
                {
                    return sdv2.Score.ToString();
                }

                // going straight is worth 1 point
                Base.Vec2 next = sdv2.DV2.Vec2 + Util.Grid2.Map.Neighbor[sdv2.DV2.Dir];
                if (paths.Contains(next))
                {
                    path.Enqueue(new ScoredDV2(new DirectedVec2(sdv2.DV2.Dir, next), sdv2.Score + 1), sdv2.Score + 1);
                }

                path.Enqueue(new ScoredDV2(new DirectedVec2(Util.Grid2.Map.RotateCW[sdv2.DV2.Dir], sdv2.DV2.Vec2), sdv2.Score + 1000), sdv2.Score + 1000);
                path.Enqueue(new ScoredDV2(new DirectedVec2(Util.Grid2.Map.RotateCCW[sdv2.DV2.Dir], sdv2.DV2.Vec2), sdv2.Score + 1000), sdv2.Score + 1000);
            }
            return string.Empty;
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);
    }
}