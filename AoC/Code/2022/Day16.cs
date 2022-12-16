using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2022
{
    class Day16 : Core.Day
    {
        public Day16() { }

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
                Output = "1651",
                RawInput =
@"Valve AA has flow rate=0; tunnels lead to valves DD, II, BB
Valve BB has flow rate=13; tunnels lead to valves CC, AA
Valve CC has flow rate=2; tunnels lead to valves DD, BB
Valve DD has flow rate=20; tunnels lead to valves CC, AA, EE
Valve EE has flow rate=3; tunnels lead to valves FF, DD
Valve FF has flow rate=0; tunnels lead to valves EE, GG
Valve GG has flow rate=0; tunnels lead to valves FF, HH
Valve HH has flow rate=22; tunnel leads to valve GG
Valve II has flow rate=0; tunnels lead to valves AA, JJ
Valve JJ has flow rate=21; tunnel leads to valve II"
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

        private class Room
        {
            public string Id { get; set; }
            public int Rate { get; set; }
            public int OpenedAt { get; set; }
            public Dictionary<string, int> Rooms { get; set; }

            public Room()
            {
                Id = string.Empty;
                Rate = 0;
                OpenedAt = 0;
                Rooms = new Dictionary<string, int>();
            }

            public int GetPressure()
            {
                return OpenedAt * Rate;
            }

            public static Room Parse(string input)
            {
                string[] split = input.Split(" =;,".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                Room room = new Room();
                room.Id = split[1];
                room.Rate = int.Parse(split[5]);
                foreach (var v in split.Skip(10))
                {
                    room.Rooms.Add(v, 1);
                }
                return room;
            }

            public override string ToString()
            {
                return $"{Id} @ {Rate} -> {string.Join(",", Rooms)}";
            }
        }

        public class IdToNew : Base.KeyVal<string, bool>
        {
            public IdToNew() : base()
            {

            }

            public IdToNew(string id, bool isNew) : base(id, isNew)
            {

            }
        }

        private class RoomState
        {
            public int CurTime { get; set; }
            public string CurId { get; set; }
            public Dictionary<string, int> Opened { get; set; }
            public HashSet<string> Closed { get; set; }
            public List<IdToNew> History { get; set; }

            public RoomState(Dictionary<string, Room> rooms, int time, Room curRoom, bool open)
            {
                CurTime = time;
                CurId = curRoom.Id;
                Opened = new Dictionary<string, int>();
                Closed = new HashSet<string>(rooms.Keys);
                History = new List<IdToNew>();
                History.Add(new IdToNew(curRoom.Id, open));

                if (open && Closed.Contains(CurId))
                {
                    History.Add(new IdToNew(CurId, true));
                    Closed.Remove(CurId);
                    Opened.Add(CurId, CurTime);
                }
                else
                {
                    History.Add(new IdToNew(CurId, false));
                }
            }

            public RoomState(RoomState prev, int time, Room nextRoom, int stepsToRoom, bool open)
            {
                CurId = nextRoom.Id;
                Opened = new Dictionary<string, int>(prev.Opened);
                Closed = new HashSet<string>(prev.Closed);
                History = new List<IdToNew>(prev.History);

                if (open && Closed.Contains(CurId))
                {
                    CurTime = time - stepsToRoom - 1;
                    History.Add(new IdToNew(CurId, true));
                    Closed.Remove(CurId);
                    Opened.Add(CurId, CurTime);
                }
                else
                {
                    CurTime = time - stepsToRoom;
                    History.Add(new IdToNew(CurId, false));
                }
            }

            public bool HasCycle()
            {
                HashSet<string> cycles = new HashSet<string>();
                foreach (var pair in History)
                {
                    if (!pair.Val)
                    {
                        if (cycles.Contains(pair.Key))
                        {
                            return true;
                        }

                        cycles.Add(pair.Key);
                    }
                    else if (pair.Val)
                    {
                        cycles.Clear();
                    }
                }
                return false;
            }

            public override string ToString()
            {
                return $"{CurTime} | {string.Join(" ", History.Select(p => $"{string.Format("{0}", p.Val ? "+" : "-")}{p.First}"))}";
            }
        }

        private int GetPressureReleased(Dictionary<string, Room> rooms, RoomState rs)
        {
            int total = 0;
            foreach (var pair in rs.Opened)
            {
                total += rooms[pair.Key].Rate * pair.Value;
            }
            return total;
        }

        private int GetPotentialReleased(Dictionary<string, Room> rooms, RoomState rs)
        {
            int total = 0;
            foreach (string id in rs.Closed)
            {
                total += rooms[id].Rate * (rs.CurTime - 2);
            }
            return total;
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables)
        {
            Dictionary<string, Room> rooms = inputs.Select(Room.Parse).ToDictionary(r => r.Id, r => r);
            // try to remove 0 rooms
            List<string> zeroRooms = rooms.Where(pair => pair.Value.Rate == 0 && pair.Value.Id != "AA").Select(pair => pair.Key).ToList();

            foreach (string zeroRoom in zeroRooms)
            {
                Room zero = rooms[zeroRoom];
                Dictionary<string, int> nextRooms = zero.Rooms;
                foreach (var pair in nextRooms)
                {
                    Room cur = rooms[pair.Key];
                    int distCached = nextRooms[cur.Id];
                    cur.Rooms.Remove(zeroRoom);
                    foreach (var dup in nextRooms)
                    {
                        if (dup.Key != cur.Id)
                        {
                            int dist = nextRooms[dup.Key];
                            cur.Rooms[dup.Key] = dist + distCached;
                        }
                    }
                }
                rooms.Remove(zeroRoom);
            }

            var starters = new Dictionary<string, int>(rooms["AA"].Rooms);
            {
                string zeroRoom = "AA";
                Room zero = rooms[zeroRoom];
                Dictionary<string, int> nextRooms = zero.Rooms;
                foreach (var pair in nextRooms)
                {
                    Room cur = rooms[pair.Key];
                    int distCached = nextRooms[cur.Id];
                    cur.Rooms.Remove(zeroRoom);
                    foreach (var dup in nextRooms)
                    {
                        if (dup.Key != cur.Id)
                        {
                            int dist = nextRooms[dup.Key];
                            cur.Rooms[dup.Key] = dist + distCached;
                        }
                    }
                }
                rooms.Remove(zeroRoom);
            }

            PriorityQueue<RoomState, int> roomStates = new PriorityQueue<RoomState, int>(Comparer<int>.Create((x, y) => y - x));
            foreach (var s in starters)
            {
                RoomState rs = new RoomState(rooms, 30 - s.Value - 1, rooms[s.Key], true);
                roomStates.Enqueue(rs, GetPressureReleased(rooms, rs) + GetPotentialReleased(rooms, rs));
                rs = new RoomState(rooms, 30 - s.Value, rooms[s.Key], false);
                roomStates.Enqueue(rs, GetPressureReleased(rooms, rs) + GetPotentialReleased(rooms, rs));
            }
            HashSet<string> visited = new HashSet<string>();
            while (roomStates.Count > 0)
            {
                RoomState rs = roomStates.Dequeue();

                // win condition
                if (rs.Closed.Count == 0)
                {
                    return GetPressureReleased(rooms, rs).ToString();
                }

                // next possible states
                foreach (var pair in rooms[rs.CurId].Rooms)
                {
                    // move to and open

                    RoomState next = new RoomState(rs, rs.CurTime, rooms[pair.Key], pair.Value, true);
                    if (rooms[next.CurId].Rate != 0)
                    {
                        if (!next.HasCycle())
                        {
                            roomStates.Enqueue(next, GetPressureReleased(rooms, next) + GetPotentialReleased(rooms, next));
                        }
                    }

                    // move to
                    if (next.Opened.Count != rs.Opened.Count || rooms[next.CurId].Rate == 0)
                    {
                        next = new RoomState(rs, rs.CurTime, rooms[pair.Key], pair.Value, false);
                        if (!next.HasCycle())
                        {
                            roomStates.Enqueue(next, GetPressureReleased(rooms, next) + GetPotentialReleased(rooms, next));
                        }
                    }
                }
            }


            return string.Empty;
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);
    }
}