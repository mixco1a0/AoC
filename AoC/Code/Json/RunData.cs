using System.Collections.Generic;

namespace AoC.Json
{
    public class Stats
    {
        public double Min { get; set; }
        public double Max { get; set; }
        public double Avg { get; set; }
        public long Count { get; set; }
    }

    public class VersionData
    {
        public Dictionary<string, Stats> Data { get; set; }
    }

    public class YearData
    {
        public Dictionary<string, VersionData> Data { get; set; }
    }

    public class RunData
    {
        List<YearData> Data { get; set; }
    }
}