using System.Collections.Generic;
using System.Linq;

namespace AoC._2020
{
    class Day13 : Day
    {
        public Day13() { }
        protected override List<TestDatum> GetTestData()
        {
            List<TestDatum> testData = new List<TestDatum>();
            testData.Add(new TestDatum
            {
                TestPart = TestPart.One,
                Output = "295",
                RawInput =
@"939
7,13,x,x,59,x,31,19"
            });
            testData.Add(new TestDatum
            {
                TestPart = TestPart.Two,
                Output = "",
                RawInput =
@""
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
            return "";
        }

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            return "";
        }
    }
}