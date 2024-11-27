using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2022
{
    class Day03 : Core.Day
    {
        public Day03() { }

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
                Output = "157",
                RawInput =
@"vJrwpWtwJgWrhcsFMMfFFhFp
jqHRNqRjqzjGDLGLrsFMfFZSrLrFZsSL
PmmdzqPrVvPwwTWBwg
wMqvLMZHhHMvwLHjbvcjnnSBnvTQFn
ttgJtRGJQctTZtZT
CrZsJsPPZsGzwwsLwLmpwMDw"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
                Output = "70",
                RawInput =
@"vJrwpWtwJgWrhcsFMMfFFhFp
jqHRNqRjqzjGDLGLrsFMfFZSrLrFZsSL
PmmdzqPrVvPwwTWBwg
wMqvLMZHhHMvwLHjbvcjnnSBnvTQFn
ttgJtRGJQctTZtZT
CrZsJsPPZsGzwwsLwLmpwMDw"
            });
            return testData;
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool checkCompartments)
        {
            // a = 97 -> 1  pt  (-96)
            // A = 65 -> 27 pts (-38)
            int priorities = 0;
            for (int i = 0; i < inputs.Count;)
            {
                char shared = ' ';
                if (checkCompartments)
                {
                    int size = inputs[i].Length/2;
                    string l = inputs[i].Substring(0, size);
                    string r = inputs[i].Substring(size);
                    shared = l.Intersect(r).Single();
                    ++i;
                }
                else
                {
                    shared = inputs[i++].Intersect(inputs[i++].Intersect(inputs[i++])).Single();
                }

                if (shared >= 'a' && shared <= 'z')
                {
                    priorities += (int)shared - 96;
                }
                else
                {
                    priorities += (int)shared - 38;
                }
            }
            return priorities.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);
    }
}