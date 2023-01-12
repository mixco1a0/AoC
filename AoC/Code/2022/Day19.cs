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

        public override bool SkipTestData => false;

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

        private enum EMineral
        {
            Ore,
            Clay,
            Obsidian,
            Geode,
            None
        }

        private record BasicBot(int OreCost)
        {
            public bool CanBuild(Resources r)
            {
                return r.Ore >= OreCost;
            }

            public void Build(ref Resources r)
            {
                r.Ore -= OreCost;
            }
        }
        private record ObsidianBot(int OreCost, int ClayCost)
        {
            public bool CanBuild(Resources r)
            {
                return r.Ore >= OreCost && r.Clay >= ClayCost;
            }

            public void Build(ref Resources r)
            {
                r.Ore -= OreCost;
                r.Clay -= ClayCost;
            }
        }
        private record GeodeBot(int OreCost, int ObsidianCost)
        {
            public bool CanBuild(Resources r)
            {
                return r.Ore >= OreCost && r.Obsidian >= ObsidianCost;
            }

            public void Build(ref Resources r)
            {
                r.Ore -= OreCost;
                r.Obsidian -= ObsidianCost;
            }
        }

        private class MineralCount
        {
            public int Ore { get; set; }
            public int Clay { get; set; }
            public int Obsidian { get; set; }
            public int Geode { get; set; }

            protected MineralCount(int ore, int clay, int obsidian, int geode)
            {
                Ore = ore;
                Clay = clay;
                Obsidian = obsidian;
                Geode = geode;
            }

            public override string ToString()
            {
                return $"O={Ore}, C={Clay}, B={Obsidian}, G={Geode}";
            }
        }
        private class Resources : MineralCount
        {
            public Resources() : base(0, 0, 0, 0) { }

            public Resources(Resources r) : base(r.Ore, r.Clay, r.Obsidian, r.Geode) { }

            public void Mine(Bots b, int time)
            {
                Ore += time * b.Ore;
                Clay += time * b.Clay;
                Obsidian += time * b.Obsidian;
                Geode += time * b.Geode;
            }

            public override string ToString()
            {
                return $"Res[{base.ToString()}]";
            }
        }
        private class Bots : MineralCount
        {
            public Bots() : base(1, 0, 0, 0) { }

            public Bots(Bots b) : base(b.Ore, b.Clay, b.Obsidian, b.Geode) { }

            public override string ToString()
            {
                return $"Bot[{base.ToString()}]";
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
            public Resources _Resources;
            public Bots _Bots;
            public Queue<EMineral> BotOrder { get; set; }

            public Operation(Blueprint blueprint, int time)
            {
                CurBlueprint = blueprint;
                Time = time;
                _Resources = new Resources();
                _Bots = new Bots();
                BotOrder = new Queue<EMineral>();
                BotOrder.Enqueue(EMineral.Ore);
            }

            private Operation(Operation prev)
            {
                CurBlueprint = prev.CurBlueprint;
                Time = prev.Time;
                _Resources = new Resources(prev._Resources);
                _Bots = new Bots(prev._Bots);
                BotOrder = new Queue<EMineral>(prev.BotOrder);
            }

            public Operation FastForward(int time, EMineral newBot)
            {
                Operation newOp = new Operation(this);
                newOp.Mine(time);
                newOp.Build(newBot);
                newOp.Time -= time;
                newOp.BotOrder.Enqueue(newBot);
                return newOp;
            }

            private void Mine(int time)
            {
                _Resources.Mine(_Bots, time);
            }

            private void Build(EMineral material)
            {
                switch (material)
                {
                    case EMineral.Ore:
                        CurBlueprint.OreBot.Build(ref _Resources);
                        ++_Bots.Ore;
                        break;
                    case EMineral.Clay:
                        CurBlueprint.ClayBot.Build(ref _Resources);
                        ++_Bots.Clay;
                        break;
                    case EMineral.Obsidian:
                        CurBlueprint.ObsidianBot.Build(ref _Resources);
                        ++_Bots.Obsidian;
                        break;
                    case EMineral.Geode:
                        CurBlueprint.GeodeBot.Build(ref _Resources);
                        ++_Bots.Geode;
                        break;
                }
            }

            public int GetFixedGeodes()
            {
                return _Resources.Geode + _Bots.Geode * Time;
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

            if ((op._Bots.Ore - 1) > op.CurBlueprint.MaxOre)
            {
                return;
            }

            if (op._Bots.Clay > op.CurBlueprint.MaxClay)
            {
                return;
            }

            if (op._Bots.Obsidian > op.CurBlueprint.MaxObsidian)
            {
                return;
            }

            if (op._Bots.Obsidian > 0)
            {
                // fast forward to having built the geode bot
                int reqOre = Math.Max(0, op.CurBlueprint.GeodeBot.OreCost - op._Resources.Ore);
                int reqObs = Math.Max(0, op.CurBlueprint.GeodeBot.ObsidianCost - op._Resources.Obsidian);
                int oreTime = (int)Math.Ceiling((float)reqOre / (float)op._Bots.Ore);
                int obsTime = (int)Math.Ceiling((float)reqObs / (float)op._Bots.Obsidian);
                int time = Math.Max(oreTime, obsTime) + 1;
                if (op.Time >= BotMaxBuildTime[(int)EMineral.Geode])
                {
                    Search(op.FastForward(time, EMineral.Geode), ref maxGeodes);
                }
            }

            if (op._Bots.Clay > 0)
            {
                // fast forward to having built the obsidian bot
                int reqOre = Math.Max(0, op.CurBlueprint.ObsidianBot.OreCost - op._Resources.Ore);
                int reqClay = Math.Max(0, op.CurBlueprint.ObsidianBot.ClayCost - op._Resources.Clay);
                int oreTime = (int)Math.Ceiling((float)reqOre / (float)op._Bots.Ore);
                int clayTime = (int)Math.Ceiling((float)reqClay / (float)op._Bots.Clay);
                int time = Math.Max(oreTime, clayTime) + 1;
                if (op.Time >= BotMaxBuildTime[(int)EMineral.Obsidian])
                {
                    Search(op.FastForward(time, EMineral.Obsidian), ref maxGeodes);
                }
            }

            // fast forward to having built the clay bot
            {
                int req = Math.Max(0, op.CurBlueprint.ClayBot.OreCost - op._Resources.Ore);
                int time = (int)Math.Ceiling((float)req / (float)op._Bots.Ore) + 1;
                if (op.Time >= BotMaxBuildTime[(int)EMineral.Clay])
                {
                    Search(op.FastForward(time, EMineral.Clay), ref maxGeodes);
                }
            }

            // fast forward to having built the ore bot
            {
                int req = Math.Max(0, op.CurBlueprint.OreBot.OreCost - op._Resources.Ore);
                int time = (int)Math.Ceiling((float)req / (float)op._Bots.Ore) + 1;
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