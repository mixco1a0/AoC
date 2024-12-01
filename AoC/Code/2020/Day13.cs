using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2020
{
    class Day13 : Core.Day
    {
        public Day13() { }

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

        protected override List<Core.TestDatum> GetTestData()
        {
            List<Core.TestDatum> testData = new List<Core.TestDatum>();
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.One,
                Output = "295",
                RawInput =
@"939
7,13,x,x,59,x,31,19"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
                Output = "3417",
                RawInput =
@"NA
17,x,13,19"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
                Output = "1068781",
                RawInput =
@"NA
7,13,x,x,59,x,31,19"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
                Output = "1202161486",
                RawInput =
@"NA
1789,37,47,1889"
            });
            return testData;
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            int time = int.Parse(inputs[0]);
            List<int> buses = inputs[1].Split(new char[] { 'x', ',' }, System.StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
            List<int> busWait = buses.Select(bus => time % bus).ToList();
            int waitTime = 0;
            while (true)
            {
                ++waitTime;
                for (int i = 0; i < busWait.Count; ++i)
                {
                    busWait[i] = (busWait[i] + 1) % buses[i];
                    if (busWait[i] == 0)
                    {
                        return (buses[i] * waitTime).ToString();
                    }
                }
            }
        }

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            List<KeyValuePair<int, int>> buses = inputs[1].Split(",", StringSplitOptions.RemoveEmptyEntries).Select((bus, index) => new { Digit = bus, Index = index }).Where(pair => pair.Digit != "x").Select(pair => new KeyValuePair<int, int>(int.Parse(pair.Digit), pair.Index)).ToList();

            long increment = 1;
            long start = 0;
            for (int i = 2; i <= buses.Count; ++i)
            {
                long cycleStart, cycle;
                GetPartialSolution(buses.Take(i).ToList(), start, increment, out cycleStart, out cycle);
                start = cycleStart;
                increment = cycle;
            }
            return start.ToString();
        }

        private void GetPartialSolution(List<KeyValuePair<int, int>> buses, long start, long increment, out long cycleStart, out long cycle)
        {
            cycle = 1;
            cycleStart = 0;

            bool cycleStarted = false;
            for (long time = start; time < long.MaxValue; time += increment)
            {
                bool found = true;
                for (int b = 0; b < buses.Count && found; ++b)
                {
                    if ((time + buses[b].Value) % buses[b].Key != 0)
                    {
                        found = false;
                    }
                }

                if (found)
                {
                    if (!cycleStarted)
                    {
                        cycleStarted = true;
                        cycleStart = time;
                    }
                    else
                    {
                        cycle = time - cycleStart;
                        return;
                    }
                }
            }
        }
    }
}