using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2022
{
    class Day15 : Core.Day
    {
        public Day15() { }

        public override string GetSolutionVersion(Core.Part part)
        {
            switch (part)
            {
                // case Core.Part.One:
                //     return "v1";
                // case Core.Part.Two:
                //     return "v1";
                default:
                    return base.GetSolutionVersion(part);
            }
        }

        public override bool SkipTestData => false;

        protected override List<Core.TestDatum> GetTestData()
        {
            List<Core.TestDatum> testData = new List<Core.TestDatum>();
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.One,
                Variables = new Dictionary<string, string> { { nameof(_Row), "10" } },
                Output = "26",
                RawInput =
@"Sensor at x=2, y=18: closest beacon is at x=-2, y=15
Sensor at x=9, y=16: closest beacon is at x=10, y=16
Sensor at x=13, y=2: closest beacon is at x=15, y=3
Sensor at x=12, y=14: closest beacon is at x=10, y=16
Sensor at x=10, y=20: closest beacon is at x=10, y=16
Sensor at x=14, y=17: closest beacon is at x=10, y=16
Sensor at x=8, y=7: closest beacon is at x=2, y=10
Sensor at x=2, y=0: closest beacon is at x=2, y=10
Sensor at x=0, y=11: closest beacon is at x=2, y=10
Sensor at x=20, y=14: closest beacon is at x=25, y=17
Sensor at x=17, y=20: closest beacon is at x=21, y=22
Sensor at x=16, y=7: closest beacon is at x=15, y=3
Sensor at x=14, y=3: closest beacon is at x=15, y=3
Sensor at x=20, y=1: closest beacon is at x=15, y=3"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
                Variables = new Dictionary<string, string> { { nameof(_Row), "10" }, { nameof(_MaxX), "20" }, { nameof(_MaxY), "20" } },
                Output = "56000011",
                RawInput =
@"Sensor at x=2, y=18: closest beacon is at x=-2, y=15
Sensor at x=9, y=16: closest beacon is at x=10, y=16
Sensor at x=13, y=2: closest beacon is at x=15, y=3
Sensor at x=12, y=14: closest beacon is at x=10, y=16
Sensor at x=10, y=20: closest beacon is at x=10, y=16
Sensor at x=14, y=17: closest beacon is at x=10, y=16
Sensor at x=8, y=7: closest beacon is at x=2, y=10
Sensor at x=2, y=0: closest beacon is at x=2, y=10
Sensor at x=0, y=11: closest beacon is at x=2, y=10
Sensor at x=20, y=14: closest beacon is at x=25, y=17
Sensor at x=17, y=20: closest beacon is at x=21, y=22
Sensor at x=16, y=7: closest beacon is at x=15, y=3
Sensor at x=14, y=3: closest beacon is at x=15, y=3
Sensor at x=20, y=1: closest beacon is at x=15, y=3"
            });
            return testData;
        }

        private int _Row { get; }
        private int _MaxX { get; }
        private int _MaxY { get; }

        private class Sensor
        {
            public Base.LongPosition Pos { get; set; }
            public Base.LongPosition ClosestBeacon { get; set; }

            public Sensor()
            {
                Pos = new Base.LongPosition();
                ClosestBeacon = new Base.LongPosition();
            }

            public long GetManhatten()
            {
                return Pos.Manhattan(ClosestBeacon);
            }

            public static Sensor Parse(string input)
            {
                int[] split = input.Split(" =,:".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Where(s => int.TryParse(s, out int i)).Select(int.Parse).ToArray();
                Sensor sensor = new Sensor();
                sensor.Pos.X = split[0];
                sensor.Pos.Y = split[1];
                sensor.ClosestBeacon.X = split[2];
                sensor.ClosestBeacon.Y = split[3];
                return sensor;
            }
        }

        private List<Base.LongRange> GetBlockedRanges(List<Sensor> sensors, long row)
        {
            List<Base.LongRange> blockedRanges = new List<Base.LongRange>();
            foreach (Sensor sensor in sensors)
            {
                long manhattan = sensor.GetManhatten();
                long distToRow = Math.Abs(sensor.Pos.Y - row);
                if (distToRow > manhattan)
                {
                    continue;
                }

                blockedRanges.Add(new Base.LongRange(sensor.Pos.X - (manhattan - distToRow), sensor.Pos.X + (manhattan - distToRow)));
            }
            return blockedRanges.OrderBy(r => r.First).ToList();
        }

        private List<Base.LongRange> CompressRanges(List<Base.LongRange> blockedRanges, int minX, int maxX)
        {
            List<Base.LongRange> compressed = new List<Base.LongRange>();
            foreach (Base.LongRange range in blockedRanges)
            {
                int matchingIdx = -1;
                for (int i = 0; i < compressed.Count; ++i)
                {
                    if (compressed[i].HasInc(range.First) || compressed[i].HasInc(range.Last))
                    {
                        matchingIdx = i;
                        break;
                    }
                }
                if (matchingIdx >= 0)
                {
                    compressed[matchingIdx].First = Math.Max(Math.Min(compressed[matchingIdx].First, range.First), minX);
                    compressed[matchingIdx].Last = Math.Min(Math.Max(compressed[matchingIdx].Last, range.Last), maxX);
                }
                else
                {
                    compressed.Add(range);
                }
            }

            return compressed;
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool findBeacon)
        {
            List<Sensor> sensors = inputs.Select(Sensor.Parse).ToList();
            if (!findBeacon)
            {
                GetVariable(nameof(_Row), 2000000, variables, out int row);
                List<Base.LongRange> blockedRanges = GetBlockedRanges(sensors, row);
                List<Base.LongRange> compressedRanges = CompressRanges(blockedRanges, int.MinValue, int.MaxValue);
                return compressedRanges.Select(c => c.Last - c.First).Sum().ToString();
            }
            else
            {
                GetVariable(nameof(_MaxX), 4000000, variables, out int maxX);
                GetVariable(nameof(_MaxY), 4000000, variables, out int maxY);
                for (long y = 0; y < maxY; ++y)
                {
                    List<Base.LongRange> blockedRanges = GetBlockedRanges(sensors, y);
                    List<Base.LongRange> compressedRanges = CompressRanges(blockedRanges, 0, maxX);
                    if (compressedRanges.Select(c => c.Last - c.First).Sum() != maxX)
                    {
                        return ((compressedRanges[0].Last + 1) * 4000000 + y).ToString();
                    }
                }
            }

            return string.Empty;
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);
    }
}