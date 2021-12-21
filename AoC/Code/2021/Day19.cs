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

        private void RotateZAndAdd(ref List<Vector3> allOrientations, Vector3 original)
        {
            allOrientations.Add(original);
            allOrientations.Add(new Vector3(original.Y, -original.X, original.Z));
            allOrientations.Add(new Vector3(-original.X, -original.Y, original.Z));
            allOrientations.Add(new Vector3(-original.Y, original.X, original.Z));
        }

        private List<Vector3> GetAllOrientations(Vector3 original)
        {
            Vector3 newOrientation;
            List<Vector3> allOrientations = new List<Vector3>();

            // Z stays positive
            RotateZAndAdd(ref allOrientations, original);
            newOrientation = new Vector3(original.X, -original.Z, original.Y);
            RotateZAndAdd(ref allOrientations, newOrientation);
            newOrientation = new Vector3(original.X, -original.Y, -original.Z);
            RotateZAndAdd(ref allOrientations, newOrientation);
            newOrientation = new Vector3(original.X, original.Z, -original.Y);
            RotateZAndAdd(ref allOrientations, newOrientation);
            newOrientation = new Vector3(-original.Z, original.Y, original.X);
            RotateZAndAdd(ref allOrientations, newOrientation);
            newOrientation = new Vector3(original.Z, original.Y, -original.X);
            RotateZAndAdd(ref allOrientations, newOrientation);

            return allOrientations;
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables)
        {
            // Vector3 testCase = new Vector3(-2, -3, 1);
            // List<Vector3> allOrientations = GetAllOrientations(testCase);

            // Func<float, float> ToRadians = (degrees) => (float)(Math.PI / 180.0f * degrees);
            // Quaternion rotateZ90Q = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, -ToRadians(90));
            // Vector3 rotateZ90 = Vector3.Transform(testCase, rotateZ90Q);
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

            // Func<float, float> ToRadians = (degrees) => (float)(Math.PI / 180.0f * degrees);

            // Vector3 testVector = new Vector3(8, 0, 7);
            // List<Vector3> testVectors = new List<Vector3>();
            // testVectors.Add(testVector);
            // for (int x = 0; x < 4; ++x)
            // {
            //     Vector3 xRotate = Vector3.Transform(testVector, new Quaternion(ToRadians(x * 90), 0, 0, 0));
            //     testVectors.Add(xRotate);
            //     for (int y = 0; y < 4; ++y)
            //     {
            //         Vector3 yRotate = Vector3.Transform(testVector, new Quaternion(0, ToRadians(y * 90), 0, 0));
            //         testVectors.Add(xRotate);
            //         for (int z = 0; z < 4; ++z)
            //         {
            //             Vector3 zRotate = Vector3.Transform(testVector, new Quaternion(0, 0, ToRadians(z * 90), 0));
            //             testVectors.Add(zRotate);
            //         }
            //     }
            // }
            // HashSet<float> testUnion = vecs.First().Value.Union(vecs.Skip(1).First().Value).ToHashSet();
            return string.Empty;
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);
    }
}