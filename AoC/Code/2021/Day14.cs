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

        private record Rule(int PairId, int[] NextPairIds, int SoloId) { }

        private void GenerateIds(List<string> inputs, out Rule[] rules, out long[] pairs, out long[] solos)
        {
            string polymer = inputs.First();
            Dictionary<char, int> soloIds = new Dictionary<char, int>();
            Dictionary<string, Core.Pair<char, int>> pairIds = new Dictionary<string, Core.Pair<char, int>>();
            int curSoloId = 0, curPairId = 0;
            foreach (string input in inputs.Skip(2))
            {
                string[] split = input.Split(" ->".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                char soloChar = split[1][0];
                if (!soloIds.ContainsKey(soloChar))
                {
                    soloIds[soloChar] = curSoloId++;
                }

                string pairString = split[0];
                pairIds[pairString] = new Core.Pair<char, int>(soloChar, curPairId++);
            }

            solos = new long[soloIds.Count];
            foreach (char c in polymer)
            {
                int soloId = soloIds[c];
                solos[soloId]++;
            }

            rules = new Rule[pairIds.Keys.Count];
            foreach (var pair in pairIds)
            {
                int soloId = soloIds[pair.Value.First];
                int pairId = pair.Value.Last;

                int nextPairId1 = pairIds[$"{pair.Key.First()}{pair.Value.First}"].Last;
                int nextPairId2 = pairIds[$"{pair.Value.First}{pair.Key.Last()}"].Last;
                rules[pair.Value.Last] = new Rule(pairId, new int[2] { nextPairId1, nextPairId2 }, soloId);
            }

            pairs = new long[rules.Length];
            for (int i = 0; i < polymer.Length - 1; ++i)
            {
                string pairString = polymer.Substring(i, 2);
                pairs[pairIds[pairString].Last]++;
            }
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, long maxGen)
        {
            Rule[] rules;
            long[] pairs;
            long[] solos;
            GenerateIds(inputs, out rules, out pairs, out solos);
            for (long curGen = 0; curGen < maxGen; ++curGen)
            {
                long[] pairCopies = new long[pairs.Length];
                for (long pairId = 0; pairId < pairs.Length; ++pairId)
                {
                    if (pairs[pairId] == 0)
                    {
                        continue;
                    }

                    pairCopies[rules[pairId].NextPairIds[0]] += pairs[pairId];
                    pairCopies[rules[pairId].NextPairIds[1]] += pairs[pairId];
                    solos[rules[pairId].SoloId] += pairs[pairId];
                }
                pairs = pairCopies;
            }
            return (solos.Max() - solos.Min()).ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, 10);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, 40);
    }
}