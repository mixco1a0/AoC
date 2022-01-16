using System.Collections.Generic;
using System.Linq;

using AoC.Core;

namespace AoC._2016
{
    class Day19 : Day
    {
        public Day19() { }

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
                Output = "3",
                RawInput =
@"5"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.Two,
                Output = "2",
                RawInput =
@"5"
            });
            return testData;
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            int elfCount = int.Parse(inputs.First());
            List<int> elves = Enumerable.Range(1, elfCount).ToList();
            while (elves.Count > 1)
            {
                bool removeFront = elves.Count % 2 != 0;
                elves = elves.Select((e, i) => new { e, i }).Where(p => p.i % 2 == 0).Select(p => p.e).ToList();
                if (elves.Count > 1 && removeFront)
                {
                    elves.RemoveAt(0);
                }
            }
            return elves.First().ToString();
        }

        private class Elf
        {
            public Elf(int id, Elf next, Elf prev)
            {
                Id = id;
                Next = next;
                Prev = prev;
            }
            public Elf Next { get; set; }
            public Elf Prev { get; set; }
            public int Id { get; set; }
            public override string ToString()
            {
                return $"{Prev.Id} <= {Id} => {Next.Id}";
            }
        }

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            int elfCount = int.Parse(inputs.First());
            List<Elf> elves = Enumerable.Range(1, elfCount).Select(e => new Elf(e, null, null)).ToList();
            elves[0].Prev = elves.Last();
            elves.Last().Next = elves[0];
            for (int e = 0; e < elves.Count; ++e)
            {
                if (e + 1 < elves.Count)
                {
                    elves[e].Next = elves[e + 1];
                }

                if (e > 0)
                {
                    elves[e].Prev = elves[e - 1];
                }
            }
            Elf curElf = elves.First();
            Elf targetElf = elves[elves.Count / 2];
            while (curElf.Id != curElf.Next.Id)
            {
                targetElf.Prev.Next = targetElf.Next;
                targetElf.Next.Prev = targetElf.Prev;
                targetElf = (elfCount % 2 == 1 ? targetElf.Next.Next : targetElf.Next);
                curElf = curElf.Next;
                --elfCount;
            }
            return curElf.Id.ToString();
        }
    }
}