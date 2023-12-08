using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2023
{
    class Day08 : Core.Day
    {
        public Day08() { }

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
                Output = "2",
                RawInput =
@"RL

AAA = (BBB, CCC)
BBB = (DDD, EEE)
CCC = (ZZZ, GGG)
DDD = (DDD, DDD)
EEE = (EEE, EEE)
GGG = (GGG, GGG)
ZZZ = (ZZZ, ZZZ)"
            }); testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.One,
                Output = "6",
                RawInput =
@"LLR

AAA = (BBB, BBB)
BBB = (AAA, ZZZ)
ZZZ = (ZZZ, ZZZ)"
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

        public class Network
        {
            public string Id { get; set; }
            public string Left { get; set; }
            public string Right { get; set; }
            public static Network Parse(string input)
            {
                string[] split = Util.String.Split(input, "=(,)");
                return new Network() { Id = split[0], Left = split[1], Right = split[2] };
            }
            public override string ToString()
            {
                return $"{Id} = ({Left}, {Right})";
            }
        }

        public class Map
        {
            public string Instructions { get; set; }
            public List<Network> Networks { get; set; }
            public Dictionary<string, Network> MappedNetworks { get; set; }

            public Map(List<string> inputs)
            {
                Instructions = inputs.First();
                Networks = inputs.Skip(2).Select(Network.Parse).ToList();
                MappedNetworks = Networks.ToDictionary(n => n.Id, n => n);
            }

            public char GetDirection(int stepCount)
            {
                return Instructions[stepCount % Instructions.Length];
            }
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables)
        {
            Map map = new Map(inputs);
            int stepCount = 0;
            string curNodeId = "AAA";
            Network curNetwork = null;
            while (true)
            {
                if (curNodeId == "ZZZ")
                {
                    break;
                }

                char direction = map.GetDirection(stepCount);
                curNetwork = map.MappedNetworks[curNodeId];
                if (direction == 'L')
                {
                    curNodeId = curNetwork.Left;
                }
                else
                {
                    curNodeId = curNetwork.Right;
                }
                ++stepCount;
            }
            return stepCount.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);
    }
}