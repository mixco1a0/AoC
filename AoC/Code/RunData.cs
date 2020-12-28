using System;
using System.Collections.Generic;

namespace AoC
{
    public class Stats
    {
        public double Min { get; set; }
        public double Max { get; set; }
        public double Avg { get; set; }
        public long Count { get; set; }

        public Stats()
        {
            Min = double.MaxValue;
            Max = double.MinValue;
            Avg = 0.0;
            Count = 0;
        }

        public void AddData(double elapsedMs)
        {
            Min = Math.Min(Min, elapsedMs);
            Max = Math.Max(Max, elapsedMs);

            double avg = Avg * (double)Count;
            avg += elapsedMs;
            ++Count;
            Avg = avg / (double)Count;
        }
    }

    public class Parts
    {
        public Dictionary<TestPart, Stats> StatData { get; set; }

        public Parts()
        {
            StatData = new Dictionary<TestPart, Stats>();
        }

        public void AddData(TestPart testPart, double elapsedMs)
        {
            if (!StatData.ContainsKey(testPart))
            {
                StatData[testPart] = new Stats();
            }
            StatData[testPart].AddData(elapsedMs);
        }

        public Stats Get(TestPart testPart)
        {
            if (StatData.ContainsKey(testPart))
            {
                return StatData[testPart];
            }
            return null;
        }
    }

    public class Versions
    {
        public Dictionary<string, Parts> PartData { get; set; }

        public Versions()
        {
            PartData = new Dictionary<string, Parts>();
        }

        public void AddData(AoC.Day day)
        {
            Dictionary<TestPart, double> results = day.TimeResults;
            foreach (var pair in results)
            {
                string version = day.GetSolutionVersion(pair.Key);
                if (version == "v0")
                {
                    continue;
                }

                if (!PartData.ContainsKey(version))
                {
                    PartData[version] = new Parts();
                }
                PartData[version].AddData(pair.Key, pair.Value);
            }
        }

        public Stats Get(string version, TestPart testPart)
        {
            if (PartData.ContainsKey(version))
            {
                return PartData[version].Get(testPart);
            }
            return null;
        }
    }

    public class Days
    {
        public Dictionary<string, Versions> DayData { get; set; }

        public Days()
        {
            DayData = new Dictionary<string, Versions>();
        }

        public void AddData(AoC.Day day)
        {
            if (!DayData.ContainsKey(day.DayName))
            {
                DayData[day.DayName] = new Versions();
            }
            DayData[day.DayName].AddData(day);
        }

        public Stats Get(string day, string version, TestPart testPart)
        {
            if (DayData.ContainsKey(day))
            {
                return DayData[day].Get(version, testPart);
            }
            return null;
        }
    }

    public class RunData
    {
        public Dictionary<string, Days> YearData { get; set; }

        public RunData()
        {
            YearData = new Dictionary<string, Days>();
        }

        public void AddData(AoC.Day day)
        {
            if (!YearData.ContainsKey(day.Year))
            {
                YearData[day.Year] = new Days();
            }
            YearData[day.Year].AddData(day);
        }

        public Stats Get(string year, string day, string version, TestPart testPart)
        {
            if (YearData.ContainsKey(year))
            {
                return YearData[year].Get(day, version, testPart);
            }
            return null;
        }
    }
}