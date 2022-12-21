using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2022
{
    class Day21 : Core.Day
    {
        public Day21() { }

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
                Output = "152",
                RawInput =
@"root: pppw + sjmn
dbpl: 5
cczh: sllz + lgvd
zczc: 2
ptdq: humn - dvpt
dvpt: 3
lfqf: 4
humn: 5
ljgn: 2
sjmn: drzm * dbpl
sllz: 4
pppw: cczh / lfqf
lgvd: ljgn * ptdq
drzm: hmdt - zczc
hmdt: 32"
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
            Raw = ' ',
            Add = '+',
            Sub = '-',
            Mult = '*',
            Div = '/'
        }

        private class Monkey
        {
            public string Id { get; set; }
            public string[] Others { get; set; }
            public long Value { get; set; }
            public EOp Op { get; set; }

            public Monkey()
            {
                Id = string.Empty;
                Others = new string[2];
                Value = 0;
                Op = EOp.Raw;
            }

            public static Monkey Parse(string input)
            {
                string[] split = input.Split(" :".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                Monkey m = new Monkey();
                m.Id = split[0];
                if (long.TryParse(split[1], out long val))
                {
                    m.Op = EOp.Raw;
                    m.Value = val;
                }
                else
                {
                    m.Others[0] = split[1];
                    m.Op = (EOp)split[2][0];
                    m.Others[1] = split[3];
                }
                return m;
            }

            public long Perform(Dictionary<string, long> values)
            {
                long a = values[Others[0]];
                long b = values[Others[1]];
                switch (Op)
                {
                    case EOp.Add:
                        return a + b;
                    case EOp.Sub:
                        return a - b;
                    case EOp.Mult:
                        return a * b;
                    case EOp.Div:
                        return a / b;
                }
                return 0;
            }
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables)
        {
            Queue<Monkey> monkeys = new Queue<Monkey>(inputs.Select(Monkey.Parse));
            Dictionary<string, long> values = new Dictionary<string, long>();
            while (monkeys.Count > 0)
            {
                Monkey monkey = monkeys.Dequeue();
                if (monkey.Op == EOp.Raw)
                {
                    DebugWriteLine($"{monkey.Id} => {monkey.Value}");
                    values[monkey.Id] = monkey.Value;
                    continue;
                }

                if (values.ContainsKey(monkey.Others[0]) && values.ContainsKey(monkey.Others[1]))
                {
                    values[monkey.Id] = monkey.Perform(values);
                    DebugWriteLine($"{monkey.Id} => {monkey.Others[0]} [{values[monkey.Others[0]]}] {(char)monkey.Op} {monkey.Others[1]} [{values[monkey.Others[1]]}] = {values[monkey.Id]}");
                    continue;
                }

                monkeys.Enqueue(monkey);
            }
            return values["root"].ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);
    }
}