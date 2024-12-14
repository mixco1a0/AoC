using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2024
{
    class Day12 : Core.Day
    {
        public Day12() { }

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
                    Output = "140",
                    RawInput =
@"AAAA
BBCD
BBCC
EEEC"
                },
                new Core.TestDatum
                {
                    TestPart = Core.Part.One,
                    Output = "772",
                    RawInput =
@"OOOOO
OXOXO
OOOOO
OXOXO
OOOOO"
                },
                new Core.TestDatum
                {
                    TestPart = Core.Part.One,
                    Output = "1930",
                    RawInput =
@"RRRRIICCFF
RRRRIICCCF
VVRRRCCFFF
VVRCCCJFFF
VVVVCJJCFE
VVIVCCJJEE
VVIIICJJEE
MIIIIIJJEE
MIIISIJEEE
MMMISSJEEE"
                },
                new Core.TestDatum
                {
                    TestPart = Core.Part.Two,
                    Output = "80",
                    RawInput =
@"AAAA
BBCD
BBCC
EEEC"
                },
                new Core.TestDatum
                {
                    TestPart = Core.Part.Two,
                    Output = "436",
                    RawInput =
@"OOOOO
OXOXO
OOOOO
OXOXO
OOOOO"
                },
                new Core.TestDatum
                {
                    TestPart = Core.Part.Two,
                    Output = "236",
                    RawInput =
@"EEEEE
EXXXX
EEEEE
EXXXX
EEEEE"
                },
                new Core.TestDatum
                {
                    TestPart = Core.Part.Two,
                    Output = "368",
                    RawInput =
@"AAAAAA
AAABBA
AAABBA
ABBAAA
ABBAAA
AAAAAA"
                },
                new Core.TestDatum
                {
                    TestPart = Core.Part.Two,
                    Output = "1206",
                    RawInput =
@"RRRRIICCFF
RRRRIICCCF
VVRRRCCFFF
VVRCCCJFFF
VVVVCJJCFE
VVIVCCJJEE
VVIIICJJEE
MIIIIIJJEE
MIIISIJEEE
MMMISSJEEE"
                },
            ];
            return testData;
        }

        private class Value(int area, int perimeter, int faces) : Base.Pair<int, int>(area, perimeter)
        {
            public int Area { get => First; set => First = value; }
            public int Perimeter { get => Last; set => Last = value; }
            public int Faces { get; set; } = faces;

            public override string ToString()
            {
                return $"[A={Area}, P={Perimeter}, F={Faces}]";
            }
        }

        private record Wall(Base.Vec2 Vec2, Util.Grid2.Dir Dir);

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool useDiscount)
        {
            Base.Grid2Char grid = new(inputs);
            Base.Grid2Bool flags = new(inputs);
            Queue<Base.Vec2> differentNodes = [];
            Dictionary<Base.Vec2, Value> values = [];
            differentNodes.Enqueue(new(0, 0));
            while (differentNodes.Count > 0)
            {
                Base.Vec2 curOrigin = differentNodes.Dequeue();
                if (flags[curOrigin])
                {
                    continue;
                }

                // get the current initial state
                values[curOrigin] = new(0, 0, 0);
                char curRegion = grid[curOrigin];

                // walk out the contiguous region 
                Queue<Base.Vec2> sameNodes = [];
                List<Base.Vec2> curRegionNodes = [];
                List<Wall> walls = [];
                sameNodes.Enqueue(curOrigin);
                while (sameNodes.Count > 0)
                {
                    Base.Vec2 curNode = sameNodes.Dequeue();
                    if (flags[curNode])
                    {
                        continue;
                    }
                    flags[curNode] = true;
                    curRegionNodes.Add(curNode);

                    int nodePerimeter = 0;
                    foreach (Util.Grid2.Dir dir in Util.Grid2.Iter.Cardinal)
                    {
                        Base.Vec2 nextNode = curNode + Util.Grid2.Map.Neighbor[dir];
                        if (!grid.Contains(nextNode))
                        {
                            walls.Add(new(curNode, dir));
                            ++nodePerimeter;
                            continue;
                        }

                        if (grid[nextNode] != curRegion)
                        {
                            walls.Add(new(curNode, dir));
                            differentNodes.Enqueue(nextNode);
                            ++nodePerimeter;
                            continue;
                        }

                        sameNodes.Enqueue(nextNode);
                    }

                    values[curOrigin].Area += 1;
                    values[curOrigin].Perimeter += nodePerimeter;
                }

                // calculate the faces
                if (useDiscount)
                {
                    int faceCount = 0;
                    foreach (Util.Grid2.Dir dir in Util.Grid2.Iter.Cardinal)
                    {
                        IEnumerable<Wall> dirWalls = walls.Where(w => w.Dir == dir);
                        IEnumerable<IGrouping<int, Wall>> groupedWalls;

                        bool isNS = dir == Util.Grid2.Dir.North || dir == Util.Grid2.Dir.South;
                        if (isNS)
                        {
                            dirWalls = dirWalls.OrderBy(w => w.Vec2.X);
                            groupedWalls = dirWalls.GroupBy(w => w.Vec2.Y);
                        }
                        else
                        {
                            dirWalls = dirWalls.OrderBy(w => w.Vec2.Y);
                            groupedWalls = dirWalls.GroupBy(w => w.Vec2.X);
                        }

                        foreach (var grouping in groupedWalls)
                        {
                            Base.Vec2 prev = new(grid.MaxCol, grid.MaxRow);
                            foreach (Wall w in grouping)
                            {
                                Base.Vec2 test = w.Vec2 - prev;
                                if (isNS)
                                {
                                    if (test.X != 1)
                                    {
                                        ++faceCount;
                                    }
                                }
                                else
                                {
                                    if (test.Y != 1)
                                    {
                                        ++faceCount;
                                    }
                                }
                                prev = w.Vec2;
                            }
                        }
                    }
                    values[curOrigin].Faces += faceCount;
                }
            }

            return values.Select(v => v.Value.Area * (useDiscount ? v.Value.Faces : v.Value.Perimeter)).Sum().ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);
    }
}