using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AoC._2023
{
    class Day23 : Core.Day
    {
        public Day23() { }

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
                Output = "94",
                RawInput =
@"#.#####################
#.......#########...###
#######.#########.#.###
###.....#.>.>.###.#.###
###v#####.#v#.###.#.###
###.>...#.#.#.....#...#
###v###.#.#.#########.#
###...#.#.#.......#...#
#####.#.#.#######.#.###
#.....#.#.#.......#...#
#.#####.#.#.#########v#
#.#...#...#...###...>.#
#.#.#v#######v###.###v#
#...#.>.#...>.>.#.###.#
#####v#.#.###v#.#.###.#
#.....#...#...#.#.#...#
#.#########.###.#.#.###
#...###...#...#...#.###
###.###.#.###v#####v###
#...#...#.#.>.>.#.>.###
#.###.###.#.###.#.#v###
#.....###...###...#...#
#####################.#"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
                Output = "154",
                RawInput =
@"#.#####################
#.......#########...###
#######.#########.#.###
###.....#.>.>.###.#.###
###v#####.#v#.###.#.###
###.>...#.#.#.....#...#
###v###.#.#.#########.#
###...#.#.#.......#...#
#####.#.#.#######.#.###
#.....#.#.#.......#...#
#.#####.#.#.#########v#
#.#...#...#...###...>.#
#.#.#v#######v###.###v#
#...#.>.#...>.>.#.###.#
#####v#.#.###v#.#.###.#
#.....#...#...#.#.#...#
#.#########.###.#.#.###
#...###...#...#...#.###
###.###.#.###v#####v###
#...#...#.#.>.>.#.>.###
#.###.###.#.###.#.#v###
#.....###...###...#...#
#####################.#"
            });
            return testData;
        }

        private static char Path = '.';
        private static char Forest = '#';

        static readonly Base.Vec2L[] GridMoves = new Base.Vec2L[] { new Base.Vec2L(0, 1), new Base.Vec2L(1, 0), new Base.Vec2L(-1, 0), new Base.Vec2L(0, -1) };
        static readonly Dictionary<char, Base.Vec2[]> PathMoves = new Dictionary<char, Base.Vec2[]>()
        {
            {Path, new Base.Vec2[] { new Base.Vec2(0, 1), new Base.Vec2(1, 0), new Base.Vec2(-1, 0), new Base.Vec2(0, -1) }},
            {'^',  new Base.Vec2[] { new Base.Vec2(0, -1) }},
            {'>',  new Base.Vec2[] { new Base.Vec2(1, 0)  }},
            {'<',  new Base.Vec2[] { new Base.Vec2(-1, 0) }},
            {'v',  new Base.Vec2[] { new Base.Vec2(0, 1)  }},
        };

        private void ParseInput(List<string> inputs, bool slippery, out char[,] grid, out Base.Vec2 start, out Base.Vec2 end, out int xMax, out int yMax)
        {
            Util.Grid.ParseInput(inputs, out grid, out xMax, out yMax);

            if (!slippery)
            {
                for (int x = 0; x < xMax; ++x)
                {
                    for (int y = 0; y < yMax; ++y)
                    {
                        if (grid[x, y] != Path && grid[x, y] != Forest)
                        {
                            grid[x, y] = Path;
                        }
                    }
                }
            }

            start = new Base.Vec2();
            end = new Base.Vec2();
            for (int x = 0; x < xMax; ++x)
            {
                if (grid[x, 0] == Path)
                {
                    start = new Base.Vec2(x, 0);
                }
                if (grid[x, yMax - 1] == Path)
                {
                    end = new Base.Vec2(x, yMax - 1);
                }
            }
        }

        private class Trail
        {
            public Base.Vec2 Pos { get; set; }
            public Dictionary<Base.Vec2, int> Paths { get; set; }

            public Trail()
            {
                Pos = new Base.Vec2();
                Paths = new Dictionary<Base.Vec2, int>();
            }

            public override string ToString()
            {
                StringBuilder stringBuilder = new StringBuilder();
                if (Paths.Count > 0)
                {
                    foreach (var pair in Paths)
                    {
                        stringBuilder.Append($"{pair.Key} -> {pair.Value} | ");
                    }
                    stringBuilder.Length -= 3;
                    return $"{Pos} -> | {stringBuilder}";
                }
                return $"{Pos}";
            }
        }

        private void GenerateTrails(char[,] grid, Base.Vec2 start, Base.Vec2 curPos, Base.Vec2 end, int xMax, int yMax, ref Dictionary<Base.Vec2, Trail> trails)
        {
            // Log($"Checking {start}");
            HashSet<Base.Vec2> visited = new HashSet<Base.Vec2>() { start };
            HashSet<Base.Vec2> split = new HashSet<Base.Vec2>();
            int length = start.Manhattan(curPos);
            while (true)
            {
                char path = grid[curPos.X, curPos.Y];
                // grid[curPos.X, curPos.Y] = '@';
                // Util.Grid.PrintGrid(grid);
                // grid[curPos.X, curPos.Y] = path;

                if (curPos.Equals(end))
                {
                    trails[start].Paths[curPos] = length;
                    return;
                }

                List<Base.Vec2> potentials = new List<Base.Vec2>();
                foreach (Base.Vec2 movePos2 in PathMoves[path])
                {
                    Base.Vec2 nextPos2 = curPos + movePos2;
                    if (nextPos2.X >= 0 && nextPos2.X < xMax && nextPos2.Y >= 0 && nextPos2.Y < yMax && grid[nextPos2.X, nextPos2.Y] != Forest)
                    {
                        potentials.Add(nextPos2);
                    }
                }

                IEnumerable<Base.Vec2> unique = potentials.Where(p => !visited.Contains(p));
                if (unique.Count() == 1)
                {
                    visited.Add(curPos);
                    curPos = unique.First();
                    ++length;
                    continue;
                }
                else if (potentials.Count > 1)
                {
                    split = potentials.ToHashSet();
                    break;
                }
                else
                {
                    return;
                }
            }

            if (start.Equals(curPos))
            {
                return;
            }

            if (trails[start].Paths.ContainsKey(curPos))
            {
                trails[start].Paths[curPos] = Math.Max(length, trails[start].Paths[curPos]);
            }
            else
            {
                trails[start].Paths[curPos] = length;
            }

            if (!trails.ContainsKey(curPos))
            {
                trails[curPos] = new Trail() { Pos = curPos };

                foreach (Base.Vec2 pos2 in split)
                {
                    GenerateTrails(grid, curPos, pos2, end, xMax, yMax, ref trails);
                }
            }
        }

        private bool GetLongestTrail(Dictionary<Base.Vec2, Trail> trails, Base.Vec2 start, Base.Vec2 end, HashSet<Base.Vec2> history, out int longestTrail)
        {
            history.Add(start);
            longestTrail = 0;
            if (start.Equals(end))
            {
                return true;
            }

            bool anyValid = false;
            foreach (var pair in trails[start].Paths)
            {
                if (!history.Contains(pair.Key))
                {
                    bool valid = GetLongestTrail(trails, pair.Key, end, new HashSet<Base.Vec2>(history), out int curLongest);
                    anyValid |= valid;
                    if (valid)
                    {
                        int curPath = pair.Value + curLongest;
                        longestTrail = Math.Max(curPath, longestTrail);
                    }
                }
            }
            return anyValid;
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool slippery)
        {
            ParseInput(inputs, slippery, out char[,] grid, out Base.Vec2 start, out Base.Vec2 end, out int xMax, out int yMax);
            Dictionary<Base.Vec2, Trail> trails = new Dictionary<Base.Vec2, Trail>();
            trails[start] = new Trail() { Pos = start };
            GenerateTrails(grid, start, start, end, xMax, yMax, ref trails);
            trails[end] = new Trail() { Pos = end };
            GetLongestTrail(trails, start, end, new HashSet<Base.Vec2>(), out int longestTrail);
            return longestTrail.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);
    }
}