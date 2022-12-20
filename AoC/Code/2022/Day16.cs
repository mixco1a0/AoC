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

        private class TravelNode
        {
            public string RoomId { get; set; }
            public string History { get; set; }
            public long CurTime { get; set; }
            public Dictionary<string, Room> Rooms;
            public Dictionary<string, long> Times { get; set; }
            public long Pressure { get; set; }
            public HashSet<string> Used { get; set; }

            public TravelNode(string roomId, long curTime, Dictionary<string, Room> rooms)
            {
                RoomId = roomId;
                History = RoomId;
                CurTime = curTime;
                Rooms = new Dictionary<string, Room>();
                foreach (var pair in rooms)
                {
                    Rooms[pair.Key] = new Room(pair.Value);
                }
                Times = new Dictionary<string, long>();
                Pressure = 0;
                Used = new HashSet<string>();
                Used.Add(roomId);
            }

            public TravelNode(string roomId, long curTime, TravelNode previous)
            {
                RoomId = roomId;
                History = $"{previous.History}|{roomId}";
                CurTime = curTime;
                Rooms = new Dictionary<string, Room>();
                foreach (var pair in previous.Rooms)
                {
                    Rooms[pair.Key] = new Room(pair.Value);
                }
                Times = new Dictionary<string, long>(previous.Times);
                Times[roomId] = curTime;
                Pressure = 0;
                Used = new HashSet<string>(previous.Used);
                Used.Add(roomId);
            }
        }

        public class RoomTime : Base.Pair<string, long>
        {
            public RoomTime() : base() { }

            public RoomTime(string id, long time) : base(id, time) { }
        }

        private long GetPressure(Dictionary<string, Room> rooms, Dictionary<string, long> times)
        {
            long pressure = 0;
            foreach (var pair in rooms)
            {
                if (times.ContainsKey(pair.Key))
                {
                    pressure += times[pair.Key] * rooms[pair.Key].Rate;
                }
            }
            return pressure;
        }

        private long GetPotentialPressure(Dictionary<string, Room> rooms, Dictionary<string, Dictionary<string, RoomPath>> roomPaths, TravelNode curNode)
        {
            string curRoomId = curNode.RoomId;
            List<string> potentialRooms = rooms.Where(r => r.Value.Rate != 0 && !curNode.Used.Contains(r.Key)).Select(r => r.Key).ToList();
            long potentialPressure = 0;
            foreach (string p in potentialRooms)
            {
                long path = roomPaths[curRoomId][p].Path;
                if (curNode.CurTime - path - 1 > 0)
                {
                    potentialPressure += (curNode.CurTime - path - 1) * rooms[p].Rate;
                }
            }
            return potentialPressure;
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, long maxTime, bool elephantHelper)
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
            Dictionary<string, TravelNode> travelNodes = new Dictionary<string, TravelNode>();
            travelNodes["AA"] = new TravelNode("AA", maxTime, rooms);

            long maxPressure = long.MinValue;
            PriorityQueue<RoomTime, long> roomTraversal = new PriorityQueue<RoomTime, long>(Comparer<long>.Create((a, b) => (int)(b - a))); // 
            roomTraversal.Enqueue(new RoomTime("AA", maxTime), maxTime);
            while (roomTraversal.Count > 0)
            {
                RoomTime roomTime = roomTraversal.Dequeue();
                TravelNode curNode = travelNodes[roomTime.First];
                string curNodeId = curNode.RoomId;

                long pressure = GetPressure(rooms, curNode.Times);
                bool recordPressure = pressure > maxPressure;
                // if (recordPressure)
                // {
                //     DebugWriteLine($"{pressure} | {curNode.History}");
                // }
                maxPressure = Math.Max(pressure, maxPressure);

                // check if complete, update maxPressure
                if (curNode.Used.Count == roomCount)
                {
                    return GetPressure(rooms, curNode.Times).ToString();
                }

                if (curNode.CurTime < 0)
                {
                    travelNodes.Remove(curNode.History);
                    continue;
                }

                Dictionary<string, RoomPath> roomNodes = roomPaths[curNodeId];
                int nextStateCount = 0;
                foreach (var pair in roomNodes)
                {
                    if (curNode.Used.Contains(pair.Key) || rooms[pair.Key].Rate == 0)
                    {
                        continue;
                    }

                    TravelNode nextNode = new TravelNode(pair.Key, roomTime.Last - 1 - pair.Value.Path, curNode);
                    if (nextNode.CurTime <= 0)
                    {
                        continue;
                    }

                    ++nextStateCount;
                    travelNodes[nextNode.History] = nextNode;
                    Room nextRoom = rooms[nextNode.RoomId];
                    long nextPressure = GetPressure(rooms, nextNode.Times);
                    long potentialPressure = GetPotentialPressure(rooms, roomPaths, nextNode);
                    if (nextPressure + potentialPressure < maxPressure)
                    {
                        continue;
                    }
                    roomTraversal.Enqueue(new RoomTime(nextNode.History, nextNode.CurTime), nextPressure + potentialPressure - nextNode.CurTime);
                }

                // if this had the highest potential, and has not future steps, it must be the winner
                if (nextStateCount == 0 && recordPressure)
                {
                    return GetPressure(rooms, curNode.Times).ToString();
                }

                travelNodes.Remove(curNode.History);
            }

            return maxPressure.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, 30, false);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, 26, true);
    }
}