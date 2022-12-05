using System.Xml.Linq;
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
                Output = "MCD",
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

        private void ParseShip(ref List<string> inputs, out Dictionary<int, Stack<char>> ship)
        {
            Dictionary<int, int> key = new Dictionary<int, int>();
            List<Dictionary<int, char>> stacks = new List<Dictionary<int, char>>();
            for (int i = 0; i < inputs.Count; ++i)
            {
                string input = inputs[i];
                Dictionary<int, char> crates = input.Select((letter, index) => (letter, index)).Where(pair => char.IsLetter(pair.letter)).ToDictionary(pair => pair.index, pair => pair.letter);
                if (crates.Count == 0)
                {
                    crates = input.Select((number, index) => (number, index)).Where(pair => char.IsNumber(pair.number)).ToDictionary(pair => pair.index, pair => pair.number);
                    key = crates.ToDictionary(pair => pair.Key, pair => int.Parse($"{pair.Value}"));
                    inputs.RemoveRange(0, i + 1);
                    break;
                }
                stacks.Add(crates);
            }

            ship = new Dictionary<int, Stack<char>>();
            foreach (var pair in key)
            {
                ship[pair.Value] = new Stack<char>();
            }

            stacks.Reverse();
            foreach (var stack in stacks)
            {
                foreach (var crate in stack)
                {
                    ship[key[crate.Key]].Push(crate.Value);
                }
            }
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool isCrateMover9001)
        {
            ParseShip(ref inputs, out Dictionary<int, Stack<char>> ship);

            // go through instructions now
            foreach (string input in inputs)
            {
                if (string.IsNullOrWhiteSpace(input))
                {
                    continue;
                }

                MoveInstruction mi = MoveInstruction.Parse(input);
                if (isCrateMover9001)
                {
                    StringBuilder moving = new StringBuilder();
                    for (int j = 0; j < mi.Count; ++j)
                    {
                        moving.Append(ship[mi.From].Pop());
                    }
                    foreach (char m in moving.ToString().Reverse())
                    {
                        ship[mi.To].Push(m);
                    }
                }
                else
                {
                    for (int j = 0; j < mi.Count; ++j)
                    {
                        ship[mi.To].Push(ship[mi.From].Pop());
                    }
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
            => SharedSolution(inputs, variables, false);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);
    }
}