using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2024
{
    class Day20 : Core.Day
    {
        public Day20() { }

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
                    Output = "10",
                    Variables = new Dictionary<string, string> { { nameof(_PicosSaved), "10" } },
                    RawInput =
@"###############
#...#...#.....#
#.#.#.#.#.###.#
#S#...#.#.#...#
#######.#.#.###
#######.#.#...#
#######.#.###.#
###..E#...#...#
###.#######.###
#...###...#...#
#.#####.#.###.#
#.#...#.#.#...#
#.#.#.#.#.#.###
#...#...#...###
###############"
                },
                new Core.TestDatum
                {
                    TestPart = Core.Part.One,
                    Output = "1",
                    Variables = new Dictionary<string, string> { { nameof(_PicosSaved), "50" } },
                    RawInput =
@"###############
#...#...#.....#
#.#.#.#.#.###.#
#S#...#.#.#...#
#######.#.#.###
#######.#.#...#
#######.#.###.#
###..E#...#...#
###.#######.###
#...###...#...#
#.#####.#.###.#
#.#...#.#.#...#
#.#.#.#.#.#.###
#...#...#...###
###############"
                },
                new Core.TestDatum
                {
                    TestPart = Core.Part.Two,
                    Output = "",
                    Variables = new Dictionary<string, string> { { nameof(_PicosSaved), "0" } },
                    RawInput =
@""
                },
            ];
            return testData;
        }
#pragma warning disable IDE1006 // Naming Styles
        private static int _PicosSaved { get; }
#pragma warning restore IDE1006 // Naming Styles

        private enum Space
        {
            Start = 'S',
            Path = '.',
            End = 'E',
            Wall = '#',
        }

        Base.Vec2 Start { get; set; }
        Base.Vec2 End { get; set; }
        Base.Grid2Char Grid { get; set; }
        Dictionary<Base.Vec2, int> Path { get; set; }

        private void InitializePath(List<string> inputs)
        {
            Start = new();
            End = new();
            Grid = new(inputs);
            Path = [];
            foreach (Base.Vec2 vec2 in Grid)
            {
                switch ((Space)Grid[vec2])
                {
                    case Space.Start:
                        Start = vec2;
                        Path[vec2] = 0;
                        Grid[vec2] = (char)Space.Path;
                        break;
                    case Space.Path:
                        Path[vec2] = int.MaxValue;
                        break;
                    case Space.End:
                        End = vec2;
                        Path[vec2] = int.MaxValue;
                        Grid[vec2] = (char)Space.Path;
                        break;
                    case Space.Wall:
                        break;
                }
            }
        }

        private void GeneratePathScores()
        {
            int score = 0;
            for (Base.Vec2 pos = Start; !pos.Equals(End);)
            {
                foreach (Util.Grid2.Dir dir in Util.Grid2.Iter.Cardinal)
                {
                    Base.Vec2 next = pos + Util.Grid2.Map.Neighbor[dir];
                    if (Grid.Contains(next) && Grid[next] == (char)Space.Path && Path[next] == int.MaxValue)
                    {
                        Path[pos] = score++;
                        pos = next;
                        break;
                    }
                }
            }
            Path[End] = score;
        }

        private int FindCheats(int picosSaved, int cheatPicos)
        {
            int cheatCount = 0;
            foreach (Base.Vec2 pos in Path.Keys)
            {
                Base.Grid2CharScanner scanner = new(Grid, pos, cheatPicos);
                foreach (Base.Vec2 next in scanner)
                {
                    if (next.Equals(pos))
                    {
                        continue;
                    }

                    if (Path.TryGetValue(next, out int score))
                    {
                        int savings = score - Path[pos] - pos.Manhattan(next);
                        if (savings >= picosSaved)
                        {
                            ++cheatCount;
                        }
                        // Log($"{score - Path[pos] - 2} picos saved");
                        // Base.Vec2 next = pos + Util.Grid2.Map.Neighbor[dir];
                        // Grid.PrintNextArrow(Core.Log.ELevel.Spam, next, dir);
                    }
                }
            }
            return cheatCount;
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, int cheatPicos)
        {
            GetVariable(nameof(_PicosSaved), 100, variables, out int picosSaved);
            InitializePath(inputs);
            GeneratePathScores();
            return FindCheats(picosSaved, cheatPicos).ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, 2);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, 20);
    }
}