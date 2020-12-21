using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2020
{
    class Day20 : Day
    {
        public Day20() { }
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
                Output = "20899048083289",
                RawInput =
@"Tile 2311:
..##.#..#.
##..#.....
#...##..#.
####.#...#
##.##.###.
##...#.###
.#.#.#..##
..#....#..
###...#.#.
..###..###

Tile 1951:
#.##...##.
#.####...#
.....#..##
#...######
.##.#....#
.###.#####
###.##.##.
.###....#.
..#.#..#.#
#...##.#..

Tile 1171:
####...##.
#..##.#..#
##.#..#.#.
.###.####.
..###.####
.##....##.
.#...####.
#.##.####.
####..#...
.....##...

Tile 1427:
###.##.#..
.#..#.##..
.#.##.#..#
#.#.#.##.#
....#...##
...##..##.
...#.#####
.#.####.#.
..#..###.#
..##.#..#.

Tile 1489:
##.#.#....
..##...#..
.##..##...
..#...#...
#####...#.
#..#.#.#.#
...#.#.#..
##.#...##.
..##.##.##
###.##.#..

Tile 2473:
#....####.
#..#.##...
#.##..#...
######.#.#
.#...#.#.#
.#########
.###.#..#.
########.#
##...##.#.
..###.#.#.

Tile 2971:
..#.#....#
#...###...
#.#.###...
##.##..#..
.#####..##
.#..####.#
#..#.#..#.
..####.###
..#.#.###.
...#.#.#.#

Tile 2729:
...#.#.#.#
####.#....
..#.#.....
....#..#.#
.##..##.#.
.#.####...
####.#.#..
##.####...
##..#.##..
#.##...##.

Tile 3079:
#.#.#####.
.#..######
..#.......
######....
####.#..#.
.#...#.##.
#.#####.##
..#.###...
..#.......
..#.###...
"
            });
            testData.Add(new TestDatum
            {
                TestPart = TestPart.Two,
                Output = "273",
                RawInput =
@"Tile 2311:
..##.#..#.
##..#.....
#...##..#.
####.#...#
##.##.###.
##...#.###
.#.#.#..##
..#....#..
###...#.#.
..###..###

Tile 1951:
#.##...##.
#.####...#
.....#..##
#...######
.##.#....#
.###.#####
###.##.##.
.###....#.
..#.#..#.#
#...##.#..

Tile 1171:
####...##.
#..##.#..#
##.#..#.#.
.###.####.
..###.####
.##....##.
.#...####.
#.##.####.
####..#...
.....##...

Tile 1427:
###.##.#..
.#..#.##..
.#.##.#..#
#.#.#.##.#
....#...##
...##..##.
...#.#####
.#.####.#.
..#..###.#
..##.#..#.

Tile 1489:
##.#.#....
..##...#..
.##..##...
..#...#...
#####...#.
#..#.#.#.#
...#.#.#..
##.#...##.
..##.##.##
###.##.#..

Tile 2473:
#....####.
#..#.##...
#.##..#...
######.#.#
.#...#.#.#
.#########
.###.#..#.
########.#
##...##.#.
..###.#.#.

Tile 2971:
..#.#....#
#...###...
#.#.###...
##.##..#..
.#####..##
.#..####.#
#..#.#..#.
..####.###
..#.#.###.
...#.#.#.#

Tile 2729:
...#.#.#.#
####.#....
..#.#.....
....#..#.#
.##..##.#.
.#.####...
####.#.#..
##.####...
##..#.##..
#.##...##.

Tile 3079:
#.#.#####.
.#..######
..#.......
######....
####.#..#.
.#...#.##.
#.#####.##
..#.###...
..#.......
..#.###...
"
            });
            return testData;
        }

        class Tile
        {
            public string ID { get; set; }
            public string Top { get; private set; }
            public string TopR { get { return String.Join("", Top.Reverse()); } }
            public string Right { get; private set; }
            public string RightR { get { return String.Join("", Right.Reverse()); } }
            public string Bottom { get; private set; }
            public string BottomR { get { return String.Join("", Bottom.Reverse()); } }
            public string Left { get; private set; }
            public string LeftR { get { return String.Join("", Left.Reverse()); } }
            public List<string> Raw { get; set; }

            public List<string> Actions { get; set; }
            public Dictionary<string, int> BorderCounts { get; set; }

            public Tile()
            {
                ID = "";
            }

            public void Eval(ref List<string> allSides)
            {
                Top = Raw.First();
                Right = string.Join("", Raw.Select(c => c.Last()));
                Bottom = Raw.Last();
                Left = string.Join("", Raw.Select(c => c[..1]));
                Actions = new List<string>();
                BorderCounts = new Dictionary<string, int>();

                if (allSides != null)
                {
                    allSides.Add(Top);
                    allSides.Add(TopR);
                    allSides.Add(Right);
                    allSides.Add(RightR);
                    allSides.Add(Bottom);
                    allSides.Add(BottomR);
                    allSides.Add(Left);
                    allSides.Add(LeftR);
                }
            }

            public void SetCounts(int t, int r, int b, int l)
            {
                BorderCounts[Top] = t;
                BorderCounts[TopR] = t;
                BorderCounts[Right] = r;
                BorderCounts[RightR] = r;
                BorderCounts[Bottom] = b;
                BorderCounts[BottomR] = b;
                BorderCounts[Left] = l;
                BorderCounts[LeftR] = l;
            }

            public bool IsAdjacent(char side, Tile tile)
            {
                if (tile.ID == ID)
                {
                    return false;
                }

                string sideToCheck = "";
                switch (side)
                {
                    case 'T':
                        sideToCheck = Top;
                        break;
                    case 'R':
                        sideToCheck = Right;
                        break;
                    case 'B':
                        sideToCheck = Bottom;
                        break;
                    case 'L':
                        sideToCheck = Left;
                        break;
                }

                return tile.Top == sideToCheck ||
                        tile.TopR == sideToCheck ||
                        tile.Right == sideToCheck ||
                        tile.RightR == sideToCheck ||
                        tile.Bottom == sideToCheck ||
                        tile.BottomR == sideToCheck ||
                        tile.Left == sideToCheck ||
                        tile.LeftR == sideToCheck;
            }

            public void Match(char side, string source)
            {
                switch (side)
                {
                    case 'R':
                        MatchR(source);
                        break;
                    case 'B':
                        MatchB(source);
                        break;
                }
            }

            private void MatchR(string sourceRight)
            {
                ///   -   -
                //   | > > |
                //    -   -
                if (sourceRight == Left)
                {
                    return;
                }

                ///   -   -
                //   | > < |
                //    -   -
                else if (sourceRight == LeftR)
                {
                    FlipV();
                }

                ///   -   v      -   ^
                //   | > | |    | > | |
                //    -   -      -   -
                else if (sourceRight == Top || sourceRight == TopR)
                {
                    RotateL();
                }

                ///   -   -      -   -
                //   | > | |    | > | |
                //    -   ^      -   v
                else if (sourceRight == Bottom || sourceRight == BottomR)
                {
                    RotateR();
                }

                ///   -   -      -   -
                //   | > | <    | > | >
                //    -   -      -   -
                else
                {
                    RotateR();
                }

                MatchR(sourceRight);
            }

            private void MatchB(string sourceBottom)
            {
                ///   -   v
                //   | | | |
                //    v   -
                if (sourceBottom == Top)
                {
                    return;
                }

                ///   -   ^
                //   | | | |
                //    v   -
                else if (sourceBottom == TopR)
                {
                    FlipH();
                }

                ///   -   -
                //   | | | *
                //    v   -
                else if (sourceBottom == Right || sourceBottom == RightR)
                {
                    RotateL();
                }

                ///   -   -
                //   | | * |
                //    v   -
                else if (sourceBottom == Left || sourceBottom == LeftR)
                {
                    RotateR();
                }

                ///   -   -
                //   | | | |
                //    v   *
                else
                {
                    RotateR();
                }

                MatchB(sourceBottom);
            }

            public void RotateR()
            {
                Actions.Add("R");
                string temp = Top;
                Top = LeftR;
                Left = Bottom;
                Bottom = RightR;
                Right = temp;

                List<string> newRaw = new List<string>();
                for (int i = 0; i < Raw.Count(); ++i)
                {
                    newRaw.Add(string.Join("", Raw.Select(r => r.ElementAt(i)).Reverse()));
                }
                Raw = newRaw;
            }

            public void RotateL()
            {
                Actions.Add("L");
                string temp = TopR;
                Top = Right;
                Right = BottomR;
                Bottom = Left;
                Left = temp;

                List<string> newRaw = new List<string>();
                for (int i = 0; i < Raw.Count; ++i)
                {
                    newRaw.Add(string.Join("", Raw.Select(r => r.ElementAt(Raw.Count - 1 - i))));
                }
                Raw = newRaw;
            }

            public void FlipV()
            {
                Actions.Add("V");
                string temp = Top;
                Top = Bottom;
                Bottom = temp;

                Right = RightR;
                Left = LeftR;

                Raw.Reverse();
            }

            public void FlipH()
            {
                Actions.Add("H");
                string temp = Right;
                Right = Left;
                Left = temp;

                Top = TopR;
                Bottom = BottomR;
                
                for (int i = 0; i < Raw.Count; ++i)
                {
                    Raw[i] = string.Join("", Raw[i].Reverse());
                }
            }

            public override string ToString()
            {
                return $"ID={ID} Actions={string.Join("", Actions)}";
            }
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            string curId = "";
            List<string> curRaw = new List<string>();
            List<string> allSides = new List<string>();
            List<Tile> tiles = new List<Tile>();
            foreach (string input in inputs)
            {
                if (input.Contains(":"))
                {
                    curId = input;
                }
                else if (string.IsNullOrWhiteSpace(input))
                {
                    string id = curId.Split(" :".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Last();
                    tiles.Add(new Tile { ID = id, Raw = curRaw });
                    tiles.Last().Eval(ref allSides);

                    curId = string.Empty;
                    curRaw.Clear();
                }
                else
                {
                    curRaw.Add(input);
                }
            }

            List<long> uniqueIds = new List<long>();
            long mult = 1;
            foreach (Tile tile in tiles)
            {
                int tC = allSides.Where(s => s == tile.Top).Count();
                int rC = allSides.Where(s => s == tile.Right).Count();
                int bC = allSides.Where(s => s == tile.Bottom).Count();
                int lC = allSides.Where(s => s == tile.Left).Count();

                if ((tC == 1 && rC == 1) ||
                    (rC == 1 && bC == 1) ||
                    (bC == 1 && lC == 1) ||
                    (lC == 1 && tC == 1))
                {
                    //uniqueIds.Add();
                    mult *= int.Parse(tile.ID);
                }
            }

            return mult.ToString();
        }

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            string curId = "";
            List<string> curRaw = new List<string>();
            List<string> allSides = new List<string>();
            List<Tile> tiles = new List<Tile>();
            foreach (string input in inputs)
            {
                if (input.Contains(":"))
                {
                    curId = input;
                }
                else if (string.IsNullOrWhiteSpace(input))
                {
                    string id = curId.Split(" :".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Last();
                    tiles.Add(new Tile { ID = id, Raw = new List<string>(curRaw) });
                    tiles.Last().Eval(ref allSides);

                    curId = string.Empty;
                    curRaw.Clear();
                }
                else
                {
                    curRaw.Add(input);
                }
            }

            DebugWriteLine($"Get New Starting Tile");
            Tile startingTile = null;
            foreach (Tile tile in tiles)
            {
                int tC = allSides.Where(s => s == tile.Top).Count();
                int rC = allSides.Where(s => s == tile.Right).Count();
                int bC = allSides.Where(s => s == tile.Bottom).Count();
                int lC = allSides.Where(s => s == tile.Left).Count();
                tile.SetCounts(tC, rC, bC, lC);

                if ((tC == 1 && rC == 1) ||
                    (rC == 1 && bC == 1) ||
                    (bC == 1 && lC == 1) ||
                    (lC == 1 && tC == 1))
                {
                    // get starting tile and orient it correctly
                    startingTile = tile;
                    ///   1 
                    //   ? ?
                    //    - 
                    if (tC == 1)
                    {
                        ///   1 
                        //   | 1
                        //    - 
                        if (rC == 1)
                        {
                            tile.FlipH();
                        }
                        ///   1 
                        //   1 |
                        //    - 
                    }
                    ///   _ 
                    //   ? ?
                    //    1 
                    else
                    {
                        ///   - 
                        //   | 1
                        //    1 
                        if (rC == 1)
                        {
                            tile.FlipH();
                        }
                        ///   _
                        //   1 |
                        //    1
                        tile.FlipV();
                        ///   1 
                        //   1 |
                        //    - 
                    }
                    DebugWriteLine($"Match Found : Tile #{startingTile.ID} [Actions: {string.Join("", startingTile.Actions)}]");
                    break;
                }
            }

            // // build tileset row by row
            List<List<Tile>> tileSet = new List<List<Tile>>();
            do
            {
                DebugWriteLine($"Starting New Row");
                List<Tile> row = new List<Tile>();
                BuildRow(startingTile, ref row, ref tiles);
                tileSet.Add(row);

                DebugWriteLine("");
                DebugWriteLine($"Get New Starting Tile");
                startingTile = GetAdjacent(row[0], 'B', tiles);
            } while (startingTile != null);

            // grid is sorted
            // todo: go through todos and fix raw square

            return "";
        }

        private void BuildRow(Tile startingTile, ref List<Tile> row, ref List<Tile> tiles)
        {
            do
            {
                row.Add(startingTile);
                startingTile = GetAdjacent(startingTile, 'R', tiles);
            } while (startingTile != null);
        }

        private Tile GetAdjacent(Tile tile, char side, List<Tile> tiles)
        {
            Tile adjacentTile = tiles.Where(t => tile.IsAdjacent(side, t)).FirstOrDefault();
            if (adjacentTile != null)
            {
                adjacentTile.Match(side, side == 'R' ? tile.Right : tile.Bottom);
                DebugWriteLine($"Match Found : Tile #{adjacentTile.ID} [Actions: {string.Join("", adjacentTile.Actions)}]");
            }
            else
            {
                DebugWriteLine($"No Match Found");
            }
            return adjacentTile;
        }

        private void PrintTile(Tile tile)
        {
            DebugWriteLine($"Tile: #{tile.ID}");
            foreach (string raw in tile.Raw)
            {
                DebugWriteLine($"   {raw}");
            }
        }
    }
}