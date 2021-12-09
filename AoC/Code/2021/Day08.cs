using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AoC._2021
{
    class Day08 : Day
    {
        public Day08() { }

        public override string GetSolutionVersion(Part part)
        {
            switch (part)
            {
                case Part.One:
                    return "v2";
                case Part.Two:
                    return "v2";
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
                Output = "5353",
                RawInput =
@"acedgfb cdfbe gcdfa fbcad dab cefabd cdfgeb eafb cagedb ab | cdfeb fcadb cdfeb cdbaf"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.Two,
                Output = "61229",
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
            return testData;
        }

        private class Signal
        {
            public List<string> Patterns { get; set; }
            public List<string> Output { get; set; }

            public int Decode()
            {
                string[] translator = new string[10];
                translator[1] = Patterns.Single(p => p.Length == 2);
                translator[4] = Patterns.Single(p => p.Length == 4);
                translator[7] = Patterns.Single(p => p.Length == 3);
                translator[8] = Patterns.Single(p => p.Length == 7);
                translator[9] = Patterns.Single(p => p.Length == 6 && p.Except(translator[7]).Except(translator[4]).Count() == 1);
                translator[0] = Patterns.Single(p => p.Length == 6 && p != translator[9] && p.Except(translator[7]).Count() == 3);
                translator[6] = Patterns.Single(p => p.Length == 6 && p != translator[9] && p.Except(translator[7]).Count() == 4);
                translator[5] = Patterns.Single(p => p.Length == 5 && translator[6].Except(p).Count() == 1);
                translator[3] = Patterns.Single(p => p.Length == 5 && p != translator[5] && translator[9].Except(p).Count() == 1);
                translator[2] = Patterns.Single(p => p.Length == 5 && p != translator[5] && p != translator[3]);
                for (int i = 0; i < translator.Length; ++i)
                {
                    translator[i] = string.Concat(translator[i].OrderBy(c => c));
                }

                StringBuilder code = new StringBuilder();
                foreach (string output in Output)
                {
                    code.Append(translator.Select((translated, idx) => new { translated = translated, idx = idx }).Single(p => p.translated == output).idx);
                }
                return int.Parse(code.ToString());
            }

            public static Signal Parse(string input)
            {
                Signal signal = new Signal();
                string[] split = input.Split('|', StringSplitOptions.RemoveEmptyEntries);
                signal.Patterns = split[0].Split(' ', StringSplitOptions.RemoveEmptyEntries).OrderBy(s => s.Length).ToList();
                signal.Output = split[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(s => string.Concat(s.OrderBy(c => c))).ToList();
                return signal;
            }
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool decode)
        {
            List<Signal> signals = inputs.Select(Signal.Parse).ToList();
            HashSet<int> uniqueValues = new HashSet<int>() { 2, 4, 3, 7 };
            int sum = 0;
            foreach (Signal signal in signals)
            {
                if (decode)
                {
                    sum += signal.Decode();
                }
                else
                {
                    sum += signal.Output.Where(p => uniqueValues.Contains(p.Length)).Count();
                }
            }
            return sum.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);
    }
}