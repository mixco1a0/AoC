using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2015
{
    class Day09 : Core.Day
    {
        public Day09() { }

        public override string GetSolutionVersion(Core.Part part)
        {
            switch (part)
            {
                case Core.Part.One:
                    return "v1";
                case Core.Part.Two:
                    return "v1";
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
                Output = "605",
                RawInput =
@"London to Dublin = 464
London to Belfast = 518
Dublin to Belfast = 141"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
                Output = "982",
                RawInput =
@"London to Dublin = 464
London to Belfast = 518
Dublin to Belfast = 141"
            });
            return testData;
        }

        public record Distance(string A, string B, int Dist) { }

        public record ToInfo(string Destination, int Distance) { }

        private int FindPath(Dictionary<string, List<ToInfo>> map, bool findMin)
        {
            List<string> cities = map.Keys.ToList();
            int min = int.MaxValue;
            int max = int.MinValue;
            foreach (string city in cities)
            {
                min = Math.Min(FindPathRecurse(map, city, cities.Where(c => c != city).ToList(), findMin), min);
                max = Math.Max(FindPathRecurse(map, city, cities.Where(c => c != city).ToList(), findMin), max);
            }
            return findMin ? min : max;
        }

        private int FindPathRecurse(Dictionary<string, List<ToInfo>> map, string startCity, List<string> visitableCities, bool findMin)
        {
            if (visitableCities.Count == 0)
            {
                return 0;
            }

            List<ToInfo> curToInfo = map[startCity];
            int dist = 0;
            if (findMin)
            {
                dist = curToInfo.Where(i => visitableCities.Contains(i.Destination)).Min(i => i.Distance);
            }
            else
            {
                dist = curToInfo.Where(i => visitableCities.Contains(i.Destination)).Max(i => i.Distance);
            }
            string nextCity = curToInfo.Where(i => visitableCities.Contains(i.Destination)).Where(i => i.Distance == dist).First().Destination;
            Log(Core.Log.ELevel.Spam, $"{startCity} >--[{dist}]--> {nextCity}");
            return dist + FindPathRecurse(map, nextCity, visitableCities.Where(c => c != nextCity).ToList(), findMin);
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool findMin)
        {
            Dictionary<string, List<ToInfo>> map = new Dictionary<string, List<ToInfo>>();
            List<Distance> distances = new List<Distance>();
            foreach (string input in inputs)
            {
                string[] splits = input.Split(" =".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                string cityA = splits[0];
                string cityB = splits[2];
                int distance = int.Parse(splits[3]);

                if (!map.ContainsKey(cityA))
                {
                    map[cityA] = new List<ToInfo>();
                }
                map[cityA].Add(new ToInfo(cityB, distance));

                if (!map.ContainsKey(cityB))
                {
                    map[cityB] = new List<ToInfo>();
                }
                map[cityB].Add(new ToInfo(cityA, distance));
            }
            return FindPath(map, findMin).ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);
    }
}