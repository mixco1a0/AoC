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

        static readonly Base.Pos2L[] GridMoves = new Base.Pos2L[] { new Base.Pos2L(0, 1), new Base.Pos2L(1, 0), new Base.Pos2L(-1, 0), new Base.Pos2L(0, -1) };
        static readonly Dictionary<char, Base.Pos2[]> PathMoves = new Dictionary<char, Base.Pos2[]>()
        {
            {Path, new Base.Pos2[] { new Base.Pos2(0, 1), new Base.Pos2(1, 0), new Base.Pos2(-1, 0), new Base.Pos2(0, -1) }},
            {'^',  new Base.Pos2[] { new Base.Pos2(0, -1) }},
            {'>',  new Base.Pos2[] { new Base.Pos2(1, 0)  }},
            {'<',  new Base.Pos2[] { new Base.Pos2(-1, 0) }},
            {'v',  new Base.Pos2[] { new Base.Pos2(0, 1)  }},
        };

        private void ParseInput(List<string> inputs, bool slippery, out char[,] grid, out Base.Pos2 start, out Base.Pos2 end, out int xMax, out int yMax)
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

            start = new Base.Pos2();
            end = new Base.Pos2();
            for (int x = 0; x < xMax; ++x)
            {
                if (grid[x, 0] == Path)
                {
                    start = new Base.Pos2(x, 0);
                }
                if (grid[x, yMax - 1] == Path)
                {
                    end = new Base.Pos2(x, yMax - 1);
                }
            }
        }

        private class Trail
        {
            public Base.Pos2 Pos { get; set; }
            public Dictionary<Base.Pos2, int> Paths { get; set; }

            public Trail()
            {
                Pos = new Base.Pos2();
                Paths = new Dictionary<Base.Pos2, int>();
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

        private void GenerateTrails(char[,] grid, Base.Pos2 start, Base.Pos2 curPos, Base.Pos2 end, int xMax, int yMax, ref Dictionary<Base.Pos2, Trail> trails)
        {
            // Log($"Checking {start}");
            HashSet<Base.Pos2> visited = new HashSet<Base.Pos2>() { start };
            HashSet<Base.Pos2> split = new HashSet<Base.Pos2>();
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

                List<Base.Pos2> potentials = new List<Base.Pos2>();
                foreach (Base.Pos2 movePos2 in PathMoves[path])
                {
                    Base.Pos2 nextPos2 = curPos + movePos2;
                    if (nextPos2.X >= 0 && nextPos2.X < xMax && nextPos2.Y >= 0 && nextPos2.Y < yMax && grid[nextPos2.X, nextPos2.Y] != Forest)
                    {
                        potentials.Add(nextPos2);
                    }
                }

                IEnumerable<Base.Pos2> unique = potentials.Where(p => !visited.Contains(p));
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

                foreach (Base.Pos2 pos2 in split)
                {
                    GenerateTrails(grid, curPos, pos2, end, xMax, yMax, ref trails);
                }
            }
        }

        private bool GetLongestTrail(Dictionary<Base.Pos2, Trail> trails, Base.Pos2 start, Base.Pos2 end, HashSet<Base.Pos2> history, out int longestTrail)
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
                    bool valid = GetLongestTrail(trails, pair.Key, end, new HashSet<Base.Pos2>(history), out int curLongest);
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
            ParseInput(inputs, slippery, out char[,] grid, out Base.Pos2 start, out Base.Pos2 end, out int xMax, out int yMax);
            Dictionary<Base.Pos2, Trail> trails = new Dictionary<Base.Pos2, Trail>();
            trails[start] = new Trail() { Pos = start };
            GenerateTrails(grid, start, start, end, xMax, yMax, ref trails);
            trails[end] = new Trail() { Pos = end };
            GetLongestTrail(trails, start, end, new HashSet<Base.Pos2>(), out int longestTrail);
            return longestTrail.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);
    }
}