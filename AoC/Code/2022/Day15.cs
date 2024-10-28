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
                case Core.Part.One:
                    return "v2";
                case Core.Part.Two:
                    return "v2";
                default:
                    return base.GetSolutionVersion(part);
            }
        }

        public override bool SkipTestData => true;

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
            public Base.Vec2L Pos { get; set; }
            public Base.Vec2L ClosestBeacon { get; set; }

            public Sensor()
            {
                Pos = new Base.Vec2L();
                ClosestBeacon = new Base.Vec2L();
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

        private List<Base.RangeL> GetBlockedRanges(List<Sensor> sensors, long row)
        {
            List<Base.RangeL> blockedRanges = new List<Base.RangeL>();
            foreach (Sensor sensor in sensors)
            {
                long manhattan = sensor.GetManhatten();
                long distToRow = Math.Abs(sensor.Pos.Y - row);
                if (distToRow > manhattan)
                {
                    continue;
                }

                blockedRanges.Add(new Base.RangeL(sensor.Pos.X - (manhattan - distToRow), sensor.Pos.X + (manhattan - distToRow)));
            }
            return blockedRanges.OrderBy(r => r.Min).ToList();
        }

        private List<Base.RangeL> CompressRanges(List<Base.RangeL> blockedRanges, int minX, int maxX)
        {
            List<Base.RangeL> compressed = new List<Base.RangeL>();
            foreach (Base.RangeL range in blockedRanges)
            {
                int matchingIdx = -1;
                for (int i = 0; i < compressed.Count; ++i)
                {
                    Base.RangeL curCompressed = compressed[i];
                    if (curCompressed.HasIncOr(range) || range.HasIncOr(curCompressed))
                    {
                        matchingIdx = i;
                        break;
                    }
                    else if (curCompressed.Min == range.Max + 1 || range.Min == curCompressed.Max + 1)
                    {
                        matchingIdx = i;
                        break;
                    }
                }
                if (matchingIdx >= 0)
                {
                    compressed[matchingIdx].Min = Math.Max(Math.Min(compressed[matchingIdx].Min, range.Min), minX);
                    compressed[matchingIdx].Max = Math.Min(Math.Max(compressed[matchingIdx].Max, range.Max), maxX);
                }
                else
                {
                    compressed.Add(new Base.RangeL(Math.Max(minX, range.Min), Math.Min(maxX, range.Max)));
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
                List<Base.RangeL> blockedRanges = GetBlockedRanges(sensors, row);
                List<Base.RangeL> compressedRanges = CompressRanges(blockedRanges, int.MinValue, int.MaxValue);
                return compressedRanges.Select(c => c.Max - c.Min).Sum().ToString();
            }
            else
            {
                GetVariable(nameof(_MaxX), 4000000, variables, out int maxX);
                GetVariable(nameof(_MaxY), 4000000, variables, out int maxY);
                for (long y = maxY; y > 0; --y)
                {
                    List<Base.RangeL> blockedRanges = GetBlockedRanges(sensors, y);
                    List<Base.RangeL> compressedRanges = CompressRanges(blockedRanges, 0, maxX);
                    if (compressedRanges.Select(c => c.Max - c.Min).Sum() != maxX)
                    {
                        return ((compressedRanges[0].Max + 1) * 4000000 + y).ToString();
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