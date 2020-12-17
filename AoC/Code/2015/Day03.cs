using System.Collections.Generic;
using System.Linq;

namespace AoC._2015
{
    class Day03 : Day
    {
        public Day03() { }

        protected override List<TestDatum> GetTestData()
        {
            List<TestDatum> testData = new List<TestDatum>();
            testData.Add(new TestDatum
            {
                TestPart = TestPart.One,
                Output = "2",
                RawInput =
@">"
            });
            testData.Add(new TestDatum
            {
                TestPart = TestPart.One,
                Output = "4",
                RawInput =
@"^>v<"
            });
            testData.Add(new TestDatum
            {
                TestPart = TestPart.One,
                Output = "2",
                RawInput =
@"^v^v^v^v^v"
            });
            testData.Add(new TestDatum
            {
                TestPart = TestPart.Two,
                Output = "3",
                RawInput =
@"^v"
            });
            testData.Add(new TestDatum
            {
                TestPart = TestPart.Two,
                Output = "3",
                RawInput =
@"^>v<"
            });
            testData.Add(new TestDatum
            {
                TestPart = TestPart.Two,
                Output = "11",
                RawInput =
@"^v^v^v^v^v"
            });
            return testData;
        }

        private struct Coords
        {
            public int X { get; set; }
            public int Y { get; set; }

            public bool Equals(Coords coords)
            {
                return X == coords.X && Y == coords.Y;
            }
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            Coords cur = new Coords { X = 0, Y = 0 };
            List<Coords> allCoords = new List<Coords>();
            allCoords.Add(cur);
            foreach (char c in string.Join(' ', inputs))
            {
                switch (c)
                {
                    case '>':
                        ++cur.X;
                        break;
                    case '<':
                        --cur.X;
                        break;
                    case '^':
                        ++cur.Y;
                        break;
                    case 'v':
                        --cur.Y;
                        break;
                }
                allCoords.Add(cur);
            }
            return allCoords.Distinct().Count().ToString();
        }

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            Coords curSanta = new Coords { X = 0, Y = 0 };
            Coords curRobot = new Coords { X = 0, Y = 0 };
            List<Coords> allCoords = new List<Coords>();
            allCoords.Add(curSanta);
            bool santaMove = true;
            foreach (char c in string.Join(' ', inputs))
            {
                switch (c)
                {
                    case '>':
                        if (santaMove)
                        {
                            ++curSanta.X;
                        }
                        else
                        {
                            ++curRobot.X;
                        }
                        break;
                    case '<':
                        if (santaMove)
                        {
                            --curSanta.X;
                        }
                        else
                        {
                            --curRobot.X;
                        }
                        break;
                    case '^':
                        if (santaMove)
                        {
                            ++curSanta.Y;
                        }
                        else
                        {
                            ++curRobot.Y;
                        }
                        break;
                    case 'v':
                        if (santaMove)
                        {
                            --curSanta.Y;
                        }
                        else
                        {
                            --curRobot.Y;
                        }
                        break;
                }

                if (santaMove)
                {
                    allCoords.Add(curSanta);
                }
                else
                {
                    allCoords.Add(curRobot);
                }
                santaMove = !santaMove;
            }
            return allCoords.Distinct().Count().ToString();
        }
    }
}