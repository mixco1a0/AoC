using System.Collections.Generic;
using System.Linq;

namespace AoC._2015
{
    class Day24 : Core.Day
    {
        public Day24() { }

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
                Output = "99",
                RawInput =
@"1
2
3
4
5
7
8
9
10
11"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
                Output = "44",
                RawInput =
@"1
2
3
4
5
7
8
9
10
11"
            });
            return testData;
        }

        record QuantumEntanglement(long Value, int Count, List<int> Weights) { }
        static QuantumEntanglement MaxQE = new QuantumEntanglement(int.MaxValue, int.MaxValue, new List<int>());

        private QuantumEntanglement OptimizedEQ(QuantumEntanglement a, QuantumEntanglement b)
        {
            if (a.Count < b.Count)
            {
                return a;
            }

            if (b.Count < a.Count)
            {
                return b;
            }

            if (a.Value < b.Value)
            {
                return a;
            }

            return b;
        }

        private QuantumEntanglement GenerateQuantumEntanglement(List<int> weights)
        {
            long mult = 1;
            weights.ForEach(i => mult *= (long)i);
            return new QuantumEntanglement(mult, weights.Count, weights);
        }

        private void Find(int maxSum, List<int> pending, List<int> a, ref QuantumEntanglement best)
        {
            // check for possible match
            int sumA = 0;
            a.ForEach(i => sumA += i);
            if (sumA == maxSum)
            {
                long val = best.Value;
                best = OptimizedEQ(best, GenerateQuantumEntanglement(a));
                return;
            }

            // ignore anything worse than current
            if (best.Weights.Count > 0 && a.Count >= best.Weights.Count)
            {
                return;
            }

            // check next weight
            foreach (int p in pending)
            {
                if (sumA + p <= maxSum)
                {
                    List<int> newList = new List<int>(a);
                    newList.Add(p);
                    Find(maxSum, pending.Where(i => i != p).ToList(), newList, ref best);
                }

                if (best != MaxQE)
                {
                    return;
                }
            }
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            int maxSum = 0;
            List<int> numbers = inputs.Select(int.Parse).ToList();
            numbers.ForEach(n => maxSum += n);
            maxSum /= 3;
            numbers.Sort();
            numbers.Reverse();
            QuantumEntanglement best = MaxQE;
            Find(maxSum, numbers, new List<int>(), ref best);
            return best.Value.ToString();
        }

        private void Find2(int maxSum, List<int> pending, List<int> a, ref QuantumEntanglement best)
        {
            // check for possible match
            int sumA = 0;
            a.ForEach(i => sumA += i);
            if (sumA == maxSum)
            {
                long val = best.Value;
                best = OptimizedEQ(best, GenerateQuantumEntanglement(a));
                return;
            }

            // ignore anything worse than current
            if (best.Weights.Count > 0 && a.Count >= best.Weights.Count)
            {
                return;
            }

            // check next weight
            foreach (int p in pending)
            {
                if (sumA + p <= maxSum)
                {
                    List<int> newList = new List<int>(a);
                    newList.Add(p);
                    Find2(maxSum, pending.Where(i => i != p).ToList(), newList, ref best);
                }
            }
        }

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            int maxSum = 0;
            List<int> numbers = inputs.Select(int.Parse).ToList();
            numbers.ForEach(n => maxSum += n);
            maxSum /= 4;
            numbers.Sort();
            numbers.Reverse();
            QuantumEntanglement best = MaxQE;
            Find2(maxSum, numbers, new List<int>(), ref best);
            return best.Value.ToString();
        }
    }
}