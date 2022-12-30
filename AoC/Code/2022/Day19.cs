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

        private enum EMaterial
        {
            Ore,
            Clay,
            Obsidian,
            Geode
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

            public bool ShouldSave(Resources r)
            {
                bool should = false;

                return should;
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

        private class Resources
        {
            public int Ore { get; set; }
            public int Clay { get; set; }
            public int Obsidian { get; set; }
            public int Geode { get; set; }

            public Resources()
            {
                Ore = 0;
                Clay = 0;
                Obsidian = 0;
                Geode = 0;
            }

            public Resources(Resources r)
            {
                Ore = r.Ore;
                Clay = r.Clay;
                Obsidian = r.Obsidian;
                Geode = r.Geode;
            }

            public void Mine(Bots b)
            {
                Ore += 1 * b.Ore;
                Clay += 1 * b.Clay;
                Obsidian += 1 * b.Obsidian;
                Geode += 1 * b.Geode;
            }

            public override string ToString()
            {
                return $"O={Ore}, C={Clay}, B={Obsidian}, G={Geode}";
            }
        }

        private class Bots
        {
            public int Ore { get; set; }
            public int Clay { get; set; }
            public int Obsidian { get; set; }
            public int Geode { get; set; }

            public Bots()
            {
                Ore = 1;
                Clay = 0;
                Obsidian = 0;
                Geode = 0;
            }

            public Bots(Bots b)
            {
                Ore = b.Ore;
                Clay = b.Clay;
                Obsidian = b.Obsidian;
                Geode = b.Geode;
            }

            public override string ToString()
            {
                return $"O={Ore}, C={Clay}, B={Obsidian}, G={Geode}";
            }
        }

        private record Blueprint(int Id, BasicBot OreBot, BasicBot ClayBot, ObsidianBot ObsidianBot, GeodeBot GeodeBot)
        {
            public static Blueprint Parse(string input)
            {
                int[] split = input.Split(": ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Where(s => int.TryParse(s, out int i)).Select(int.Parse).ToArray();
                BasicBot ore = new BasicBot(split[1]);
                BasicBot clay = new BasicBot(split[2]);
                ObsidianBot obsidian = new ObsidianBot(split[3], split[4]);
                GeodeBot geode = new GeodeBot(split[5], split[6]);
                return new Blueprint(split[0], ore, clay, obsidian, geode);
            }

            public int MaxOreRequired()
            {
                return Math.Max(OreBot.OreCost, Math.Max(ClayBot.OreCost, Math.Max(ObsidianBot.OreCost, GeodeBot.OreCost)));
            }

            public int MaxClayRequired()
            {
                return ObsidianBot.ClayCost;
            }

            public int MaxObsidianRequried()
            {
                return GeodeBot.ObsidianCost;
            }
        }

        private class Operation
        {
            public Blueprint CurBlueprint { get; set; }
            public Resources _Resources;
            public Bots _Bots;
            public Queue<EMaterial> BotOrder { get; set; }
            public List<EMaterial> InitialBotOrder { get; set; }
            private bool OnlyGeoBots { get; set; }

            public Operation(Blueprint blueprint, List<EMaterial> botOrder)
            {
                CurBlueprint = blueprint;
                _Resources = new Resources();
                _Bots = new Bots();
                BotOrder = new Queue<EMaterial>(botOrder);
                InitialBotOrder = new List<EMaterial>(botOrder);
                OnlyGeoBots = false;
            }

            public void Cycle(int cycle)
            {
                Bots cache = new Bots(_Bots);

                Build();

                // mine some more
                _Resources.Mine(cache);
            }

            public bool MetBotOrder()
            {
                var botFinalCounts = InitialBotOrder.GroupBy(b => b); //.ToDictionary(b => b, b => InitialBotOrder.Where(bo => bo == b).Count());

                foreach (var group in botFinalCounts)
                {
                    if (group.Key == EMaterial.Ore && _Bots.Ore - 1 < group.Count())
                    {
                        return false;
                    }

                    if (group.Key == EMaterial.Clay && _Bots.Clay < group.Count())
                    {
                        return false;
                    }

                    if (group.Key == EMaterial.Obsidian && _Bots.Obsidian < group.Count())
                    {
                        return false;
                    }
                }

                return true;
            }

            private void Build()
            {
                // always try to build geode bots first
                if (CurBlueprint.GeodeBot.CanBuild(_Resources))
                {
                    CurBlueprint.GeodeBot.Build(ref _Resources);
                    ++_Bots.Geode;

                    // once you can build this, there is no point in trying to build anything else
                    OnlyGeoBots = !BotOrder.Any();
                    return;
                }

                if (OnlyGeoBots)
                {
                    return;
                }

                if (BotOrder.TryPeek(out EMaterial botType))
                {
                    bool built = false;
                    switch (botType)
                    {
                        case EMaterial.Ore:
                            if (CurBlueprint.OreBot.CanBuild(_Resources))
                            {
                                CurBlueprint.OreBot.Build(ref _Resources);
                                ++_Bots.Ore;
                                built = true;
                            }
                            break;
                        case EMaterial.Clay:
                            if (CurBlueprint.ClayBot.CanBuild(_Resources))
                            {
                                CurBlueprint.ClayBot.Build(ref _Resources);
                                ++_Bots.Clay;
                                built = true;
                            }
                            break;
                        case EMaterial.Obsidian:
                            if (CurBlueprint.ObsidianBot.CanBuild(_Resources))
                            {
                                CurBlueprint.ObsidianBot.Build(ref _Resources);
                                ++_Bots.Obsidian;
                                built = true;
                            }
                            break;
                    }
                    if (built)
                    {
                        BotOrder.Dequeue();
                    }
                }
            }
        }

        private class PossibilityNode : IEquatable<PossibilityNode>
        {
            public int MinScore { get; set; }
            public int ParentBotCount { get; set; }
            public int CurOre { get; set; }
            public int CurClay { get; set; }
            public int CurObsidian { get; set; }
            public List<EMaterial> Bots { get; set; }

            public PossibilityNode(int o, int c, int b)
            {
                MinScore = 0;
                ParentBotCount = 0;
                CurOre = 0;
                CurClay = 0;
                CurObsidian = 0;
                Bots = new List<EMaterial>();
            }

            public PossibilityNode(PossibilityNode prev, EMaterial next)
            {
                MinScore = prev.MinScore;
                ParentBotCount = prev.Bots.Count;
                CurOre = prev.CurOre;
                CurClay = prev.CurClay;
                CurObsidian = prev.CurObsidian;
                Bots = new List<EMaterial>(prev.Bots);
                Bots.Add(next);

                switch (next)
                {
                    case EMaterial.Ore:
                        ++CurOre;
                        break;
                    case EMaterial.Clay:
                        ++CurClay;
                        break;
                    case EMaterial.Obsidian:
                        ++CurObsidian;
                        break;
                }
            }

            public bool Equals(PossibilityNode other)
            {
                return CurOre == other.CurOre && CurClay == other.CurClay && CurObsidian == other.CurObsidian && Bots.SequenceEqual(other.Bots);
            }

            public override string ToString()
            {
                return string.Join(",", Bots.Select(b => b.ToString().Substring(0, 3)));
            }
        }

        private List<List<EMaterial>> GetOrderPossibilitiesFor(int maxOre, int maxClay, int maxObsidian)
        {
            List<List<EMaterial>> allBotOrders = new List<List<EMaterial>>();
            Queue<PossibilityNode> botOrderNodes = new Queue<PossibilityNode>();
            botOrderNodes.Enqueue(new PossibilityNode(0, 0, 0));
            while (botOrderNodes.Count > 0)
            {
                PossibilityNode cur = botOrderNodes.Dequeue();

                // max limits
                if (cur.CurOre > maxOre || cur.CurClay > maxClay || cur.CurObsidian > maxObsidian)
                {
                    continue;
                }

                if (cur.CurObsidian > 0 && cur.CurClay == 0)
                {
                    continue;
                }

                // only add valid possibilities (ore is a check against 0 because you start with one)
                if (cur.CurOre >= 0 && cur.CurClay >= 1 && cur.CurObsidian >= 1)
                {
                    allBotOrders.Add(cur.Bots);
                }

                // this should be the last one needed
                if (cur.CurOre == maxOre && cur.CurClay == maxClay && cur.CurObsidian == maxObsidian)
                {
                    continue;
                }

                PossibilityNode next = null;
                for (EMaterial material = EMaterial.Ore; material < EMaterial.Geode; ++material)
                {
                    next = new PossibilityNode(cur, material);
                    if (!botOrderNodes.Contains(next))
                    {
                        botOrderNodes.Enqueue(next);
                    }
                }
            }
            return allBotOrders;
        }

        private void RunOperation(Blueprint bp, List<EMaterial> botOrder, int minutes, out Operation op)
        {
            op = new Operation(bp, botOrder);
            for (int i = 0; i < minutes; ++i)
            {
                op.Cycle(i);
            }
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, int minutes, bool runAllBlueprints)
        {
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
                int maxOre = bp.MaxOreRequired();
                int maxClay = bp.MaxClayRequired();
                int maxObs = bp.MaxObsidianRequried();

                HashSet<PossibilityNode> used = new HashSet<PossibilityNode>();
                DebugWriteLine(Core.Log.ELevel.Debug, $"Running blueprint #{bp.Id}");
                maxGeodes[bp.Id] = int.MinValue;
                List<EMaterial> botOrder = new List<EMaterial>();
                PriorityQueue<PossibilityNode, int> botOrderNodes = new PriorityQueue<PossibilityNode, int>(); //Comparer<int>.Create((a, b) => b - a)
                botOrderNodes.Enqueue(new PossibilityNode(0, 0, 0), 0);
                while (botOrderNodes.Count > 0)
                {
                    PossibilityNode cur = botOrderNodes.Dequeue();
                    used.Add(cur);

                    // can't make obsidian without clay
                    if (cur.CurObsidian > 0 && cur.CurClay == 0)
                    {
                        continue;
                    }

                    // only add valid possibilities (ore is a check against 0 because you start with one)
                    if (cur.CurOre >= 0 && cur.CurClay >= 1 && cur.CurObsidian >= 1)
                    {
                        RunOperation(bp, cur.Bots, minutes, out Operation op);

                        // this did worse
                        if (op._Resources.Geode < cur.MinScore)
                        {
                            continue;
                        }

                        // if (cur.MinScore != 0 && op._Resources.Geode <= cur.MinScore)
                        // {
                        //     continue;
                        // }

                        if (op._Resources.Geode > maxGeodes[bp.Id])
                        {
                            DebugWriteLine(Core.Log.ELevel.Debug, $"{op._Resources.Geode} @ {string.Join(",", cur.Bots.Select(b => b.ToString().Substring(0, 3)))}");
                        }
                        cur.MinScore = op._Resources.Geode;
                        maxGeodes[bp.Id] = Math.Max(maxGeodes[bp.Id], op._Resources.Geode);

                        if (!op.MetBotOrder())
                        {
                            continue;
                        }
                    }

                    PossibilityNode next = null;
                    for (EMaterial material = EMaterial.Obsidian; material >= EMaterial.Ore; --material)
                    {
                        next = new PossibilityNode(cur, material);
                        if ((next.CurOre + 1) > maxOre)
                        {
                            continue;
                        }

                        if (next.CurClay > maxClay)
                        {
                            continue;
                        }

                        if (next.CurObsidian > maxObs)
                        {
                            continue;
                        }

                        if (!used.Contains(next))
                        {
                            botOrderNodes.Enqueue(next, cur.MinScore * -1 + cur.Bots.Count);
                        }
                    }
                }
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
            => SharedSolution(inputs, variables, 24, true);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, 32, false);
    }
}