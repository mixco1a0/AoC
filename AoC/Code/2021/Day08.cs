using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2021
{
    class Day08 : Day
    {
        public Day08() { }

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
                Output = "26",
                RawInput =
@"be cfbegad cbdgef fgaecd cgeb fdcge agebfd fecdb fabcd edb | fdgacbe cefdb cefbgd gcbe
edbfga begcd cbg gc gcadebf fbgde acbgfd abcde gfcbed gfec | fcgedb cgb dgebacf gc
fgaebd cg bdaec gdafb agbcfd gdcbef bgcad gfac gcb cdgabef | cg cg fdcagb cbg
fbegcd cbd adcefb dageb afcb bc aefdc ecdab fgdeca fcdbega | efabcd cedba gadfec cb
aecbfdg fbg gf bafeg dbefa fcge gcbea fcaegb dgceab fcbdga | gecf egdcabf bgf bfgea
fgeab ca afcebg bdacfeg cfaedg gcfdb baec bfadeg bafgc acf | gebdcfa ecba ca fadegcb
dbcfg fgd bdegcaf fgec aegbdf ecdfab fbedc dacgb gdcebf gf | cefg dcbef fcge gbcadfe
bdfegc cbegaf gecbf dfcage bdacg ed bedf ced adcbefg gebcd | ed bcgafe cdgba cbgef
egadfb cdbfeg cegd fecab cgb gbdefca cg fgcdab egfdb bfceg | gbdfcae bgc cg cgb
gcafb gcf dcaebfg ecagb gf abcdeg gaef cafbge fdbac fegbdc | fgae cfgab fg bagce"
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

        private Dictionary<int, int> DigitCount = new Dictionary<int, int>() 
        {
            {0, 6},
            {1, 2},
            {2, 5},
            {3, 5},
            {4, 4},
            {5, 5},
            {6, 6},
            {7, 3},
            {8, 7},
            {9, 6},
        };

        private class Signal
        {
            public List<string> Patterns { get; set; }
            public List<string> Output { get; set; }

            public static Signal Parse(string input)
            {
                Signal signal = new Signal();
                string[] split = input.Split('|', StringSplitOptions.RemoveEmptyEntries);
                signal.Patterns = split[0].Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
                signal.Output = split[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
                return signal;
            }
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables)
        {
            List<Signal> signals = inputs.Select(Signal.Parse).ToList();
            List<int> uniqueValues = DigitCount.Values.GroupBy(k => k).Where(g => g.Count() == 1).Select(g => g.Key).ToList(); //.Select((d, i) => new {d, i}).Where(p => p.i == 1).Select(p => p.d).ToList();
            List<int> uniqueDigits = DigitCount.Where(p => uniqueValues.Contains(p.Value)).Select(p => p.Key).ToList();
            int sum = 0;
            foreach(Signal signal in signals)
            {
                sum += signal.Output.Where(p => uniqueValues.Contains(p.Length)).Count();
            }
            return sum.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);
    }
}