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

        public enum Part
        {
            None,
            X = 'x',
            M = 'm',
            A = 'a',
            S = 's',
            Invalid
        }

        private Part NextPart(Part part)
        {
            if (part == Part.None)
            {
                return Part.X;
            }
            else if (part == Part.X)
            {
                return Part.M;
            }
            else if (part == Part.M)
            {
                return Part.A;
            }
            else if (part == Part.A)
            {
                return Part.S;
            }
            return Part.Invalid;
        }

        public enum Op
        {
            LessThan = '<',
            MoreThan = '>',
            True = '=',
            LessThanE = '[',
            MoreThanE = ']'
        }

        public record Rule(Part Part, Op Op, int Value, string Target)
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

        public record Range(Part Part, Base.RangeL Values, bool Inclusive)
        {
            public long Get()
            {
                return Math.Abs(Values.Max - Values.Min);
            }

            public static Range Convert(Rule rule, bool inclusive)
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
                return new Range(rule.Part, range, inclusive);
            }

            public static Range Flip(Rule rule)
            {
                Base.RangeL range = new Base.RangeL();
                switch (rule.Op)
                {
                    case Op.LessThan:
                        // range.Min = MinRating;
                        // range.Max = rule.Value - 1;
                        range.Min = rule.Value;
                        range.Max = MaxRating;
                        break;
                    case Op.MoreThan:
                        // range.Min = rule.Value + 1;
                        // range.Max = MaxRating;
                        range.Min = 0;
                        range.Max = rule.Value;
                        break;
                    case Op.True:
                        range.Min = MinRating;
                        range.Max = MaxRating;
                        break;
                    case Op.LessThanE:
                        break;
                    case Op.MoreThanE:
                        break;
                }
                return new Range(rule.Part, range, false);
            }

            public override string ToString()
            {
                char i = Inclusive ? 'O' : 'X';
                return $"{(char)Part} -> ({i}){Values}";
            }
        }

        public record MultiRange(Part Part, List<Base.RangeL> Values, List<bool> Inclusive)
        {

            private record Helper(Base.RangeL RangeL, bool Inclusive)
            {
                public override string ToString()
                {
                char i = Inclusive ? 'O' : 'X';
                return $"({i}){RangeL}";
                }
            }

            public MultiRange GetCopy()
            {
                return new MultiRange(Part, Values.ToList(), Inclusive.ToList());
            }

            public void Compress()
            {
                List<Helper> helpers = new List<Helper>();
                for (int i = 0; i < Values.Count; ++i)
                {
                    helpers.Add(new Helper(Values[i], Inclusive[i]));
                }

                List<Base.RangeL> list = Values.ToList();
                Compress(helpers, true, out List<Base.RangeL> inclusive);
                Compress(helpers, false, out List<Base.RangeL> exclusive);
                if (inclusive.Count > 0 && exclusive.Count > 0)
                {
                    while (Compress(inclusive, exclusive, out List<Base.RangeL> newInclusives))
                    {
                        helpers = new List<Helper>();
                        for (int i = 0; i < newInclusives.Count; ++i)
                        {
                            helpers.Add(new Helper(newInclusives[i], true));
                        }
                        Compress(helpers, true, out inclusive);
                    }
                }

                Values.Clear();
                Values.AddRange(inclusive);
            }

            private void Compress(List<Helper> helpers, bool inclusive, out List<Base.RangeL> ranges)
            {
                List<Base.RangeL> list = helpers.Where(h => h.Inclusive == inclusive).Select(h => h.RangeL).ToList();
                for (int i = 0; i < list.Count() && list.Count() > 1;)
                {
                    Base.RangeL cur = list[i];
                    Base.RangeL compressed = new Base.RangeL(long.MaxValue, long.MinValue);
                    IEnumerable<Base.RangeL> rangeLs = list.Where(l => l.HasIncOr(cur));
                    int rangeLsCount = rangeLs.Count();
                    foreach (Base.RangeL rangeL in rangeLs)
                    {
                        compressed.Min = Math.Min(compressed.Min, rangeL.Min);
                        compressed.Max = Math.Max(compressed.Max, rangeL.Max);
                    }
                    list.RemoveAll(l => l.HasIncOr(compressed));

                    if (rangeLsCount > 1)
                    {
                        list.Add(compressed);
                        i = 0;
                    }
                    else
                    {
                        ++i;
                    }
                }
                ranges = list;
            }

            private bool Compress(List<Base.RangeL> inclusive, List<Base.RangeL> exclusive, out List<Base.RangeL> newInclusives)
            {
                newInclusives = new List<Base.RangeL>();
                bool compressed = false;
                foreach (Base.RangeL i in inclusive)
                {
                    foreach (Base.RangeL e in exclusive)
                    {
                        if (i.HasIncOr(e))
                        {
                            compressed = true;
                            if (i.Max >= e.Max && i.Min <= e.Min)
                            {
                                // split
                                newInclusives.Add(new Base.RangeL(i.Min, e.Min - 1));
                                newInclusives.Add(new Base.RangeL(e.Max + 1, i.Max));
                            }
                            else if (i.Max > e.Max)
                            {
                                newInclusives.Add(new Base.RangeL(e.Max + 1, i.Max));
                            }
                            else
                            {
                                newInclusives.Add(new Base.RangeL(i.Min, e.Min - 1));
                            }
                        }
                    }
                }
                return compressed;
            }

            public override string ToString()
            {
                List<Helper> helpers = new List<Helper>();
                for (int i = 0; i < Values.Count; ++i)
                {
                    helpers.Add(new Helper(Values[i], Inclusive[i]));
                }
                return $"{(char)Part} -> {string.Join(" | ", helpers)}";
            }
        }

        public class WorkflowState
        {
            public MultiRange X;
            public MultiRange M;
            public MultiRange A;
            public MultiRange S;

            public WorkflowState()
            {
                X = null;
                M = null;
                A = null;
                S = null;
            }

            public WorkflowState(MultiRange x, MultiRange m, MultiRange a, MultiRange s)
            {
                X = x;
                M = m;
                A = a;
                S = s;
            }

            public static WorkflowState Default()
            {
                MultiRange x = new MultiRange(Part.X, new List<Base.RangeL>(), new List<bool>());
                MultiRange m = new MultiRange(Part.M, new List<Base.RangeL>(), new List<bool>());
                MultiRange a = new MultiRange(Part.A, new List<Base.RangeL>(), new List<bool>());
                MultiRange s = new MultiRange(Part.S, new List<Base.RangeL>(), new List<bool>());
                return new WorkflowState(x, m, a, s);
            }

            public WorkflowState GetCopy()
            {
                return new WorkflowState(X.GetCopy(), M.GetCopy(), A.GetCopy(), S.GetCopy());
            }

            public long Get()
            {
                return Get(X) * Get(M) * Get(A) * Get(S);
            }

            private long Get(MultiRange multiRange)
            {
                if (multiRange.Values.Count == 0)
                {
                    return MaxRating;
                }

                long count = 0;
                foreach (Base.RangeL r in multiRange.Values)
                {
                    count += r.Max - r.Min + 1;
                }
                return count;
            }
        }

        private long GetDistinctCombinations(List<Workflow> workflows)
        {
            Dictionary<string, List<Rule>> workflowDictionary = workflows.ToDictionary(w => w.Id, w => w.Rules);
            // RatingState ratingState = new RatingState();
            List<List<Range>> accepted = new List<List<Range>>();
            List<List<Range>> rejected = new List<List<Range>>();
            long allCombos = WalkWorkflows(Initial, workflowDictionary, new List<Range>(), WorkflowState.Default());
            return allCombos;
        }

        private long WalkWorkflows(string workflowId, Dictionary<string, List<Rule>> workflowDictionary, List<Range> preReqs, WorkflowState workflowState)
        {
            long count = 0;
            List<Rule> rules = workflowDictionary[workflowId];
            List<Range> opposites = new List<Range>();
            foreach (Rule rule in rules)
            {
                WorkflowState next = workflowState.GetCopy();

                List<Range> reqs = new List<Range>();
                reqs.AddRange(preReqs);
                reqs.AddRange(opposites);
                if (rule.Op != Op.True)
                {
                    Range newRange = Range.Convert(rule, true);
                    reqs.Add(newRange);
                }

                if (rule.Target == Accept)
                {
                    CollapseRanges(reqs, ref next);
                    long c = next.Get();
                    // Log($"accepted => [{c}] {string.Join(" | ", reqs)}");
                    count += c;
                }
                else if (rule.Target == Reject)
                {
                    CollapseRanges(reqs, ref next);
                    long c = next.Get();
                    // Log($"rejected => [{c}] {string.Join(" | ", reqs)}");
                    // count -= c;
                }
                else
                {
                    CollapseRanges(reqs, ref next);
                    count += WalkWorkflows(rule.Target, workflowDictionary, reqs, next);
                }

                opposites.Add(Range.Convert(rule, false));
            }
            return count;
        }

        private void CollapseRanges(List<Range> reqs, ref WorkflowState workflowState)
        {
            CollapseRanges(reqs, ref workflowState.X);
            CollapseRanges(reqs, ref workflowState.M);
            CollapseRanges(reqs, ref workflowState.A);
            CollapseRanges(reqs, ref workflowState.S);
        }

        private void CollapseRanges(List<Range> reqs, ref MultiRange multiRange)
        {
            Part part = multiRange.Part;
            multiRange = new MultiRange(part, new List<Base.RangeL>(), new List<bool>());
            IEnumerable<Range> subset = reqs.Where(l => l.Part == part);
            if (subset.Count() > 0)
            {
                foreach (Range range in subset)
                {
                    multiRange.Values.Add(new Base.RangeL(range.Values));
                    multiRange.Inclusive.Add(range.Inclusive);
                }
                multiRange.Compress();
            }
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