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
            public Dictionary<string, int> Rooms { get; set; }

            public Room()
            {
                Id = string.Empty;
                Rate = 0;
                Rooms = new Dictionary<string, int>();
            }

            public Room(Room other)
            {
                Id = other.Id;
                Rate = other.Rate;
                Rooms = new Dictionary<string, int>(other.Rooms);
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

        // private class RoomState
        // {
        //     public int CurTime { get; set; }
        //     public string CurId { get; set; }
        //     public Dictionary<string, int> Opened { get; set; }
        //     public HashSet<string> Closed { get; set; }
        //     public List<IdToNew> History { get; set; }

        //     public RoomState(Dictionary<string, Room> rooms, int time, Room curRoom, bool open)
        //     {
        //         CurTime = time;
        //         CurId = curRoom.Id;
        //         Opened = new Dictionary<string, int>();
        //         Closed = new HashSet<string>(rooms.Keys);
        //         History = new List<IdToNew>();
        //         History.Add(new IdToNew(curRoom.Id, open));

        //         if (open && Closed.Contains(CurId))
        //         {
        //             History.Add(new IdToNew(CurId, true));
        //             Closed.Remove(CurId);
        //             Opened.Add(CurId, CurTime);
        //         }
        //         else
        //         {
        //             History.Add(new IdToNew(CurId, false));
        //         }
        //     }

        //     public RoomState(RoomState prev, int time, Room nextRoom, int stepsToRoom, bool open)
        //     {
        //         CurId = nextRoom.Id;
        //         Opened = new Dictionary<string, int>(prev.Opened);
        //         Closed = new HashSet<string>(prev.Closed);
        //         History = new List<IdToNew>(prev.History);

        //         if (open && Closed.Contains(CurId))
        //         {
        //             CurTime = time - stepsToRoom - 1;
        //             History.Add(new IdToNew(CurId, true));
        //             Closed.Remove(CurId);
        //             Opened.Add(CurId, CurTime);
        //         }
        //         else
        //         {
        //             CurTime = time - stepsToRoom;
        //             History.Add(new IdToNew(CurId, false));
        //         }
        //     }

        //     public bool HasCycle()
        //     {
        //         HashSet<string> cycles = new HashSet<string>();
        //         foreach (var pair in History)
        //         {
        //             if (!pair.Val)
        //             {
        //                 if (cycles.Contains(pair.Key))
        //                 {
        //                     return true;
        //                 }

        //                 cycles.Add(pair.Key);
        //             }
        //             else if (pair.Val)
        //             {
        //                 cycles.Clear();
        //             }
        //         }
        //         return false;
        //     }

        //     public override string ToString()
        //     {
        //         return $"{CurTime} | {string.Join(" ", History.Select(p => $"{string.Format("{0}", p.Val ? "+" : "-")}{p.First}"))}";
        //     }
        // }

        // private int GetPressureReleased(Dictionary<string, Room> rooms, RoomState rs)
        // {
        //     int total = 0;
        //     foreach (var pair in rs.Opened)
        //     {
        //         total += rooms[pair.Key].Rate * pair.Value;
        //     }
        //     return total;
        // }

        // private int GetPotentialReleased(Dictionary<string, Room> rooms, RoomState rs)
        // {
        //     int total = 0;
        //     foreach (string id in rs.Closed)
        //     {
        //         total += rooms[id].Rate * (rs.CurTime - 2);
        //     }
        //     return total;
        // }

        private void RemoveRoom(ref Dictionary<string, Room> rooms, string roomId)
        {
            Room removedRoom = rooms[roomId];
            Dictionary<string, int> removedRoomConnections = removedRoom.Rooms;
            foreach (var rrc in removedRoomConnections)
            {
                // grab a connection
                Room connection = rooms[rrc.Key];
                int distanceToConnection = removedRoomConnections[connection.Id];
                connection.Rooms.Remove(roomId);

                // connect this room to all of the other ones
                foreach (var rrc2 in removedRoomConnections)
                {
                    if (rrc2.Key == connection.Id)
                    {
                        continue;
                    }

                    int distanceToOtherConnection = removedRoomConnections[rrc2.Key];
                    connection.Rooms[rrc2.Key] = distanceToConnection + distanceToOtherConnection;
                }
            }

            rooms.Remove(roomId);
        }

        private class RoomState
        {
            public Dictionary<string, Room> Rooms { get; set; }
        }

        private class RoomNode
        {
            public string Prev { get; set; }
            public bool Done { get; set; }
            public int Path { get; set; }

            public RoomNode()
            {
                Prev = string.Empty;
                Done = false;
                Path = int.MaxValue;
            }

            public override string ToString()
            {
                string done = Done ? "#|" : ".|";
                return $"{done} {Path} [{Prev}]";
            }
        }

        private void GetRoomToOthers(Dictionary<string, Room> rooms, string startingRoomId, out Dictionary<string, RoomNode> roomNodes)
        {
            roomNodes = rooms.ToDictionary(pair => pair.Key, pair => new RoomNode());
            roomNodes[startingRoomId].Prev = startingRoomId;
            roomNodes[startingRoomId].Path = 0;
            PriorityQueue<string, int> roomTraversal = new PriorityQueue<string, int>();
            roomTraversal.Enqueue(startingRoomId, 0);
            while (roomTraversal.Count > 0)
            {
                string curRoomId = roomTraversal.Dequeue();
                Room curRoom = rooms[curRoomId];
                RoomNode curNode = roomNodes[curRoomId];
                if (curNode.Done)
                {
                    continue;
                }
                curNode.Done = true;

                RoomNode prevNode = roomNodes[curNode.Prev];
                curNode.Path = prevNode.Path;
                if (curNode.Prev != curRoomId)
                {
                    curNode.Path += curRoom.Rooms[curNode.Prev];
                }

                foreach (var nextRoomPair in curRoom.Rooms)
                {
                    RoomNode nextNode = roomNodes[nextRoomPair.Key];
                    if (!nextNode.Done)
                    {
                        if (!string.IsNullOrWhiteSpace(nextNode.Prev))
                        {
                            RoomNode existing = roomNodes[nextNode.Prev];
                            if (curNode.Path < existing.Path)
                            {
                                nextNode.Prev = curRoomId;
                            }
                        }
                        else
                        {
                            nextNode.Prev = curRoomId;
                        }
                        roomTraversal.Enqueue(nextRoomPair.Key, curNode.Path + curRoom.Rooms[nextRoomPair.Key]);
                    }
                }
            }
        }

        private class TravelNode
        {
            public string RoomId { get; set; }
            public string History { get; set; }
            public int CurTime { get; set; }
            public Dictionary<string, Room> Rooms;
            public Dictionary<string, int> Times { get; set; }
            public int Pressure { get; set; }

            public TravelNode(string roomId, int curTime, Dictionary<string, Room> rooms)
            {
                RoomId = roomId;
                History = RoomId;
                CurTime = curTime;
                Rooms = new Dictionary<string, Room>();
                foreach (var pair in rooms)
                {
                    Rooms[pair.Key] = new Room(pair.Value);
                }
                Times = new Dictionary<string, int>();
                Pressure = 0;
            }

            public TravelNode(string roomId, int curTime, TravelNode previous)
            {
                RoomId = roomId;
                History = $"{previous.History}|{roomId}";
                CurTime = curTime;
                Rooms = new Dictionary<string, Room>();
                foreach (var pair in previous.Rooms)
                {
                    Rooms[pair.Key] = new Room(pair.Value);
                }
                Times = new Dictionary<string, int>(previous.Times);
                Times[roomId] = curTime;
                Pressure = 0;
            }
        }

        public class RoomTime : Base.Pair<string, int>
        {
            public RoomTime() : base()
            {

            }

            public RoomTime(string id, int time) : base(id, time)
            {

            }
        }

        private int GetPressure(Dictionary<string, Room> rooms, Dictionary<string, int> times)
        {
            int pressure = 0;
            foreach (var pair in rooms)
            {
                if (times.ContainsKey(pair.Key))
                {
                    pressure += times[pair.Key] * rooms[pair.Key].Rate;
                }
            }
            return pressure;
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables)
        {
            Dictionary<string, Room> rooms = inputs.Select(Room.Parse).ToDictionary(r => r.Id, r => r);
            // try to remove 0 rooms except for the starting point
            List<string> zeroRooms = rooms.Where(pair => pair.Value.Rate == 0 && pair.Value.Id != "AA").Select(pair => pair.Key).ToList();
            foreach (string zeroRoom in zeroRooms)
            {
                RemoveRoom(ref rooms, zeroRoom);
            }

            Dictionary<string, TravelNode> travelNodes = new Dictionary<string, TravelNode>();
            travelNodes["AA"] = new TravelNode("AA", 30, rooms);

            int maxPressure = int.MinValue;
            Queue<RoomTime> roomTraversal = new Queue<RoomTime>();
            roomTraversal.Enqueue(new RoomTime("AA", 30));
            while (roomTraversal.Count > 0)
            {
                RoomTime roomTime = roomTraversal.Dequeue();
                TravelNode curNode = travelNodes[roomTime.First];
                string curNodeId = curNode.RoomId;

                // check if complete, update maxPressure
                if (curNode.Rooms.Count == 1 || curNode.CurTime == 0)
                {
                    int pressure = GetPressure(rooms, curNode.Times);
                    maxPressure = Math.Max(pressure, maxPressure);
                    travelNodes.Remove(curNode.History);
                    continue;
                }

                if (curNode.CurTime < 0)
                {
                    travelNodes.Remove(curNode.History);
                    continue;
                }

                // get next states
                GetRoomToOthers(curNode.Rooms, curNodeId, out Dictionary<string, RoomNode> roomNodes);

                // remove current node for the next mapping
                RemoveRoom(ref curNode.Rooms, curNodeId);

                if (curNode.History.Contains("AA|DD|BB|JJ|HH"))
                {
                    DebugWriteLine("");
                }

                foreach (var pair in roomNodes)
                {

                    if (pair.Key == curNodeId)
                    {
                        continue;
                    }

                    TravelNode nextNode = new TravelNode(pair.Key, roomTime.Last - 1 - pair.Value.Path, curNode);
                    if (nextNode.CurTime <= 0)
                    {
                        continue;
                    }

                    travelNodes[nextNode.History] = nextNode;
                    roomTraversal.Enqueue(new RoomTime(nextNode.History, nextNode.CurTime));
                }

                travelNodes.Remove(curNode.History);
            }

            return maxPressure.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);
    }
}