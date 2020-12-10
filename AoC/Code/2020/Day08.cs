using System.Collections.Generic;
using System.Linq;

namespace AoC._2020
{
    class Day08 : Day
    {
        public Day08() { }

        public override string GetSolutionVersion(TestPart testPart)
        {
            switch (testPart)
            {
                case TestPart.One:
                    return "1";
                case TestPart.Two:
                    return "1";
                default:
                    return base.GetSolutionVersion(testPart);
            }
        }

        protected override List<TestDatum> GetTestData()
        {
            List<TestDatum> testData = new List<TestDatum>();
            testData.Add(new TestDatum
            {
                TestPart = TestPart.One,
                Output = "5",
                RawInput =
@"nop +0
acc +1
jmp +4
acc +3
jmp -3
acc -99
acc +1
jmp -4
acc +6"
            });
            testData.Add(new TestDatum
            {
                TestPart = TestPart.Two,
                Output = "8",
                RawInput =
@"nop +0
acc +1
jmp +4
acc +3
jmp -3
acc -99
acc +1
jmp -4
acc +6"
            });
            return testData;
        }

        struct Op
        {
            public string Operation { get; set; }
            public int Arg { get; set; }
        }

        private string GetAcc(List<Op> operations, out int idx, out List<KeyValuePair<int, Op>> opsRun)
        {
            idx = 0;
            int acc = 0;
            opsRun = new List<KeyValuePair<int, Op>>();
            HashSet<int> alreadyVisited = new HashSet<int>();
            while (true)
            {
                if (idx >= operations.Count)
                    break;

                Op cur = operations[idx];
                //Debug($"[{idx}] {cur.Operation} {cur.Arg}");

                if (alreadyVisited.Contains(idx))
                    break;


                opsRun.Add(new KeyValuePair<int, Op>(idx, cur));
                alreadyVisited.Add(idx);
                switch (cur.Operation)
                {
                    case "acc":
                        acc += cur.Arg;
                        idx++;
                        break;
                    case "jmp":
                        idx += cur.Arg;
                        break;
                    case "nop":
                        idx++;
                        break;
                    default:
                        break;
                }
            }
            return acc.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            int idx;
            List<KeyValuePair<int, Op>> opsRun;
            List<Op> operations = inputs.Select(input => new Op { Operation = input[0..3], Arg = int.Parse(input[4..]) }).ToList();
            return GetAcc(operations, out idx, out opsRun);
        }

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            int idx;
            List<KeyValuePair<int, Op>> opsRun;
            List<Op> operations = inputs.Select(input => new Op { Operation = input[0..3], Arg = int.Parse(input[4..]) }).ToList();

            string acc = GetAcc(operations, out idx, out opsRun);
            if (idx < operations.Count())
                return RunChange(operations, opsRun);

            // try changing things instead
            return acc.ToString();
        }

        private string RunChange(List<Op> operations, List<KeyValuePair<int, Op>> opsRun)
        {
            Op prev;
            foreach (var pair in opsRun)
            {
                // cache
                prev = operations[pair.Key];

                // swap operation
                if (operations[pair.Key].Operation == "nop")
                {
                    operations[pair.Key] = new Op { Operation = "jmp", Arg = prev.Arg };
                }
                else if (operations[pair.Key].Operation == "jmp")
                {
                    operations[pair.Key] = new Op { Operation = "nop", Arg = prev.Arg };
                }
                else
                {
                    continue;
                }

                if (ReRun(operations))
                {
                    int idx;
                    List<KeyValuePair<int, Op>> dummy;
                    return GetAcc(operations, out idx, out dummy);
                }
                else
                {
                    operations[pair.Key] = prev;
                }
            }

            return "NaN";
        }

        private bool ReRun(List<Op> operations)
        {
            //Debug($"-NEW-RUN------------");
            int idx;
            List<KeyValuePair<int, Op>> opsRun;
            GetAcc(operations, out idx, out opsRun);
            return idx >= operations.Count();
        }
    }
}