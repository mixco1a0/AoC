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
                Core.Part.One => "v1",
                Core.Part.Two => "v1",
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
                    Output = "5",
                    RawInput =
@"#####
#..E#
#.#.#
#S..#
#####"
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

        private record ScoredDV2(DirectedVec2 DV2, int Score);

        private static void GetPaths(List<string> inputs, out Base.Grid2Char grid, out HashSet<Base.Vec2> paths, out Base.Vec2 start, out Base.Vec2 end)
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
                        paths.Add(vec2);
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

        // private static void PrintPath(Base.Grid2Char grid, List<DirectedVec2> path)
        // {
        //     Base.Grid2Char g = new(grid);
        //     foreach (var p in path)
        //     {
        //         g[p.Vec2] = Util.Grid2.Map.Arrow[p.Dir];
        //     }
        //     g.Print(Core.Log.ELevel.Spam);
        // }

        private static readonly Dictionary<Util.Grid2.Dir, int> DirToIdx = new()
        {
            {Util.Grid2.Dir.North, 0},
            {Util.Grid2.Dir.East, 1},
            {Util.Grid2.Dir.South, 2},
            {Util.Grid2.Dir.West, 3}
        };

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool findAllTiles)
        {
            GetPaths(inputs, out Base.Grid2Char grid, out HashSet<Base.Vec2> paths, out Base.Vec2 start, out Base.Vec2 end);

            // keep track of each path's best possible score in every direction
            Dictionary<Base.Vec2, int[]> pathScores = [];
            if (findAllTiles)
            {
                foreach (Base.Vec2 p in paths)
                {
                    pathScores[p] = [int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue];
                }
                pathScores[start] = [int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue];
                pathScores[end] = [int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue];
            }

            int minScore = int.MaxValue;
            HashSet<DirectedVec2> visited = [];
            PriorityQueue<ScoredDV2, int> path = new();
            path.Enqueue(new ScoredDV2(new DirectedVec2(Util.Grid2.Dir.East, start), 0), 0);
            while (path.Count > 0)
            {
                ScoredDV2 sdv2 = path.Dequeue();

                // only visit each location + direction once
                if (visited.Contains(sdv2.DV2))
                {
                    continue;
                }
                visited.Add(sdv2.DV2);

                // update the location's best score
                if (findAllTiles)
                {
                    if (sdv2.Score > minScore)
                    {
                        continue;
                    }
                    pathScores[sdv2.DV2.Vec2][DirToIdx[sdv2.DV2.Dir]] = sdv2.Score;
                }

                // win condition
                if (sdv2.DV2.Vec2.Equals(end))
                {
                    if (findAllTiles)
                    {
                        minScore = sdv2.Score;
                    }
                    else
                    {
                        return sdv2.Score.ToString();
                    }
                }

                // going straight is worth 1 point
                Base.Vec2 next = sdv2.DV2.Vec2 + Util.Grid2.Map.Neighbor[sdv2.DV2.Dir];
                if (paths.Contains(next))
                {
                    path.Enqueue(new ScoredDV2(new DirectedVec2(sdv2.DV2.Dir, next), sdv2.Score + 1), sdv2.Score + 1);
                }

                // only turn clockwise if going straight is possible after
                next = sdv2.DV2.Vec2 + Util.Grid2.Map.Neighbor[Util.Grid2.Map.RotateCW[sdv2.DV2.Dir]];
                if (paths.Contains(next))
                {
                    path.Enqueue(new ScoredDV2(new DirectedVec2(Util.Grid2.Map.RotateCW[sdv2.DV2.Dir], sdv2.DV2.Vec2), sdv2.Score + 1000), sdv2.Score + 1000);
                }

                // only turn counter clockwise if going straight is possible after
                next = sdv2.DV2.Vec2 + Util.Grid2.Map.Neighbor[Util.Grid2.Map.RotateCCW[sdv2.DV2.Dir]];
                if (paths.Contains(next))
                {
                    path.Enqueue(new ScoredDV2(new DirectedVec2(Util.Grid2.Map.RotateCCW[sdv2.DV2.Dir], sdv2.DV2.Vec2), sdv2.Score + 1000), sdv2.Score + 1000);
                }
            }

            // go from end to start going down in score
            visited.Clear();
            HashSet<Base.Vec2> bestPath = [];
            Queue<DirectedVec2> pathCheck = [];
            foreach (Util.Grid2.Dir dir in Util.Grid2.Iter.Cardinal)
            {
                if (pathScores[end][DirToIdx[dir]] == minScore)
                {
                    pathCheck.Enqueue(new(dir, end));
                }
            }

            while (pathCheck.Count > 0)
            {
                DirectedVec2 dv2 = pathCheck.Dequeue();
                if (visited.Contains(dv2))
                {
                    continue;
                }
                visited.Add(dv2);
                bestPath.Add(dv2.Vec2);

                // move backwards
                int curScore = pathScores[dv2.Vec2][DirToIdx[dv2.Dir]];
                Base.Vec2 prevPath = dv2.Vec2 + Util.Grid2.Map.Neighbor[Util.Grid2.Map.Opposite[dv2.Dir]];
                if (paths.Contains(prevPath))
                {
                    int prevScore = pathScores[prevPath][DirToIdx[dv2.Dir]];
                    if (prevScore < curScore)
                    {
                        pathCheck.Enqueue(new(dv2.Dir, prevPath));
                    }
                }

                // undo clockwise rotate
                Util.Grid2.Dir ccw = Util.Grid2.Map.RotateCCW[dv2.Dir];
                if (pathScores[dv2.Vec2][DirToIdx[ccw]] < curScore)
                {
                    pathCheck.Enqueue(new(ccw, dv2.Vec2));
                }

                // undo counter clockwise rotate
                Util.Grid2.Dir cw = Util.Grid2.Map.RotateCW[dv2.Dir];
                if (pathScores[dv2.Vec2][DirToIdx[cw]] < curScore)
                {
                    pathCheck.Enqueue(new(cw, dv2.Vec2));
                }
            }

            return bestPath.Count.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);
    }
}