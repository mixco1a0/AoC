using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2020
{
    class Day16 : Day
    {
        public Day16() { }
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
                Output = "71",
                RawInput =
@"class: 1-3 or 5-7
row: 6-11 or 33-44
seat: 13-40 or 45-50

your ticket:
7,1,14

nearby tickets:
7,3,47
40,4,50
55,2,20
38,6,12"
            });
            return testData;
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            List<Base.Range> ranges = new List<Base.Range>();
            bool myTicket = false;
            int invalids = 0;
            foreach (string input in inputs)
            {
                if (input.Contains("or"))
                {
                    string[] split = input.Split("abcdefghijklmnopqrstuvwxyz: ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    string[] lower = split[0].Split('-');
                    string[] higher = split[1].Split('-');
                    ranges.Add(new Base.Range { Min = int.Parse(lower[0]), Max = int.Parse(lower[1]) });
                    ranges.Add(new Base.Range { Min = int.Parse(higher[0]), Max = int.Parse(higher[1]) });
                }
                else if (input.Contains(","))
                {
                    if (!myTicket)
                    {
                        myTicket = true;
                        continue;
                    }

                    int[] split = input.Split(',').Select(int.Parse).ToArray();
                    for (int i = 0; i < split.Length; ++i)
                    {
                        if (ranges.Where(range => range.HasInc(split[i])).Count() <= 0)
                        {
                            invalids += split[i];
                        }
                    }
                }
            }
            return invalids.ToString();
        }

        class TicketInfo
        {
            public string Name { get; set; }
            public Base.Range Lower { get; set; }
            public Base.Range Higher { get; set; }

            public override string ToString()
            {
                return $"{Name}: {Lower.Min}-{Lower.Max} or {Higher.Min}-{Higher.Max}";
            }
        }

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            List<TicketInfo> ticketInfo = new List<TicketInfo>();
            bool myTicket = false;
            List<int> myTicketValues = new List<int>();
            List<List<int>> validTickets = new List<List<int>>();
            foreach (string input in inputs)
            {
                if (input.Contains("or"))
                {
                    string name = input.Split(':').First();
                    string[] split = input.Split("abcdefghijklmnopqrstuvwxyz: ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    string[] lower = split[0].Split('-');
                    string[] higher = split[1].Split('-');
                    ticketInfo.Add(new TicketInfo { Name = name, Lower = new Base.Range { Min = int.Parse(lower[0]), Max = int.Parse(lower[1]) }, Higher = new Base.Range { Min = int.Parse(higher[0]), Max = int.Parse(higher[1]) } });
                }
                else if (input.Contains(","))
                {
                    if (!myTicket)
                    {
                        myTicket = true;
                        myTicketValues = input.Split(',').Select(int.Parse).ToList();
                        continue;
                    }

                    int[] split = input.Split(',').Select(int.Parse).ToArray();
                    bool valid = true;
                    for (int i = 0; i < split.Length && valid; ++i)
                    {
                        if (ticketInfo.Where(info => info.Lower.HasInc(split[i]) || info.Higher.HasInc(split[i])).Count() <= 0)
                        {
                            valid = false;
                        }
                    }

                    if (valid)
                    {
                        validTickets.Add(input.Split(',').Select(int.Parse).ToList());
                    }
                }
            }

            // solve each index
            List<List<string>> possibilities = new List<List<string>>();
            for (int i = 0; i < myTicketValues.Count; ++i)
            {
                possibilities.Add(new List<string>());
                foreach (List<int> ticket in validTickets)
                {
                    int valueToCheck = ticket[i];
                    var inRange = ticketInfo.Where(info => info.Lower.HasInc(valueToCheck) || info.Higher.HasInc(valueToCheck)).ToList();
                    if (inRange.Count() > 0)
                    {
                        if (possibilities[i].Count == 0)
                        {
                            possibilities[i].AddRange(inRange.Select(info => info.Name));
                        }
                        else
                        {
                            possibilities[i] = possibilities[i].Intersect(inRange.Select(info => info.Name)).ToList();
                        }
                    }

                    if (possibilities[i].Count == 0)
                    {
                        break;
                    }
                }
            }

            List<string> removals = new List<string>();
            while (true)
            {
                foreach (List<string> list in possibilities)
                {
                    if (list.Count == 1)
                    {
                        removals.Add(list.First());
                    }
                }

                bool removed = false;
                foreach (List<string> list in possibilities)
                {
                    if (list.Count != 1)
                    {
                        removed = true;
                        list.RemoveAll(l => removals.Contains(l));
                    }
                }

                if (!removed)
                {
                    break;
                }
            }

            Dictionary<string, int> nameToIdx = possibilities.Select(list => list.First()).Select((value,index) => new {Value=value,Index=index}).ToDictionary(pair => pair.Value, pair => pair.Index);
            var smalls = nameToIdx.Where(pair => pair.Key.Contains("departure")).ToList();
            long multValue = 1;
            foreach (var pair in smalls)
            {
                multValue *= myTicketValues[pair.Value];
            }

            return multValue.ToString();
        }
    }
}