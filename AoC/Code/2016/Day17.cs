using System.Text;
using System.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2016
{
    class Day17 : Day
    {
        public Day17() { }
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
                Output = "DDRRRD",
                RawInput =
@"ihgpwlah"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.One,
                Output = "DDUDRLRRUDRD",
                RawInput =
@"kglvqrro"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.One,
                Output = "DRURDRUDDLLDLUURRDULRLDUUDDDRR",
                RawInput =
@"ulqzkmiv"
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

        private record DoorStatus(bool[] Status)
        {
            static public DoorStatus Parse(string input)
            {
                // get md5
                string hash = string.Empty;
                using (MD5 md5 = MD5.Create())
                {
                    byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                    byte[] hashBytes = md5.ComputeHash(inputBytes);
                    hash = BitConverter.ToString(hashBytes).Replace("-", string.Empty).ToLower();
                }

                // set door status
                bool[] open = new bool[4];
                for (int i = 0; i < 4; ++i)
                {
                    open[i] = !(hash[i] == 'a' || Char.IsDigit(hash[i]));
                }
                return new DoorStatus(open);
            }

            static public Coords[] Directions = new Coords[4] { new Coords(0, -1), new Coords(0, 1), new Coords(-1, 0), new Coords(1, 0) };

            static public char[] Letters = new char[4] { 'U', 'D', 'L', 'R' };
        }

        private record WalkStatus(string Path, Coords Coords) { }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables)
        {
            Queue<WalkStatus> pendingWalks = new Queue<WalkStatus>();
            pendingWalks.Enqueue(new WalkStatus(inputs.First(), new Coords(0, 0)));
            while (pendingWalks.Count > 0)
            {
                WalkStatus ws = pendingWalks.Dequeue();
                if (ws.Coords.X == 3 && ws.Coords.Y == 3)
                {
                    return ws.Path.Substring(inputs.First().Length);
                }

                DoorStatus ds = DoorStatus.Parse(ws.Path);
                for (int i = 0; i < ds.Status.Count(); ++i)
                {
                    if (ds.Status[i])
                    {
                        Coords newCoords = ws.Coords + DoorStatus.Directions[i];
                        if (newCoords.X >=0 && newCoords.X <= 3 && newCoords.Y >= 0 && newCoords.Y <= 3)
                        {
                            pendingWalks.Enqueue(new WalkStatus($"{ws.Path}{DoorStatus.Letters[i]}", newCoords));
                        }
                    }
                }
            }

            return "";
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);
    }
}