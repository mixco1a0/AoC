using System.Text;
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
                Output = "79",
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

--- scanner 2 ---
649,640,665
682,-795,504
-784,533,-524
-644,584,-595
-588,-843,648
-30,6,44
-674,560,763
500,723,-460
609,671,-379
-555,-800,653
-675,-892,-343
697,-426,-610
578,704,681
493,664,-388
-671,-858,530
-667,343,800
571,-461,-707
-138,-166,112
-889,563,-600
646,-828,498
640,759,510
-630,509,768
-681,-892,-333
673,-379,-804
-742,-814,-386
577,-820,562

--- scanner 3 ---
-589,542,597
605,-692,669
-500,565,-823
-660,373,557
-458,-679,-417
-488,449,543
-626,468,-788
338,-750,-386
528,-832,-391
562,-778,733
-938,-730,414
543,643,-506
-524,371,-870
407,773,750
-104,29,83
378,-903,-323
-778,-728,485
426,699,580
-438,-605,-362
-469,-447,-387
509,732,623
647,635,-688
-868,-804,481
614,-800,639
595,780,-596

--- scanner 4 ---
727,592,562
-293,-554,779
441,611,-461
-714,465,-776
-743,427,-804
-660,-479,-426
832,-632,460
927,-485,-438
408,393,-506
466,436,-512
110,16,151
-258,-428,682
-393,719,612
-211,-452,876
808,-476,-593
-575,615,604
-485,667,467
-680,325,-822
-627,-443,-432
872,-547,-609
833,512,582
807,604,487
839,-516,451
891,-625,532
-652,-548,-490
30,-46,-14
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
            Invalid = -1,
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
            public static readonly int InvalidId = -1;

            public class Info
            {
                public int Id { get; set; }
                public Vector3 Pos { get; set; }
                public List<KeyValuePair<int, Vector3>> ToOthers { get; set; }

                public Info()
                {
                    Id = InvalidId;
                    Pos = Vector3.Zero;
                    ToOthers = new List<KeyValuePair<int, Vector3>>();
                }

                public Info(int id, Vector3 pos, List<KeyValuePair<int, Vector3>> localBeacons)
                {
                    Id = id;
                    Pos = pos;
                    ToOthers = new List<KeyValuePair<int, Vector3>>();
                    foreach (KeyValuePair<int, Vector3> pair in localBeacons)
                    {
                        if (Pos != pair.Value)
                        {
                            ToOthers.Add(new KeyValuePair<int, Vector3>(pair.Key, Pos - pair.Value));
                        }
                    }
                }

                public Info(Info other)
                {
                    Id = other.Id;
                    Pos = other.Pos;
                    ToOthers = new List<KeyValuePair<int, Vector3>>(other.ToOthers);
                }

                public override string ToString()
                {
                    return $"{Id,3} [{Pos.ToString()}]";
                }
            }

            public Info Local { get; init; }
            public Info Global { get; set; }

            public Beacon(int localId, Vector3 localPos, List<KeyValuePair<int, Vector3>> localBeacons)
            {
                Local = new Info(localId, localPos, localBeacons);
                Global = new Info();
            }

            public Beacon(Beacon.Info info, List<KeyValuePair<int, Vector3>> localBeacons)
            {
                Local = new Info(info.Id, info.Pos, localBeacons);
                Global = new Info();
            }

            public override string ToString()
            {
                if (Global.Id == InvalidId)
                {
                    return Local.ToString();
                }
                return Global.ToString();
            }
        }

        private Dictionary<RotationIndex, Vector3> GetAllOrientations(Vector3 original, List<RotationIndex> rotations)
        {
            Dictionary<RotationIndex, Vector3> allRotations = new Dictionary<RotationIndex, Vector3>();
            for (RotationIndex ri = RotationIndex.Start; ri <= RotationIndex.End; ++ri)
            {
                if (rotations.Count == 0 || rotations.Contains(ri))
                {
                    allRotations[ri] = ToRotationIndex(original, ri);
                }
            }

            return allRotations;
        }

        private Dictionary<int, List<Beacon>> GetLocalScannerData(List<string> inputs)
        {
            List<KeyValuePair<int, Vector3>> localBeacons = new List<KeyValuePair<int, Vector3>>();
            Dictionary<int, List<Beacon>> allScannerData = new Dictionary<int, List<Beacon>>();
            int curScannerId = Beacon.InvalidId;
            int curLocalBeaconId = Beacon.InvalidId;
            foreach (string input in inputs)
            {
                if (input.Contains("scanner"))
                {
                    curScannerId = input.Split(" ", StringSplitOptions.RemoveEmptyEntries).Where(i => int.TryParse(i, out int ii)).Select(int.Parse).First();
                }
                else if (!string.IsNullOrEmpty(input))
                {
                    float[] vec3 = input.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(float.Parse).ToArray();
                    localBeacons.Add(new KeyValuePair<int, Vector3>(++curLocalBeaconId, new Vector3(vec3)));
                }
                else
                {
                    allScannerData[curScannerId] = new List<Beacon>();
                    foreach (var pair in localBeacons)
                    {
                        allScannerData[curScannerId].Add(new Beacon(pair.Key, pair.Value, localBeacons));
                    }

                    localBeacons.Clear();
                    curLocalBeaconId = Beacon.InvalidId;
                }
            }
            return allScannerData;
        }

        private void PromoteToGlobal(int scannerId, ref Dictionary<int, List<Beacon>> localScanners, ref Dictionary<int, Beacon> globalBeacons, RotationIndex ri, Dictionary<int, int> localToGlobalId)
        {
            List<Beacon> beacons = localScanners[scannerId];
            if (globalBeacons.Count == 0)
            {
                foreach (Beacon beacon in beacons)
                {
                    beacon.Global = new Beacon.Info(beacon.Local);
                    globalBeacons.Add(beacon.Local.Id, beacon);
                }
            }
            else
            {
                int nextGlobalIdx = globalBeacons.Keys.Max();
                List<KeyValuePair<int, Vector3>> rotatedPos = beacons.Select(l => new KeyValuePair<int, Vector3>(l.Local.Id, ToRotationIndex(l.Local.Pos, ri))).ToList();
                foreach (Beacon beacon in beacons.Where(b => localToGlobalId.ContainsKey(b.Local.Id)))
                {
                    beacon.Global = new Beacon.Info(localToGlobalId[beacon.Local.Id], ToRotationIndex(beacon.Local.Pos, ri), rotatedPos);
                }
                foreach (Beacon beacon in beacons.Where(b => !localToGlobalId.ContainsKey(b.Local.Id)))
                {
                    beacon.Global = new Beacon.Info(++nextGlobalIdx, ToRotationIndex(beacon.Local.Pos, ri), rotatedPos);
                    globalBeacons.Add(beacon.Global.Id, beacon);
                }
            }
            localScanners.Remove(scannerId);

            DebugWriteLine($"Adding scanner [{scannerId,2}] to globals [{globalBeacons.Count} total beacons]");
        }

        private bool CombineScanners(ref Dictionary<int, Beacon> globalBeacons, ref Dictionary<int, List<Beacon>> localScanners)
        {
            List<KeyValuePair<int, List<Beacon>>> sortedPendingScanners = new List<KeyValuePair<int, List<Beacon>>>(localScanners);
            sortedPendingScanners = sortedPendingScanners.OrderBy(s => s.Value.Count).ToList();
            PromoteToGlobal(sortedPendingScanners[0].Key, ref localScanners, ref globalBeacons, RotationIndex.PosXPosYPosZ, null);
            sortedPendingScanners = new List<KeyValuePair<int, List<Beacon>>>(localScanners);
            sortedPendingScanners = sortedPendingScanners.OrderBy(s => s.Value.Count).ToList();
            Queue<KeyValuePair<int, List<Beacon>>> pendingScanners = new Queue<KeyValuePair<int, List<Beacon>>>(sortedPendingScanners);

            HashSet<int> pendingScannerIds = new HashSet<int>();
            HashSet<int> skippedScanners = new HashSet<int>();
            while (pendingScanners.Count > 0)
            {
                KeyValuePair<int, List<Beacon>> localScanner = pendingScanners.Dequeue();
                {
                    int scannerId = localScanner.Key;
                    List<Beacon> localBeacons = localScanner.Value;

                    // DebugWriteLine($"Checking scanner [{scannerId,2}] ({pendingScanners.Count + 1,3} pending scanners)");

                    RotationIndex rotationToGlobal = RotationIndex.Invalid;
                    Dictionary<int, int> localToGlobalId = new Dictionary<int, int>();
                    Dictionary<int, List<int>> localToGlobalIds = new Dictionary<int, List<int>>();
                    foreach (Beacon localBeacon in localBeacons)
                    {
                        List<int> potentialIds = new List<int>();
                        List<int> matchingConnectedIds = new List<int>();
                        List<RotationIndex> matchingRotations = new List<RotationIndex>();
                        foreach (KeyValuePair<int, Vector3> toOther in localBeacon.Local.ToOthers)
                        {
                            Dictionary<RotationIndex, Vector3> allOrientations = GetAllOrientations(toOther.Value, matchingRotations);
                            foreach (KeyValuePair<int, Beacon> globalBeacon in globalBeacons)
                            {
                                List<Vector3> shared = globalBeacon.Value.Global.ToOthers.Select(to => to.Value).Intersect(allOrientations.Values).Distinct().ToList();
                                if (shared.Count > 0)
                                {
                                    potentialIds.Add(globalBeacon.Key);
                                    IEnumerable<int> ids = globalBeacon.Value.Global.ToOthers.Where(to => shared.Contains(to.Value)).Select(to => to.Key);
                                    if (matchingConnectedIds.Count == 0)
                                    {
                                        matchingConnectedIds = ids.ToList();
                                    }
                                    else
                                    {
                                        matchingConnectedIds = matchingConnectedIds.Union(ids).Distinct().ToList();
                                    }

                                    IEnumerable<RotationIndex> rot = allOrientations.Where(pair => shared.Contains(pair.Value)).Select(pair => pair.Key);
                                    if (matchingRotations.Count == 0)
                                    {
                                        matchingRotations = rot.ToList();
                                    }
                                    else
                                    {
                                        matchingRotations = matchingRotations.Intersect(rot).ToList();
                                    }
                                }
                            }
                        }

                        int potentialMatches = potentialIds.Distinct().Count();
                        if (potentialMatches == 1)
                        {
                            // DebugWriteLine($"Beacon [L{localBeacon.Local.Id,3}] matches global beacon [G{potentialIds.First(),3}]");
                            localToGlobalId[localBeacon.Local.Id] = potentialIds.First();
                            rotationToGlobal = matchingRotations.First();
                        }
                        else if (potentialMatches == 0)
                        {
                            // DebugWriteLine($"Beacon [L{localBeacon.Local.Id,3}] no matches");
                        }
                        else
                        {
                            localToGlobalIds[localBeacon.Local.Id] = new List<int>(potentialIds.Distinct());
                            StringBuilder sb = new StringBuilder($"Beacon [L{localBeacon.Local.Id,3}] matches {potentialMatches} global beacons [");
                            foreach (int pid in potentialIds.Distinct())
                            {
                                sb.Append($"G{pid,3} |");
                            }
                            sb[sb.Length - 1] = ']';
                            // DebugWriteLine(sb.ToString());
                        }
                    }

                    int preReduction = localToGlobalId.Count;
                    // DebugWriteLine($"Scanner [{scannerId,2}] matched {localToGlobalId.Count} globals");
                    bool complete = localToGlobalId.Count >= 12;
                    int globalIdCount = localToGlobalId.Count;
                    while (!complete)
                    {
                        int prevGlobalIdCount = localToGlobalId.Count;
                        foreach (KeyValuePair<int, List<int>> pair in localToGlobalIds)
                        {
                            pair.Value.RemoveAll(id => localToGlobalId.ContainsValue(id));
                        }
                        foreach (KeyValuePair<int, List<int>> pair in localToGlobalIds.Where(p => p.Value.Count == 1))
                        {
                            localToGlobalId[pair.Key] = pair.Value.First();
                        }
                        complete = globalIdCount == localToGlobalId.Count;
                        globalIdCount = localToGlobalId.Count;
                    }

                    if (localToGlobalId.Count != preReduction)
                    {
                        // DebugWriteLine($"Scanner [{scannerId,2}] matches {localToGlobalId.Count} globals [logic to get {localToGlobalId.Count - preReduction}]");
                    }
                    if (localToGlobalId.Count >= 12 && localToGlobalIds.Count == 0)
                    {
                        PromoteToGlobal(scannerId, ref localScanners, ref globalBeacons, rotationToGlobal, localToGlobalId);
                        skippedScanners.Clear();
                    }
                    else
                    {
                        if (skippedScanners.Contains(localScanner.Key))
                        {
                            return false;
                        }
                        skippedScanners.Add(localScanner.Key);
                        pendingScanners.Enqueue(localScanner);
                    }
                }
            }
            return true;
        }

        private int ProcessScanners(Dictionary<int, List<Beacon>> localScanners)
        {
            Dictionary<int, List<Beacon>> combinedScanners = new Dictionary<int, List<Beacon>>();
            List<Dictionary<int, Beacon>> combinedGlobalBeacons = new List<Dictionary<int, Beacon>>();
            while (localScanners.Count > 0)
            {
                DebugWriteLine($"Attempting to combine [{localScanners.Count,3}] scanners together");
                Dictionary<int, Beacon> curGlobalBeacons = new Dictionary<int, Beacon>();
                combinedGlobalBeacons.Add(curGlobalBeacons);
                if (CombineScanners(ref curGlobalBeacons, ref localScanners))
                {
                    if (combinedGlobalBeacons.Count == 1)
                    {
                        return combinedGlobalBeacons.First().Count;
                    }

                    DebugWriteLine($"Failed to combine [{localScanners.Count,3}] scanners");

                    // turn all global data into local, and try again
                    localScanners.Clear();
                    int curScannerGroupingIdx = Beacon.InvalidId;
                    foreach (Dictionary<int, Beacon> globalBeacons in combinedGlobalBeacons)
                    {
                        localScanners[++curScannerGroupingIdx] = new List<Beacon>();
                        List<KeyValuePair<int, Vector3>> asList = globalBeacons.ToList().Select(lb => new KeyValuePair<int, Vector3>(lb.Key, lb.Value.Global.Pos)).ToList();
                        foreach (KeyValuePair<int, Beacon> pair in globalBeacons)
                        {
                            localScanners[curScannerGroupingIdx].Add(new Beacon(pair.Value.Global, asList));
                        }
                    }
                    combinedGlobalBeacons.Clear();
                }
                else
                {
                    DebugWriteLine($"Failed to combine [{localScanners.Count,3}] scanners");
                }
            }

            return combinedGlobalBeacons.First().Count;
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables)
        {
            Dictionary<int, List<Beacon>> localScanners = GetLocalScannerData(inputs);
            int beaconCount = ProcessScanners(localScanners);
            return beaconCount.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);
    }
}