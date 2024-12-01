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
                case Core.Part.One:
                    return "v2";
                case Core.Part.Two:
                    return "v2";
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
                Output = "6",
                RawInput =
@"LR

11A = (11B, XXX)
11B = (XXX, 11Z)
11Z = (11B, XXX)
22A = (22B, XXX)
22B = (22C, 22C)
22C = (22Z, 22Z)
22Z = (22B, 22B)
XXX = (XXX, XXX)"
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

        public record InitialWalk(string Start, string End, long StepCount);

        public class Map
        {
            public string Instructions { get; set; }
            public List<Network> Networks { get; set; }
            public Dictionary<string, Network> MappedNetworks { get; set; }
            public List<InitialWalk> InitialWalks { get; set; }
            public Action<string> PrintFunc { get; set; }

            public Map(List<string> inputs, Action<string> printFunc)
            {
                Instructions = inputs.First();
                Networks = inputs.Skip(2).Select(Network.Parse).ToList();
                MappedNetworks = Networks.ToDictionary(n => n.Id, n => n);
                InitialWalks = new List<InitialWalk>();
                PrintFunc = printFunc;
            }

            public char GetDirection(long stepCount)
            {
                long index = stepCount % (long)Instructions.Length;
                return Instructions[(int)index];
            }

            public void GenerateInitialWalks(Func<string, bool> isStartNode, Func<string, bool> isEndNode)
            {
                IEnumerable<string> startNodes = Networks.Where(n => isStartNode(n.Id)).Select(n => n.Id);
                IEnumerable<string> endNodes = Networks.Where(n => isEndNode(n.Id)).Select(n => n.Id);

                // find each start to each end
                foreach (string startNode in startNodes)
                {
                    // find all potential ends
                    HashSet<string> processed = new HashSet<string>();
                    Queue<string> pending = new Queue<string>();
                    pending.Enqueue(startNode);
                    while (pending.Count > 0)
                    {
                        string cur = pending.Dequeue();
                        if (processed.Contains(cur))
                        {
                            continue;
                        }
                        processed.Add(cur);
                        pending.Enqueue(MappedNetworks[cur].Left);
                        pending.Enqueue(MappedNetworks[cur].Right);
                    }

                    IEnumerable<string> possibleEndNodes = processed.Intersect(endNodes);
                    if (possibleEndNodes.Count() == 0)
                    {
                        continue;
                    }

                    foreach (string endNode in possibleEndNodes)
                    {
                        string finalNode = new string(endNode);
                        long stepCount = Walk(startNode, 0, ref finalNode);
                        InitialWalks.Add(new InitialWalk(startNode, finalNode, stepCount));
                        // PrintFunc($"{startNode} -> {endNode} in {stepCount} steps");
                    }
                }
            }

            private long Walk(string startNode, long stepCountStart, ref string endNode)
            {
                long stepCount = stepCountStart;
                string curNodeId = startNode;
                Network curNetwork = null;
                while (true)
                {
                    if (curNodeId == endNode && stepCount > stepCountStart)
                    {
                        break;
                    }

                    char direction = GetDirection(stepCount);
                    curNetwork = MappedNetworks[curNodeId];
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
                return stepCount;
            }

            public long Get()
            {
                return Util.Number.LeastCommonMultiple(InitialWalks.Select(iw => iw.StepCount));
            }
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool ghostWalk)
        {
            Map map = new Map(inputs, (_) => { });
            // Map map = new Map(inputs, DebugWriteLine);
            if (ghostWalk)
            {
                map.GenerateInitialWalks((id) => id.EndsWith('A'), (id) => id.EndsWith('Z'));
                return map.Get().ToString();
            }
            else
            {
                map.GenerateInitialWalks((id) => id == "AAA", (id) => id == "ZZZ");
                return map.InitialWalks.First().StepCount.ToString();
            }
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);
    }
}