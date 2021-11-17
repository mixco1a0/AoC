using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2016
{
    // TODO: convert from strings to bits
    class Day11 : Day
    {
        public Day11() { }
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
        protected override List<TestDatum> GetTestData()
        {
            List<TestDatum> testData = new List<TestDatum>();
            testData.Add(new TestDatum
            {
                TestPart = Part.One,
                Output = "11",
                RawInput =
@"The first floor contains a hydrogen-compatible microchip and a lithium-compatible microchip.
The second floor contains a hydrogen generator.
The third floor contains a lithium generator.
The fourth floor contains nothing relevant."
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.Two,
                Output = "",
                RawInput =
@""
            });
            return testData;
        }

        private static string[] IgnoreWords =
        {
            "the",
            "floor",
            "contains",
            "a",
            "and",
            "nothing",
            "relevant",
            "microchip",
            "generator"
        };

        private class Floor
        {
            public Floor(int id)
            {
                ID = id;
                Generators = new HashSet<string>();
                Microchips = new HashSet<string>();
                Ignore = false;
            }

            public Floor(Floor other)
            {
                ID = other.ID;
                Generators = new HashSet<string>(other.Generators);
                Microchips = new HashSet<string>(other.Microchips);
                Ignore = other.Ignore;
            }

            public int ID { get; set; }
            public HashSet<string> Generators { get; set; }
            public HashSet<string> Microchips { get; set; }
            public bool Ignore { get; set; }

            public bool CheckIgnore()
            {
                return Microchips.Count == 0 && Generators.Count == 0;
            }

            public override string ToString()
            {
                return $"[{ID}] G={string.Join(',', Generators)} | C={string.Join(',', Microchips)}";
            }
        }

        private Floor[] ParseFloors(List<string> inputs)
        {
            Floor[] floors = new Floor[inputs.Count];
            int i = 0;
            foreach (string input in inputs)
            {
                floors[i] = new Floor(i);
                string[] split = input.Trim('.').Split(' ', StringSplitOptions.RemoveEmptyEntries);
                foreach (string s in split.Skip(2))
                {
                    string clean = s.Trim(",.".ToCharArray());
                    if (IgnoreWords.Contains(clean))
                    {
                        continue;
                    }

                    int idx = clean.IndexOf('-');
                    if (idx >= 0)
                    {
                        floors[i].Microchips.Add(s[..3].ToUpper());
                    }
                    else
                    {
                        floors[i].Generators.Add(s[..3].ToUpper());
                    }
                }
                ++i;
            }
            return floors;
        }

        private class Elevator
        {
            public Elevator()
            {
                Current = -1;
                Target = 0;
                First = "";
                IsFirstGenerator = false;
                Second = "";
                IsSecondGenerator = false;
            }
            public Elevator(Elevator other)
            {
                Current = other.Current;
                Target = other.Target;
                First = other.First;
                IsFirstGenerator = other.IsFirstGenerator;
                Second = other.Second;
                IsSecondGenerator = other.IsSecondGenerator;
            }
            public int Current { get; set; }
            public int Target { get; set; }
            public string First { get; set; }
            public bool IsFirstGenerator { get; set; }
            public string Second { get; set; }
            public bool IsSecondGenerator { get; set; }

            public void Arrive(ref Floor[] floors)
            {
                if (!string.IsNullOrWhiteSpace(First))
                {
                    if (IsFirstGenerator)
                    {
                        floors[Current].Generators.Remove(First);
                        floors[Target].Generators.Add(First);
                    }
                    else
                    {
                        floors[Current].Microchips.Remove(First);
                        floors[Target].Microchips.Add(First);
                    }
                }

                if (!string.IsNullOrWhiteSpace(Second))
                {
                    if (IsSecondGenerator)
                    {
                        floors[Current].Generators.Remove(Second);
                        floors[Target].Generators.Add(Second);
                    }
                    else
                    {
                        floors[Current].Microchips.Remove(Second);
                        floors[Target].Microchips.Add(Second);
                    }
                }

                First = string.Empty;
                Second = string.Empty;
                Current = Target;
                Target = -1;
            }

            private record Possibility(string Name, bool IsGenerator);

            public void GetAllPossibleAttempts(Floor[] floors, Floor floor, ref List<Elevator> attempts)
            {
                // determine the optimal strategy
                int targetFloor = 0;
                for (int i = floors.Count() - 1; i >= 0; --i)
                {
                    if (!floors[i].Ignore)
                    {
                        targetFloor = i;
                    }
                }

                // if bottom floor is not cleared
                //      1 down, 2 up, 1 up, 2 down
                // else
                //      2 up, 1 up, 1 down, 2 down

                int floorDown = Current - 1;
                int floorUp = Current + 1;

                // only check one pair of matching generator to microchip
                List<Possibility> pairedPossibilities = new List<Possibility>();
                List<string> shared = floor.Generators.Intersect(floor.Microchips).ToList();
                shared.Sort();
                if (shared.Count() > 0)
                {
                    pairedPossibilities.Add(new Possibility(shared.First(), true));
                    pairedPossibilities.Add(new Possibility(shared.First(), false));
                }

                IEnumerable<Possibility> allPossibilities = floor.Generators.Select(g => new Possibility(g, true)).Union(floor.Microchips.Select(m => new Possibility(m, false)));
                allPossibilities = allPossibilities.Where(p => !shared.Contains(p.Name)).Union(pairedPossibilities);
                foreach (Possibility first in allPossibilities)
                {
                    Elevator singleMove = new Elevator(this);
                    singleMove.First = first.Name;
                    singleMove.IsFirstGenerator = first.IsGenerator;

                    if (floorDown <= targetFloor)
                    {
                        attempts.Add(new Elevator(singleMove) { Target = floorDown });
                    }

                    foreach (Possibility second in allPossibilities)
                    {
                        if (first == second)
                        {
                            continue;
                        }

                        Elevator doubleMove = new Elevator(singleMove);
                        doubleMove.Second = second.Name;
                        doubleMove.IsSecondGenerator = second.IsGenerator;

                        attempts.Add(new Elevator(doubleMove) { Target = floorUp });
                    }

                    attempts.Add(new Elevator(singleMove) { Target = floorUp });
                    if (floorDown > targetFloor)
                    {
                        attempts.Add(new Elevator(singleMove) { Target = floorDown });
                    }

                    foreach (Possibility second in allPossibilities)
                    {
                        if (first == second)
                        {
                            continue;
                        }

                        Elevator doubleMove = new Elevator(singleMove);
                        doubleMove.Second = second.Name;
                        doubleMove.IsSecondGenerator = second.IsGenerator;

                        attempts.Add(new Elevator(doubleMove) { Target = floorDown });
                    }
                }
            }
        }

        private int SimulateRun(Floor[] floors, Elevator elevator, Dictionary<string, int> cycles, int stepCount, ref int minStepCount)
        {
            // prevent extended sequences
            if (stepCount >= minStepCount || elevator.Target >= floors.Count() || elevator.Target < 0 || floors[elevator.Target].Ignore)
            {
                return int.MaxValue;
            }

            // arrive and check for win condition
            elevator.Arrive(ref floors);
            bool complete = true;
            for (int i = 0; complete && i < floors.Count() - 1; ++i)
            {
                complete &= (floors[i].Generators.Count == 0 && floors[i].Microchips.Count == 0);
            }
            if (complete)
            {
                return stepCount;
            }

            // cycle detection
            StringBuilder sb = new StringBuilder();
            sb.Append("E.");
            sb.Append(elevator.Current);
            foreach (Floor floor in floors)
            {
                IEnumerable<string> shared = floor.Generators.Intersect(floor.Microchips);
                IEnumerable<string> gOnly = floor.Generators.Except(floor.Microchips);
                IEnumerable<string> mOnly = floor.Microchips.Except(floor.Generators);
                sb.Append("|");
                sb.Append(floor.ID);
                sb.Append("|S.");
                sb.Append(shared.Count());
                sb.Append("|G.");
                sb.Append(gOnly.Count());
                sb.Append("|M.");
                sb.Append(mOnly.Count());
            }
            string cycleState = sb.ToString();
            if (cycles.ContainsKey(cycleState))
            {
                if (cycles[cycleState] <= stepCount)
                {
                    return int.MaxValue;
                }
            }
            cycles[cycleState] = stepCount;

            // prevent backtracking to previous floors
            bool ignore = true;
            for (int i = 0; ignore && i < floors.Count(); ++i)
            {
                ignore = floors[i].CheckIgnore();
                if (ignore)
                {
                    floors[i].Ignore = true;
                }
            }

            Floor curFloor = floors[elevator.Current];

            // check for fail conditions
            if (curFloor.Generators.Count > 0)
            {
                foreach (string m in curFloor.Microchips)
                {
                    if (!curFloor.Generators.Contains(m))
                    {
                        return int.MaxValue;
                    }
                }
            }

            // get a list of all possible elevator rides, try them out
            List<Elevator> attempts = new List<Elevator>();
            elevator.GetAllPossibleAttempts(floors, curFloor, ref attempts);
            int attemptCount = 1;
            foreach (Elevator attempt in attempts)
            {
                int simulatedRun = SimulateRun(floors.Select(f => new Floor(f)).ToArray(), attempt, cycles, stepCount + 1, ref minStepCount);
                if (simulatedRun < minStepCount)
                {
                    DebugWriteLine($"New Best {simulatedRun}!!!");
                }
                minStepCount = Math.Min(simulatedRun, minStepCount);
                ++attemptCount;
            }

            return minStepCount;
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, string[] additionalItems)
        {
            Floor[] floors = ParseFloors(inputs);
            foreach (string item in additionalItems)
            {
                floors[0].Generators.Add(item[..3].ToUpper());
                floors[0].Microchips.Add(item[..3].ToUpper());
            }
            Elevator elevator = new Elevator();
            int minStepCount = int.MaxValue;
            SimulateRun(floors.ToList().ToArray(), elevator, new Dictionary<string, int>(), 0, ref minStepCount);
            return minStepCount.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, new string[] { });

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, new string[] { "elerium", "dilithium" });
    }
}