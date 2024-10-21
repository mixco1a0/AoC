using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2023
{
    class Day15 : Core.Day
    {
        public Day15() { }

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

        public override bool SkipTestData => true;

        protected override List<Core.TestDatum> GetTestData()
        {
            List<Core.TestDatum> testData = new List<Core.TestDatum>();
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.One,
                Output = "1320",
                RawInput =
@"rn=1,cm-,qp=3,cm=2,qp-,pc=4,ot=9,ab=5,pc-,pc=6,ot=7"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
                Output = "145",
                RawInput =
@"rn=1,cm-,qp=3,cm=2,qp-,pc=4,ot=9,ab=5,pc-,pc=6,ot=7"
            });
            return testData;
        }

        private class Sequence
        {
            public string Raw { get; set; }
            public string Id { get; set; }
            public bool AddOp { get; set; }
            public int Lens { get; set; }

            public static Sequence Parse(string input)
            {
                Sequence sequence = new Sequence();
                sequence.Raw = input;
                string[] split = Util.String.Split(input, "=-");
                sequence.Id = split[0];
                sequence.AddOp = input.Contains('=');
                if (sequence.AddOp)
                {
                    sequence.Lens = int.Parse(split[1]);
                }
                return sequence;
            }

            public long RawHash()
            {
                long hash = 0;
                foreach (char raw in Raw)
                {
                    hash += (int)raw;
                    hash *= 17;
                    hash %= 256;
                }
                return hash;
            }

            public long IdHash()
            {
                long hash = 0;
                foreach (char id in Id)
                {
                    hash += (int)id;
                    hash *= 17;
                    hash %= 256;
                }
                return hash;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Id, Lens);
            }

            public override bool Equals(object obj)
            {
                Sequence other = obj as Sequence;
                if (other == null)
                {
                    return false;
                }

                return Id.Equals(other.Id);
            }
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool hashOnly)
        {
            List<Sequence> sequences = Util.String.Split(inputs.First(), ',').Select(Sequence.Parse).ToList();
            if (hashOnly)
            {
                return sequences.Select(s => s.RawHash()).Sum().ToString();
            }

            Dictionary<long, List<Sequence>> boxes = new Dictionary<long, List<Sequence>>();
            foreach (Sequence sequence in sequences)
            {
                long hash = sequence.IdHash();
                if (!boxes.ContainsKey(hash))
                {
                    boxes[hash] = new List<Sequence>();
                }

                var lenses = boxes[hash];
                if (sequence.AddOp)
                {
                    int index = lenses.IndexOf(sequence);
                    if (index >= 0)
                    {
                        boxes[hash][index] = sequence;
                    }
                    else
                    {
                        boxes[hash].Add(sequence);
                    }
                }
                else
                {
                    boxes[hash].Remove(sequence);
                    if (boxes[hash].Count == 0)
                    {
                        boxes.Remove(hash);
                    }
                }
            }

            long focusingPower = 0;
            foreach (var pair in boxes)
            {
                for (int i = 0; i < pair.Value.Count; ++i)
                {
                    focusingPower += (pair.Key + 1) * (i + 1) * pair.Value[i].Lens;
                }
            }
            return focusingPower.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);
    }
}