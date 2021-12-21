using System.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2021
{
    class Day19 : Day
    {
        public Day19() { }

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
                Output = "12",
                RawInput =
@"--- scanner 0 ---
404,-588,-901
528,-643,409
-838,591,734
390,-675,-793
-537,-823,-458
-485,-357,347
-345,-311,381
-661,-816,-575
-876,649,763
-618,-824,-621
553,345,-567
474,580,667
-447,-329,318
-584,868,-557
544,-627,-890
564,392,-477
455,729,728
-892,524,684
-689,845,-530
423,-701,434
7,-33,-71
630,319,-379
443,580,662
-789,900,-551
459,-707,401

--- scanner 1 ---
686,422,578
605,423,415
515,917,-361
-336,658,858
95,138,22
-476,619,847
-340,-569,-846
567,-361,727
-460,603,-452
669,-402,600
729,430,532
-500,-761,534
-322,571,750
-466,-666,-811
-429,-592,574
-355,545,-477
703,-491,-529
-328,-685,520
413,935,-424
-391,539,-444
586,-435,557
-364,-763,-893
807,-499,-711
755,-354,-619
553,889,-390
"
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

        private enum RotationIndex : int
        {
            Start = 0,
            // no initial rotation
            // rotate around Z axis
            PosXPosYPosZ = Start,
            PosYNegXPosZ,
            NegXNegYPosZ,
            NegYPosXPosZ,
            // rotate X axis 90d
            // rotate around Z axis
            PosXNegZPosY,
            NegZNegXPosY,
            NegXPosZPosY,
            PosZPosXPosY,
            // rotate X axis 180d
            // rotate around Z axis
            PosXNegYNegZ,
            NegYNegXNegZ,
            NegXPosYNegZ,
            PosYPosXNegZ,
            // rotate X axis 270d
            // rotate around Z axis
            PosXPosZNegY,
            PosZNegXNegY,
            NegXNegZNegY,
            NegZPosXNegY,
            // rotate Y axis 270d
            // rotate around Z axis
            NegZPosYPosX,
            PosYPosZPosX,
            PosZNegYPosX,
            NegYNegZPosX,
            // rotate Y axis 90d
            // rotate around Z axis
            PosZPosYNegX,
            PosYNegZNegX,
            NegZNegYNegX,
            NegYPosZNegX,
            End = NegYPosZNegX
        }

        private Vector3 ToRotationIndex(Vector3 v, RotationIndex targetRotation)
        {
            switch (targetRotation)
            {
                // Z = positive Z axis
                case RotationIndex.PosXPosYPosZ:
                    break;
                case RotationIndex.PosYNegXPosZ:
                    return new Vector3(v.Y, -v.X, v.Z);
                case RotationIndex.NegXNegYPosZ:
                    return new Vector3(-v.X, -v.Y, v.Z);
                case RotationIndex.NegYPosXPosZ:
                    return new Vector3(-v.Y, v.X, v.Z);
                // Z = positive Y axis
                case RotationIndex.PosXNegZPosY:
                    return new Vector3(v.X, -v.Z, v.Y);
                case RotationIndex.NegZNegXPosY:
                    return new Vector3(-v.Z, -v.X, v.Y);
                case RotationIndex.NegXPosZPosY:
                    return new Vector3(-v.X, v.Z, v.Y);
                case RotationIndex.PosZPosXPosY:
                    return new Vector3(v.Z, v.X, v.Y);
                // Z = negative Z axis
                case RotationIndex.PosXNegYNegZ:
                    return new Vector3(v.X, -v.Y, -v.Z);
                case RotationIndex.NegYNegXNegZ:
                    return new Vector3(-v.Y, -v.X, -v.Z);
                case RotationIndex.NegXPosYNegZ:
                    return new Vector3(-v.X, v.Y, -v.Z);
                case RotationIndex.PosYPosXNegZ:
                    return new Vector3(v.Y, v.X, -v.Z);
                // Z = negative Y axis
                case RotationIndex.PosXPosZNegY:
                    return new Vector3(v.X, v.Z, -v.Y);
                case RotationIndex.PosZNegXNegY:
                    return new Vector3(v.Z, -v.X, -v.Y);
                case RotationIndex.NegXNegZNegY:
                    return new Vector3(-v.X, -v.Z, -v.Y);
                case RotationIndex.NegZPosXNegY:
                    return new Vector3(-v.Z, v.X, -v.Y);
                // Z = positive X axis
                case RotationIndex.NegZPosYPosX:
                    return new Vector3(-v.Z, v.Y, v.X);
                case RotationIndex.PosYPosZPosX:
                    return new Vector3(v.Y, v.Z, v.X);
                case RotationIndex.PosZNegYPosX:
                    return new Vector3(v.Z, -v.Y, v.X);
                case RotationIndex.NegYNegZPosX:
                    return new Vector3(-v.Y, -v.Z, v.X);
                // Z = negative X axis
                case RotationIndex.PosZPosYNegX:
                    return new Vector3(v.Z, v.Y, -v.X);
                case RotationIndex.PosYNegZNegX:
                    return new Vector3(v.Y, -v.Z, -v.X);
                case RotationIndex.NegZNegYNegX:
                    return new Vector3(-v.Z, -v.Y, -v.X);
                case RotationIndex.NegYPosZNegX:
                    return new Vector3(-v.Y, v.Z, -v.X);
            }
            return v;
        }

        private class Beacon
        {
            public int Id { get; set; }
            public Vector3 Position { get; init; }
            public List<int> ToOtherIds { get; set; }
            public List<Vector3> ToOthers { get; set; }

            public Beacon(Vector3 position, IEnumerable<Vector3> beacons, int id)
            {
                Id = -1;
                Position = position;

                ToOtherIds = new List<int>();
                ToOthers = new List<Vector3>();
                foreach (Vector3 beacon in beacons)
                {
                    if (beacon == Position)
                    {
                        continue;
                    }

                    ToOthers.Add((Position - beacon));
                }
            }
        }

        private List<Vector3> GetAllOrientations(Vector3 original)
        {
            List<Vector3> allOrientations = new List<Vector3>();
            for (RotationIndex ri = RotationIndex.Start; ri <= RotationIndex.End; ++ri)
            {
                allOrientations.Add(ToRotationIndex(original, ri));
            }

            return allOrientations;
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables)
        {
            Vector3 testCase = new Vector3(1, 2, 3);
            List<Vector3> asasd = GetAllOrientations(testCase);
            var anotherOne = asasd.Distinct().ToList();

            Func<float, float> ToRadians = (degrees) => (float)(Math.PI / 180.0f * degrees);
            Quaternion rotateZ90Q = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, -ToRadians(90));
            Vector3 rotateZ90 = Vector3.Transform(testCase, rotateZ90Q);
            // return string.Empty;

            List<KeyValuePair<int, Vector3>> curBeacons = new List<KeyValuePair<int, Vector3>>();
            Dictionary<int, List<Beacon>> allBeacons = new Dictionary<int, List<Beacon>>();
            int curId = -1;
            int curBeaconId = -1;
            foreach (string input in inputs)
            {
                if (input.Contains("scanner"))
                {
                    curId = input.Split(" ", StringSplitOptions.RemoveEmptyEntries).Where(i => int.TryParse(i, out int ii)).Select(int.Parse).First();
                }
                else if (!string.IsNullOrEmpty(input))
                {
                    float[] vec3 = input.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(float.Parse).ToArray();
                    curBeacons.Add(new KeyValuePair<int, Vector3>(++curBeaconId, new Vector3(vec3)));
                }
                else
                {
                    allBeacons[curId] = new List<Beacon>();
                    foreach (var pair in curBeacons)
                    {
                        allBeacons[curId].Add(new Beacon(pair.Value, curBeacons.Select(c => c.Value), pair.Key));
                    }
                    curBeacons.Clear();
                }
            }

            Dictionary<int, Beacon> knownBeacons = new Dictionary<int, Beacon>();
            int beaconId = 0;
            foreach (Beacon knownBeacon in allBeacons[0])
            {
                knownBeacon.Id = ++beaconId;
                knownBeacons[beaconId] = knownBeacon;
            }

            foreach (Beacon unknownBeacon in allBeacons[1])
            {
                foreach (Vector3 toOther in unknownBeacon.ToOthers)
                {
                    List<Vector3> allOrientations = GetAllOrientations(toOther);
                    foreach (var knownPairs in knownBeacons)
                    {
                        foreach (Vector3 toOtherOriented in allOrientations)
                        {
                            if (knownPairs.Value.ToOthers.Contains(toOtherOriented))
                            {
                                // something should happen here
                                DebugWriteLine($"Match found! original={knownPairs.Value.Position}, compare={unknownBeacon.Position.ToString()}");
                            }
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