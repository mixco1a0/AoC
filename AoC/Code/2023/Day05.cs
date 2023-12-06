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
                Output = "",
                RawInput =
@""
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

            public List<long> Seeds { get; set; }
            public List<Base.KeyVal<Base.RangeL, long>> SoilMap { get; set; }
            public List<Base.KeyVal<Base.RangeL, long>> FertilizerMap { get; set; }
            public List<Base.KeyVal<Base.RangeL, long>> WaterMap { get; set; }
            public List<Base.KeyVal<Base.RangeL, long>> LightMap { get; set; }
            public List<Base.KeyVal<Base.RangeL, long>> TempMap { get; set; }
            public List<Base.KeyVal<Base.RangeL, long>> HumidityMap { get; set; }
            public List<Base.KeyVal<Base.RangeL, long>> LocMap { get; set; }
            public Action<string> PrintFunc { get; set; }

            public Almanac(List<string> inputs)
            {
                Seeds = new List<long>();
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
                        Seeds = input.Split(": ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Where(s => long.TryParse(s, out long result)).Select(long.Parse).ToList();
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
                //convert seeds to soil
                List<long> numbersToConvert = new List<long>(Seeds);
                Convert(Mapping.Soil, ref numbersToConvert);
                Convert(Mapping.Fertilizer, ref numbersToConvert);
                Convert(Mapping.Water, ref numbersToConvert);
                Convert(Mapping.Light, ref numbersToConvert);
                Convert(Mapping.Temp, ref numbersToConvert);
                Convert(Mapping.Humidity, ref numbersToConvert);
                Convert(Mapping.Loc, ref numbersToConvert);
                return numbersToConvert.Min();
            }

            private void Convert(Mapping mapping, ref List<long> numbers)
            {
                GetMap(mapping, out List<Base.KeyVal<Base.RangeL, long>> list);
                List<long> converted = new List<long>();
                foreach (long number in numbers)
                {
                    IEnumerable<Base.KeyVal<Base.RangeL, long>> keyVals = list.Where(pair => pair.Key.HasInc(number));
                    if (keyVals.Any())
                    {
                        converted.Add(number + keyVals.First().Val);
                    }
                    else
                    {
                        // value stays the same
                        converted.Add(number);
                    }
                    PrintFunc($"[{mapping.ToString()}] | Num [{number}] -> {converted.Last()}");
                }
                numbers = converted;
            }
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables)
        {
            Almanac almanac = new Almanac(inputs);
            // almanac.PrintFunc = DebugWriteLine;
            return almanac.GetLowestLocation().ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);
    }
}