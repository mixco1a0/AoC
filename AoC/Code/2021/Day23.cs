using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AoC.Core;

namespace AoC._2021
{
    class Day23 : Day
    {
        public Day23() { }

        public override string GetSolutionVersion(Part part)
        {
            switch (part)
            {
                // case Part.One:
                //     return "v2";
                // case Part.Two:
                //     return "v1";
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
                Output = "12521",
                RawInput =
@"#############
#...........#
###B#C#B#D###
  #A#D#C#A#
  #########"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.Two,
                Output = "44169",
                RawInput =
@"#############
#...........#
###B#C#B#D###
  #A#D#C#A#
  #########"
            });
            return testData;
        }

        private static readonly Dictionary<char, int> AmphipodEnergy = new Dictionary<char, int>() { { 'A', 1 }, { 'B', 10 }, { 'C', 100 }, { 'D', 1000 } };
        private static readonly Dictionary<int, char> RoomToAmphipod = new Dictionary<int, char>() { { 0, 'A' }, { 1, 'B' }, { 2, 'C' }, { 3, 'D' } };
        private static readonly Dictionary<char, int> AmphipodToRoom = RoomToAmphipod.ToDictionary(atr => atr.Value, atr => atr.Key);
        private static readonly Dictionary<int, int> RoomToHall = new Dictionary<int, int>() { { 0, 2 }, { 1, 4 }, { 2, 6 }, { 3, 8 } };

        private class BurrowState
        {
            public static char Empty { get => '.'; }
            public string Id { get => GenerateId(true); }
            public char[] Hallway { get; init; }
            public int RoomSize { get; init; }
            public char[][] Rooms { get; init; }
            public int Energy { get; private set; }

            public BurrowState(int roomSize)
            {
                Hallway = new char[11];
                for (int i = 0; i < Hallway.Length; ++i)
                {
                    Hallway[i] = Empty;
                }
                RoomSize = roomSize;
                Rooms = new char[4][];
                for (int i = 0; i < Rooms.Length; ++i)
                {
                    Rooms[i] = new char[RoomSize];
                    for (int j = 0; j < RoomSize; ++j)
                    {
                        Rooms[i][j] = Empty;
                    }
                }
                Energy = 0;
            }

            private BurrowState(BurrowState other)
            {
                Hallway = new char[11];
                for (int i = 0; i < Hallway.Length; ++i)
                {
                    Hallway[i] = other.Hallway[i];
                }
                RoomSize = other.RoomSize;
                Rooms = new char[4][];
                for (int i = 0; i < Rooms.Length; ++i)
                {
                    Rooms[i] = new char[RoomSize];
                    for (int j = 0; j < RoomSize; ++j)
                    {
                        Rooms[i][j] = other.Rooms[i][j];
                    }
                }
                Energy = other.Energy;
            }

