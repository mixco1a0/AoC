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
                Output = "2208",
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
            List<Coords> coords = new List<Coords>();
            foreach (string input in inputs)
            {
                coords.Add(FlipCoords(input));
            }
            // coords.ForEach(c => DebugWriteLine(c.ToString()));
            coords = coords.OrderBy(_ => _.ToString()).ToList();
            HashSet<KeyValuePair<int, int>> tiles = new HashSet<KeyValuePair<int, int>>();
            foreach (Coords cur in coords)
            {
                KeyValuePair<int, int> key = new KeyValuePair<int, int>(cur.X, cur.Y);
                if (tiles.Contains(key))
                {
                    tiles.Remove(key);
                }
                else
                {
                    tiles.Add(key);
                }
            }

            for (int i = 1; i <= 100; ++i)
            {
                int count = DaySwap(ref tiles);
                // DebugWriteLine($"Day {i}: {count}");;
            }

            return tiles.Count().ToString();
        }

        private int DaySwap(ref HashSet<KeyValuePair<int, int>> tiles)
        {
            HashSet<KeyValuePair<int, int>> newState = new HashSet<KeyValuePair<int, int>>();
            HashSet<KeyValuePair<int, int>> whitesToCheck = new HashSet<KeyValuePair<int, int>>();
            foreach (var tile in tiles)
            {
                int x = tile.Key;
                int y = tile.Value;
                // check E/W
                int sum = 0;
                if (tiles.Contains(new KeyValuePair<int, int>(x + 1, y)))
                {
                    ++sum;
                }
                else
                {
                    whitesToCheck.Add(new KeyValuePair<int, int>(x + 1, y));
                }
                if (tiles.Contains(new KeyValuePair<int, int>(x - 1, y)))
                {
                    ++sum;
                }
                else
                {
                    whitesToCheck.Add(new KeyValuePair<int, int>(x - 1, y));
                }
                if (tiles.Contains(new KeyValuePair<int, int>(x + 1, y - 1)))
                {
                    ++sum;
                }
                else
                {
                    whitesToCheck.Add(new KeyValuePair<int, int>(x + 1, y - 1));
                }
                if (tiles.Contains(new KeyValuePair<int, int>(x, y - 1)))
                {
                    ++sum;
                }
                else
                {
                    whitesToCheck.Add(new KeyValuePair<int, int>(x, y - 1));
                }
                if (tiles.Contains(new KeyValuePair<int, int>(x, y + 1)))
                {
                    ++sum;
                }
                else
                {
                    whitesToCheck.Add(new KeyValuePair<int, int>(x, y + 1));
                }
                if (tiles.Contains(new KeyValuePair<int, int>(x - 1, y + 1)))
                {
                    ++sum;
                }
                else
                {
                    whitesToCheck.Add(new KeyValuePair<int, int>(x - 1, y + 1));
                }

                if (sum == 0 || sum > 2)
                {
                    // flips to white, dont add to the new state
                }
                else
                {
                    // stays black, add to new state
                    newState.Add(new KeyValuePair<int, int>(x, y));
                }
            }

            foreach (var tile in whitesToCheck)
            {
                int x = tile.Key;
                int y = tile.Value;
                int sum = 0;
                if (tiles.Contains(new KeyValuePair<int, int>(x + 1, y)))
                {
                    ++sum;
                }
                if (tiles.Contains(new KeyValuePair<int, int>(x - 1, y)))
                {
                    ++sum;
                }
                if (tiles.Contains(new KeyValuePair<int, int>(x + 1, y - 1)))
                {
                    ++sum;
                }
                if (tiles.Contains(new KeyValuePair<int, int>(x, y - 1)))
                {
                    ++sum;
                }
                if (tiles.Contains(new KeyValuePair<int, int>(x, y + 1)))
                {
                    ++sum;
                }
                if (tiles.Contains(new KeyValuePair<int, int>(x - 1, y + 1)))
                {
                    ++sum;
                }
                if (sum == 2)
                {
                    newState.Add(new KeyValuePair<int, int>(x, y));
                }
            }

            tiles = newState;
            return tiles.Count;
        }
    }
}