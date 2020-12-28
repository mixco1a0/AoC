using System;
using System.Collections.Generic;

namespace AoC
{
    public class PerfStat
    {
        public double Min { get; set; }
        public double Max { get; set; }
        public double Avg { get; set; }
        public long Count { get; set; }

        public PerfStat()
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

    public class PerfVersion
    {
        public Dictionary<string, PerfStat> VersionData { get; set; }

        public PerfVersion()
        {
            VersionData = new Dictionary<string, PerfStat>();
        }

        public void AddData(string version, double elapsedMs)
        {
            if (version != "v0")
            {
                if (!VersionData.ContainsKey(version))
                {
                    VersionData[version] = new PerfStat();
                }
                VersionData[version].AddData(elapsedMs);
            }
        }

        public PerfStat GetData(string version)
        {
            if (VersionData.ContainsKey(version))
            {
                return VersionData[version];
            }
            return null;
        }
    }

    public class PerfPart
    {
        public Dictionary<TestPart, PerfVersion> PartData { get; set; }

        public PerfPart()
        {
            PartData = new Dictionary<TestPart, PerfVersion>();
        }

        public void AddData(Day day)
        {
            Dictionary<TestPart, double> results = day.TimeResults;
            foreach (var pair in results)
            {
                if (!PartData.ContainsKey(pair.Key))
                {
                    PartData[pair.Key] = new PerfVersion();
                }
                PartData[pair.Key].AddData(day.GetSolutionVersion(pair.Key), pair.Value);
            }
        }

        public PerfStat Get(TestPart testPart, string version)
        {
            if (PartData.ContainsKey(testPart))
            {
                return PartData[testPart].GetData(version);
            }
            return null;
        }
    }

    public class PerfDay
    {
        public Dictionary<string, PerfPart> DayData { get; set; }

        public PerfDay()
        {
            DayData = new Dictionary<string, PerfPart>();
        }

        public void AddData(AoC.Day day)
        {
            if (!DayData.ContainsKey(day.DayName))
            {
                DayData[day.DayName] = new PerfPart();
            }
            DayData[day.DayName].AddData(day);
        }

        public PerfStat Get(string day, TestPart testPart, string version)
        {
            if (DayData.ContainsKey(day))
            {
                return DayData[day].Get(testPart, version);
            }
            return null;
        }
    }

    public class PerfData
    {
        public Dictionary<string, PerfDay> YearData { get; set; }

        public PerfData()
        {
            YearData = new Dictionary<string, PerfDay>();
        }

        public void AddData(AoC.Day day)
        {
            if (!YearData.ContainsKey(day.Year))
            {
                YearData[day.Year] = new PerfDay();
            }
            YearData[day.Year].AddData(day);
        }

        public PerfStat Get(string year, string day, TestPart testPart, string version)
        {
            if (YearData.ContainsKey(year))
            {
                return YearData[year].Get(day, testPart, version);
            }
            return null;
        }
    }
}