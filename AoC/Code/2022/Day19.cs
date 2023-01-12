using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2022
{
    class Day19 : Core.Day
    {
        public Day19() { }

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

        public override bool SkipTestData => true;

        protected override List<Core.TestDatum> GetTestData()
        {
            List<Core.TestDatum> testData = new List<Core.TestDatum>();
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.One,
                Output = "33",
                RawInput =
@"Blueprint 1: Each ore robot costs 4 ore. Each clay robot costs 2 ore. Each obsidian robot costs 3 ore and 14 clay. Each geode robot costs 2 ore and 7 obsidian.
Blueprint 2: Each ore robot costs 2 ore. Each clay robot costs 3 ore. Each obsidian robot costs 3 ore and 8 clay. Each geode robot costs 3 ore and 12 obsidian."
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
                Output = "3472",
                RawInput =
@"Blueprint 1: Each ore robot costs 4 ore. Each clay robot costs 2 ore. Each obsidian robot costs 3 ore and 14 clay. Each geode robot costs 2 ore and 7 obsidian.
Blueprint 2: Each ore robot costs 2 ore. Each clay robot costs 3 ore. Each obsidian robot costs 3 ore and 8 clay. Each geode robot costs 3 ore and 12 obsidian."
            });
            return testData;
        }

        private int MaxMinutes { get; set; }
        private int[] BotMaxBuildTime { get; set; }

        private enum EMineral : int
        {
            Ore,
            Clay,
            Obsidian,
            Geode,
            Count
        }

        private record BasicBot(int OreCost)
        {
            public bool CanBuild(int[] r)
            {
                return r[(int)EMineral.Ore] >= OreCost;
            }

            public void Build(ref int[] r)
            {
                r[(int)EMineral.Ore] -= OreCost;
            }
        }
        private record ObsidianBot(int OreCost, int ClayCost)
        {
            public bool CanBuild(int[] r)
            {
                return r[(int)EMineral.Ore] >= OreCost && r[(int)EMineral.Clay] >= ClayCost;
            }

            public void Build(ref int[] r)
            {
                r[(int)EMineral.Ore] -= OreCost;
                r[(int)EMineral.Clay] -= ClayCost;
            }
        }
        private record GeodeBot(int OreCost, int ObsidianCost)
        {
            public bool CanBuild(int[] r)
            {
                return r[(int)EMineral.Ore] >= OreCost && r[(int)EMineral.Obsidian] >= ObsidianCost;
            }

            public void Build(ref int[] r)
            {
                r[(int)EMineral.Ore] -= OreCost;
                r[(int)EMineral.Obsidian] -= ObsidianCost;
            }
        }

        private record Blueprint(int Id, BasicBot OreBot, BasicBot ClayBot, ObsidianBot ObsidianBot, GeodeBot GeodeBot, int MaxOre, int MaxClay, int MaxObsidian)
        {
            public static Blueprint Parse(string input)
            {
                int[] split = input.Split(": ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Where(s => int.TryParse(s, out int i)).Select(int.Parse).ToArray();
                BasicBot ore = new BasicBot(split[1]);
                BasicBot clay = new BasicBot(split[2]);
                ObsidianBot obsidian = new ObsidianBot(split[3], split[4]);
                GeodeBot geode = new GeodeBot(split[5], split[6]);
                return new Blueprint(split[0], ore, clay, obsidian, geode, Math.Max(ore.OreCost, Math.Max(clay.OreCost, Math.Max(obsidian.OreCost, geode.OreCost))), obsidian.ClayCost, geode.ObsidianCost);
            }
        }

        private class Operation
        {
            public Blueprint CurBlueprint { get; set; }
            public int Time { get; set; }
            protected int[] Resources;
            protected int[] Bots;

            public Operation(Blueprint blueprint, int time)
            {
                CurBlueprint = blueprint;
                Time = time;
                Resources = new int[(int)EMineral.Count] { 0, 0, 0, 0 };
                Bots = new int[(int)EMineral.Count] { 1, 0, 0, 0 };
            }

            private Operation(Operation prev)
            {
                CurBlueprint = prev.CurBlueprint;
                Time = prev.Time;
                Resources = new int[(int)EMineral.Count];
                Bots = new int[(int)EMineral.Count];
                for (int i = (int)EMineral.Ore; i < (int)EMineral.Count; ++i)
                {
                    Resources[i] = prev.Resources[i];
                    Bots[i] = prev.Bots[i];
                }
            }

            public Operation FastForward(int time, EMineral newBot)
            {
                Operation newOp = new Operation(this);
                newOp.Mine(time);
                newOp.Build(newBot);
                newOp.Time -= time;
                return newOp;
            }

            public int BotCount(EMineral m)
            {
                return Bots[(int)m];
            }

            public int ResourceCount(EMineral m)
            {
                return Resources[(int)m];
            }

            private void Mine(int time)
            {
                for (int i = (int)EMineral.Ore; i < (int)EMineral.Count; ++i)
                {
                    Resources[i] += Bots[i] * time;
                }
            }

            private void Build(EMineral material)
            {
                switch (material)
                {
                    case EMineral.Ore:
                        CurBlueprint.OreBot.Build(ref Resources);
                        ++Bots[(int)EMineral.Ore];
                        break;
                    case EMineral.Clay:
                        CurBlueprint.ClayBot.Build(ref Resources);
                        ++Bots[(int)EMineral.Clay];
                        break;
                    case EMineral.Obsidian:
                        CurBlueprint.ObsidianBot.Build(ref Resources);
                        ++Bots[(int)EMineral.Obsidian];
                        break;
                    case EMineral.Geode:
                        CurBlueprint.GeodeBot.Build(ref Resources);
                        ++Bots[(int)EMineral.Geode];
                        break;
                }
            }

            public int GetFixedGeodes()
            {
                return Resources[(int)EMineral.Geode] + Bots[(int)EMineral.Geode] * Time;
            }

            public int GetPotentialGeodes()
            {
                //return _Resources.Geode + (Time + 2 * _Bots.Geode) * Time / 2;
                return GetFixedGeodes() + ((Time - 1) * Time) / 2;
            }
        }

        private void Search(Operation op, ref int maxGeodes)
        {
            if (op.Time >= 0)
            {
                maxGeodes = Math.Max(maxGeodes, op.GetFixedGeodes());
            }

            if (op.Time <= 0 || op.GetPotentialGeodes() <= maxGeodes)
            {
                return;
            }

            if ((op.BotCount(EMineral.Ore) - 1) > op.CurBlueprint.MaxOre)
            {
                return;
            }

            if (op.BotCount(EMineral.Clay) > op.CurBlueprint.MaxClay)
            {
                return;
            }

            if (op.BotCount(EMineral.Obsidian) > op.CurBlueprint.MaxObsidian)
            {
                return;
            }

            if (op.BotCount(EMineral.Obsidian) > 0)
            {
                // fast forward to having built the geode bot
                int reqOre = Math.Max(0, op.CurBlueprint.GeodeBot.OreCost - op.ResourceCount(EMineral.Ore));
                int reqObs = Math.Max(0, op.CurBlueprint.GeodeBot.ObsidianCost - op.ResourceCount(EMineral.Obsidian));
                int oreTime = (int)Math.Ceiling((float)reqOre / (float)op.BotCount(EMineral.Ore));
                int obsTime = (int)Math.Ceiling((float)reqObs / (float)op.BotCount(EMineral.Obsidian));
                int time = Math.Max(oreTime, obsTime) + 1;
                if (op.Time >= BotMaxBuildTime[(int)EMineral.Geode])
                {
                    Search(op.FastForward(time, EMineral.Geode), ref maxGeodes);
                }
            }

            if (op.BotCount(EMineral.Clay) > 0)
            {
                // fast forward to having built the obsidian bot
                int reqOre = Math.Max(0, op.CurBlueprint.ObsidianBot.OreCost - op.ResourceCount(EMineral.Ore));
                int reqClay = Math.Max(0, op.CurBlueprint.ObsidianBot.ClayCost - op.ResourceCount(EMineral.Clay));
                int oreTime = (int)Math.Ceiling((float)reqOre / (float)op.BotCount(EMineral.Ore));
                int clayTime = (int)Math.Ceiling((float)reqClay / (float)op.BotCount(EMineral.Clay));
                int time = Math.Max(oreTime, clayTime) + 1;
                if (op.Time >= BotMaxBuildTime[(int)EMineral.Obsidian])
                {
                    Search(op.FastForward(time, EMineral.Obsidian), ref maxGeodes);
                }
            }

            // fast forward to having built the clay bot
            {
                int req = Math.Max(0, op.CurBlueprint.ClayBot.OreCost - op.ResourceCount(EMineral.Ore));
                int time = (int)Math.Ceiling((float)req / (float)op.BotCount(EMineral.Ore)) + 1;
                if (op.Time >= BotMaxBuildTime[(int)EMineral.Clay])
                {
                    Search(op.FastForward(time, EMineral.Clay), ref maxGeodes);
                }
            }

            // fast forward to having built the ore bot
            {
                int req = Math.Max(0, op.CurBlueprint.OreBot.OreCost - op.ResourceCount(EMineral.Ore));
                int time = (int)Math.Ceiling((float)req / (float)op.BotCount(EMineral.Ore)) + 1;
                if (op.Time >= BotMaxBuildTime[(int)EMineral.Ore])
                {
                    Search(op.FastForward(time, EMineral.Ore), ref maxGeodes);
                }
            }
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, int minutes, int[] botMaxBuildTime, bool runAllBlueprints)
        {
            MaxMinutes = minutes;
            BotMaxBuildTime = botMaxBuildTime;

            List<Blueprint> blueprints = null;
            if (runAllBlueprints)
            {
                blueprints = inputs.Select(Blueprint.Parse).ToList();
            }
            else
            {
                blueprints = inputs.Select(Blueprint.Parse).Take(3).ToList();
            }

            Dictionary<int, int> maxGeodes = new Dictionary<int, int>();
            foreach (Blueprint bp in blueprints)
            {
                DebugWriteLine(Core.Log.ELevel.Debug, $"Running blueprint #{bp.Id}");
                int maxGeodeForBlueprint = int.MinValue;
                Search(new Operation(bp, minutes), ref maxGeodeForBlueprint);
                maxGeodes[bp.Id] = maxGeodeForBlueprint;
                DebugWriteLine(Core.Log.ELevel.Debug, $"   Max={maxGeodes[bp.Id]}");
            }

            if (runAllBlueprints)
            {
                return maxGeodes.Select(pair => pair.Key * pair.Value).Sum().ToString();
            }
            else
            {
                int score = 1;
                foreach (int val in maxGeodes.Values)
                {
                    score *= val;
                }
                return score.ToString();
            }
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, 24, new int[] { 16, 6, 3, 2 }, true);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, 32, new int[] { 20, 8, 3, 2 }, false);
    }
}