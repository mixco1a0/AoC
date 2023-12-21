using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2023
{
    class Day20 : Core.Day
    {
        public Day20() { }

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
                Output = "32000000",
                RawInput =
@"%a -> b
broadcaster -> a, b, c
%b -> c
%c -> inv
&inv -> a"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.One,
                Output = "11687500",
                RawInput =
@"broadcaster -> a
%a -> inv, con
&inv -> b
%b -> con
&con -> output"
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

        private static string Rx = "rx";

        enum EType
        {
            None = '_',
            Broadcast = '@',
            FlipFlop = '%',
            Conjunction = '&'
        }

        private record Pulse(string Source, string Target, bool High)
        {
            public override string ToString()
            {
                string high = High ? "-hi->" : "-lo->";
                return $"{Source} {high} {Target}";
            }
        }

        private class Module
        {
            public string Id { get; set; }
            public EType Type { get; set; }
            public List<string> Targets { get; set; }
            public Dictionary<string, bool> Received { get; set; }
            public bool On { get; set; }

            public Module(string id)
            {
                Id = id;
                Type = EType.None;
                Targets = new List<string>();
                Received = new Dictionary<string, bool>();
                On = false;
            }

            public Module(string id, EType type, IEnumerable<string> targets)
            {
                Id = id;
                Type = type;
                Targets = new List<string>(targets);
                Received = new Dictionary<string, bool>();
                On = false;
            }

            public static Module Parse(string input)
            {
                string[] split = Util.String.Split(input, " ->,");

                string id = string.Empty;
                EType type = EType.None;
                if (char.IsLetter(split[0][0]))
                {
                    id = split[0];
                    type = EType.Broadcast;
                }
                else if (split[0][0] == (char)EType.FlipFlop)
                {
                    id = split[0][1..];
                    type = EType.FlipFlop;
                }
                else if (split[0][0] == (char)EType.Conjunction)
                {
                    id = split[0][1..];
                    type = EType.Conjunction;
                }
                return new Module(id, type, split.Skip(1));
            }

            public void Receive(Dictionary<string, Module> moduleMap, Pulse pulse, ref Queue<Pulse> pulses)
            {
                Received[pulse.Source] = pulse.High;
                if (Type == EType.Broadcast)
                {
                    foreach (string target in Targets)
                    {
                        pulses.Enqueue(new Pulse(Id, target, pulse.High));
                    }
                }
                else if (Type == EType.FlipFlop)
                {
                    if (!pulse.High)
                    {
                        On = !On;
                        foreach (string target in Targets)
                        {
                            pulses.Enqueue(new Pulse(Id, target, On));
                        }
                    }
                }
                else if (Type == EType.Conjunction)
                {
                    bool high = Received.Count(pair => !pair.Value) != 0;
                    foreach (string target in Targets)
                    {
                        pulses.Enqueue(new Pulse(Id, target, high));
                    }
                }
            }

            public override string ToString()
            {
                return $"{(char)Type}{Id} -> {string.Join(',', Targets)}";
            }
        }

        private void ParseInput(List<string> inputs, out List<Module> modules)
        {
            modules = inputs.Select(Module.Parse).ToList();
            IEnumerable<string> moduleIds = modules.Select(m => m.Id);
            List<string> noneModules = modules.SelectMany(m => m.Targets).Where(t => !moduleIds.Contains(t)).ToList();
            foreach (string noneModule in noneModules)
            {
                modules.Add(new Module(noneModule));
            }
            foreach (Module module in modules.Where(m => m.Type == EType.Conjunction))
            {
                foreach (string sender in modules.Where(m => m.Targets.Contains(module.Id)).Select(m => m.Id))
                {
                    module.Received[sender] = false;
                }
            }
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, long maxIterations)
        {
            ParseInput(inputs, out List<Module> modules);
            Dictionary<string, Module> moduleMap = modules.ToDictionary(m => m.Id, m => m);
            Dictionary<string, bool> moduleStates = new Dictionary<string, bool>();
            Queue<Pulse> pulses = new Queue<Pulse>();
            long lowCount = 0, highCount = 0;
            string mainId = modules.Where(m => m.Targets.Contains(Rx)).FirstOrDefault()?.Id;
            Dictionary<string, long> cycleModules = modules.Where(m => m.Targets.Contains(mainId)).Select(m => m.Id).ToDictionary(id => id, id => (long)0);
            for (long i = 0; i < maxIterations; ++i)
            {
                pulses.Enqueue(new Pulse("button", "broadcaster", false));
                while (pulses.Count > 0)
                {
                    Pulse pulse = pulses.Dequeue();
                    if (pulse.High)
                    {
                        ++highCount;
                    }
                    else
                    {
                        ++lowCount;
                    }

                    if (maxIterations == long.MaxValue)
                    {
                        if (pulse.High && cycleModules.ContainsKey(pulse.Source) && cycleModules[pulse.Source] == 0)
                        {
                            cycleModules[pulse.Source] = i + 1;
                        }
                    }

                    moduleMap[pulse.Target].Receive(moduleMap, pulse, ref pulses);
                }

                if (maxIterations == long.MaxValue)
                {
                    long cycles = 1;
                    foreach (var pair in cycleModules)
                    {
                        cycles *= pair.Value;
                    }
                    if (cycles != 0)
                    {
                        return cycles.ToString();
                    }
                }
            }

            return (highCount * lowCount).ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, 1000);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, long.MaxValue);
    }
}