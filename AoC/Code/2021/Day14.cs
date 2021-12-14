using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2021
{
    class Day14 : Day
    {
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
                //Variables = new Dictionary<string, string>() { { "steps", "5" } },
                Output = "1588",
                RawInput =
@"NNCB

CH -> B
HH -> N
CB -> H
NH -> C
HB -> C
HC -> B
HN -> C
NN -> C
BH -> H
NC -> B
NB -> B
BN -> B
BB -> N
BC -> B
CC -> N
CN -> C"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.Two,
                Output = "2188189693529",
                RawInput =
@"NNCB

CH -> B
HH -> N
CB -> H
NH -> C
HB -> C
HC -> B
HN -> C
NN -> C
BH -> H
NC -> B
NB -> B
BN -> B
BB -> N
BC -> B
CC -> N
CN -> C"
            });
            return testData;
        }

        private record PolymerPair(string End, Dictionary<char, long> LetterCounts) { }

        private PolymerPair GetPolymerGeneration(Dictionary<string, char> insertionRules, string start)
        {
            string polymer = start;
            for (int i = 0; i < 10; ++i)
            {
                StringBuilder sb = new StringBuilder();
                for (int j = 0; j < polymer.Length - 1; ++j)
                {
                    sb.Append(polymer[j]);
                    sb.Append(insertionRules[polymer.Substring(j, 2)]);
                }
                sb.Append(polymer.Last());
                polymer = sb.ToString();
            }
            return new PolymerPair(polymer, polymer.GroupBy(c => c).ToDictionary(g => g.Key, g => (long)g.Count()));
        }

        private void RunGeneration(string start, int generationsLeft, Dictionary<string, PolymerPair> generationData, ref Dictionary<char, long> elementCounts)
        {
            if (generationsLeft == 0)
            {
                return;
            }

            for (int i = 0; i < start.Length - 1; ++i)
            {
                string cur = start.Substring(i, 2);
                PolymerPair curPair = generationData[cur];
                foreach (KeyValuePair<char, long> c2l in curPair.LetterCounts)
                {
                    elementCounts[c2l.Key] += c2l.Value;
                }
                elementCounts[cur[0]] -= 1;
                RunGeneration(curPair.End, generationsLeft - 1, generationData, ref elementCounts);
            }
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, int generationCount)
        {
            string polymer = inputs.First();
            Dictionary<string, char> insertionRules = new Dictionary<string, char>();
            foreach (string input in inputs.Skip(2))
            {
                string[] split = input.Split(" ->".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                insertionRules[split[0]] = split[1][0];
            }

            Dictionary<string, string[]> insertionPairs = new Dictionary<string, string[]>();
            foreach (KeyValuePair<string, char> rule in insertionRules)
            {
                insertionPairs[rule.Key] = new string[] { $"{rule.Key.First()}{rule.Value}", $"{rule.Value}{rule.Key.Last()}" };
            }

            Dictionary<char, long> elementAdjustments = new Dictionary<char, long>();

            Dictionary<string, long> currentGen = new Dictionary<string, long>();
            foreach (KeyValuePair<string, string[]> pair in insertionPairs)
            {
                currentGen[pair.Key] = 0;
            }

            for (int i = 0; i < polymer.Length - 1; ++i)
            {
                string cur = polymer.Substring(i, 2);
                currentGen[cur]++;
                if (!elementAdjustments.ContainsKey(cur.Last()))
                {
                    elementAdjustments[cur.Last()] = 0;
                }
                elementAdjustments[cur.Last()] += 1;
            }

            for (int i = 0; i < generationCount; ++i)
            {
                Dictionary<string, long> nextGen = new Dictionary<string, long>();
                foreach (KeyValuePair<string, long> pair in currentGen)
                {
                    string[] toAdd = insertionPairs[pair.Key];
                    if (!nextGen.ContainsKey(toAdd[0]))
                    {
                        nextGen[toAdd[0]] = 0;
                    }
                    nextGen[toAdd[0]] += pair.Value;
                    if (!nextGen.ContainsKey(toAdd[1]))
                    {
                        nextGen[toAdd[1]] = 0;
                    }
                    nextGen[toAdd[1]] += pair.Value;
                    if (!elementAdjustments.ContainsKey(toAdd[1].First()))
                    {
                        elementAdjustments[toAdd[1].First()] = 0;
                    }
                    elementAdjustments[toAdd[1].First()] += pair.Value;
                }
                currentGen = nextGen;
            }

            Dictionary<char, long> finalCounts = elementAdjustments.ToDictionary(pair => pair.Key, pair => pair.Value * -1L);
            foreach (KeyValuePair<string, long> pair in currentGen)
            {
                foreach (char c in pair.Key)
                {
                    finalCounts[c] += pair.Value;
                }
            }

            // RunGeneration(polymer, generationCount, generationData, ref elementCounts);
            return (elementAdjustments.Values.Max() - elementAdjustments.Values.Min()).ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, 10);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, 40);
    }
}