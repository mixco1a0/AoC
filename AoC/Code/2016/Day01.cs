using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2016
{
    class Day01 : Day
    {
        public Day01() { }
        public override string GetSolutionVersion(Part part)
        {
            switch (part)
            {
                case Part.One:
                    return "v1";
                case Part.Two:
                    return "v1";
                default:
                    return base.GetSolutionVersion(part);
            }
        }
        protected override List<TestDatum> GetTestData()
        {
            List<TestDatum> testData = new List<TestDatum>();
            testData.Add(new TestDatum
            {
                TestPart = Part.One,
                Output = "5",
                RawInput =
@"R2, L3"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.One,
                Output = "2",
                RawInput =
@"R2, R2, R2"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.One,
                Output = "12",
                RawInput =
@"R5, L5, R5, R3"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.Two,
                Output = "4",
                RawInput =
@"R8, R4, R4, R8"
            });
            return testData;
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool segmentCheck)
        {
            // used for segment checks
            List<Base.Segment> visited = new List<Base.Segment>();
            Base.Point prev = new Base.Point(0, 0);

            int coordX = 0, coordY = 0, curDirection = 0;
            string[] input = inputs[0].Split(" ,".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            foreach (string i in input)
            {
                curDirection += (i[0] == 'R' ? 1 : -1);
                if (curDirection < 0)
                {
                    curDirection += 4;
                }
                switch (curDirection % 4)
                {
                    case 0:
                        coordY += int.Parse(i[1..]);
                        break;
                    case 1:
                        coordX += int.Parse(i[1..]);
                        break;
                    case 2:
                        coordY -= int.Parse(i[1..]);
                        break;
                    case 3:
                        coordX -= int.Parse(i[1..]);
                        break;
                }

                if (segmentCheck)
                {
                    Base.Segment cur = new Base.Segment(prev, new Base.Point(coordX, coordY));
                    //DebugWriteLine($"({cur.A.X,4},{cur.A.Y,4}) -> ({cur.B.X,4}, {cur.B.Y,4})");
                    Base.Point intersection = null;
                    // check for intersection
                    foreach (Base.Segment visit in visited.Take(visited.Count - 1))
                    {
                        intersection = cur.GetIntersection(visit);
                        if (intersection != null)
                        {
                            break;
                        }
                    }
                    if (intersection != null)
                    {
                        coordX = intersection.X;
                        coordY = intersection.Y;
                        break;
                    }
                    visited.Add(cur);
                    prev = cur.B;
                }
            }
            return (Math.Abs(coordX) + Math.Abs(coordY)).ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);
    }
}