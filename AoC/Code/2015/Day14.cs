using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2015
{
    class Day14 : Day
    {
        private static int sTimes = 2503;
        public Day14() { }
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
                Variables = new Dictionary<string, string> { { nameof(sTimes), "1000" } },
                Output = "1120",
                RawInput =
@"Comet can fly 14 km/s for 10 seconds, but then must rest for 127 seconds.
Dancer can fly 16 km/s for 11 seconds, but then must rest for 162 seconds."
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

        private class Reindeer
        {
            public string Name { get; set; }
            public int Speed { get; set; }
            public int Burst { get; set; }
            public int Cooldown { get; set; }
            public int Distance { get; set; }

            private int m_timeTillSwap;
            private enum State
            {
                Flying,
                Resting
            }
            private State m_state;

            static public Reindeer Parse(string input)
            {
                Reindeer reindeer = new Reindeer();
                string[] split = input.Split(' ');
                reindeer.Name = split[0];
                int[] values = split.Where(s => { int v; return int.TryParse(s, out v); }).Select(int.Parse).ToArray();
                reindeer.Speed = values[0];
                reindeer.Burst = values[1];
                reindeer.Cooldown = values[2];

                reindeer.Distance = 0;
                reindeer.m_timeTillSwap = reindeer.Burst;
                reindeer.m_state = State.Flying;
                return reindeer;
            }

            public void Update(int seconds)
            {
                m_timeTillSwap -= seconds;

                switch (m_state)
                {
                    case State.Flying:
                        Distance += Speed;
                        if (m_timeTillSwap == 0)
                        {
                            m_state = State.Resting;
                            m_timeTillSwap = Cooldown;
                        }
                        break;
                    case State.Resting:
                        if (m_timeTillSwap == 0)
                        {
                            m_state = State.Flying;
                            m_timeTillSwap = Burst;
                        }
                        break;
                }
            }

            public override string ToString()
            {
                return $"{Name} - {Distance} | {m_state} | {m_timeTillSwap}";
            }
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            int times = sTimes;
            if (variables != null && variables.ContainsKey(nameof(sTimes)))
            {
                times = int.Parse(variables[nameof(sTimes)]);
            }

            List<Reindeer> allReindeer = inputs.Select(Reindeer.Parse).ToList();
            for (int i = 0; i < times; ++i)
            {
                foreach (Reindeer reindeer in allReindeer)
                {
                    reindeer.Update(1);
                }
            }
            return allReindeer.Max(r => r.Distance).ToString();
        }

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            return "";
        }
    }
}