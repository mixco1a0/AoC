using System.Text;
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

        private class Room
        {
            public string Id { get; set; }
            public long Rate { get; set; }
            public Dictionary<string, long> Rooms { get; set; }

            public Room()
            {
                Id = string.Empty;
                Rate = 0;
                Rooms = new Dictionary<string, long>();
            }

            public Room(Room other)
            {
                Id = other.Id;
                Rate = other.Rate;
                Rooms = new Dictionary<string, long>(other.Rooms);
            }

            public static Room Parse(string input)
            {
                string[] split = input.Split(" =;,".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                Room room = new Room();
                room.Id = split[1];
                room.Rate = long.Parse(split[5]);
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

        private class RoomPath
        {
            public string Prev { get; set; }
            public bool Done { get; set; }
            public long Path { get; set; }

            public RoomPath()
            {
                Prev = string.Empty;
                Done = false;
                Path = long.MaxValue;
            }

            public override string ToString()
            {
                string done = Done ? "#|" : "?|";
                return $"{done} {Path} [{Prev}]";
            }
        }

        private void GetRoomToOthers(Dictionary<string, Room> rooms, string startingRoomId, out Dictionary<string, RoomPath> roomPaths)
        {
            roomPaths = rooms.ToDictionary(pair => pair.Key, pair => new RoomPath());
            roomPaths[startingRoomId].Prev = startingRoomId;
            roomPaths[startingRoomId].Path = 0;
            PriorityQueue<string, long> roomTraversal = new PriorityQueue<string, long>();
            roomTraversal.Enqueue(startingRoomId, 0);
            while (roomTraversal.Count > 0)
            {
                string curRoomId = roomTraversal.Dequeue();
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
                        if (!string.IsNullOrWhiteSpace(nextNode.Prev))
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
            public string[] RoomIds { get; set; }
            public string[] History { get; set; }
            public long[] CurTime { get; set; }
            public Dictionary<string, long>[] Times { get; set; }
            public HashSet<string> Used { get; set; }

            public int Count { get { return RoomIds.Length; } }

            public TravelPlan(string[] roomIds, long curTime, Dictionary<string, Room> rooms)
            {
                RoomIds = roomIds;
                History = RoomIds;
                CurTime = new long[Count];
                Times = new Dictionary<string, long>[Count];
                for (int i = 0; i < Count; ++i)
                {
                    CurTime[i] = curTime;
                    Times[i] = new Dictionary<string, long>();
                }
                Used = new HashSet<string>();
                foreach (string ri in roomIds)
                {
                    if (!Used.Contains(ri))
                    {
                        Used.Add(ri);
                    }
                }
            }

            public TravelPlan(string[] roomIds, long[] curTime, TravelPlan previous)
            {
                RoomIds = roomIds;
                History = new string[Count];
                CurTime = curTime;
                Times = new Dictionary<string, long>[Count];
                for (int i = 0; i < Count; ++i)
                {
                    if (!previous.History[i].EndsWith(roomIds[i]))
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append(previous.History[i]);
                        sb.Append('|');
                        sb.Append(roomIds[i]);
                        History[i] = sb.ToString();
                    }
                    else
                    {
                        History[i] = previous.History[i];
                    }
                    Times[i] = new Dictionary<string, long>(previous.Times[i]);
                    Times[i][RoomIds[i]] = curTime[i];
                }
                Used = new HashSet<string>(previous.Used);
                foreach (string ri in roomIds)
                {
                    if (!Used.Contains(ri))
                    {
                        Used.Add(ri);
                    }
                }
            }

            public string GetHistory()
            {
                StringBuilder bs = new StringBuilder();
                bs.AppendJoin('#', History);
                return bs.ToString();
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
        }

        private long GetPressure(Dictionary<string, Room> rooms, TravelPlan curNode)
        {
            long pressure = 0;
            foreach (Dictionary<string, long> times in curNode.Times)
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

        private long GetPotentialPressure(Dictionary<string, Room> rooms, Dictionary<string, Dictionary<string, RoomPath>> roomPaths, TravelPlan curNode)
        {
            List<string> potentialRooms = rooms.Where(r => r.Value.Rate != 0 && !curNode.Used.Contains(r.Key)).Select(r => r.Key).ToList();
            long potentialPressure = 0;
            for (int i = 0; i < curNode.RoomIds.Length; ++i)
            {
                string curRoomId = curNode.RoomIds[i];
                foreach (string p in potentialRooms)
                {
                    long path = roomPaths[curRoomId][p].Path;
                    if (curNode.CurTime[i] - path - 1 > 0)
                    {
                        potentialPressure += (curNode.CurTime[i] - path - 1) * rooms[p].Rate;
                    }
                }
            }
            return potentialPressure;
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, long maxTime, int explorerCount)
        {
            Dictionary<string, Room> rooms = inputs.Select(Room.Parse).ToDictionary(r => r.Id, r => r);
            Dictionary<string, Dictionary<string, RoomPath>> roomPaths = new Dictionary<string, Dictionary<string, RoomPath>>();
            {
                GetRoomToOthers(rooms, "AA", out Dictionary<string, RoomPath> roomNodes);
                roomPaths["AA"] = roomNodes;
            }
            foreach (var pair in rooms)
            {
                if (pair.Value.Rate != 0)
                {
                    GetRoomToOthers(rooms, pair.Key, out Dictionary<string, RoomPath> roomNodes);
                    roomPaths[pair.Key] = roomNodes;
                }
            }

            int roomCount = rooms.Where(r => r.Value.Rate != 0).Count() + 1;
            Dictionary<string, TravelPlan> travelPlans = new Dictionary<string, TravelPlan>();
            TravelPlan initialPlan = new TravelPlan(Enumerable.Range(0, explorerCount).Select(i => "AA").ToArray(), maxTime, rooms);
            travelPlans[initialPlan.GetHistory()] = initialPlan;

            long maxPressure = long.MinValue;
            PriorityQueue<string, long> roomTraversal = new PriorityQueue<string, long>(Comparer<long>.Create((a, b) => (int)(b - a))); // 
            roomTraversal.Enqueue(initialPlan.GetHistory(), maxTime);
            while (roomTraversal.Count > 0)
            {
                string curRoomTraversal = roomTraversal.Dequeue();
                TravelPlan curNode = travelPlans[curRoomTraversal];
                string[] curNodeIds = curNode.RoomIds;

                long pressure = GetPressure(rooms, curNode);
                bool recordPressure = pressure > maxPressure;
                maxPressure = Math.Max(pressure, maxPressure);

                if (curNode.OutOfTime())
                {
                    travelPlans.Remove(curNode.GetHistory());
                    continue;
                }

                // get the necessary paths from current room to the others
                Dictionary<string, RoomPath>[] roomNodes = new Dictionary<string, RoomPath>[explorerCount];
                for (int i = 0; i < explorerCount; ++i)
                {
                    roomNodes[i] = roomPaths[curNodeIds[i]];
                }

                int nextStateCount = 0;
                if (explorerCount > 1)
                {
                    nextStateCount = HandleNextStates(rooms, roomNodes, roomPaths, curNode, maxPressure, ref roomTraversal, ref travelPlans);
                }
                else
                {
                    nextStateCount = HandleNextStates(rooms, roomNodes[0], roomPaths, curNode, maxPressure, ref roomTraversal, ref travelPlans);
                }
                // if this had the highest potential, and has no future steps, it must be the winner
                if (nextStateCount == 0 && recordPressure)
                {
                    return GetPressure(rooms, curNode).ToString();
                }

                travelPlans.Remove(curNode.GetHistory());
            }

            return maxPressure.ToString();
        }

        private int HandleNextStates(Dictionary<string, Room> rooms,
                                     Dictionary<string, RoomPath> roomNodes,
                                     Dictionary<string, Dictionary<string, RoomPath>> roomPaths,
                                     TravelPlan curNode,
                                     long maxPressure,
                                     ref PriorityQueue<string, long> roomTraversal,
                                     ref Dictionary<string, TravelPlan> travelPlans)
        {
            int nextStateCount = 0;
            foreach (var pair in roomNodes)
            {
                if (curNode.Used.Contains(pair.Key) || rooms[pair.Key].Rate == 0)
                {
                    continue;
                }

                TravelPlan nextNode = new TravelPlan(new string[] { pair.Key }, new long[] { (curNode.CurTime[0] - 1 - pair.Value.Path) }, curNode);
                if (nextNode.CurTime[0] <= 0)
                {
                    continue;
                }

                ++nextStateCount;
                travelPlans[nextNode.History[0]] = nextNode;
                Room nextRoom = rooms[nextNode.RoomIds[0]];
                long nextPressure = GetPressure(rooms, nextNode);
                long potentialPressure = GetPotentialPressure(rooms, roomPaths, nextNode);
                if (nextPressure + potentialPressure < maxPressure)
                {
                    continue;
                }
                roomTraversal.Enqueue(nextNode.History[0], nextPressure + potentialPressure);
            }
            return nextStateCount;
        }

        private int HandleNextStates(Dictionary<string, Room> rooms,
                                     Dictionary<string, RoomPath>[] roomNodes,
                                     Dictionary<string, Dictionary<string, RoomPath>> roomPaths,
                                     TravelPlan curNode,
                                     long maxPressure,
                                     ref PriorityQueue<string, long> roomTraversal,
                                     ref Dictionary<string, TravelPlan> travelPlans)
        {
            int nextStateCount = 0;
            
            int moveIdx = 0;
            int stayIdx = 1;
            if (curNode.CurTime[0] < curNode.CurTime[1])
            {
                moveIdx = 1;
                stayIdx = 0;
            }

            foreach (var pair in roomNodes[moveIdx])
            {
                if (curNode.Used.Contains(pair.Key) || rooms[pair.Key].Rate == 0)
                {
                    continue;
                }

                string[] nextRoomIds = new string[curNode.Count];
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
                travelPlans[nextNode.GetHistory()] = nextNode;
                Room nextRoom = rooms[nextNode.RoomIds[moveIdx]];
                long nextPressure = GetPressure(rooms, nextNode);
                long potentialPressure = GetPotentialPressure(rooms, roomPaths, nextNode);
                if (nextPressure + potentialPressure < maxPressure)
                {
                    continue;
                }
                roomTraversal.Enqueue(nextNode.GetHistory(), nextPressure + potentialPressure);
            }
            return nextStateCount;
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, 30, 1);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, 26, 2);
    }
}