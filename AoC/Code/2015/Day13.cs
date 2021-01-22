using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2015
{
    class Day13 : Day
    {
        public Day13() { }
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
                Output = "330",
                RawInput =
@"Alice would gain 54 happiness units by sitting next to Bob.
Alice would lose 79 happiness units by sitting next to Carol.
Alice would lose 2 happiness units by sitting next to David.
Bob would gain 83 happiness units by sitting next to Alice.
Bob would lose 7 happiness units by sitting next to Carol.
Bob would lose 63 happiness units by sitting next to David.
Carol would lose 62 happiness units by sitting next to Alice.
Carol would gain 60 happiness units by sitting next to Bob.
Carol would gain 55 happiness units by sitting next to David.
David would gain 46 happiness units by sitting next to Alice.
David would lose 7 happiness units by sitting next to Bob.
David would gain 41 happiness units by sitting next to Carol."
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

        private record Units(string Name, int Happiness);
        private record Pair(string Name1, int Happiness, string Name2);

        private int ArrangeSeats(Dictionary<string, List<Units>> people, string person, List<string> peopleSitting, int tab)
        {
            if (!peopleSitting.Contains(person))
            {
                peopleSitting.Add(person);

                List<string> usables = people.Keys.Where(k => !peopleSitting.Contains(k)).ToList();
                if (usables.Count == 0)
                {
                    usables.Add(peopleSitting.First());
                }
                int max = int.MinValue;
                foreach (string usable in usables)
                {
                    List<Units> units = people.Where(p => p.Key == person).First().Value;
                    string nextTo = peopleSitting.First();

                    int h1 = 0;
                    if (people.Keys.Count > peopleSitting.Count)
                    {
                        Units usableUnit = units.Where(u => u.Name == usable).First();
                        h1 = usableUnit.Happiness;
                        nextTo = usableUnit.Name;
                    }
                    else
                    {
                        h1 = units.Where(u => u.Name == nextTo).First().Happiness;
                    }
                    int curHappiness = 0;
                    curHappiness += h1;
                    curHappiness += people.Where(p => p.Key == nextTo).First().Value.Where(u => u.Name == person).First().Happiness;
                    // DebugWriteLine($"{new string('\t', tab)}{person} <={curHappiness}=> {nextTo}");
                    curHappiness += ArrangeSeats(people, nextTo, new List<string>(peopleSitting), tab + 1);

                    max = Math.Max(max, curHappiness);
                }
                return max;
            }
            return 0;
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            Dictionary<string, List<Units>> people = new Dictionary<string, List<Units>>();
            foreach (string input in inputs)
            {
                string[] split = input.Split(" .".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                if (!people.Keys.Contains(split[0]))
                {
                    people[split[0]] = new List<Units>();
                }
                people[split[0]].Add(new Units(split.Last(), (split[2] == "gain" ? 1 : -1) * int.Parse(split[3])));
            }
            List<Pair> pairs = new List<Pair>();

            int max = int.MinValue;
            foreach (var pair in people)
            {
                int h = ArrangeSeats(people, pair.Key, new List<string>(), 0);
                max = Math.Max(max, h);
            }
            return max.ToString();
        }

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            return "";
        }
    }
}