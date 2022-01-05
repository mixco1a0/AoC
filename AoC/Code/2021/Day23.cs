using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
                //     return "v1";
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
                Output = "",
                RawInput =
@""
            });
            return testData;
        }

        private static readonly Dictionary<char, int> AmphipodEnergy = new Dictionary<char, int>() { { 'A', 1 }, { 'B', 10 }, { 'C', 100 }, { 'D', 1000 } };
        private static readonly Dictionary<int, char> RoomToAmphipod = new Dictionary<int, char>() { { 0, 'A' }, { 1, 'B' }, { 2, 'C' }, { 3, 'D' } };
        private static readonly Dictionary<char, int> AmphipodToRoom = RoomToAmphipod.ToDictionary(atr => atr.Value, atr => atr.Key);

        private class BurrowState
        {
            public static char Empty { get => '.'; }
            public char[] Hallway { get; init; }
            public char[][] Rooms { get; init; }
            public int Energy { get; private set; }

            private static readonly Dictionary<int, int> RoomToHall = new Dictionary<int, int>() { { 0, 2 }, { 1, 4 }, { 2, 6 }, { 3, 8 } };

            public BurrowState(int energy)
            {
                Hallway = new char[11];
                for (int i = 0; i < Hallway.Length; ++i)
                {
                    Hallway[i] = Empty;
                }
                Rooms = new char[4][];
                for (int i = 0; i < Rooms.Length; ++i)
                {
                    Rooms[i] = new char[2];
                    Rooms[i][0] = Empty;
                    Rooms[i][1] = Empty;
                }
                Energy = energy;
            }

            private BurrowState(BurrowState other)
            {
                Hallway = new char[11];
                for (int i = 0; i < Hallway.Length; ++i)
                {
                    Hallway[i] = other.Hallway[i];
                }
                Rooms = new char[4][];
                for (int i = 0; i < Rooms.Length; ++i)
                {
                    Rooms[i] = new char[2];
                    Rooms[i][0] = other.Rooms[i][0];
                    Rooms[i][1] = other.Rooms[i][1];
                }
                Energy = other.Energy;
            }

            public static BurrowState Parse(string rooms1, string rooms2)
            {
                BurrowState bs = new BurrowState(0);
                char[] rooms1Amphipod = rooms1.Split('#', StringSplitOptions.RemoveEmptyEntries).Select(s => s[0]).ToArray();
                char[] rooms2Amphipod = rooms2.Split("# ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(s => s[0]).ToArray();
                for (int i = 0; i < bs.Rooms.Length; ++i)
                {
                    bs.Rooms[i][0] = rooms1Amphipod[i];
                    bs.Rooms[i][1] = rooms2Amphipod[i];
                }
                return bs;
            }

            public List<BurrowState> GetNextStates()
            {
                List<BurrowState> nextStates = new List<BurrowState>();
                if (Hallway.Any(h => h != Empty))
                {
                    Dictionary<int, char> hallwayPairs = Hallway.Select((h, i) => new { idx = i, val = h }).Where(hi => hi.val != Empty).ToDictionary(hi => hi.idx, hi => hi.val);
                    foreach (KeyValuePair<int, char> pair in hallwayPairs)
                    {
                        // is the amphipods room empty, or housing same amphipods?
                        int roomIdx = AmphipodToRoom[pair.Value];
                        int roomToHall = RoomToHall[roomIdx];
                        if (Rooms[roomIdx][0] == Empty && (Rooms[roomIdx][1] == Empty || Rooms[roomIdx][1] == pair.Value))
                        {
                            // is there anyone in the way?
                            bool pathClear = true;
                            int start = Math.Min(pair.Key, roomToHall);
                            int end = Math.Max(pair.Key, roomToHall);
                            for (int i = start; pathClear && i <= end; ++i)
                            {
                                if (i == pair.Key)
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
                                if (Rooms[roomIdx][1] == Empty)
                                {
                                    // move to second slot
                                    sendToRoom.Rooms[roomIdx][1] = pair.Value;
                                    end += 2;
                                }
                                else
                                {
                                    // move to first slot
                                    sendToRoom.Rooms[roomIdx][0] = pair.Value;
                                    ++end;
                                }
                                sendToRoom.Hallway[pair.Key] = Empty;
                                sendToRoom.Energy += (end - start) * AmphipodEnergy[pair.Value];
                                nextStates.Add(sendToRoom);
                            }
                        }
                    }

                    // if (nextStates.Any())
                    // {
                    //     return nextStates;
                    // }
                }
                // send someone in to the hallway
                for (int i = 0; i < Rooms.Length; ++i)
                {
                    // next state sends them out and to the left, then more left, more left, ...
                    char targetAmphipod = RoomToAmphipod[i];
                    char amphipod = Empty;
                    int roomSlot = 0;
                    if (Rooms[i][0] != Empty && (Rooms[i][0] != targetAmphipod || Rooms[i][1] != targetAmphipod))
                    {
                        amphipod = Rooms[i][0];
                    }
                    else if (Rooms[i][1] != Empty && Rooms[i][1] != targetAmphipod)
                    {
                        amphipod = Rooms[i][1];
                        roomSlot = 1;
                    }

                    if (amphipod != Empty)
                    {
                        // send the amphipod into the hall first
                        BurrowState sendToHall = new BurrowState(this);
                        sendToHall.Rooms[i][roomSlot] = Empty;
                        sendToHall.Hallway[RoomToHall[i]] = amphipod;
                        sendToHall.Energy += AmphipodEnergy[amphipod] * (1 + roomSlot);

                        int baseHallway = RoomToHall[i];

                        // go left
                        int moveCount = 0;
                        for (int h = baseHallway - 1; h >= 0; --h)
                        {
                            if (sendToHall.Hallway[h] == Empty)
                            {
                                if (RoomToHall.Values.Contains(h))
                                {
                                    ++moveCount;
                                    // if stopped above the hallway to own room, check if it can fit
                                    int targetRoom = RoomToHall.First(rth => rth.Value == h).Key;
                                    if (AmphipodToRoom[amphipod] == targetRoom)
                                    {
                                        if (Rooms[targetRoom][0] == Empty && (Rooms[targetRoom][1] == Empty || Rooms[targetRoom][1] == amphipod))
                                        {
                                            ++moveCount;
                                            roomSlot = 0;
                                            if (Rooms[targetRoom][1] == Empty)
                                            {
                                                // move down 2
                                                ++moveCount;
                                                roomSlot = 1;
                                            }
                                            BurrowState goHome = new BurrowState(sendToHall);
                                            goHome.Rooms[targetRoom][roomSlot] = amphipod;
                                            goHome.Hallway[baseHallway] = Empty;
                                            goHome.Energy += AmphipodEnergy[amphipod] * moveCount;
                                            nextStates.Add(goHome);
                                            break;
                                        }
                                    }
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

                        // go right
                        moveCount = 0;
                        for (int h = baseHallway + 1; h < Hallway.Length; ++h)
                        {
                            if (sendToHall.Hallway[h] == Empty)
                            {
                                if (RoomToHall.Values.Contains(h))
                                {
                                    ++moveCount;
                                    // if stopped above the hallway to own room, check if it can fit
                                    int targetRoom = RoomToHall.First(rth => rth.Value == h).Key;
                                    if (AmphipodToRoom[amphipod] == targetRoom)
                                    {
                                        if (Rooms[targetRoom][0] == Empty && (Rooms[targetRoom][1] == Empty || Rooms[targetRoom][1] == amphipod))
                                        {
                                            ++moveCount;
                                            roomSlot = 0;
                                            if (Rooms[targetRoom][1] == Empty)
                                            {
                                                // move down 2
                                                ++moveCount;
                                                roomSlot = 1;
                                            }
                                            BurrowState goHome = new BurrowState(sendToHall);
                                            goHome.Rooms[targetRoom][roomSlot] = amphipod;
                                            goHome.Hallway[baseHallway] = Empty;
                                            goHome.Energy += AmphipodEnergy[amphipod] * moveCount;
                                            nextStates.Add(goHome);
                                            break;
                                        }
                                    }
                                    continue;
                                }

                                BurrowState nextRight = new BurrowState(sendToHall);
                                nextRight.Hallway[h] = amphipod;
                                nextRight.Hallway[baseHallway] = Empty;
                                nextRight.Energy += AmphipodEnergy[amphipod] * ++moveCount;
                                nextStates.Add(nextRight);
                            }
                            else
                            {
                                break;
                            }
                        }

                    }
                }
                return nextStates;
            }

            public bool IsComplete()
            {
                bool isComplete = !Hallway.Any(h => h != Empty);
                for (int i = 0; isComplete && i < Rooms.Length; ++i)
                {
                    isComplete = (Rooms[i][0] == RoomToAmphipod[i] && Rooms[i][1] == RoomToAmphipod[i]);
                }
                return isComplete;
            }

            public string Id(bool simple)
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
                        sb.Append(room[0]);
                        sb.Append(room[1]);
                    }
                    else
                    {
                        sb.Append('_');
                        if (room[0] == RoomToAmphipod[curRoom] && room[1] == RoomToAmphipod[curRoom])
                        {
                            sb.Append('#');
                            sb.Append('#');
                        }
                        else if (room[1] == RoomToAmphipod[curRoom])
                        {
                            sb.Append(room[0]);
                            sb.Append('#');
                        }
                        else
                        {
                            sb.Append(room[0]);
                            sb.Append(room[1]);
                        }
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
                sb.Clear();
                sb.Append("###");
                foreach (char[] room in Rooms)
                {
                    sb.Append(room[0]);
                    sb.Append("#");
                }
                sb.Append("##");
                printFunc(sb.ToString());
                sb.Clear();
                sb.Append("  #");
                foreach (char[] room in Rooms)
                {
                    sb.Append(room[1]);
                    sb.Append("#");
                }
                sb.Append("  ");
                printFunc(sb.ToString());
                printFunc("  #########  ");
                return sb.ToString();
            }

            public override string ToString()
            {
                return $"{Id(false)} ({Energy,-6})";
            }
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables)
        {
            PriorityQueue<BurrowState, int> burrowStates = new PriorityQueue<BurrowState, int>();
            burrowStates.Enqueue(BurrowState.Parse(inputs[2], inputs[3]), 0);
            HashSet<string> visited = new HashSet<string>();
            visited.Add(burrowStates.Peek().Id(true));
            Dictionary<string, BurrowState> histories = new Dictionary<string, BurrowState>();
            while (burrowStates.Count > 0)
            {
                BurrowState bs = burrowStates.Dequeue();

                if (bs.IsComplete())
                {
                    BurrowState cur = bs;
                    int step = 0;
                    while (histories.ContainsKey(cur.Id(true)))
                    {
                        DebugWriteLine($"Step:N-({step++,4}) [{cur.Energy,-6}]");
                        cur.Print(DebugWriteLine);
                        DebugWriteLine("");
                        cur = histories[cur.Id(true)];
                    }
                    return bs.Energy.ToString();
                }

                List<BurrowState> nextStates = bs.GetNextStates();
                foreach (BurrowState ns in nextStates)
                {
                    string id = ns.Id(true);
                    if (!visited.Contains(id))
                    {
                        histories[id] = bs;
                        // instead of just energy, also use missing pieces as weights D => 1000 per wrong, C => 100 per wrong, etc
                        visited.Add(id);
                        burrowStates.Enqueue(ns, ns.Energy);
                    }
                }
            }
            return string.Empty;
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);
            // 19349 => too high

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);
    }
}