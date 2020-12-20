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

            public int[] Counts { get; set; }

            public void Eval(ref List<string> allSides)
            {
                Top = Raw.First();
                Right = string.Join("", Raw.Select(c => c.Last()));
                Bottom = Raw.Last();
                Left = string.Join("", Raw.Select(c => c[..1]));
                Actions = new List<string>();
                Counts = new int[4];

                allSides.Add(Top);
                allSides.Add(TopR);
                allSides.Add(Right);
                allSides.Add(RightR);
                allSides.Add(Bottom);
                allSides.Add(BottomR);
                allSides.Add(Left);
                allSides.Add(LeftR);
            }

            public void SetCounts(int t, int r, int b, int l)
            {
                Counts[0] = t;
                Counts[1] = r;
                Counts[2] = b;
                Counts[3] = l;
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

            public void Match(char side, Tile tile)
            {
                switch (side)
                {
                    case 'T':
                        MatchT(tile);
                        break;
                    case 'R':
                        MatchR(tile);
                        break;
                    case 'B':
                        MatchB(tile);
                        break;
                    case 'L':
                        MatchL(tile);
                        break;
                }
            }

            private void MatchT(Tile tile)
            {

            }

            private void MatchR(Tile tile)
            {
                if (tile.Right == tile.Left)
                {
                    return;
                }

                if (tile.Right == tile.Top)
                {

                }

            }

            private void MatchB(Tile tile)
            {

            }

            private void MatchL(Tile tile)
            {

            }

            public Tile RotateR()
            {
                return new Tile { ID = ID, Top = Left, Right = Top, Bottom = Right, Left = Bottom, Actions = Actions.Append("R").ToList() };
            }

            public Tile RotateL()
            {
                return new Tile { ID = ID, Top = Right, Right = Bottom, Bottom = Left, Left = Top, Actions = Actions.Append("L").ToList() };
            }

            public Tile FlipV()
            {
                return new Tile { ID = ID, Top = Bottom, Right = RightR, Bottom = Top, Left = LeftR, Actions = Actions.Append("V").ToList() };
            }

            public Tile FlipH()
            {
                return new Tile { ID = ID, Top = TopR, Right = Left, Bottom = BottomR, Left = Right, Actions = Actions.Append("H").ToList() };
            }

            public override string ToString()
            {
                return $"ID={ID}|T={Counts[0]}|R={Counts[1]}|B={Counts[2]}|L={Counts[3]}";
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

            List<Tile> fourCorners = new List<Tile>();
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
                    fourCorners.Add(tile);
                }
            }

            // // build tileset row by row
            List<List<Tile>> tileSet = new List<List<Tile>>();
            tileSet.Add(new List<Tile>());
            Tile startingTile = fourCorners[0];
            tiles.Remove(startingTile);
            char dir = 'R';
            while (true)
            {
                Tile adj = GetAdjacent(startingTile, dir, tiles);

                if (fourCorners.Where(fc => adj.ID == fc.ID).Count() > 0)
                {
                    break;
                }
            }


            return "";
        }

        private Tile GetAdjacent(Tile startingTile, char side, List<Tile> tiles)
        {
            Tile tile = tiles.Where(t => startingTile.IsAdjacent(side, t)).First();
            tile.Match(side, startingTile);
            return tile;
        }

        // private List<Tile> BuildRow(Tile startingTile, List<Tile> fourCorners, List<Tile> tiles)
        // {

        // }
    }
}