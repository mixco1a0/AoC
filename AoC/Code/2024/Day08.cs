using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2024
{
    class Day08 : Core.Day
    {
        public Day08() { }

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
                    Output = "14",
                    RawInput =
@"............
........0...
.....0......
.......0....
....0.......
......A.....
............
............
........A...
.........A..
............
............"
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

        private static readonly char EmptySpace = '.';

        private class AntennaMap
        {
            public Base.Grid2 Grid { get; set; }
            public Dictionary<char, List<Base.Vec2>> Antennas { get; set; }

            public AntennaMap(List<string> inputs)
            {
                Grid = new(inputs);
                Antennas = [];
                foreach (Base.Vec2 vec2 in Grid)
                {
                    char key = Grid.At(vec2);
                    if (key == EmptySpace)
                    {
                        continue;
                    }

                    if (!Antennas.ContainsKey(key))
                    {
                        Antennas.Add(key, []);
                    }
                    Antennas[key].Add(vec2);
                }
            }

            public int CountAntinodes()
            {
                HashSet<Base.Vec2> antinodes = [];
                foreach (var pair in Antennas)
                {
                    List<Base.Vec2> locations = pair.Value;
                    for (int a = 0; a < locations.Count; ++a)
                    {
                        for (int _a = a + 1; _a < locations.Count; ++_a)
                        {
                            Base.Vec2 diff = locations[_a] - locations[a];
                            Base.Vec2 pre = locations[_a] + diff;
                            if (Grid.Has(pre))
                            {
                                antinodes.Add(pre);
                            }
                            Base.Vec2 post = locations[a] - diff;
                            if (Grid.Has(post))
                            {
                                antinodes.Add(post);
                            }
                        }
                    }
                }
                return antinodes.Count;
            }

        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables)
        {
            AntennaMap am = new(inputs);
            return am.CountAntinodes().ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);
    }
}