using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2021
{
    class Day25 : Day
    {
        public Day25() { }

        public override string GetSolutionVersion(Part part)
        {
            switch (part)
            {
                // case Part.One:
                //     return "v1";
                // case Part.Two:
                //     return "v1";
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
                Output = "58",
                RawInput =
@"v...>>.vv>
.vv>>.vv..
>>.>v>...v
>>v>>.>.v.
v>v.vv.v..
>.>>..v...
.vv..>.>v.
v.v..>>v.v
....v..v.>"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.Two,
                Output = "",
                RawInput =
@""
            });
            return testData;
        }

        private readonly char East = '>';
        private readonly char South = 'v';
        private readonly char Empty = '.';

        private bool ProcessEast(string[] region, int maxX, int maxY, out string[] newRegion)
        {
            bool processed = false;
            string emptyLine = new string(Empty, maxX);
            newRegion = new string[maxY];
            for (int y = 0; y < maxY; ++y)
            {
                StringBuilder sb = new StringBuilder(emptyLine);
                for (int x = 0; x < maxX; ++x)
                {
                    char oldValue = region[y][x];
                    if (oldValue == South)
                    {
                        sb[x] = oldValue;
                        continue;
                    }

                    if (oldValue == Empty)
                    {
                        continue;
                    }

                    int newX = (x + 1) % maxX;
                    if (region[y][newX] == Empty)
                    {
                        processed = true;
                        sb[x] = Empty;
                        sb[newX] = oldValue;
                    }
                    else
                    {
                        sb[x] = oldValue;
                    }
                }
                newRegion[y] = sb.ToString();
            }
            return processed;
        }

        private bool ProcessSouth(string[] region, int maxX, int maxY, out string[] newRegion)
        {
            bool processed = false;
            newRegion = new string[maxY];
            string emptyLine = new string(Empty, maxX);
            StringBuilder[] sbs = new StringBuilder[maxY];
            for (int y = 0; y < maxY; ++y)
            {
                sbs[y] = new StringBuilder(emptyLine);
            }
            for (int x = 0; x < maxX; ++x)
            {
                for (int y = 0; y < maxY; ++y)
                {
                    char oldValue = region[y][x];
                    if (oldValue == East)
                    {
                        sbs[y][x] = oldValue;
                        continue;
                    }

                    if (oldValue == Empty)
                    {
                        continue;
                    }

                    int newY = (y + 1) % maxY;

                    // shift allowed
                    if (region[newY][x] == Empty)
                    {
                        processed = true;
                        sbs[y][x] = Empty;
                        sbs[newY][x] = oldValue;
                    }
                    else
                    {
                        sbs[y][x] = oldValue;
                    }
                }
            }
            newRegion = sbs.Select(sb => sb.ToString()).ToArray();
            return processed;
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables)
        {
            string[] region = inputs.ToArray();
            int maxX = region.First().Length;
            int maxY = region.Length;
            int steps = 0;
            bool east = true, south = true;
            while (east || south)
            {
                east = ProcessEast(region, maxX, maxY, out string[] newRegion);
                south = ProcessSouth(newRegion, maxX, maxY, out region);
                ++steps;
                // Util.PrintGrid(region.ToList(), DebugWriteLine);
            }
            return steps.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
        { SharedSolution(inputs, variables); return "50"; }
    }
}