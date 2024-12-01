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
                case Core.Part.One:
                    return "v2";
                case Core.Part.Two:
                    return "v2";
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
                Output = "1707",
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
            return testData;
        }

        public static int ToId(string id)
        {
            return (id[0] << 8) | id[1];
        }

        public static string FromId(int id)
        {
            return $"{id >> 8}{id & 0xf}";
        }

        private class Room
        {
            public int Id { get; set; }
            public long Rate { get; set; }
            public Dictionary<int, long> Rooms { get; set; }

            public Room()
            {
                Id = 0x00;
                Rate = 0;
                Rooms = new Dictionary<int, long>();
            }

            public Room(Room other)
            {
                Id = other.Id;
                Rate = other.Rate;
                Rooms = new Dictionary<int, long>(other.Rooms);
            }

            public static Room Parse(string input)
            {
                string[] split = input.Split(" =;,".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                Room room = new Room();
                room.Id = ToId(split[1]);
                room.Rate = long.Parse(split[5]);
                foreach (string v in split.Skip(10))
                {
                    int id = ToId(v);
                    room.Rooms.Add(id, 1);
                }
                return room;
            }

            public override string ToString()
            {
                return $"{FromId(Id)} @ {Rate} -> {string.Join(",", Rooms)}";
            }
        }

        private class RoomPath
        {
            public int Prev { get; set; }
            public bool Done { get; set; }
            public long Path { get; set; }

            public RoomPath()
            {
                Prev = 0x00;
                Done = false;
                Path = long.MaxValue;
            }

            public override string ToString()
            {
                string prev = $"{Prev >> 8}{Prev & 0xf}";
                string done = Done ? "#|" : "?|";
                return $"{done} {Path} [{prev}]";
            }
        }

        private void GetRoomPaths(Dictionary<int, Room> rooms, int startingRoomId, out Dictionary<int, RoomPath> roomPaths)
        {
            roomPaths = rooms.ToDictionary(pair => pair.Key, pair => new RoomPath());
            roomPaths[startingRoomId].Prev = startingRoomId;
            roomPaths[startingRoomId].Path = 0;
            PriorityQueue<int, long> roomTraversal = new PriorityQueue<int, long>();
            roomTraversal.Enqueue(startingRoomId, 0);
            while (roomTraversal.Count > 0)
            {
                int curRoomId = roomTraversal.Dequeue();
                Room curRoom = rooms[curRoomId];
                RoomPath curPath = roomPaths[curRoomId];
                if (curPath.Done)
                {
                    continue;
                }
                curPath.Done = true;

                RoomPath prevNode = roomPaths[curPath.Prev];
                curPath.Path = prevNode.Path;
                if (curPath.Prev != curRoomId)
                {
                    curPath.Path += curRoom.Rooms[curPath.Prev];
                }

                foreach (var nextRoomPair in curRoom.Rooms)
                {
                    RoomPath nextNode = roomPaths[nextRoomPair.Key];
                    Room nextRoom = rooms[nextRoomPair.Key];
                    if (!nextNode.Done)
                    {
                        if (nextNode.Prev != 0x00)
                        {
                            RoomPath existing = roomPaths[nextNode.Prev];
                            if (curPath.Path < existing.Path)
                            {
                                nextNode.Prev = curRoomId;
                            }
                        }
                        else
                        {
                            nextNode.Prev = curRoomId;
                        }
                        roomTraversal.Enqueue(nextRoomPair.Key, curPath.Path + curRoom.Rooms[nextRoomPair.Key]);
                    }
                }
            }
        }

        private class TravelPlan
        {
            public int[] RoomIds { get; set; }
            public int[] PrevId { get; set; }
            public long[] CurTime { get; set; }
            public Dictionary<int, long>[] Times { get; set; }
            public HashSet<int> Used { get; set; }

            public int Count { get { return RoomIds.Length; } }

            public TravelPlan(int[] roomIds, long curTime, Dictionary<int, Room> rooms)
            {
                RoomIds = new int[roomIds.Length];
                PrevId = new int[Count];
                CurTime = new long[Count];
                Times = new Dictionary<int, long>[Count];
                Used = new HashSet<int>();
                for (int i = 0; i < Count; ++i)
                {
                    RoomIds[i] = roomIds[i];
                    PrevId[i] = roomIds[i];
                    CurTime[i] = curTime;
                    Times[i] = new Dictionary<int, long>();
                    if (!Used.Contains(roomIds[i]))
                    {
                        Used.Add(roomIds[i]);
                    }
                }
            }

            public TravelPlan(int[] roomIds, long[] curTime, TravelPlan previous)
            {
                RoomIds = new int[roomIds.Length];
                PrevId = new int[Count];
                CurTime = new long[Count];
                Times = new Dictionary<int, long>[Count];
                Used = new HashSet<int>(previous.Used);
                for (int i = 0; i < Count; ++i)
                {
                    RoomIds[i] = roomIds[i];
                    CurTime[i] = curTime[i];
                    int lastIdx = Used.Count - 1;
                    if (previous.PrevId[i] != roomIds[i])
                    {
                        PrevId[i] = roomIds[i];
                    }
                    else
                    {
                        PrevId[i] = previous.PrevId[i];
                    }
                    Times[i] = new Dictionary<int, long>(previous.Times[i]);
                    Times[i][RoomIds[i]] = curTime[i];
                    if (!Used.Contains(roomIds[i]))
                    {
                        Used.Add(roomIds[i]);
                    }
                }
            }

            public bool OutOfTime()
            {
                foreach (long ct in CurTime)
                {
                    if (ct < 0)
                    {
                        return true;
                    }
                }
                return false;
            }

            public long GetPressure(Dictionary<int, Room> rooms)
            {
                long pressure = 0;
                foreach (Dictionary<int, long> times in Times)
                {
                    foreach (var pair in rooms)
                    {
                        if (times.ContainsKey(pair.Key))
                        {
                            pressure += times[pair.Key] * rooms[pair.Key].Rate;
                        }
                    }
                }
                return pressure;
            }

            public long GetPotentialPressure(Dictionary<int, Room> rooms, Dictionary<int, Dictionary<int, RoomPath>> roomPaths)
            {
                List<int> potentialRooms = rooms.Where(r => r.Value.Rate != 0 && !Used.Contains(r.Key)).Select(r => r.Key).ToList();
                long potentialPressure = 0;
                for (int i = 0; i < RoomIds.Length; ++i)
                {
                    int curRoomId = RoomIds[i];
                    foreach (int p in potentialRooms)
                    {
                        long path = roomPaths[curRoomId][p].Path;
                        if (CurTime[i] - path - 1 > 0)
                        {
                            potentialPressure += (CurTime[i] - path - 1) * rooms[p].Rate;
                        }
                    }
                }
                return potentialPressure;
            }
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, long maxTime, int explorerCount)
        {
            int initialRoomId = ToId("AA");
            Dictionary<int, Room> rooms = inputs.Select(Room.Parse).ToDictionary(r => r.Id, r => r);
            Dictionary<int, Dictionary<int, RoomPath>> roomPaths = new Dictionary<int, Dictionary<int, RoomPath>>();
            {
                GetRoomPaths(rooms, initialRoomId, out Dictionary<int, RoomPath> roomNodes);
                roomPaths[initialRoomId] = roomNodes;
            }
            foreach (var pair in rooms)
            {
                if (pair.Value.Rate != 0)
                {
                    GetRoomPaths(rooms, pair.Key, out Dictionary<int, RoomPath> roomNodes);
                    roomPaths[pair.Key] = roomNodes;
                }
            }

            long maxPressure = long.MinValue;
            TravelPlan initialPlan = new TravelPlan(Enumerable.Range(0, explorerCount).Select(i => initialRoomId).ToArray(), maxTime, rooms);
            PriorityQueue<TravelPlan, long> roomTraversal = new PriorityQueue<TravelPlan, long>(Comparer<long>.Create((a, b) => (int)(b - a)));
            roomTraversal.Enqueue(initialPlan, maxTime);
            while (roomTraversal.Count > 0)
            {
                TravelPlan curNode = roomTraversal.Dequeue();
                int[] curNodeIds = curNode.RoomIds;

                long pressure = curNode.GetPressure(rooms);
                bool recordPressure = pressure > maxPressure;
                maxPressure = Math.Max(pressure, maxPressure);

                if (curNode.OutOfTime())
                {
                    continue;
                }

                // get the necessary paths from current room to the others
                Dictionary<int, RoomPath>[] travelPlanRoomPaths = new Dictionary<int, RoomPath>[explorerCount];
                for (int i = 0; i < explorerCount; ++i)
                {
                    travelPlanRoomPaths[i] = roomPaths[curNodeIds[i]];
                }

                int nextStateCount = 0;
                if (explorerCount > 1)
                {
                    nextStateCount = HandleNextStates(rooms, travelPlanRoomPaths, roomPaths, curNode, maxPressure, ref roomTraversal);
                }
                else
                {
                    nextStateCount = HandleNextStates(rooms, travelPlanRoomPaths[0], roomPaths, curNode, maxPressure, ref roomTraversal);
                }
                // if this had the highest potential, and has no future steps, it must be the winner
                if (nextStateCount == 0 && recordPressure)
                {
                    return curNode.GetPressure(rooms).ToString();
                }
            }

            return maxPressure.ToString();
        }

        private int HandleNextStates(Dictionary<int, Room> rooms,
                                     Dictionary<int, RoomPath> travelPlanRoomPaths,
                                     Dictionary<int, Dictionary<int, RoomPath>> allRoomPaths,
                                     TravelPlan curNode,
                                     long maxPressure,
                                     ref PriorityQueue<TravelPlan, long> roomTraversal)
        {
            int nextStateCount = 0;
            foreach (var pair in travelPlanRoomPaths)
            {
                if (curNode.Used.Contains(pair.Key) || rooms[pair.Key].Rate == 0)
                {
                    continue;
                }

                TravelPlan nextNode = new TravelPlan(new int[] { pair.Key }, new long[] { (curNode.CurTime[0] - 1 - pair.Value.Path) }, curNode);
                if (nextNode.CurTime[0] <= 0)
                {
                    continue;
                }

                ++nextStateCount;
                Room nextRoom = rooms[nextNode.RoomIds[0]];
                long nextPressure = nextNode.GetPressure(rooms);
                long potentialPressure = nextNode.GetPotentialPressure(rooms, allRoomPaths);
                if (nextPressure + potentialPressure < maxPressure)
                {
                    continue;
                }
                roomTraversal.Enqueue(nextNode, nextPressure + potentialPressure);
            }
            return nextStateCount;
        }

        private int HandleNextStates(Dictionary<int, Room> rooms,
                                     Dictionary<int, RoomPath>[] travelPlanRoomPaths,
                                     Dictionary<int, Dictionary<int, RoomPath>> allRoomPaths,
                                     TravelPlan curNode,
                                     long maxPressure,
                                     ref PriorityQueue<TravelPlan, long> roomTraversal)
        {
            int nextStateCount = 0;

            int moveIdx = 0;
            int stayIdx = 1;
            if (curNode.CurTime[0] < curNode.CurTime[1])
            {
                moveIdx = 1;
                stayIdx = 0;
            }

            foreach (var pair in travelPlanRoomPaths[moveIdx])
            {
                if (curNode.Used.Contains(pair.Key) || rooms[pair.Key].Rate == 0)
                {
                    continue;
                }

                int[] nextRoomIds = new int[curNode.Count];
                nextRoomIds[moveIdx] = pair.Key;
                nextRoomIds[stayIdx] = curNode.RoomIds[stayIdx];

                long[] nextTimes = new long[curNode.Count];
                nextTimes[moveIdx] = curNode.CurTime[moveIdx] - 1 - pair.Value.Path;
                nextTimes[stayIdx] = curNode.CurTime[stayIdx];

                TravelPlan nextNode = new TravelPlan(nextRoomIds, nextTimes, curNode);
                if (nextNode.CurTime[moveIdx] <= 0) // maybe check both times here
                {
                    continue;
                }

                ++nextStateCount;
                Room nextRoom = rooms[nextNode.RoomIds[moveIdx]];
                long nextPressure = nextNode.GetPressure(rooms);
                long potentialPressure = nextNode.GetPotentialPressure(rooms, allRoomPaths);
                if (nextPressure + potentialPressure < maxPressure)
                {
                    continue;
                }
                roomTraversal.Enqueue(nextNode, nextPressure + potentialPressure);
            }
            return nextStateCount;
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, 30, 1);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, 26, 2);
    }
}