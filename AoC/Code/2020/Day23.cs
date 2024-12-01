using System.Collections.Generic;
using System.Linq;

namespace AoC._2020
{
    class Day23 : Core.Day
    {
        public Day23() { }

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
                Output = "67384529",
                RawInput =
@"389125467"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
                Output = "149245887792",
                RawInput =
@"389125467"
            });
            return testData;
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            List<int> cups = inputs[0].ToCharArray().Select(c => int.Parse(c.ToString())).ToList();

            int curCupIdx = 0;
            int offset = 0;
            for (int i = 0; i < 100; ++i)
            {
                // DebugWriteLine($"-- move {i + 1} --");
                // DebugWriteLine($"cups: {string.Join(",", cups)}");
                List<int> removedCups = new List<int>(cups.GetRange(curCupIdx + 1, 3));
                // DebugWriteLine($"cups: {string.Join(",", removedCups)}");
                cups.RemoveRange(curCupIdx + 1, 3);

                int destIdx = -1;
                for (int c = 1; c < 10; ++c)
                {
                    int cupCount = cups.Where(cup => cup == cups[curCupIdx] - c).Count();
                    if (cupCount == 1)
                    {
                        // DebugWriteLine($"destination: {cups[curCupIdx] - c}");
                        destIdx = cups.IndexOf(cups[curCupIdx] - c);
                        break;
                    }
                }
                if (destIdx < 0)
                {
                    // DebugWriteLine($"destination: {cups.Max()}");
                    destIdx = cups.IndexOf(cups.Max());
                }
                cups.InsertRange(destIdx + 1, removedCups);
                cups.Add(cups.First());
                cups.RemoveAt(curCupIdx);
                ++offset;
            }

            while (cups[0] != 1)
            {
                cups.Add(cups.First());
                cups.RemoveAt(0);
            }
            cups.RemoveAt(0);

            return string.Join("", cups);
        }

        class Cup
        {
            public long Value { get; set; }
            public Cup Next { get; set; }

            public override string ToString()
            {
                return $"Value={Value} [Next={Next.Value}]";
            }
        }

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            List<long> cups = inputs[0].ToCharArray().Select(c => long.Parse(c.ToString())).ToList();

            long max = cups.Max();
            for (long i = max + 1; i <= 1000000; ++i)
            {
                cups.Add(i);
            }

            List<Cup> cupList = cups.Select(c => new Cup { Value = c }).ToList();
            Dictionary<long, Cup> valueToCup = new Dictionary<long, Cup>();
            for (int i = 0; i < cupList.Count; ++i)
            {
                cupList[i].Next = cupList[(i + 1) % 1000000];
                valueToCup[cupList[i].Value] = cupList[i];
            }


            Cup curCup = cupList.First();
            for (int i = 0; i < 10000000; ++i)
            {
                Cup removedCup = curCup.Next;
                curCup.Next = curCup.Next.Next.Next.Next;
                Cup newInsert = null;

                long nextInsert = curCup.Value - 1;
                for (long j = nextInsert; ; --j)
                {
                    if (j <= 0)
                    {
                        nextInsert = 1000000;
                        if (removedCup.Value == nextInsert || removedCup.Next.Value == nextInsert || removedCup.Next.Next.Value == nextInsert)
                        {
                            nextInsert = 999999;
                            if (removedCup.Value == nextInsert || removedCup.Next.Value == nextInsert || removedCup.Next.Next.Value == nextInsert)
                            {
                                nextInsert = 999998;
                                if (removedCup.Value == nextInsert || removedCup.Next.Value == nextInsert || removedCup.Next.Next.Value == nextInsert)
                                {
                                    nextInsert = 999997;
                                }
                            }
                        }
                        newInsert = valueToCup[nextInsert];
                        break;
                    }
                    if (removedCup.Value == j || removedCup.Next.Value == j || removedCup.Next.Next.Value == j)
                    {
                        continue;
                    }
                    else
                    {
                        newInsert = valueToCup[j];
                        break;
                    }
                }

                Cup oldNext = newInsert.Next;
                newInsert.Next = removedCup;
                removedCup.Next.Next.Next = oldNext;
                
                curCup = curCup.Next;
            }

            Cup one = valueToCup[1];
            return (one.Next.Value * one.Next.Next.Value).ToString();
        }
    }
}