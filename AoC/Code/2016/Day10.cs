using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2016
{
    class Day10 : Day
    {
        public Day10() { }
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
                Variables = new Dictionary<string, string> { { "chipOne", "2" }, { "chipTwo", "5" } },
                Output = "2",
                RawInput =
@"value 5 goes to bot 2
bot 2 gives low to bot 1 and high to bot 0
value 3 goes to bot 1
bot 1 gives low to output 1 and high to bot 0
bot 0 gives low to output 2 and high to output 0
value 2 goes to bot 2"
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

        public class Bot
        {
            public Bot()
            {
                Chips = new List<int>();
                Chips.Add(int.MaxValue);
                Chips.Add(int.MaxValue);
            }

            public void Assign(int number)
            {
                for (int i = 0; i < Chips.Count; ++i)
                {
                    if (Chips[i] == int.MaxValue)
                    {
                        Chips[i] = number;
                        break;
                    }
                }
                Chips.Sort();
            }

            public bool CanGive()
            {
                return Chips[0] != int.MaxValue && Chips[1] != int.MaxValue;
            }

            public void Give(ref Bot low, ref Bot high)
            {
                low.Assign(Chips.First());
                high.Assign(Chips.Last());

                Chips.Clear();
                Chips.Add(int.MaxValue);
                Chips.Add(int.MaxValue);
            }

            public bool IsComparing(int chipOne, int chipTwo)
            {
                return Chips[0] == chipOne && Chips[1] == chipTwo;
            }

            public List<int> Chips { get; set; }
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables)
        {
            int chipOne = 17;
            int chipTwo = 61;
            if (variables != null)
            {
                if (variables.ContainsKey(nameof(chipOne)))
                {
                    chipOne = int.Parse(variables[nameof(chipOne)]);
                }
                if (variables.ContainsKey(nameof(chipTwo)))
                {
                    chipTwo = int.Parse(variables[nameof(chipTwo)]);
                }
            }

            Dictionary<int, Bot> bots = new Dictionary<int, Bot>();
            foreach (string i in inputs.Where(i => i.StartsWith("value")).ToList())
            {
                List<int> values = i.Split(' ').Where(s => { int tryParse; return int.TryParse(s, out tryParse); }).Select(s => int.Parse(s)).ToList();
                if (!bots.ContainsKey(values[1]))
                {
                    bots[values[1]] = new Bot();
                }
                bots[values[1]].Assign(values[0]);

                if (bots[values[1]].IsComparing(chipOne, chipTwo))
                {
                    return values[1].ToString();
                }
            }

            Dictionary<int, Bot> outputs = new Dictionary<int, Bot>();
            List<string> instructions = inputs.Where(i => !i.StartsWith("value")).ToList();
            while (instructions.Count > 0)
            {
                string instruction = instructions.First();
                string[] split = instruction.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                int giverId = int.Parse(split[1]);
                if (!bots.ContainsKey(giverId))
                {
                    instructions.RemoveAt(0);
                    instructions.Add(instruction);
                    continue;
                }

                Bot giver = bots[giverId];
                if (!giver.CanGive())
                {
                    instructions.RemoveAt(0);
                    instructions.Add(instruction);
                    continue;
                }

                Bot low;
                int lowId = int.Parse(split[6]);
                bool checkLow = false;
                if (split[5] == "output")
                {
                    if (!outputs.ContainsKey(lowId))
                    {
                        outputs[lowId] = new Bot();
                    }
                    low = outputs[lowId];
                }
                else
                {
                    checkLow = true;
                    if (!bots.ContainsKey(lowId))
                    {
                        bots[lowId] = new Bot();
                    }
                    low = bots[lowId];
                }

                Bot high;
                int highId = int.Parse(split[11]);
                bool checkHigh = false;
                if (split[10] == "output")
                {
                    if (!outputs.ContainsKey(highId))
                    {
                        outputs[highId] = new Bot();
                    }
                    high = outputs[highId];
                }
                else
                {
                    checkHigh = true;
                    if (!bots.ContainsKey(highId))
                    {
                        bots[highId] = new Bot();
                    }
                    high = bots[highId];
                }

                giver.Give(ref low, ref high);
                if (checkLow)
                {
                    if (low.IsComparing(chipOne, chipTwo))
                    {
                        return lowId.ToString();
                    }
                }
                if (checkHigh)
                {
                    if (high.IsComparing(chipOne, chipTwo))
                    {
                        return highId.ToString();
                    }
                }
            }

            return "";
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);
    }
}