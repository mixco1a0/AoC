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
                    Output = "",
                    RawInput =
@""
                },
            ];
            return testData;
        }

        public class Value(int area, int perimeter) : Base.Pair<int, int>(area, perimeter)
        {
            public int Area { get => First; set => First = value; }
            public int Perimeter { get => Last; set => Last = value; }
            public override string ToString()
            {
                return $"[A={Area}, P={Perimeter}]";
            }
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool _)
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
                values[curOrigin] = new(0, 0);
                char curRegion = grid[curOrigin];

                // walk out the contiguous region 
                Queue<Base.Vec2> sameNodes = [];
                sameNodes.Enqueue(curOrigin);
                while (sameNodes.Count > 0)
                {
                    Base.Vec2 curNode = sameNodes.Dequeue();
                    if (flags[curNode])
                    {
                        continue;
                    }
                    flags[curNode] = true;

                    int nodePerimeter = 0;
                    foreach (Util.Grid2.Dir dir in Util.Grid2.Iter.Cardinal)
                    {
                        Base.Vec2 nextNode = curNode + Util.Grid2.Map.Neighbor[dir];
                        if (!grid.Contains(nextNode))
                        {
                            ++nodePerimeter;
                            continue;
                        }

                        if (grid[nextNode] != curRegion)
                        {
                            differentNodes.Enqueue(nextNode);
                            ++nodePerimeter;
                            continue;
                        }

                        sameNodes.Enqueue(nextNode);
                    }

                    values[curOrigin].Area += 1;
                    values[curOrigin].Perimeter += nodePerimeter;
                }

                // Log(Core.Log.ELevel.Spam, $"Region: {curRegion} @ {curOrigin} = {values[curOrigin]}");
                // grid.Print(Core.Log.ELevel.Spam);
                // flags.Print(Core.Log.ELevel.Spam);
            }
            return values.Select(v => v.Value.Area * v.Value.Perimeter).Sum().ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);
    }
}