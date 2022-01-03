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
        private static readonly Dictionary<int, char> AmphipodRoom = new Dictionary<int, char>() { { 0, 'A' }, { 1, 'B' }, { 2, 'C' }, { 3, 'D' } };

        private class BurrowState
        {
            public static char Empty { get => '.'; }
            public char[] Hallway { get; init; }
            public char[][] Rooms { get; init; }

            private static readonly Dictionary<int, int> RoomToHall = new Dictionary<int, int>() { { 0, 2 }, { 1, 4 }, { 2, 6 }, { 3, 8 } };

            public BurrowState()
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
            }

            public static BurrowState Parse(string rooms1, string rooms2)
            {
                BurrowState bs = new BurrowState();
                char[] rooms1Amphipod = rooms1.Split('#', StringSplitOptions.RemoveEmptyEntries).Select(s => s[0]).ToArray();
                char[] rooms2Amphipod = rooms2.Split("# ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(s => s[0]).ToArray();
                for (int i = 0; i < bs.Rooms.Length; ++i)
                {
                    bs.Rooms[i][0] = rooms1Amphipod[i];
                    bs.Rooms[i][1] = rooms2Amphipod[i];
                }
                return bs;
            }

            public bool IsComplete()
            {
                bool isComplete = !Hallway.Any(h => h != Empty);
                for (int i = 0; isComplete && i < Rooms.Length; ++i)
                {
                    isComplete = (Rooms[i][0] == AmphipodRoom[i] && Rooms[i][1] == AmphipodRoom[i]);
                }
                return isComplete;
            }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder("\r\n");
                sb.AppendLine("#############");
                sb.Append('#');
                sb.Append(string.Join("", Hallway));
                sb.AppendLine("#");
                sb.Append("###");
                foreach (char[] room in Rooms)
                {
                    sb.Append(room[0]);
                    sb.Append("#");
                }
                sb.AppendLine("##");
                sb.Append("  #");
                foreach (char[] room in Rooms)
                {
                    sb.Append(room[1]);
                    sb.Append("#");
                }
                sb.AppendLine("  ");
                sb.AppendLine("  #########  ");
                return sb.ToString();
            }
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables)
        {
            BurrowState start = BurrowState.Parse(inputs[2], inputs[3]);
            return string.Empty;
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);
    }
}