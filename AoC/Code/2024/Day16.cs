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
                    Output = "2006",
                    RawInput =
@"#######
##...E#
##.#.##
#S...##
#######"
                },
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
                    Output = "10",
                    RawInput =
@"#######
##...E#
##.#.##
#S...##
#######"
                },
                new Core.TestDatum
                {
                    TestPart = Core.Part.Two,
                    Output = "45",
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
                    TestPart = Core.Part.Two,
                    Output = "64",
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
            ];
            return testData;
        }

        private const char Start = 'S';
        private const char End = 'E';
        private const char Wall = '#';
        private const char Path = '.';

        private record DirectedVec2(Util.Grid2.Dir Dir, Base.Vec2 Vec2);

        private record ScoredDV2(DirectedVec2 DV2, int Score, List<DirectedVec2> History);

        private void GetPaths(List<string> inputs, out Base.Grid2Char grid, out HashSet<Base.Vec2> paths, out Base.Vec2 start, out Base.Vec2 end)
        {
            grid = new(inputs);
            paths = [];
            start = new();
            end = new();
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
        }

        private static void PrintPath(Base.Grid2Char grid, ScoredDV2 sdv2)
        {
            Base.Grid2Char g = new(grid);
            foreach (var h in sdv2.History)
            {
                g[h.Vec2] = Util.Grid2.Map.Arrow[h.Dir];
            }
            g.Print(Core.Log.ELevel.Spam);
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool findAllTiles)
        {
            GetPaths(inputs, out Base.Grid2Char grid, out HashSet<Base.Vec2> paths, out Base.Vec2 start, out Base.Vec2 end);

            int pathCount = 0;
            int minScore = int.MaxValue;

            Dictionary<Base.Vec2, int> pathScores = [];

            HashSet<Base.Vec2> bestTiles = [];
            HashSet<DirectedVec2> visited = [];
            PriorityQueue<ScoredDV2, int> path = new();
            path.Enqueue(new ScoredDV2(new DirectedVec2(Util.Grid2.Dir.East, start), 0, []), 0);
            while (path.Count > 0)
            {
                ScoredDV2 sdv2 = path.Dequeue();
                if (findAllTiles)
                {
                    // PrintPath(grid, sdv2);
                    if (sdv2.Score > minScore)
                    {
                        continue;
                    }

                    if (sdv2.History.Select(h => h.Vec2).Contains(sdv2.DV2.Vec2))
                    {
                        List<Util.Grid2.Dir> dirs = sdv2.History.Where(h => h.Vec2.Equals(sdv2.DV2.Vec2)).Select(h => h.Dir).ToList();
                        if (dirs.Contains(sdv2.DV2.Dir) || dirs.Contains(Util.Grid2.Map.Opposite[sdv2.DV2.Dir]))
                        {
                            continue;
                        }
                    }
                }
                else
                {
                    if (visited.Contains(sdv2.DV2))
                    {
                        continue;
                    }
                }
                visited.Add(sdv2.DV2);
                sdv2.History.Add(sdv2.DV2);

                // Log($"{sdv2}");
                // grid.PrintNextArrow(Core.Log.ELevel.Spam, sdv2.DV2.Vec2, sdv2.DV2.Dir);

                // win condition
                if (sdv2.DV2.Vec2.Equals(end))
                {
                    if (findAllTiles)
                    {
                        if (sdv2.Score <= minScore)
                        {
                            minScore = sdv2.Score;
                            ++pathCount;

                            PrintPath(grid, sdv2);
                            foreach (var pair in sdv2.History)
                            {
                                bestTiles.Add(pair.Vec2);
                            }
                            continue;
                        }
                    }
                    else
                    {
                        // PrintPath(grid, sdv2);
                        return sdv2.Score.ToString();
                    }
                }

                // going straight is worth 1 point
                Base.Vec2 next = sdv2.DV2.Vec2 + Util.Grid2.Map.Neighbor[sdv2.DV2.Dir];
                if (paths.Contains(next))
                {
                    path.Enqueue(new ScoredDV2(new DirectedVec2(sdv2.DV2.Dir, next), sdv2.Score + 1, [.. sdv2.History]), sdv2.Score + 1);
                }

                path.Enqueue(new ScoredDV2(new DirectedVec2(Util.Grid2.Map.RotateCW[sdv2.DV2.Dir], sdv2.DV2.Vec2), sdv2.Score + 1000, [.. sdv2.History]), sdv2.Score + 1000);
                path.Enqueue(new ScoredDV2(new DirectedVec2(Util.Grid2.Map.RotateCCW[sdv2.DV2.Dir], sdv2.DV2.Vec2), sdv2.Score + 1000, [.. sdv2.History]), sdv2.Score + 1000);
            }

            return bestTiles.Count.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);
    }
}