            public static BurrowState Parse(List<string> roomSlices)
            {
                BurrowState bs = new BurrowState(roomSlices.Count);
                for (int i = 0; i < roomSlices.Count; ++i)
                {
                    char[] roomSlice = roomSlices[i].Split("# ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(s => s[0]).ToArray();
                    for (int j = 0; j < roomSlice.Length; ++j)
                    {
                        bs.Rooms[j][i] = roomSlice[j];
                    }
                }
                return bs;
            }

            private bool CanEnterRoom(char amphipod, int roomIdx, out int roomSlot)
            {
                char[] curRoom = Rooms[roomIdx];
                bool canEnter = true;
                roomSlot = -1;
                for (int i = curRoom.Length - 1; canEnter && i >= 0; --i)
                {
                    if (curRoom[i] == Empty)
                    {
                        roomSlot = i;
                        break;
                    }
                    else if (curRoom[i] == amphipod)
                    {
                        // do nothing
                    }
                    else
                    {
                        canEnter = false;
                    }
                }
                return canEnter;
            }

            private bool CanLeaveRoom(int roomIdx, out int roomSlot)
            {
                char targetAmphipod = RoomToAmphipod[roomIdx];
                char[] curRoom = Rooms[roomIdx];
                bool canLeave = false;
                roomSlot = -1;
                for (int i = curRoom.Length - 1; i >= 0; --i)
                {
                    canLeave |= (curRoom[i] != targetAmphipod && curRoom[i] != Empty);
                    if (canLeave && curRoom[i] != Empty)
                    {
                        roomSlot = i;
                    }
                }
                return canLeave;
            }

            private List<BurrowState> GetNextHallwayStates()
            {
                List<BurrowState> nextStates = new List<BurrowState>();
                if (Hallway.Any(h => h != Empty))
                {
                    Dictionary<int, char> hallwayPairs = Hallway.Select((h, i) => new { idx = i, val = h }).Where(hi => hi.val != Empty).ToDictionary(hi => hi.idx, hi => hi.val);
                    foreach (KeyValuePair<int, char> pair in hallwayPairs)
                    {
                        int hallIdx = pair.Key;
                        char amphipod = pair.Value;

                        // is the amphipods room empty, or housing same amphipods?
                        int roomIdx = AmphipodToRoom[amphipod];
                        int roomToHall = RoomToHall[roomIdx];
                        if (CanEnterRoom(amphipod, roomIdx, out int roomSlot))
                        {
                            // is there anyone in the way?
                            bool pathClear = true;
                            int start = Math.Min(hallIdx, roomToHall);
                            int end = Math.Max(hallIdx, roomToHall);
                            for (int i = start; pathClear && i <= end; ++i)
                            {
                                if (i == hallIdx)
                                {
                                    continue;
                                }
                                if (Hallway[i] != Empty)
                                {
                                    pathClear = false;
                                }
                            }

                            if (pathClear)
                            {
                                BurrowState sendToRoom = new BurrowState(this);
                                end += roomSlot + 1;
                                sendToRoom.Rooms[roomIdx][roomSlot] = amphipod;
                                sendToRoom.Hallway[hallIdx] = Empty;
                                sendToRoom.Energy += (end - start) * AmphipodEnergy[amphipod];
                                nextStates.Add(sendToRoom);
                            }
                        }
                    }
                }
                return nextStates;
            }

            private List<BurrowState> GetNextRoomStates()
            {
                List<BurrowState> nextStates = new List<BurrowState>();
                // send someone in to the hallway
                for (int i = 0; i < Rooms.Length; ++i)
                {
                    // next state sends them out and to the left, then more left, more left, ...
                    char targetAmphipod = RoomToAmphipod[i];

                    if (CanLeaveRoom(i, out int roomSlot))
                    {
                        char amphipod = Rooms[i][roomSlot];

                        // send the amphipod into the hall first
                        BurrowState sendToHall = new BurrowState(this);
                        sendToHall.Rooms[i][roomSlot] = Empty;
                        sendToHall.Hallway[RoomToHall[i]] = amphipod;
                        sendToHall.Energy += AmphipodEnergy[amphipod] * (1 + roomSlot);

                        int baseHallway = RoomToHall[i];

                        Action<int, Func<int, bool>, Func<int, int>> walkHall = (int start, Func<int, bool> check, Func<int, int> move) =>
                        {
                            int moveCount = 0;
                            for (int h = start; check(h); h = move(h))
                            {
                                if (sendToHall.Hallway[h] == Empty)
                                {
                                    if (RoomToHall.Values.Contains(h))
                                    {
                                        ++moveCount;
                                        continue;
                                    }

                                    BurrowState nextLeft = new BurrowState(sendToHall);
                                    nextLeft.Hallway[h] = amphipod;
                                    nextLeft.Hallway[baseHallway] = Empty;
                                    nextLeft.Energy += AmphipodEnergy[amphipod] * ++moveCount;
                                    nextStates.Add(nextLeft);
                                }
                                else
                                {
                                    break;
                                }
                            }
                        };

                        // go left
                        walkHall(baseHallway - 1, h => h >= 0, h => h - 1);

                        // go right
                        walkHall(baseHallway + 1, h => h < Hallway.Length, h => h + 1);
                    }
                }
                return nextStates;
            }

            public List<BurrowState> GetNextStates()
            {
                List<BurrowState> nextStates = GetNextHallwayStates();
                if (nextStates.Any())
                {
                    return nextStates;
                }
                return GetNextRoomStates();
            }

            public bool IsComplete()
            {
                bool isComplete = !Hallway.Any(h => h != Empty);
                for (int i = 0; isComplete && i < Rooms.Length; ++i)
                {
                    isComplete = Rooms[i].Where(a => a == RoomToAmphipod[i]).Count() == RoomSize;
                }
                return isComplete;
            }

            private string GenerateId(bool simple)
            {
                // generate a ulong instead of a string
                StringBuilder sb = new StringBuilder();
                if (simple)
                {
                    sb.Append(Hallway);
                }
                else
                {
                    IEnumerable<char> hallString = Hallway.Select((h, i) => { if (h == Empty && RoomToHall.ContainsValue(i)) { return $"{RoomToHall.Single(rh => rh.Value == i).Key}".First(); } else { return h; } });
                    sb.Append(string.Join(string.Empty, hallString));
                }
                int curRoom = 0;
                foreach (char[] room in Rooms)
                {
                    if (simple)
                    {
                        for (int i = 0; i < RoomSize; ++i)
                        {
                            sb.Append(room[i]);
                        }
                    }
                    else
                    {
                        string prev = sb.ToString();
                        sb.Clear();
                        bool isFinal = true;
                        for (int i = RoomSize - 1; i >= 0; --i)
                        {
                            isFinal &= (room[i] == RoomToAmphipod[curRoom]);
                            sb.Append(isFinal ? char.ToLower(room[i]) : room[i]);
                        }
                        string cur = string.Join("", sb.ToString().Reverse());
                        sb.Clear();
                        sb.Append(prev);
                        sb.Append('|');
                        sb.Append(cur);
                    }
                    ++curRoom;
                }
                return sb.ToString();
            }

            public string Print(Action<string> printFunc)
            {
                printFunc("#############");
                StringBuilder sb = new StringBuilder();
                sb.Append('#');
                sb.Append(string.Join("", Hallway));
                sb.Append("#");
                printFunc(sb.ToString());
                for (int i = 0; i < RoomSize; ++i)
                {
                    sb.Clear();
                    sb.Append(i == 0 ? "###" : "  #");
                    foreach (char[] room in Rooms)
                    {
                        sb.Append(room[i]);
                        sb.Append("#");
                    }
                    sb.Append(i == 0 ? "##" : "  ");
                    printFunc(sb.ToString());
                }
                printFunc("  #########  ");
                return sb.ToString();
            }

            public override string ToString()
            {
                return $"{GenerateId(false)} ({Energy,-6})";
            }
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, List<string> additionalInput)
        {
            List<string> fullInput = new List<string>();
            fullInput.Add(inputs[2]);
            fullInput.AddRange(additionalInput);
            fullInput.Add(inputs[3]);

            PriorityQueue<BurrowState, int> burrowStates = new PriorityQueue<BurrowState, int>();
            burrowStates.Enqueue(BurrowState.Parse(fullInput), 0);
            HashSet<string> visited = new HashSet<string>();
            while (burrowStates.Count > 0)
            {
                BurrowState bs = burrowStates.Dequeue();

                // unique checks
                string bsId = bs.Id;
                if (visited.Contains(bsId))
                {
                    continue;
                }
                visited.Add(bsId);

                // win condition
                if (bs.IsComplete())
                {
                    return bs.Energy.ToString();
                }

                // next possible states
                List<BurrowState> nextStates = bs.GetNextStates();
                foreach (BurrowState ns in nextStates)
                {
                    burrowStates.Enqueue(ns, ns.Energy);
                }
            }
            return string.Empty;
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, new List<string>());

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, new List<string>() { "  #D#C#B#A#  ", "  #D#B#A#C#  " });
    }
}