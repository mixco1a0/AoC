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
                Variables = new Dictionary<string, string> { { nameof(_Humn), "301" } },
                Output = "301",
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
            return testData;
        }

        private int _Humn { get; }

        internal enum EOp
        {
            Raw = ' ',
            Add = '+',
            Sub = '-',
            Mult = '*',
            Div = '/',
            Equals = '='
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

            public override string ToString()
            {
                string valueString = (Op == EOp.Raw ? Value.ToString() : string.Format("{0} {1} {2}", Others[0], (char)Op, Others[1]));
                return string.Format("{0} => {1}", Id, valueString);
            }
        }

        private string ProcessMonkeys(List<string> inputs)
        {
            Queue<Monkey> monkeys = new Queue<Monkey>(inputs.Select(Monkey.Parse));
            Dictionary<string, long> values = new Dictionary<string, long>();
            while (monkeys.Count > 0)
            {
                Monkey monkey = monkeys.Dequeue();
                if (monkey.Op == EOp.Raw)
                {
                    values[monkey.Id] = monkey.Value;
                    continue;
                }

                if (values.ContainsKey(monkey.Others[0]) && values.ContainsKey(monkey.Others[1]))
                {
                    values[monkey.Id] = monkey.Perform(values);
                    continue;
                }

                monkeys.Enqueue(monkey);
            }
            return values["root"].ToString();
        }

        private void ReverseMonkeys(ref Dictionary<string, long> values, ref List<Monkey> leftOverMonkeys, Monkey curMonkey)
        {
            int nextIdx = 0;
            long match = 0;
            string next = string.Empty;
            if (values.ContainsKey(curMonkey.Others[0]))
            {
                nextIdx = 1;
                match = values[curMonkey.Others[0]];
                next = curMonkey.Others[1];
            }
            else if (values.ContainsKey(curMonkey.Others[1]))
            {
                match = values[curMonkey.Others[1]];
                next = curMonkey.Others[0];
            }
            else
            {
                return;
            }

            switch (curMonkey.Op)
            {
                case EOp.Equals:
                    values[next] = match;
                    break;
                case EOp.Add:
                    // a = ? + b => ? = a - b
                    // a = b + ? => ? = a - b
                    values[next] = values[curMonkey.Id] - match;
                    break;
                case EOp.Sub:
                    // a = ? - b => ? = a + b
                    if (nextIdx == 0)
                    {
                        values[next] = values[curMonkey.Id] + match;
                    }
                    // a = b - ? => ? = b - a
                    else
                    {
                        values[next] = match - values[curMonkey.Id];
                    }
                    break;
                case EOp.Mult:
                    // a = ? * b => ? = a / b
                    // a = b * ? => ? = a / b
                    values[next] = values[curMonkey.Id] / match;
                    break;
                case EOp.Div:
                    // a = ? / b => ? = a * b
                    if (nextIdx == 0)
                    {
                        values[next] = values[curMonkey.Id] * match;
                    }
                    // a = b / ? => ? = b / a
                    else
                    {
                        values[next] = match / values[curMonkey.Id];
                    }
                    break;
            }

            leftOverMonkeys.Remove(curMonkey);
            curMonkey = leftOverMonkeys.Find(m => m.Id == next);
            if (curMonkey != null)
            {
                ReverseMonkeys(ref values, ref leftOverMonkeys, curMonkey);
            }
        }

        private void KindaProcessMonkeys(List<Monkey> allMonkeys)
        {
            Monkey root = allMonkeys.Find(m => m.Id == "root");
            Queue<Monkey> monkeys = new Queue<Monkey>(allMonkeys);
            Dictionary<string, long> values = new Dictionary<string, long>();
            while (monkeys.Count > 0)
            {
                Monkey monkey = monkeys.Dequeue();

                if (monkey.Id == "root")
                {
                    continue;
                }

                if (monkey.Op == EOp.Raw)
                {
                    values[monkey.Id] = monkey.Value;
                    continue;
                }

                if (values.ContainsKey(monkey.Others[0]) && values.ContainsKey(monkey.Others[1]))
                {
                    values[monkey.Id] = monkey.Perform(values);
                    continue;
                }

                monkeys.Enqueue(monkey);
            }

            Log($"{root.Others[0]} [{values[root.Others[0]]}] =?= {root.Others[1]} [{values[root.Others[1]]}]");
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool needsHumanInput)
        {
            if (!needsHumanInput)
            {
                return ProcessMonkeys(inputs);
            }

            List<Monkey> allMonkeys = inputs.Select(Monkey.Parse).Where(m => m.Id != "humn").ToList();

            // debug to verify answer
            bool debugAnswer = false;
            if (debugAnswer)
            {
                GetVariable(nameof(_Humn), 3451534022348, variables, out long humn);
                allMonkeys.Add(new Monkey() { Id = "humn", Op = EOp.Raw, Value = humn });
                KindaProcessMonkeys(allMonkeys);
            }


            Monkey root = allMonkeys.Find(m => m.Id == "root");
            root.Op = EOp.Equals;
            Queue<Monkey> monkeys = new Queue<Monkey>(allMonkeys);
            Dictionary<string, long> values = new Dictionary<string, long>();

            string monkeyCycle = string.Empty;
            while (monkeys.Count > 0)
            {
                Monkey monkey = monkeys.Dequeue();
                if (string.IsNullOrWhiteSpace(monkeyCycle))
                {
                    monkeyCycle = monkey.Id;
                }
                else if (monkey.Id == monkeyCycle)
                {
                    monkeys.Enqueue(monkey);
                    break;
                }

                if (monkey.Op == EOp.Raw)
                {
                    values[monkey.Id] = monkey.Value;
                    monkeyCycle = string.Empty;
                    continue;
                }

                if (values.ContainsKey(monkey.Others[0]) && values.ContainsKey(monkey.Others[1]))
                {
                    values[monkey.Id] = monkey.Perform(values);
                    monkeyCycle = string.Empty;
                    continue;
                }

                monkeys.Enqueue(monkey);
            }

            List<Monkey> leftOverMonkeys = new List<Monkey>(monkeys);
            ReverseMonkeys(ref values, ref leftOverMonkeys, root);

            return values["humn"].ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);
    }
}