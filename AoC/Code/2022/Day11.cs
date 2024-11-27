using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2022
{
    class Day11 : Core.Day
    {
        public Day11() { }

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
                Output = "10605",
                RawInput =
@"Monkey 0:
  Starting items: 79, 98
  Operation: new = old * 19
  Test: divisible by 23
    If true: throw to monkey 2
    If false: throw to monkey 3

Monkey 1:
  Starting items: 54, 65, 75, 74
  Operation: new = old + 6
  Test: divisible by 19
    If true: throw to monkey 2
    If false: throw to monkey 0

Monkey 2:
  Starting items: 79, 60, 97
  Operation: new = old * old
  Test: divisible by 13
    If true: throw to monkey 1
    If false: throw to monkey 3

Monkey 3:
  Starting items: 74
  Operation: new = old + 3
  Test: divisible by 17
    If true: throw to monkey 0
    If false: throw to monkey 1"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
                Output = "2713310158",
                RawInput =
@"Monkey 0:
  Starting items: 79, 98
  Operation: new = old * 19
  Test: divisible by 23
    If true: throw to monkey 2
    If false: throw to monkey 3

Monkey 1:
  Starting items: 54, 65, 75, 74
  Operation: new = old + 6
  Test: divisible by 19
    If true: throw to monkey 2
    If false: throw to monkey 0

Monkey 2:
  Starting items: 79, 60, 97
  Operation: new = old * old
  Test: divisible by 13
    If true: throw to monkey 1
    If false: throw to monkey 3

Monkey 3:
  Starting items: 74
  Operation: new = old + 3
  Test: divisible by 17
    If true: throw to monkey 0
    If false: throw to monkey 1"
            });
            return testData;
        }

        internal enum EOp
        {
            Add = '+',
            Mult = '*'
        }

        public class Monkey
        {
            public static long LCDiv { get; set; }
            public int Id { get; set; }
            public List<long> Items { get; set; }
            public EOp Op { get; set; }
            public bool UseOld { get; set; }
            public long Value { get; set; }
            public long Div { get; set; }
            public int True { get; set; }
            public int False { get; set; }
            public long InspectionCount { get; set; }

            public static Monkey Parse(List<string> input)
            {
                Monkey monkey = new Monkey();
                monkey.Id = int.Parse(input[0].Split(" :".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Last());
                monkey.Items = input[1].Split(" ,".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Where(s => long.TryParse(s, out long l)).Select(long.Parse).ToList();
                monkey.Op = input[2].Contains('*') ? EOp.Mult : EOp.Add;
                monkey.UseOld = input[2].Split(" +*".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Last() == "old";
                if (!monkey.UseOld)
                {
                    monkey.Value = long.Parse(input[2].Split(" +*".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Last());
                }
                monkey.Div = long.Parse(input[3].Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Last());
                monkey.True = int.Parse(input[4].Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Last());
                monkey.False = int.Parse(input[5].Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Last());
                monkey.InspectionCount = 0;
                LCDiv *= monkey.Div;
                return monkey;
            }

            public long Worry(bool relief)
            {
                ++InspectionCount;
                long oldV = Items.First();
                Items.RemoveAt(0);
                long otherV = Value;
                if (UseOld)
                {
                    otherV = oldV;
                }
                long newV = oldV;
                if (Op == EOp.Add)
                {
                    newV += otherV;
                }
                else
                {
                    newV *= otherV;
                }
                if (relief)
                {
                    newV /= 3;
                }
                else
                {
                    newV %= LCDiv;
                }
                return newV;
            }

            public override string ToString()
            {
                return $"[{Id}@{InspectionCount}] has {string.Join(',', Items)}. new = old {(char)Op} {string.Format("{0}", UseOld ? "old" : Value)} | %{Div} => T=>{True} F=>{False}";
            }
        }

        public Monkey[] GetMonkeys(List<string> inputs)
        {
            List<Monkey> monkeys = new List<Monkey>();
            List<string> curMonkey = new List<string>();
            foreach (string input in inputs)
            {
                if (string.IsNullOrWhiteSpace(input))
                {
                    monkeys.Add(Monkey.Parse(curMonkey));
                    curMonkey.Clear();
                }
                else
                {
                    curMonkey.Add(input);
                }
            }
            monkeys.Add(Monkey.Parse(curMonkey));
            return monkeys.ToArray();
        }

        public void DoRound(ref Monkey[] monkeys, bool relief)
        {
            foreach (Monkey monkey in monkeys)
            {
                while (monkey.Items.Count != 0)
                {
                    long worry = monkey.Worry(relief);
                    if (worry % monkey.Div == 0)
                    {
                        monkeys[monkey.True].Items.Add(worry);
                    }
                    else
                    {
                        monkeys[monkey.False].Items.Add(worry);
                    }
                }
            }
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool relief, int roundCount)
        {
            Monkey.LCDiv = 1;
            Monkey[] monkeys = GetMonkeys(inputs);
            for (int i = 0; i < roundCount; ++i)
            {
                DoRound(ref monkeys, relief);
            }
            List<long> ic = monkeys.Select(m => m.InspectionCount).OrderByDescending(mic => mic).Take(2).ToList();
            return (ic[0] * ic[1]).ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true, 20);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false, 10000);
    }
}