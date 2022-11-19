using System;
using System.Collections.Generic;
using System.Linq;

using AoC.Core;

namespace AoC._2021
{
    class Day12 : Day
    {
        public Day12() { }

        public override string GetSolutionVersion(Part part)
        {
            switch (part)
            {
                case Part.One:
                    return "v2";
                case Part.Two:
                    return "v2";
                default:
                    return base.GetSolutionVersion(part);
            }
        }

        public override bool SkipTestData => true;

        protected override List<TestDatum> GetTestData()
        {
            List<TestDatum> testData = new List<TestDatum>();
            testData.Add(new TestDatum
            {
                TestPart = Part.One,
                Output = "10",
                RawInput =
@"start-A
start-b
A-c
A-b
b-d
A-end
b-end"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.One,
                Output = "19",
                RawInput =
@"dc-end
HN-start
start-kj
dc-start
dc-HN
LN-dc
HN-end
kj-sa
kj-HN
kj-dc"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.One,
                Output = "226",
                RawInput =
@"fs-end
he-DX
fs-he
start-DX
pj-DX
end-zg
zg-sl
zg-pj
pj-he
RW-he
fs-DX
pj-RW
zg-RW
start-pj
he-WI
zg-he
pj-fs
start-RW"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.Two,
                Output = "36",
                RawInput =
@"start-A
start-b
A-c
A-b
b-d
A-end
b-end"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.Two,
                Output = "103",
                RawInput =
@"dc-end
HN-start
start-kj
dc-start
dc-HN
LN-dc
HN-end
kj-sa
kj-HN
kj-dc"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.Two,
                Output = "3509",
                RawInput =
@"fs-end
he-DX
fs-he
start-DX
pj-DX
end-zg
zg-sl
zg-pj
pj-he
RW-he
fs-DX
pj-RW
zg-RW
start-pj
he-WI
zg-he
pj-fs
start-RW"
            });
            return testData;
        }

        private class Cave
        {
            public int ID { get; private set; }
            public string Name { get; private set; }
            public List<Cave> Connections { get; private set; }

            public Cave(int id, string name)
            {
                ID = id;
                Name = name;
                Connections = new List<Cave>();
            }

            public override string ToString()
            {
                return $"[{ID,3}] {Name} -> {string.Join(',', Connections.Select(c => c.Name))}";
            }
        }

        private class CaveSystem
        {
            public Cave Start { get; set; }
            public Cave End { get; set; }

            public List<Cave> Caves { get; private set; }
            private Dictionary<string, int> CaveIds { get; set; }

            public bool Extended { get; private set; }
            private int m_smallCaveId;
            private int m_largeCaveId;

            public CaveSystem(bool extended)
            {
                Start = null;
                End = null;
                Caves = new List<Cave>();
                CaveIds = new Dictionary<string, int>();
                Extended = extended;
                m_smallCaveId = -1;
                m_largeCaveId = 1;
            }

            public void AddConnectedCaves(string[] names)
            {
                const int StartId = 0;
                const int EndId = int.MaxValue;

                Cave first = null, second = null;
                for (int i = 0; i <= 1; ++i)
                {
                    if (!CaveIds.ContainsKey(names[i]))
                    {
                        if (names[i] == "start")
                        {
                            CaveIds[names[i]] = StartId;
                        }
                        else if (names[i] == "end")
                        {
                            CaveIds[names[i]] = EndId;
                        }
                        else if (char.IsUpper(names[i][0]))
                        {
                            CaveIds[names[i]] = m_largeCaveId++;
                        }
                        else
                        {
                            CaveIds[names[i]] = m_smallCaveId--;
                        }
                        Caves.Add(new Cave(CaveIds[names[i]], names[i]));
                    }

                    if (i == 0)
                    {
                        first = Caves.Single(c => c.ID == CaveIds[names[i]]);
                    }
                    else
                    {
                        second = Caves.Single(c => c.ID == CaveIds[names[i]]);
                    }
                }

                first.Connections.Add(second);
                second.Connections.Add(first);

                if (first.ID == StartId)
                {
                    Start = first;
                }
                else if (first.ID == EndId)
                {
                    End = first;
                }

                if (second.ID == StartId)
                {
                    Start = second;
                }
                else if (second.ID == EndId)
                {
                    End = second;
                }
            }

            public void Traverse(ref int pathCount)
            {
                Stack<int> visited = new Stack<int>();
                TraverseInternal(Start, ref visited, ref pathCount);
            }

            private void TraverseInternal(Cave cave, ref Stack<int> visited, ref int pathCount)
            {
                if (cave.ID == End.ID)
                {
                    ++pathCount;
                    return;
                }

                if (cave.ID < 0)
                {
                    if (Extended)
                    {
                        IEnumerable<IGrouping<int, int>> groups = visited.Where(v => v < 0).GroupBy(v => v);
                        if (groups.Any(g => g.Count() > 1) && groups.Any(g => g.Key == cave.ID))
                        {
                            return;
                        }
                    }
                    else
                    {
                        if (visited.Any(id => id == cave.ID))
                        {
                            return;
                        }
                    }
                }

                visited.Push(cave.ID);
                foreach (Cave connection in cave.Connections)
                {
                    TraverseInternal(connection, ref visited, ref pathCount);
                }
                visited.Pop();
            }
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool extended)
        {
            int pathCount = 0;
            CaveSystem caveSystem = new CaveSystem(extended);
            foreach (string input in inputs)
            {
                string[] split = input.Split('-', StringSplitOptions.RemoveEmptyEntries).ToArray();
                caveSystem.AddConnectedCaves(split);
            }
            caveSystem.Caves.ForEach(c => c.Connections.Remove(caveSystem.Start));
            caveSystem.End.Connections.Clear();
            caveSystem.Traverse(ref pathCount);
            return pathCount.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);
    }
}