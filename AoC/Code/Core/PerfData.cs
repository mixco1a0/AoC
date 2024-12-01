using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC.Core
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

            double avg = Avg * Count;
            avg += elapsedMs;
            ++Count;
            Avg = avg / Count;
        }
    }

    public class PerfVersion
    {
        public Dictionary<string, PerfStat> VersionData { get; set; }

        public PerfVersion()
        {
            VersionData = [];
        }

        public void AddData(string version, double elapsedMs)
        {
            if (version != Day.BaseVersion)
            {
                if (!VersionData.TryGetValue(version, out PerfStat value))
                {
                    value = new PerfStat();
                    VersionData[version] = value;
                }

                value.AddData(elapsedMs);
            }
            VersionData = VersionData.OrderByDescending(pair => pair.Key).ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        public PerfStat GetData(string version)
        {
            if (VersionData.TryGetValue(version, out PerfStat value))
            {
                return value;
            }
            return null;
        }
    }

    public class PerfPart
    {
        public Dictionary<Part, PerfVersion> PartData { get; set; }

        public PerfPart()
        {
            PartData = [];
        }

        public void AddData(Day day)
        {
            Dictionary<Part, double> results = day.TimeResults;
            foreach (var pair in results)
            {
                if (!PartData.TryGetValue(pair.Key, out PerfVersion value))
                {
                    value = new PerfVersion();
                    PartData[pair.Key] = value;
                }

                value.AddData(day.GetSolutionVersion(pair.Key), pair.Value);
            }
            PartData = PartData.OrderBy(pair => pair.Key).ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        public PerfStat Get(Part part, string version)
        {
            if (PartData.TryGetValue(part, out PerfVersion value))
            {
                return value.GetData(version);
            }
            return null;
        }
    }

    public class PerfDay
    {
        public Dictionary<string, PerfPart> DayData { get; set; }

        public PerfDay()
        {
            DayData = [];
        }

        public void AddData(Day day)
        {
            if (!DayData.TryGetValue(day.DayName, out PerfPart value))
            {
                value = new PerfPart();
                DayData[day.DayName] = value;
            }

            value.AddData(day);
            DayData = DayData.OrderByDescending(pair => pair.Key).ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        public PerfStat Get(string day, Part part, string version)
        {
            if (DayData.TryGetValue(day, out PerfPart value))
            {
                return value.Get(part, version);
            }
            return null;
        }
    }

    public class PerfData
    {
        public Dictionary<string, PerfDay> YearData { get; set; }

        public PerfData()
        {
            YearData = [];
        }

        public void AddData(Day day)
        {
            if (!YearData.TryGetValue(day.Year, out PerfDay value))
            {
                value = new PerfDay();
                YearData[day.Year] = value;
            }

            value.AddData(day);
            YearData = YearData.OrderByDescending(pair => pair.Key).ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        public PerfStat Get(string year, string day, Part part, string version)
        {
            if (YearData.TryGetValue(year, out PerfDay value))
            {
                return value.Get(day, part, version);
            }
            return null;
        }
    }
}