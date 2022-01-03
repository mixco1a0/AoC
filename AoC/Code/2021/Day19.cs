using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AoC._2021
{
    class Day19 : Day
    {
        public Day19() { }

        public override string GetSolutionVersion(Part part)
        {
            switch (part)
            {
                case Part.One:
                    return "v1";
                case Part.Two:
                    return "v1";
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
                Output = "3621",
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

        private static Vector3 ToRotationIndex(Vector3 v, RotationIndex targetRotation)
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

        private static readonly int InvalidId = -1;

        private interface IScanner
        {
            public int Id { get; }
            public Vector3 Pos { get; }
            public List<Beacon> Beacons { get; }
        }

        private class Scanner : IScanner
        {
            public int Id { get; set; }
            public Vector3 Pos { get; set; }
            public List<Beacon> Beacons { get; set; }

            public Scanner(int id, List<Beacon> beacons)
            {
                Id = id;
                Pos = Vector3.Zero;
                Beacons = beacons;
            }

            public override string ToString()
            {
                return $"[{Id}] {Pos.ToString()}";
            }
        }

        private class Beacon
        {
            public class Info
            {
                public int Id { get; set; }
                public Vector3 Pos { get; set; }
                public Dictionary<int, Vector3> ToOthers { get; set; }

                public Info()
                {
                    Id = InvalidId;
                    Pos = Vector3.Zero;
                    ToOthers = new Dictionary<int, Vector3>();
                }

                public Info(int id, Vector3 pos, Dictionary<int, Vector3> localBeacons)
                {
                    Id = id;
                    Pos = pos;
                    ToOthers = new Dictionary<int, Vector3>();
                    foreach (KeyValuePair<int, Vector3> pair in localBeacons)
                    {
                        if (Pos != pair.Value)
                        {
                            ToOthers[pair.Key] = Pos - pair.Value;
                        }
                    }
                }

                public Info(Info other)
                {
                    Id = other.Id;
                    Pos = other.Pos;
                    ToOthers = new Dictionary<int, Vector3>(other.ToOthers);
                }

                public override string ToString()
                {
                    return $"{Id,3} [{Pos.ToString()}]";
                }
            }

            public Info Local { get; set; }
            public Info Global { get; set; }

            public Beacon(int localId, Vector3 localPos, Dictionary<int, Vector3> localBeacons)
            {
                Init(localId, localPos, localBeacons);
            }

            public Beacon(Beacon.Info info, Dictionary<int, Vector3> localBeacons)
            {
                Local = new Info(info.Id, info.Pos, localBeacons);
                Global = new Info();
            }

            public void Init(int localId, Vector3 localPos, Dictionary<int, Vector3> localBeacons)
            {
                Local = new Info(localId, localPos, localBeacons);
                Global = new Info();
            }

            public override string ToString()
            {
                if (Global.Id == InvalidId)
                {
                    return $"[L] {Local.ToString()}";
                }
                return $"[G] {Global.ToString()}";
            }
        }

        private Dictionary<RotationIndex, Vector3> GetAllOrientations(Vector3 original, HashSet<RotationIndex> rotations)
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

        private List<Scanner> ParseScanners(List<string> inputs)
        {
            List<Scanner> scanners = new List<Scanner>();
            Dictionary<int, Vector3> localBeacons = new Dictionary<int, Vector3>();
            int curScannerId = InvalidId;
            int curLocalBeaconId = InvalidId;
            foreach (string input in inputs)
            {
                if (input.Contains("scanner"))
                {
                    curScannerId = input.Split(" ", StringSplitOptions.RemoveEmptyEntries).Where(i => int.TryParse(i, out int ii)).Select(int.Parse).First();
                }
                else if (!string.IsNullOrEmpty(input))
                {
                    float[] vec3 = input.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(float.Parse).ToArray();
                    localBeacons[++curLocalBeaconId] = new Vector3(vec3);
                }
                else
                {
                    List<Beacon> scannerBeacons = new List<Beacon>();
                    foreach (var pair in localBeacons)
                    {
                        scannerBeacons.Add(new Beacon(pair.Key, pair.Value, localBeacons));
                    }
                    scanners.Add(new Scanner(curScannerId, scannerBeacons));

                    localBeacons.Clear();
                    curLocalBeaconId = InvalidId;
                }
            }
            return scanners;
        }

        private class CombinedScanner : IScanner
        {
            public class Child
            {
                public IScanner Scanner { get; init; }
                public Vector3 Offset { get; set; }
                public RotationIndex Rotation { get; init; }

                public Child(IScanner scanner, Vector3 offset, RotationIndex rotation)
                {
                    Scanner = scanner;
                    Offset = offset;
                    Rotation = rotation;
                }

                public override string ToString()
                {
                    return $"{Scanner.Id} [{Offset.ToString()}] [{Rotation}]";
                }
            }

            public IScanner BaseScanner { get; set; }
            public int Id { get => BaseScanner.Id; }
            public Vector3 Pos { get => BaseScanner.Pos; }
            public List<Child> Children { get; set; }
            public List<Beacon> Beacons { get; init; }

            public CombinedScanner(IScanner baseScanner)
            {
                BaseScanner = baseScanner;
                Children = new List<Child>();
                if (baseScanner is CombinedScanner)
                {
                    Children.AddRange((baseScanner as CombinedScanner).Children);
                }

                foreach (Beacon beacon in BaseScanner.Beacons)
                {
                    beacon.Global = new Beacon.Info(beacon.Local);
                }
                Beacons = new List<Beacon>(BaseScanner.Beacons);
            }

            public void AddScanner(IScanner childScanner, RotationIndex rotationIndex, Dictionary<int, int> localToGlobalId)
            {
                // get a shared beacon to convert the new scanner into the old scanner's coordinate system
                Beacon newBeacon = childScanner.Beacons.First(b => localToGlobalId.ContainsKey(b.Local.Id));
                Beacon oldBeacon = Beacons.Single(b => b.Global.Id == localToGlobalId[newBeacon.Local.Id]);
                Vector3 newToOldOffset = ToRotationIndex(newBeacon.Local.Pos, rotationIndex);
                Vector3 scannerOffset = oldBeacon.Global.Pos - newToOldOffset;

                // add all of the non shared beacons to the old scanner's coordinate system
                int nextId = Beacons.Max(b => b.Global.Id);
                Dictionary<int, Vector3> allRotatedPos = childScanner.Beacons.ToDictionary(b => b.Local.Id, b => ToRotationIndex(b.Local.Pos, rotationIndex) + scannerOffset);
                foreach (Beacon childBeacon in childScanner.Beacons)
                {
                    if (!localToGlobalId.ContainsKey(childBeacon.Local.Id))
                    {
                        childBeacon.Global = new Beacon.Info(++nextId, ToRotationIndex(childBeacon.Local.Pos, rotationIndex) + scannerOffset, allRotatedPos);
                        Beacons.Add(childBeacon);
                    }
                }

                // add the new scanner as a child scanner
                Children.Add(new Child(childScanner, scannerOffset, rotationIndex));
            }

            public List<Vector3> GetAllScannerPos(Action<string> printFunc)
            {
                return GetAllScannerPos(Vector3.Zero, RotationIndex.PosXPosYPosZ, 0, printFunc);
            }

            public List<Vector3> GetAllScannerPos(Vector3 offset, RotationIndex rotationIndex, int level, Action<string> printFunc)
            {
                List<Vector3> allPos = new List<Vector3>();
                Vector3 pos = ToRotationIndex(BaseScanner.Pos, rotationIndex) + offset;
                printFunc($"*** {new string(' ', level)} Adding [C] [{BaseScanner.Id,2}] @ {pos.ToString()}");
                allPos.Add(pos);
                foreach (Child child in Children)
                {
                    if (child.Scanner is Scanner)
                    {
                        pos = ToRotationIndex(child.Scanner.Pos + child.Offset, rotationIndex) + offset;
                        printFunc($"*** {new string(' ', level + 2)} Adding [S] [{child.Scanner.Id,2}] @ {pos.ToString()}");
                        allPos.Add(pos);
                    }
                    else if (child.Scanner is CombinedScanner)
                    {
                        CombinedScanner childScanner = child.Scanner as CombinedScanner;
                        allPos.AddRange(childScanner.GetAllScannerPos(child.Offset + offset, child.Rotation, level + 2, printFunc));
                    }
                }
                return allPos;
            }

            public void ResetBeacons()
            {
                Dictionary<int, Vector3> allPos = Beacons.ToDictionary(b => b.Global.Id, b => b.Global.Pos);
                foreach (Beacon beacon in Beacons)
                {
                    beacon.Init(beacon.Global.Id, beacon.Global.Pos, allPos);
                }
            }

            public override string ToString()
            {
                return $"[{Id}] {Pos.ToString()}";
            }
        }

        private CombinedScanner CombineScanners(ref List<Scanner> localScanners)
        {
            List<CombinedScanner> combinedScanners = new List<CombinedScanner>();
            Queue<IScanner> pendingScanners = new Queue<IScanner>(localScanners);

            // DebugWriteLine($"Starting new scanner combinations [{pendingScanners.Count + 1,2} pending...]");
            // DebugWriteLine($"Matching scanner [{pendingScanners.Peek().Id,2}]");
            combinedScanners.Add(new CombinedScanner(pendingScanners.Dequeue()));

            CombinedScanner currentCombinedScanner = combinedScanners.First();
            HashSet<int> skippedScanners = new HashSet<int>();
            while (pendingScanners.Count > 0)
            {
                IScanner pendingScanner = pendingScanners.Dequeue();
                RotationIndex rotationToBase = RotationIndex.Invalid;
                Dictionary<int, int> localToGlobalId = new Dictionary<int, int>();
                Dictionary<int, HashSet<int>> localToGlobalIds = new Dictionary<int, HashSet<int>>();

                // find potentially matching beacons
                foreach (Beacon beacon in pendingScanner.Beacons)
                {
                    HashSet<int> potentialIds = new HashSet<int>();
                    HashSet<int> connectedIds = new HashSet<int>();
                    HashSet<RotationIndex> matchingRotations = new HashSet<RotationIndex>();
                    foreach (var toOther in beacon.Local.ToOthers)
                    {
                        Dictionary<RotationIndex, Vector3> allOrientations = GetAllOrientations(toOther.Value, matchingRotations);
                        foreach (Beacon potentialMatch in currentCombinedScanner.Beacons)
                        {
                            IEnumerable<Vector3> matchingVectors = potentialMatch.Global.ToOthers.Select(to => to.Value).Intersect(allOrientations.Values).Distinct();
                            if (matchingVectors.Any())
                            {
                                potentialIds.Add(potentialMatch.Global.Id);
                                IEnumerable<int> matchingIds = potentialMatch.Global.ToOthers.Where(to => matchingVectors.Contains(to.Value)).Select(to => to.Key);
                                connectedIds = connectedIds.Union(matchingIds).ToHashSet();
                                IEnumerable<RotationIndex> rotations = allOrientations.Where(ao => matchingVectors.Contains(ao.Value)).Select(ao => ao.Key);
                                if (matchingRotations.Any())
                                {
                                    matchingRotations = matchingRotations.Intersect(rotations).ToHashSet();
                                }
                                else
                                {
                                    matchingRotations = rotations.ToHashSet();
                                }
                            }
                        }
                    }

                    if (potentialIds.Count == 1)
                    {
                        localToGlobalId[beacon.Local.Id] = potentialIds.First();
                        rotationToBase = matchingRotations.First();
                    }
                    else if (potentialIds.Count > 0)
                    {
                        localToGlobalIds[beacon.Local.Id] = new HashSet<int>(potentialIds);
                    }
                }

                if (localToGlobalId.Count < 12)
                {
                    while (true)
                    {
                        int prevIdCount = localToGlobalId.Count;
                        foreach (var curIdPair in localToGlobalIds)
                        {
                            curIdPair.Value.RemoveWhere(idPair => localToGlobalId.ContainsValue(idPair));
                        }
                        var toTrim = localToGlobalIds.Where(idPair => idPair.Value.Count == 1).ToList();
                        foreach (var matchedPair in toTrim)
                        {
                            localToGlobalId[matchedPair.Key] = matchedPair.Value.First();
                            localToGlobalIds.Remove(matchedPair.Key);
                        }

                        if (prevIdCount == localToGlobalId.Count)
                        {
                            break;
                        }
                    }
                }

                if (localToGlobalId.Count >= 12 && localToGlobalIds.Count == 0)
                {
                    currentCombinedScanner.AddScanner(pendingScanner, rotationToBase, localToGlobalId);
                    localScanners.RemoveAll(ls => ls.Id == pendingScanner.Id);
                    // DebugWriteLine($"\tAdding scanner [{pendingScanner.Id,2}] to scanner {currentCombinedScanner.Id} [{currentCombinedScanner.Beacons.Count} total beacons]");

                    skippedScanners.Clear();
                }
                else
                {
                    if (skippedScanners.Contains(pendingScanner.Id))
                    {
                        combinedScanners.Add(new CombinedScanner(pendingScanner));

                        if (pendingScanners.Count == 0 && combinedScanners.Count > 1)
                        {
                            // DebugWriteLine("");
                            // DebugWriteLine("******************");
                            // DebugWriteLine("*** VectorDump ***");
                            // DebugWriteLine("******************");
                            foreach (CombinedScanner combined in combinedScanners.OrderBy(cs => cs.Beacons.Count))
                            {
                                combined.ResetBeacons();
                                // combined.GetAllScannerPos(DebugWriteLine);
                                pendingScanners.Enqueue(combined);
                            }
                            // DebugWriteLine("******************");
                            // DebugWriteLine("*** VectorDump ***");
                            // DebugWriteLine("******************");

                            combinedScanners.Clear();
                            combinedScanners.Add(new CombinedScanner(pendingScanners.Dequeue()));
                            // DebugWriteLine("");
                            // DebugWriteLine(".");
                            // DebugWriteLine($"All scanner combinations completed. Restarting process.");
                            // DebugWriteLine($"Starting new match process. [{pendingScanners.Count + 1,2} pending...]");
                            // DebugWriteLine("");
                        }
                        else
                        {
                            // DebugWriteLine("");
                            // DebugWriteLine($"Starting new scanner combinations [{pendingScanners.Count + 1,2} pending...]");
                        }
                        currentCombinedScanner = combinedScanners.Last();
                        skippedScanners.Clear();
                        // DebugWriteLine($"Matching scanner [{currentCombinedScanner.Id,2}]");
                    }
                    else
                    {
                        // DebugWriteLine($"\t\t[skipping scanner [{pendingScanner.Id,2}]]");
                        skippedScanners.Add(pendingScanner.Id);
                        pendingScanners.Enqueue(pendingScanner);
                    }
                }
            }
            return combinedScanners.First();
        }

        private int ProcessScanners(List<Scanner> scanners, bool countBeacons)
        {
            CombinedScanner combinedScanners = CombineScanners(ref scanners);
            if (countBeacons)
            {
                return combinedScanners.Beacons.Count;
            }

            Func<Vector3, Vector3, int> ManhattenDistance = (a, b) =>
            {
                return Math.Abs((int)(a.X - b.X)) + Math.Abs((int)(a.Y - b.Y)) + Math.Abs((int)(a.Z - b.Z));
            };

            int maxDist = int.MinValue;
            List<Vector3> allScannerPos = combinedScanners.GetAllScannerPos((string x) => {});
            foreach (Vector3 pos1 in allScannerPos)
            {
                foreach (Vector3 pos2 in allScannerPos)
                {
                    if (pos1 != pos2)
                    {
                        maxDist = Math.Max(maxDist, ManhattenDistance(pos1, pos2));
                    }
                }
            }
            return maxDist;
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool countBeacons)
        {
            List<Scanner> scanners = ParseScanners(inputs);
            int value = ProcessScanners(scanners, countBeacons);
            return value.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);
    }
}