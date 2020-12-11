using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2020
{
    class Day11 : Day
    {
        public Day11() { }
        protected override List<TestDatum> GetTestData()
        {
            List<TestDatum> testData = new List<TestDatum>();
            testData.Add(new TestDatum
            {
                TestPart = TestPart.One,
                Output = "37",
                RawInput =
@"L.LL.LL.LL
LLLLLLL.LL
L.L.L..L..
LLLL.LL.LL
L.LL.LL.LL
L.LLLLL.LL
..L.L.....
LLLLLLLLLL
L.LLLLLL.L
L.LLLLL.LL"
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

        private char GetLocationState(int x, int y, List<List<char>> seats)
        {
            int emptyCount = 0;
            int fullCount = 0;
            for (int i = x - 1; i <= x + 1; ++i)
            {
                if (i < 0 || i >= seats.Count)
                    continue;

                for (int j = y - 1; j <= y + 1; ++j)
                {
                    if (i == x && j == y)
                        continue;

                    if (j < 0 || j >= seats[x].Count)
                        continue;

                    try
                    {

                        if (seats[i][j] == '#')
                            ++fullCount;

                        if (seats[i][j] == 'L')
                            ++emptyCount;
                    }
                    catch (Exception e)
                    {
                        Debug("oops");
                    }
                }
            }

            switch (seats[x][y])
            {
                case 'L':
                    return fullCount > 0 ? 'L' : '#';
                case '#':
                    return fullCount >= 4 ? 'L' : '#';
            }


            return '!';
        }

        private bool ProcessSeats(ref List<List<char>> seats)
        {
            List<List<char>> newSeats = new List<List<char>>();
            foreach (List<char> row in seats)
            {
                newSeats.Add(new List<char>(row));
            }
            for (int x = 0; x < seats.Count; ++x)
            {
                for (int y = 0; y < seats[x].Count; ++y)
                {
                    if (seats[x][y] == '.')
                        continue;

                    newSeats[x][y] = GetLocationState(x, y, seats);
                }
            }

            for (int x = 0; x < seats.Count; ++x)
            {
                for (int y = 0; y < seats[x].Count; ++y)
                {
                    if (seats[x][y] != newSeats[x][y])
                    {
                        seats = newSeats;
                        return true;
                    }
                }
            }
            return false;
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            List<List<char>> seats = inputs.Select(a => a.ToCharArray().ToList()).ToList();
            while (ProcessSeats(ref seats)) ;

            return string.Join("", seats.Select(c => string.Join("", c))).Replace(".", "").Replace("L", "").Count().ToString();
        }

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            return "";
        }
    }
}