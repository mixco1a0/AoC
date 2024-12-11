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
                    Output = "9",
                    RawInput =
@"T.........
...T......
.T........
..........
..........
..........
..........
..........
..........
.........."
                },
                new Core.TestDatum
                {
                    TestPart = Core.Part.Two,
                    Output = "34",
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
            ];
            return testData;
        }

        private static readonly char EmptySpace = '.';

        private class AntennaMap
        {
            public Base.Grid2Char Grid { get; set; }
            public Dictionary<char, List<Base.Vec2>> Antennas { get; set; }

            public AntennaMap(List<string> inputs)
            {
                Grid = new(inputs);
                Antennas = [];
                foreach (Base.Vec2 vec2 in Grid)
                {
                    char key = Grid[vec2];
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

            public int CountAntinodes(bool useResonantHarmonics)
            {
                int antinodeCheckCount = useResonantHarmonics ? int.Max(Grid.MaxCol, Grid.MaxRow) : 1;
                HashSet<Base.Vec2> antinodes = [];
                foreach (var pair in Antennas)
                {
                    List<Base.Vec2> locations = pair.Value;
                    // if more than one antenna, auto antinodes
                    if (useResonantHarmonics && locations.Count > 1)
                    {
                        foreach(Base.Vec2 loc in locations)
                        {
                            antinodes.Add(loc);
                        }
                    }

                    for (int a = 0; a < locations.Count; ++a)
                    {
                        for (int _a = a + 1; _a < locations.Count; ++_a)
                        {
                            Base.Vec2 diff = locations[_a] - locations[a];
                            // check all pre locations
                            Base.Vec2 pre = locations[_a] + diff;
                            for (int preCheck = 0; preCheck < antinodeCheckCount; ++preCheck)
                            {
                                if (Grid.Contains(pre))
                                {
                                    antinodes.Add(pre);
                                    pre += diff;
                                }
                                else
                                {
                                    break;
                                }
                            }

                            // check all post locations
                            Base.Vec2 post = locations[a] - diff;
                            for (int postCheck = 0; postCheck < antinodeCheckCount; ++postCheck)
                            {
                                if (Grid.Contains(post))
                                {
                                    antinodes.Add(post);
                                    post -= diff;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                }

                // if (useResonantHarmonics)
                // {
                //     char[,] printedGrid = Grid.Grid.Clone() as char[,];
                //     foreach (Base.Vec2 vec2 in Grid)
                //     {
                //         if (antinodes.Contains(vec2))
                //         {
                //             printedGrid[vec2.X, vec2.Y] = '#';
                //         }
                //     }
                //     Util.Grid.Print2D(Core.Log.ELevel.Spam, printedGrid);
                // }

                return antinodes.Count;
            }

        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool useResonantHarmonics)
        {
            AntennaMap am = new(inputs);
            return am.CountAntinodes(useResonantHarmonics).ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);
    }
}