using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AoC._2024
{
    class Day10 : Core.Day
    {
        public Day10() { }

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
                    Output = "2",
                    RawInput =
@"...0...
...1...
...2...
6543456
7.....7
8.....8
9.....9"
                },
                new Core.TestDatum
                {
                    TestPart = Core.Part.One,
                    Output = "36",
                    RawInput =
@"89010123
78121874
87430965
96549874
45678903
32019012
01329801
10456732"
                },
                new Core.TestDatum
                {
                    TestPart = Core.Part.Two,
                    Output = "3",
                    RawInput =
@".....0.
..4321.
..5..2.
..6543.
..7..4.
..8765.
..9...."
                },
                new Core.TestDatum
                {
                    TestPart = Core.Part.Two,
                    Output = "81",
                    RawInput =
@"89010123
78121874
87430965
96549874
45678903
32019012
01329801
10456732"
                },
            ];
            return testData;
        }

        private readonly char TrailStart = '0';
        private readonly char TrailEnd = '9';

        private void GetTrailheads(Base.Grid2Char grid, out HashSet<Base.Vec2> trailheads)
        {
            trailheads = [];
            foreach (Base.Vec2 vec2 in grid)
            {
                if (grid[vec2] == TrailStart)
                {
                    trailheads.Add(vec2);
                }
            }
        }

        private int GetTrailheadScore(Base.Grid2Char grid, Base.Vec2 trailhead)
        {
            Queue<Base.Vec2> trails = [];
            trails.Enqueue(trailhead);
            HashSet<Base.Vec2> visited = [];
            HashSet<Base.Vec2> endPoints = [];
            while (trails.Count > 0)
            {
                Base.Vec2 cur = trails.Dequeue();
                visited.Add(cur);
                if (grid[cur] == TrailEnd)
                {
                    endPoints.Add(cur);
                    continue;
                }

                foreach (Util.Grid2.Dir dir in Util.Grid2.Iter.Cardinal)
                {
                    Base.Vec2 next = cur + Util.Grid2.Map.Neighbor[dir];
                    if (grid.Contains(next) && !visited.Contains(next))
                    {
                        if (grid[next] == (grid[cur] + 1))
                        {
                            trails.Enqueue(next);
                        }
                    }
                }
            }
            return endPoints.Count;
        }

        private void PrintGrid(Base.Grid2Char grid, Base.Vec2 next, Util.Grid2.Dir dir)
        {
            StringBuilder sb = new();
            Core.Log.WriteLine(Core.Log.ELevel.Spam, $"Printing grid {grid.MaxCol}x{grid.MaxRow}:");
            for (int _r = 0; _r < grid.MaxRow; ++_r)
            {
                sb.Clear();
                sb.Append($"{_r,4}| ");
                for (int _c = 0; _c < grid.MaxCol; ++_c)
                {
                    if (next.X == _c && next.Y == _r)
                    {
                        sb.Append(Util.Grid2.Map.Arrow[dir]);
                    }
                    else
                    {
                        sb.Append(grid[_c, _r]);
                    }
                }
                Core.Log.WriteLine(Core.Log.ELevel.Spam, sb.ToString());
            }
        }

        private record Vec2Hash(Base.Vec2 Vec2, int History);

        private int GetTrailheadRating(Base.Grid2Char grid, Base.Vec2 trailhead)
        {
            Queue<Vec2Hash> trails = [];
            trails.Enqueue(new (trailhead, 0));
            Dictionary<Base.Vec2, HashSet<int>> visited = [];
            while (trails.Count > 0)
            {
                Vec2Hash cur = trails.Dequeue();
                if (grid[cur.Vec2] == TrailEnd)
                {
                    continue;
                }

                foreach (Util.Grid2.Dir dir in Util.Grid2.Iter.Cardinal)
                {
                    Base.Vec2 next = cur.Vec2 + Util.Grid2.Map.Neighbor[dir];
                    if (grid.Contains(next))
                    {
                        int hash = HashCode.Combine(cur.History, next);
                        if (visited.TryGetValue(next, out HashSet<int> value))
                        {
                            if (value.Contains(hash))
                            {
                                continue;
                            }
                        }
                        else
                        {
                            visited.Add(next, []);
                        }

                        if (grid[next] == (grid[cur.Vec2] + 1))
                        {
                            trails.Enqueue(new(next, hash));
                            visited[next].Add(hash);
                            // PrintGrid(grid, next, dir);
                        }
                    }
                }
            }
            return visited.Where(pair => grid[pair.Key] == TrailEnd).Select(pair => pair.Value.Count).Sum();
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool getRating)
        {
            Base.Grid2Char grid = new(inputs);
            GetTrailheads(grid, out HashSet<Base.Vec2> trailheads);
            if (!getRating)
            {
                return trailheads.Select(th => GetTrailheadScore(grid, th)).Sum().ToString();
            }
            else
            {
                return trailheads.Select(th => GetTrailheadRating(grid, th)).Sum().ToString();
            }
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);
    }
}