using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2023
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
                Output = "19114",
                RawInput =
@"px{a<2006:qkq,m>2090:A,rfg}
pv{a>1716:R,A}
lnx{m>1548:A,A}
rfg{s<537:gd,x>2440:R,A}
qs{s>3448:A,lnx}
qkq{x<1416:A,crn}
crn{x>2662:A,R}
in{s<1351:px,qqz}
qqz{s>2770:qs,m<1801:hdj,R}
gd{a>3333:R,R}
hdj{m>838:A,pv}

{x=787,m=2655,a=1222,s=2876}
{x=1679,m=44,a=2067,s=496}
{x=2036,m=264,a=79,s=2244}
{x=2461,m=1339,a=466,s=291}
{x=2127,m=1623,a=2188,s=1013}"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
                Output = "167409079868000",
                RawInput =
@"px{a<2006:qkq,m>2090:A,rfg}
pv{a>1716:R,A}
lnx{m>1548:A,A}
rfg{s<537:gd,x>2440:R,A}
qs{s>3448:A,lnx}
qkq{x<1416:A,crn}
crn{x>2662:A,R}
in{s<1351:px,qqz}
qqz{s>2770:qs,m<1801:hdj,R}
gd{a>3333:R,R}
hdj{m>838:A,pv}

{x=787,m=2655,a=1222,s=2876}
{x=1679,m=44,a=2067,s=496}
{x=2036,m=264,a=79,s=2244}
{x=2461,m=1339,a=466,s=291}
{x=2127,m=1623,a=2188,s=1013}"
            });
            return testData;
        }

        private const string Accept = "A";
        private const string Reject = "R";
        private const string Initial = "in";

        private const int MinRating = 1;
        private const int MaxRating = 4000;

        private enum Part
        {
            None,
            X = 'x',
            M = 'm',
            A = 'a',
            S = 's'
        }

        private enum Op
        {
            LessThan = '<',
            MoreThan = '>',
            True = '=',
            LessThanE = '[',
            MoreThanE = ']'
        }

        private record Rule(Part Part, Op Op, int Value, string Target)
        {
            public override string ToString()
            {
                if (Op == Op.True)
                {
                    return $"-> {Target}";
                }
                return $"{(char)Part}{(char)Op}{Value} -> {Target}";
            }
        }

        private record Workflow(string Id, List<Rule> Rules)
        {
            public static Workflow Parse(string input)
            {
                string[] split = Util.String.Split(input, "{,}");
                List<Rule> rules = new List<Rule>();
                foreach (string s in split.Skip(1))
                {
                    if (s.Contains(':'))
                    {
                        string[] split2 = Util.String.Split(s, "<>:");
                        rules.Add(new Rule((Part)split2[0][0], s.Contains('>') ? Op.MoreThan : Op.LessThan, int.Parse(split2[1]), split2[2]));
                    }
                    else
                    {
                        rules.Add(new Rule(Part.None, Op.True, 0, s));
                    }
                }
                return new Workflow(split[0], rules);
            }

            public string Evaluate(PartRating partRating)
            {
                foreach (Rule rule in Rules)
                {
                    if (rule.Op == Op.True)
                    {
                        return rule.Target;
                    }
                    else if (rule.Op == Op.LessThan)
                    {
                        if (partRating.Ratings[rule.Part] < rule.Value)
                        {
                            return rule.Target;
                        }
                    }
                    else if (rule.Op == Op.MoreThan)
                    {
                        if (partRating.Ratings[rule.Part] > rule.Value)
                        {
                            return rule.Target;
                        }
                    }
                }

                return Reject;
            }
        }

        private record PartRating(Dictionary<Part, int> Ratings)
        {
            public static PartRating Parse(string input)
            {
                string[] split = Util.String.Split(input, "{=, }");
                Dictionary<Part, int> ratings = new Dictionary<Part, int>();
                Part key = Part.None;
                foreach (string s in split)
                {
                    if (key == Part.None)
                    {
                        key = (Part)s.First();
                    }
                    else
                    {
                        ratings[key] = int.Parse(s);
                        key = Part.None;
                    }
                }
                return new PartRating(ratings);
            }

            public int GetTotalRating()
            {
                return Ratings.Select(r => r.Value).Sum();
            }

            public override string ToString()
            {
                return $"{string.Join(",", Ratings.Select(pair => $"{pair.Key}={pair.Value}"))}";
            }
        }

        private void ParseInput(List<string> inputs, out List<Workflow> workflows, out List<PartRating> partRatings)
        {
            IEnumerable<string> rawWorkflows = inputs.Where(i => !string.IsNullOrWhiteSpace(i) && char.IsAsciiLetter(i[0]));
            workflows = rawWorkflows.Select(Workflow.Parse).ToList();

            IEnumerable<string> rawPartRatings = inputs.Where(i => !string.IsNullOrWhiteSpace(i) && i[0] == '{');
            partRatings = rawPartRatings.Select(PartRating.Parse).ToList();
        }

        private record Range(Part Part, Base.RangeL Values)
        {
            public long Get()
            {
                return Math.Abs(Values.Max - Values.Min);
            }

            public static Range Convert(Rule rule)
            {
                Base.RangeL range = new Base.RangeL();
                switch (rule.Op)
                {
                    case Op.LessThan:
                        range.Min = MinRating;
                        range.Max = rule.Value - 1;
                        break;
                    case Op.MoreThan:
                        range.Min = rule.Value + 1;
                        range.Max = MaxRating;
                        break;
                    case Op.True:
                        range.Min = MinRating;
                        range.Max = MaxRating;
                        break;
                    case Op.LessThanE:
                        range.Min = 0;
                        range.Max = rule.Value;
                        break;
                    case Op.MoreThanE:
                        range.Min = rule.Value;
                        range.Max = MaxRating;
                        break;
                }
                return new Range(rule.Part, range);
            }

            public override string ToString()
            {
                return $"{(char)Part} -> {Values}";
            }
        }

        private long GetDistinctCombinations(List<Workflow> workflows)
        {
            Dictionary<string, List<Rule>> workflowDictionary = workflows.ToDictionary(w => w.Id, w => w.Rules);
            // RatingState ratingState = new RatingState();
            List<List<Range>> accepted = new List<List<Range>>();
            List<List<Range>> rejected = new List<List<Range>>();
            CollapseWorkflow(Initial, workflowDictionary, new List<Range>(), ref accepted, ref rejected);
            CollapseAccepted(accepted, out List<List<Range>> collapsed);
            long allCombinations = 0;
            foreach (List<Range> ranges in accepted)
            {
                long curRangeCombinations = 1;
                foreach (Range range in ranges)
                {
                    curRangeCombinations *= range.Get();
                }
                allCombinations += curRangeCombinations;
            }
            return allCombinations;
        }

        private void CollapseWorkflow(string workflowId, Dictionary<string, List<Rule>> workflowDictionary, List<Range> preReqs, ref List<List<Range>> accepted, ref List<List<Range>> rejected)
        {
            List<Rule> rules = workflowDictionary[workflowId];
            foreach (Rule rule in rules)
            {
                List<Range> reqs = new List<Range>();
                reqs.AddRange(preReqs);
                if (rule.Op != Op.True)
                {
                    Range newRange = Range.Convert(rule);
                    reqs.Add(newRange);
                }

                if (rule.Target == Accept)
                {
                    accepted.Add(reqs);
                }
                else if (rule.Target == Reject)
                {
                    rejected.Add(reqs);
                }
                else
                {
                    CollapseWorkflow(rule.Target, workflowDictionary, reqs, ref accepted, ref rejected);
                }
            }
        }


        // private class RatingState
        // {
        //     List<Base.Range> XStates { get; set; }
        //     List<Base.Range> MStates { get; set; }
        //     List<Base.Range> AStates { get; set; }
        //     List<Base.Range> SStates { get; set; }

        //     public RatingState()
        //     {
        //         XStates = new List<Base.Range>();
        //         MStates = new List<Base.Range>();
        //         AStates = new List<Base.Range>();
        //         SStates = new List<Base.Range>();
        //     }
        // }
        
        private void CollapseAccepted(List<List<Range>> accepted, out List<List<Range>> collapsed)
        {
            // flatten shared ranges
            // include missing letters
            collapsed = new List<List<Range>>();
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool findAll)
        {
            ParseInput(inputs, out List<Workflow> workflows, out List<PartRating> partRatings);
            Dictionary<PartRating, string> partLocations = new Dictionary<PartRating, string>();
            foreach (PartRating pr in partRatings)
            {
                partLocations[pr] = Initial;
            }

            if (findAll)
            {
                return GetDistinctCombinations(workflows).ToString();
            }


            bool process = true;
            while (process)
            {
                process = false;
                Dictionary<PartRating, string> newLocations = new Dictionary<PartRating, string>();
                foreach (var pl in partLocations)
                {
                    if (pl.Value == Accept || pl.Value == Reject)
                    {
                        newLocations[pl.Key] = pl.Value;
                        continue;
                    }

                    process = true;
                    newLocations[pl.Key] = workflows.Where(w => w.Id == pl.Value).First().Evaluate(pl.Key);
                }
                partLocations = newLocations;
            }
            return partLocations.Where(pair => pair.Value == Accept).Select(pair => pair.Key.GetTotalRating()).Sum().ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);
    }
}