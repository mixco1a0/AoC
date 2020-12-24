using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2020
{
    class Day24 : Day
    {
        public Day24() { }
        public override string GetSolutionVersion(TestPart testPart)
        {
            switch (testPart)
            {
                // case TestPart.One:
                //     return "v1";
                // case TestPart.Two:
                //     return "v1";
                default:
                    return base.GetSolutionVersion(testPart);
            }
        }
        protected override List<TestDatum> GetTestData()
        {
            List<TestDatum> testData = new List<TestDatum>();
            testData.Add(new TestDatum
            {
                TestPart = TestPart.One,
                Output = "10",
                RawInput =
@"sesenwnenenewseeswwswswwnenewsewsw
neeenesenwnwwswnenewnwwsewnenwseswesw
seswneswswsenwwnwse
nwnwneseeswswnenewneswwnewseswneseene
swweswneswnenwsewnwneneseenw
eesenwseswswnenwswnwnwsewwnwsene
sewnenenenesenwsewnenwwwse
wenwwweseeeweswwwnwwe
wsweesenenewnwwnwsenewsenwwsesesenwne
neeswseenwwswnwswswnw
nenwswwsewswnenenewsenwsenwnesesenew
enewnwewneswsewnwswenweswnenwsenwsw
sweneswneswneneenwnewenewwneswswnese
swwesenesewenwneswnwwneseswwne
enesenwswwswneneswsenwnewswseenwsese
wnwnesenesenenwwnenwsewesewsesesew
nenewswnwewswnenesenwnesewesw
eneswnwswnwsenenwnwnwwseeswneewsenese
neswnwewnwnwseenwseesewsenwsweewe
wseweeenwnesenwwwswnew"
            });
            testData.Add(new TestDatum
            {
                TestPart = TestPart.Two,
                Output = "",
                RawInput =
@""
            });
            return testData;
        }

        class Coords
        {
            public Coords()
            {
                X = 0; Y = 0;
            }

            public int X { get; set; }
            public int Y { get; set; }

            public override string ToString()
            {
                return $"X={X}, Y={Y}";
            }
        }

        private Coords FlipCoords(string input)
        {
            Coords coords = new Coords();
            int idx = 1;
            for (int i = 0; i < input.Length; ++i, ++idx)
            {
                int start = i;
                switch (input[i])
                {
                    case 'e':
                        ++coords.X;
                        break;
                    case 'w':
                        --coords.X;
                        break;
                    case 'n':
                        ++i;
                        --coords.Y;
                        switch (input[i])
                        {
                            case 'e':
                                ++coords.X;
                                break;
                            case 'w':
                                break;
                        }
                        break;
                    case 's':
                        ++i;
                        ++coords.Y;
                        switch (input[i])
                        {
                            case 'e':
                                break;
                            case 'w':
                                --coords.X;
                                break;
                        }
                        break;
                }
            }
            return coords;
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            List<Coords> coords = new List<Coords>();
            foreach (string input in inputs)
            {
                coords.Add(FlipCoords(input));
            }
            // coords.ForEach(c => DebugWriteLine(c.ToString()));
            coords = coords.OrderBy(_ => _.ToString()).ToList();
            Dictionary<string, bool> colors = new Dictionary<string, bool>();
            foreach (Coords cur in coords)
            {
                string key = cur.ToString();
                if (colors.ContainsKey(key))
                {
                    colors[key] = !colors[key];
                }
                else
                {
                    colors[key] = true;
                }
            }
            return colors.Where(pair => pair.Value).Count().ToString();
        }

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            return "";
        }
    }
}