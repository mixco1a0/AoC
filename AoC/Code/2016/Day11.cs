using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AoC._2016
{
    class Day11 : Core.Day
    {
        public Day11() { }

        public override string GetSolutionVersion(Core.Part part)
        {
            switch (part)
            {
                case Core.Part.One:
                    return "v3";
                case Core.Part.Two:
                    return "v3";
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
                Output = "11",
                RawInput =
@"The first floor contains a hydrogen-compatible microchip and a lithium-compatible microchip.
The second floor contains a hydrogen generator.
The third floor contains a lithium generator.
The fourth floor contains nothing relevant."
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
                FirstIsGenerator = false;
                Second = "";
                SecondIsGenerator = false;
            }
            public Elevator(Elevator other)
            {
                Current = other.Current;
                Target = other.Target;
                First = other.First;
                FirstIsGenerator = other.FirstIsGenerator;
                Second = other.Second;
                SecondIsGenerator = other.SecondIsGenerator;
            }
            public int Current { get; set; }
            public int Target { get; set; }
            public string First { get; set; }
            public bool FirstIsGenerator { get; set; }
            public string Second { get; set; }
            public bool SecondIsGenerator { get; set; }

            public void Arrive(ref Floor[] floors)
            {
                if (!string.IsNullOrWhiteSpace(First))
                {
                    if (FirstIsGenerator)
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
                    if (SecondIsGenerator)
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

            private record Possibility(string Name, bool IsGenerator) { }

            public void GetAllPossibleAttempts(Floor[] floors, Floor floor, ref List<Elevator> attempts)
            {
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
                    singleMove.FirstIsGenerator = first.IsGenerator;

                    if (floorUp < floors.Count())
                    {
                        foreach (Possibility second in allPossibilities)
                        {
                            if (first == second)
                            {
                                continue;
                            }

                            Elevator doubleMove = new Elevator(singleMove);
                            doubleMove.Second = second.Name;
                            doubleMove.SecondIsGenerator = second.IsGenerator;

                            attempts.Add(new Elevator(doubleMove) { Target = floorUp });
                        }
                        attempts.Add(new Elevator(singleMove) { Target = floorUp });
                    }

                    if (floorDown >= 0)
                    {
                        attempts.Add(new Elevator(singleMove) { Target = floorDown });
                        foreach (Possibility second in allPossibilities)
                        {
                            if (first == second)
                            {
                                continue;
                            }

                            Elevator doubleMove = new Elevator(singleMove);
                            doubleMove.Second = second.Name;
                            doubleMove.SecondIsGenerator = second.IsGenerator;

                            attempts.Add(new Elevator(doubleMove) { Target = floorDown });
                        }
                    }
                }
            }
        }

        private record BuildingState(Floor[] Floors, Elevator Elevator, int Steps, string ID)
        {
            public void Print()
            {
                StringBuilder sb = new StringBuilder();
                List<string> ids = new List<string>();
                Floors.ToList().ForEach(f => ids.AddRange(f.Generators.Union(f.Microchips)));
                HashSet<string> pairedIds = ids.ToHashSet();
                // Util.Log.WriteLine($"[{ID}] @ {Steps}");
                // foreach (Floor floor in Floors.Reverse())
                // {
                //     sb.AppendFormat("F{0} {1}", floor.ID + 1, Elevator.Current == floor.ID ? "E  " : ".  ");
                //     foreach (string pid in pairedIds)
                //     {
                //         sb.AppendFormat("{0}{1}", floor.Generators.Contains(pid) ? $"{pid.First()}G " : ".  ", floor.Microchips.Contains(pid) ? $"{pid.First()}M " : ".  ");
                //     }
                //     Util.Log.WriteLine(sb.ToString());
                //     sb.Clear();
                // }
                // Util.Log.WriteLine("");
            }
        }

        private int SimulateRun(Dictionary<string, int> cycles, ref Queue<BuildingState> next)
        {
            while (next.Count > 0)
            {
                BuildingState bs = next.Dequeue();
                Floor[] floors = bs.Floors;
                Elevator elevator = bs.Elevator;
                int stepCount = bs.Steps;
                string id = bs.ID;

                // prevent extended sequences
                if (elevator.Target >= floors.Count() || elevator.Target < 0 || floors[elevator.Target].Ignore)
                {
                    continue;
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
                    bs.Print();
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
                        continue;
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
                    bool skip = false;
                    foreach (string m in curFloor.Microchips)
                    {
                        if (!skip && !curFloor.Generators.Contains(m))
                        {
                            skip = true;
                            break;
                        }
                    }
                    if (skip)
                    {
                        continue;
                    }
                }

                // get a list of all possible elevator rides, try them out
                List<Elevator> attempts = new List<Elevator>();
                elevator.GetAllPossibleAttempts(floors, curFloor, ref attempts);
                int attemptCount = 0;
                foreach (Elevator attempt in attempts)
                {
                    next.Enqueue(new BuildingState(floors.Select(f => new Floor(f)).ToArray(), attempt, stepCount + 1, $"{id}.{attemptCount++}"));
                }
            }

            return int.MaxValue;
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
            Queue<BuildingState> remainingStates = new Queue<BuildingState>();
            remainingStates.Enqueue(new BuildingState(floors.Select(f => new Floor(f)).ToArray(), new Elevator(elevator), 0, "0"));
            int minStepCount = SimulateRun(new Dictionary<string, int>(), ref remainingStates);
            return minStepCount.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, new string[] { });

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, new string[] { "elerium", "dilithium" });
    }
}

