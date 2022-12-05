using System.Text;
using System.Runtime.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2022
{
    class Day05 : Core.Day
    {
        public Day05() { }

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
                Output = "CMZ",
                RawInput =
@"    [D]    
[N] [C]    
[Z] [M] [P]
 1   2   3 

move 1 from 2 to 1
move 3 from 1 to 3
move 2 from 2 to 1
move 1 from 1 to 2"
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

        internal record MoveInstruction(int Count, int From, int To)
        {
            static public MoveInstruction Parse(string input)
            {
                string[] split = input.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                int[] ints = split.Where(s => int.TryParse(s, out int i)).Select(int.Parse).ToArray();
                return new MoveInstruction(ints[0], ints[1], ints[2]);
            }
        };

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables)
        {

            List<Dictionary<int, char>> crates = new List<Dictionary<int, char>>();
            int inputsIdx = 0;
            bool shipParseComplete = false;
            for (; inputsIdx < inputs.Count && !shipParseComplete; ++inputsIdx)
            {
                string input = inputs[inputsIdx];
                Dictionary<int, char> crateRow = input.Select((letter, index) => (letter, index)).Where(pair => char.IsLetter(pair.letter)).ToDictionary(pair => pair.index, pair => pair.letter);
                if (crateRow.Count == 0)
                {
                    crateRow = input.Select((number, index) => (number, index)).Where(pair => char.IsNumber(pair.number)).ToDictionary(pair => pair.index, pair => pair.number);
                    shipParseComplete = true;
                }
                crates.Add(crateRow);
            }

            Dictionary<int, Stack<char>> ship = new Dictionary<int, Stack<char>>();
            crates.Reverse();
            Dictionary<int, int> key = crates[0].ToDictionary(pair => pair.Key, pair => int.Parse($"{pair.Value}"));
            crates.RemoveAt(0);

            foreach (var pair in key)
            {
                ship[pair.Value] = new Stack<char>();
            }

            foreach (var crate in crates)
            {
                foreach (var pair in crate)
                {
                    ship[key[pair.Key]].Push(pair.Value);
                }
            }
            
            // go through instructions now
            for (int i = inputsIdx; i < inputs.Count; ++i)
            {
                if (string.IsNullOrWhiteSpace(inputs[i]))
                {
                    continue;
                }

                MoveInstruction mi = MoveInstruction.Parse(inputs[i]);
                for (int j = 0; j < mi.Count; ++j)
                {
                    ship[mi.To].Push(ship[mi.From].Pop());
                }
            }

            StringBuilder sb = new StringBuilder();
            foreach (var pair in ship)
            {
                sb.Append(pair.Value.Peek());
            }
            
            return sb.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);
    }
}