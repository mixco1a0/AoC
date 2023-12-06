using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2023
{
    class Day05 : Core.Day
    {
        public Day05() { }

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
                Output = "35",
                RawInput =
@"seeds: 79 14 55 13

seed-to-soil map:
50 98 2
52 50 48

soil-to-fertilizer map:
0 15 37
37 52 2
39 0 15

fertilizer-to-water map:
49 53 8
0 11 42
42 0 7
57 7 4

water-to-light map:
88 18 7
18 25 70

light-to-temperature map:
45 77 23
81 45 19
68 64 13

temperature-to-humidity map:
0 69 1
1 0 69

humidity-to-location map:
60 56 37
56 93 4"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
                Output = "46",
                RawInput =
@"seeds: 79 14 55 13

seed-to-soil map:
50 98 2
52 50 48

soil-to-fertilizer map:
0 15 37
37 52 2
39 0 15

fertilizer-to-water map:
49 53 8
0 11 42
42 0 7
57 7 4

water-to-light map:
88 18 7
18 25 70

light-to-temperature map:
45 77 23
81 45 19
68 64 13

temperature-to-humidity map:
0 69 1
1 0 69

humidity-to-location map:
60 56 37
56 93 4"
            });
            return testData;
        }

        public class Almanac
        {
            private enum Mapping
            {
                Seed,
                Soil,
                Fertilizer,
                Water,
                Light,
                Temp,
                Humidity,
                Loc
            };

            public List<Base.RangeL> Seeds { get; set; }
            public List<Base.KeyVal<Base.RangeL, long>> SoilMap { get; set; }
            public List<Base.KeyVal<Base.RangeL, long>> FertilizerMap { get; set; }
            public List<Base.KeyVal<Base.RangeL, long>> WaterMap { get; set; }
            public List<Base.KeyVal<Base.RangeL, long>> LightMap { get; set; }
            public List<Base.KeyVal<Base.RangeL, long>> TempMap { get; set; }
            public List<Base.KeyVal<Base.RangeL, long>> HumidityMap { get; set; }
            public List<Base.KeyVal<Base.RangeL, long>> LocMap { get; set; }
            public Action<string> PrintFunc { get; set; }

            public Almanac(List<string> inputs, bool simpleParse)
            {
                Seeds = new List<Base.RangeL>();
                SoilMap = new List<Base.KeyVal<Base.RangeL, long>>();
                FertilizerMap = new List<Base.KeyVal<Base.RangeL, long>>();
                WaterMap = new List<Base.KeyVal<Base.RangeL, long>>();
                LightMap = new List<Base.KeyVal<Base.RangeL, long>>();
                TempMap = new List<Base.KeyVal<Base.RangeL, long>>();
                HumidityMap = new List<Base.KeyVal<Base.RangeL, long>>();
                LocMap = new List<Base.KeyVal<Base.RangeL, long>>();
                PrintFunc = (_) => { };

                Mapping mapping = Mapping.Seed;
                foreach (string input in inputs)
                {
                    if (string.IsNullOrWhiteSpace(input))
                    {
                        ++mapping;
                        continue;
                    }

                    if (mapping != Mapping.Seed)
                    {
                        if (!char.IsAsciiDigit(input[0]))
                        {
                            continue;
                        }
                    }

                    if (mapping == Mapping.Seed)
                    {
                        Seeds = input.Split(": ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Where(s => long.TryParse(s, out long result)).Select(long.Parse).Select(s => new Base.RangeL(s, s)).ToList();
                        if (!simpleParse)
                        {
                            Queue<Base.RangeL> complexList = new Queue<Base.RangeL>(Seeds);
                            Seeds.Clear();
                            while (complexList.Count > 0)
                            {
                                Base.RangeL seed = complexList.Dequeue();
                                Base.RangeL count = complexList.Dequeue();
                                Seeds.Add(new Base.RangeL(seed.Min, seed.Min + count.Min));
                            }
                        }
                    }
                    else
                    {
                        GetMap(mapping, out List<Base.KeyVal<Base.RangeL, long>> list);
                        ParseList(input, ref list);
                    }
                }
            }

            private void GetMap(Mapping mapping, out List<Base.KeyVal<Base.RangeL, long>> list)
            {
                list = null;
                switch (mapping)
                {
                    case Mapping.Soil:
                        list = SoilMap;
                        break;
                    case Mapping.Fertilizer:
                        list = FertilizerMap;
                        break;
                    case Mapping.Water:
                        list = WaterMap;
                        break;
                    case Mapping.Light:
                        list = LightMap;
                        break;
                    case Mapping.Temp:
                        list = TempMap;
                        break;
                    case Mapping.Humidity:
                        list = HumidityMap;
                        break;
                    case Mapping.Loc:
                        list = LocMap;
                        break;
                }
            }

            private void ParseList(string input, ref List<Base.KeyVal<Base.RangeL, long>> curList)
            {
                long[] split = input.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToArray();
                //split[0] = destination
                //split[1] = source
                //split[2] = range length
                long diff = split[0] - split[1];
                Base.RangeL sourceToDest = new Base.RangeL(split[1], split[1] + split[2] - 1);
                curList.Add(new Base.KeyVal<Base.RangeL, long>(sourceToDest, diff));
            }

            public long GetLowestLocation()
            {
                List<Base.RangeL> numbersToConvert = new List<Base.RangeL>(Seeds);
                Convert(Mapping.Soil, ref numbersToConvert);
                Convert(Mapping.Fertilizer, ref numbersToConvert);
                Convert(Mapping.Water, ref numbersToConvert);
                Convert(Mapping.Light, ref numbersToConvert);
                Convert(Mapping.Temp, ref numbersToConvert);
                Convert(Mapping.Humidity, ref numbersToConvert);
                Convert(Mapping.Loc, ref numbersToConvert);
                return numbersToConvert.Min(n => n.Min);
            }

            private void Convert(Mapping mapping, ref List<Base.RangeL> ranges)
            {
                GetMap(mapping, out List<Base.KeyVal<Base.RangeL, long>> list);
                List<Base.RangeL> converted = new List<Base.RangeL>();
                foreach (Base.RangeL range in ranges)
                {
                    for (long curNumber = range.Min; curNumber <= range.Max; ++curNumber)
                    {
                        IEnumerable<Base.KeyVal<Base.RangeL, long>> keyVals = list.Where(pair => pair.Key.HasInc(curNumber));
                        if (keyVals.Any())
                        {
                            Base.KeyVal<Base.RangeL, long> curKey = keyVals.First();
                            long maxCount = Math.Min(curKey.Key.Max, range.Max) - curNumber;
                            long convertedValue = curNumber + curKey.Val;

                            Base.RangeL preConvertedRange = new Base.RangeL(curNumber, curNumber + maxCount);
                            Base.RangeL convertedRange = new Base.RangeL(convertedValue, convertedValue + maxCount);
                            PrintFunc($"[{mapping.ToString()}] | [key found] converted [{preConvertedRange}] -> {convertedRange}");
                            converted.Add(convertedRange);
                            curNumber += maxCount;
                        }
                        else
                        {
                            IEnumerable<long> minKeys = list.Select(pair => pair.Key.Min).Where(k => k >= curNumber);
                            if (minKeys.Any())
                            {
                                long maxValue = Math.Min(minKeys.Min() - 1, range.Max);
                                converted.Add(new Base.RangeL(curNumber, maxValue));
                                curNumber = maxValue;
                                PrintFunc($"[{mapping.ToString()}] | [partial unfound] converted [{converted.Last()}] -> {converted.Last()}");
                            }
                            else
                            {
                                converted.Add(new Base.RangeL(curNumber, range.Max));
                                PrintFunc($"[{mapping.ToString()}] | [complete unfound] converted [{converted.Last()}] -> {converted.Last()}");
                                break;
                            }
                        }
                    }
                }
                ranges = converted;
            }
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool simpleParse)
        {
            Almanac almanac = new Almanac(inputs, simpleParse);
            // almanac.PrintFunc = DebugWriteLine;
            return almanac.GetLowestLocation().ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);
    }
}