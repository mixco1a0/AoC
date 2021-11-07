using System.Diagnostics;
using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2015
{
    class Day24 : Day
    {
        public Day24() { }
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
            testData.Add(new TestDatum
            {
                TestPart = Part.Two,
                Output = "",
                RawInput =
@""
            });
            return testData;
        }

        record QuantumEntanglement(long Value, int Count, List<int> Weights);
        static QuantumEntanglement MaxQE = new QuantumEntanglement(int.MaxValue, int.MaxValue, new List<int>());

        // class FullSet
        // {
        //     public FullSet(List<int> a, List<int> b, List<int> c)
        //     {
        //         A = GetString(a);
        //         B = GetString(b);
        //         C = GetString(c);
        //     }

        //     public string A { get; set; }
        //     public string B { get; set; }
        //     public string C { get; set; }
        // }

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

        /*

        private static string GetString(List<int> list)
        {
            StringBuilder sb = new StringBuilder();
            list.ForEach(i => {sb.Append(i); sb.Append(".");});
            return sb.ToString();
        }

        private bool WasHandled(FullSet set, List<int> target)
        {
            string t = GetString(target);
            return t == set.A || t == set.B || t == set.C;
        }

        private bool WasHandled(FullSet set, List<int> a, List<int> b, List<int> c)
        {
            bool wasA = WasHandled(set, a);
            bool wasB = WasHandled(set, b);
            bool wasC = WasHandled(set, c);
            int handledCount = (wasA ? 1 : 0) + (wasB ? 1 : 0) + (wasC ? 1 : 0);
            return handledCount >= 2;
        }

        private QuantumEntanglement FillGroup(int maxSum, List<int> pending, List<int> a, List<int> b, List<int> c, ref List<FullSet> handled)
        {
            int sumA = 0, sumB = 0, sumC = 0;
            a.ForEach(i => sumA += i);
            b.ForEach(i => sumB += i);
            c.ForEach(i => sumC += i);

            if (pending.Count == 0 && sumA == sumB && sumB == sumC && sumC == maxSum)
            {
                QuantumEntanglement qeA = GenerateQuantumEntanglement(a);
                QuantumEntanglement qeB = GenerateQuantumEntanglement(b);
                QuantumEntanglement qeC = GenerateQuantumEntanglement(c);
                handled.Add(new FullSet(a, b, c));
                return OptimizedEQ(OptimizedEQ(qeA, qeB), qeC);
            }

            int fillCount = (sumA == maxSum ? 1 : 0) + (sumB == maxSum ? 1 : 0) + (sumC == maxSum ? 1 : 0);
            if (fillCount == 2)
            {
                bool wasHandled = false;
                foreach (FullSet full in handled)
                {
                    wasHandled = WasHandled(full, a, b, c);
                    if (wasHandled)
                    {
                        return MaxQE;
                    }
                }
            }

            QuantumEntanglement curQE = MaxQE;
            List<int> newList = null;
            foreach (int p in pending)
            {
                if (sumA + p <= maxSum)
                {
                    newList = a.Where(_ => true).ToList();
                    newList.Add(p);
                    newList.Sort();
                    curQE = OptimizedEQ(curQE, FillGroup(maxSum, pending.Where(i => i != p).ToList(), newList, b, c, ref handled));
                }
            }
            if (sumA != maxSum)
            {
                return curQE;
            }

            foreach (int p in pending)
            {
                if (sumB + p <= maxSum)
                {
                    newList = b.Where(_ => true).ToList();
                    newList.Add(p);
                    newList.Sort();
                    curQE = OptimizedEQ(curQE, FillGroup(maxSum, pending.Where(i => i != p).ToList(), a, newList, c, ref handled));
                }
            }
            if (sumB != maxSum)
            {
                return curQE;
            }

            newList = c.Where(_ => true).ToList();
            foreach (int p in pending)
            {
                newList.Add(p);
            }
            newList.Sort();
            curQE = OptimizedEQ(curQE, FillGroup(maxSum, new List<int>(), a, b, newList, ref handled));

            return curQE;
        }
        */

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
            //List<FullSet> handled = new List<FullSet>();
            numbers.Sort();
            numbers.Reverse();
            QuantumEntanglement best = MaxQE;
            Find(maxSum, numbers, new List<int>(), ref best);
            return best.Value.ToString();
        }

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            return "";
        }
    }
}