/*

TODO: still need some optimization in generating next steps, too slow.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AoC._2016
{
    class Day11 : Day
    {
        public Day11() { }
        public override string GetSolutionVersion(Part part)
        {
            switch (part)
            {
                case Part.One:
                    return "v3";
                case Part.Two:
                    return "v3";
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

        private void SetItem(ref ushort floor, bool isChip, uint bit)
        {
            uint mask = (uint)Math.Pow(2, bit);
            if (!isChip)
            {
                mask = mask << 8;
            }
            floor = (ushort)(floor | mask);
        }

        static private bool IsFloorStateValid(ulong floorData)
        {
            bool valid = true;
            byte[] split = BitConverter.GetBytes(floorData);
            for (int i = 0; valid && i + 1 < split.Length; i += 2)
            {
                byte microchips = split[i];
                if (microchips == 0)
                {
                    continue;
                }

                byte generators = split[i + 1];
                if (generators == 0)
                {
                    continue;
                }

                for (int bit = 0; valid && bit < 8; ++bit)
                {
                    byte mask = (byte)(1 << bit);
                    if ((microchips & mask) != 0)
                    {
                        valid &= (((microchips & mask) & generators) != 0 || ((microchips & mask) | ~generators) == 0);
                    }
                }
            }
            return valid;
        }

        private ulong ParseFloors(List<string> inputs, string[] additionalItems)
        {
            int curFloor = 0;
            ushort[] floors = new ushort[inputs.Count];
            Dictionary<string, uint> ids = new Dictionary<string, uint>();
            foreach (string input in inputs)
            {
                string[] split = input.Trim('.').Split(' ', StringSplitOptions.RemoveEmptyEntries);
                foreach (string s in split.Skip(2))
                {
                    string clean = s.Trim(",.".ToCharArray());
                    if (IgnoreWords.Contains(clean))
                    {
                        continue;
                    }

                    string id = s[..3];
                    if (!ids.ContainsKey(id))
                    {
                        ids[id] = (uint)ids.Count;
                    }

                    int idx = clean.IndexOf('-');
                    if (idx >= 0)
                    {
                        SetItem(ref floors[curFloor], true, ids[id]);
                    }
                    else
                    {
                        SetItem(ref floors[curFloor], false, ids[id]);
                    }
                }

                if (curFloor == 0)
                {
                    foreach (string s in additionalItems)
                    {
                        string id = s[..3];
                        ids[id] = (uint)ids.Count;
                        SetItem(ref floors[curFloor], true, ids[id]);
                        SetItem(ref floors[curFloor], false, ids[id]);
                    }
                }
                ++curFloor;
            }

            return (ulong)floors[3] << 48 | (ulong)floors[2] << 32 | (ulong)floors[1] << 16 | (ulong)floors[0];
        }

        static private void MoveItem(ref ulong currentFloorState, int bit, uint elevator, bool moveUp)
        {
            // clear source bit
            int sourceBit = (int)(16 * elevator + bit);
            currentFloorState &= ~((ulong)((long)1 << sourceBit));

            //set target bit
            uint targetElevator = elevator + (uint)(moveUp ? 1 : -1);
            int targetBit = (int)(16 * targetElevator + bit);
            currentFloorState |= (ulong)((long)1 << targetBit);
        }

        private record FloorState(ulong Data, uint Elevator, uint Steps, string History)
        {
            public List<FloorState> GetAllPossibleNextStates()
            {
                List<FloorState> nextStates = new List<FloorState>();
                int elevatorBitShift = (int)(16 * Elevator);
                ushort currentFloor = (ushort)(Data >> elevatorBitShift);
                int curHistoryIdx = 0;

                // only check one pair of matching microchip to generator
                byte microchips = (byte)currentFloor;
                byte generators = (byte)(currentFloor >> 8);
                byte matchingPairs = (byte)(microchips & generators);
                ushort matchingBits = (ushort)((ushort)(matchingPairs << 8) | (ushort)matchingPairs);
                bool foundMatch = false;

                for (int bit1 = 0; bit1 < 16; ++bit1)
                {
                    ushort mask1 = (ushort)(1 << bit1);
                    if ((mask1 & currentFloor) == 0)
                    {
                        continue;
                    }

                    if ((mask1 & matchingBits) != 0)
                    {
                        if (foundMatch)
                        {
                            continue;
                        }
                        foundMatch = true;
                    }

                    // go up
                    if (Elevator < 3)
                    {
                        ulong oneItemMovedFloorState = Data;
                        MoveItem(ref oneItemMovedFloorState, bit1, Elevator, true);

                        for (int bit2 = bit1 + 1; bit2 < 16; ++bit2)
                        {
                            if (bit1 == bit2)
                            {
                                continue;
                            }

                            ulong twoItemMovedFloorState = oneItemMovedFloorState;
                            ushort mask2 = (ushort)(1 << bit2);
                            if ((mask2 & currentFloor) == 0)
                            {
                                continue;
                            }

                            MoveItem(ref twoItemMovedFloorState, bit2, Elevator, true);

                            // move 2 item2 up
                            if (IsFloorStateValid(twoItemMovedFloorState))
                            {
                                nextStates.Add(new FloorState(twoItemMovedFloorState, Elevator + 1, Steps + 1, $"{History}.{curHistoryIdx++}"));
                            }
                        }

                        // move 1 item up
                        if (IsFloorStateValid(oneItemMovedFloorState))
                        {
                            nextStates.Add(new FloorState(oneItemMovedFloorState, Elevator + 1, Steps + 1, $"{History}.{curHistoryIdx++}"));
                        }
                    }

                    // go down
                    if (Elevator > 0)
                    {
                        // ignore floors that are already empty
                        {
                            int floorDownBitShift = (int)(16 * (Elevator - 1));
                            ushort floorDown = (ushort)(Data >> elevatorBitShift);
                            if (floorDown == 0)
                            {
                                continue;
                            }
                        }

                        ulong oneItemMovedFloorState = Data;
                        MoveItem(ref oneItemMovedFloorState, bit1, Elevator, false);

                        for (int bit2 = bit1 + 1; bit2 < 16; ++bit2)
                        {
                            if (bit1 == bit2)
                            {
                                continue;
                            }

                            ulong twoItemMovedFloorState = oneItemMovedFloorState;
                            ushort mask2 = (ushort)(1 << bit2);
                            if ((mask2 & currentFloor) == 0)
                            {
                                continue;
                            }

                            MoveItem(ref twoItemMovedFloorState, bit2, Elevator, false);

                            // move 2 item2 up
                            if (IsFloorStateValid(twoItemMovedFloorState))
                            {
                                nextStates.Add(new FloorState(twoItemMovedFloorState, Elevator - 1, Steps + 1, $"{History}.{curHistoryIdx++}"));
                            }
                        }

                        // move 1 item up
                        if (IsFloorStateValid(oneItemMovedFloorState))
                        {
                            nextStates.Add(new FloorState(oneItemMovedFloorState, Elevator - 1, Steps + 1, $"{History}.{curHistoryIdx++}"));
                        }
                    }

                }
                return nextStates;
            }

            public ulong GetCycleState()
            {
                Func<byte, byte> CountBits = (b) =>
                {
                    byte bits = 0;
                    for (byte i = 0, bit = 1; i < 8; ++i, bit = (byte)(bit << 1))
                    {
                        bits += (byte)((b & bit) == 0 ? 0 : 1);
                    }
                    return bits;
                };
                ulong cycle = Elevator << 4;
                byte[] split = BitConverter.GetBytes(Data);
                for (int i = 0; i + 1 < 8; i += 2)
                {
                    cycle |= ((byte)(split[i] & split[i + 1]));
                    cycle = cycle << 4;
                    cycle |= ((byte)(split[i] & ~split[i + 1]));
                    cycle = cycle << 4;
                    cycle |= ((byte)(~split[i] & split[i + 1]));
                    cycle = cycle << 4;
                }
                return cycle;
            }

            public void Print(Action<string> PrintFunc)
            {
                StringBuilder sb = new StringBuilder();
                PrintFunc($"{Steps} @ {History}");
                for (int i = 3; i >= 0; --i)
                {
                    sb.AppendFormat("F{0} {1}", i + 1, Elevator == i ? "E  " : ".  ");

                    ushort curFloor = (ushort)(Data >> (i * 16));
                    byte curG = (byte)((curFloor >> 8) & 0xff);
                    byte curM = (byte)(curFloor & 0xff);
                    for (int j = 0; j < 8; ++j)
                    {
                        byte mask = (byte)(1 << j);
                        sb.AppendFormat("{0}{1}", (curG & mask) != 0 ? $"{j}G " : ".  ", (curM & mask) != 0 ? $"{j}M " : ".  ");
                    }
                    PrintFunc(sb.ToString());
                    sb.Clear();
                }
                PrintFunc("");
            }
        }

        private void DebugPrintFloors(FloorState fs)
        {
            fs.Print(DebugWriteLine);
        }

        private uint SimulateRun(ulong initialFloorState)
        {
            Queue<FloorState> remainingStates = new Queue<FloorState>();
            remainingStates.Enqueue(new FloorState(initialFloorState, 0, 0, "0"));

            Dictionary<int, uint> visited = new Dictionary<int, uint>();
            Dictionary<ulong, uint> cycles = new Dictionary<ulong, uint>();
            uint curSteps = 0;
            while (remainingStates.Count > 0)
            {
                FloorState curState = remainingStates.Dequeue();
                if (curState.Steps > curSteps)
                {
                    curSteps = curState.Steps;
                    DebugWriteLine($"@ {curState.Steps} steps");
                }

                // check win condition
                if ((curState.Data << 16) == 0)
                {
                    return curState.Steps;
                }
                //curState.Print(DebugWriteLine);

                visited[curState.GetHashCode()] = curState.Steps;
                cycles[curState.GetCycleState()] = curState.Steps;

                // get list of next possibles
                foreach (FloorState nextState in curState.GetAllPossibleNextStates())
                {
                    if (visited.ContainsKey(nextState.GetHashCode()) && visited[nextState.GetHashCode()] <= nextState.Steps)
                    {
                        continue;
                    }

                    ulong cycleState = nextState.GetCycleState();
                    if (cycles.ContainsKey(cycleState) && cycles[cycleState] <= nextState.Steps)
                    {
                        continue;
                    }
                    remainingStates.Enqueue(nextState);
                }
            }

            return uint.MaxValue;
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, string[] additionalItems)
        {
            ulong initialFloorState = ParseFloors(inputs, additionalItems);
            uint minStepCount = SimulateRun(initialFloorState);
            return minStepCount.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, new string[] { });

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, new string[] { "elerium", "dilithium" });
    }
}
*/