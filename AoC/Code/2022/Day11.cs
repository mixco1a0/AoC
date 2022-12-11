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
                // case Core.Part.One:
                //     return "v1";
                // case Core.Part.Two:
                //     return "v1";
                default:
                    return base.GetSolutionVersion(part);
            }
        }

        public override bool SkipTestData => false;

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
                Output = "",
                RawInput =
@""
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
            public int Id { get; set; }
            public List<int> Items { get; set; }
            public EOp Op { get; set; }
            public bool UseOld { get; set; }
            public int Value { get; set; }
            public int Div { get; set; }
            public int True { get; set; }
            public int False { get; set; }
            public int InspectionCount { get; set; }

            public static Monkey Parse(List<string> input)
            {
                Monkey monkey = new Monkey();
                monkey.Id = int.Parse(input[0].Split(" :".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Last());
                monkey.Items = input[1].Split(" ,".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Where(s => int.TryParse(s, out int i)).Select(int.Parse).ToList();
                monkey.Op = input[2].Contains('*') ? EOp.Mult : EOp.Add;
                monkey.UseOld = input[2].Split(" +*".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Last() == "old";
                if (!monkey.UseOld)
                {
                    monkey.Value = int.Parse(input[2].Split(" +*".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Last());
                }
                monkey.Div = int.Parse(input[3].Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Last());
                monkey.True = int.Parse(input[4].Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Last());
                monkey.False = int.Parse(input[5].Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Last());
                monkey.InspectionCount = 0;
                return monkey;
            }

            public int Worry()
            {
                ++InspectionCount;
                int oldV = Items.First();
                Items.RemoveAt(0);
                int otherV = Value;
                if (UseOld)
                {
                    otherV = oldV;
                }
                int newV = oldV;
                if (Op == EOp.Add)
                {
                    newV += otherV;
                }
                else
                {
                    newV *= otherV;
                }
                newV /= 3;
                return newV;
            }

            public override string ToString()
            {
                return $"[{Id}] has {string.Join(',', Items)}. new = old {(char)Op} {string.Format("{0}", UseOld ? "old" : Value)} | %{Div} => T=>{True} F=>{False}";
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

        public void DoRound(ref Monkey[] monkeys)
        {
            foreach (Monkey monkey in monkeys)
            {
                while (monkey.Items.Count != 0)
                {
                    int worry = monkey.Worry();
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

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables)
        {
            Monkey[] monkeys = GetMonkeys(inputs);
            for (int i = 0; i < 20; ++i)
            {
                DoRound(ref monkeys);
            }
            List<int> ic = monkeys.Select(m => m.InspectionCount).OrderByDescending(mic => mic).Take(2).ToList();
            return (ic[0] * ic[1]).ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);
    }
}