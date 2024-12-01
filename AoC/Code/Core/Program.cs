using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using System.Text.RegularExpressions;
using System.Threading;

using Newtonsoft.Json;

namespace AoC.Core
{
    class Program
    {
        private int m_curProcessor = 0;

        private const long DefaultRecordCount = 1000;
        private readonly long m_recordCount = DefaultRecordCount;
        private long RecordCount { get { return m_recordCount; } }

        private const long DefaultMaxPerfTimeoutMs = 3600000;
        private const long MaxPerfTimeoutPerCoreMs = 150000;
        private readonly long m_maxPerfTimeoutMs = DefaultMaxPerfTimeoutMs;
        private long MaxPerfTimeMs { get { return m_maxPerfTimeoutMs; } }

        private Config Args { get; set; }

        public Program(string[] args)
        {
            try
            {
                Day.UseLogs = true;

                // parse command args
                Args = ParseConfig(args);

                if (Args.Has(Config.ESupportedArgument.Help))
                {
                    Args.PrintHelp();
                    return;
                }

                // get the number of records to keep for perf tests
                if (Args.HasValue(Config.ESupportedArgument.PerfRecordCount))
                {
                    m_recordCount = long.Parse(Args[Config.ESupportedArgument.PerfRecordCount]);
                }

                // get the timeout runs should adhere to
                if (Args.HasValue(Config.ESupportedArgument.PerfTimeout))
                {
                    m_maxPerfTimeoutMs = long.Parse(Args[Config.ESupportedArgument.PerfTimeout]);
                }

                // get the namespace to use
                string baseNamespace = GetLatestNamespace();
                if (Args.HasValue(Config.ESupportedArgument.Namespace))
                {
                    baseNamespace = Args[Config.ESupportedArgument.Namespace];
                }

                // run the day specified or the latest day
                if (Args.HasValue(Config.ESupportedArgument.Day))
                {
                    Day day = RunDay(baseNamespace, Args[Config.ESupportedArgument.Day]);
                    if (day == null)
                    {
                        Log.WriteLine(Log.ELevel.Error, $"Unable to find {baseNamespace}.{Args[Config.ESupportedArgument.Day]}");
                    }
                    else
                    {
                        Log.WriteLine(Log.ELevel.Info, "");
                    }
                }
                else if (!Args.Has(Config.ESupportedArgument.SkipLatest))
                {
                    Day latestDay = RunLatestDay(baseNamespace);
                    if (latestDay == null)
                    {
                        Log.WriteLine(Log.ELevel.Error, $"Unable to find any solutions for namespace {baseNamespace}");
                    }
                    else
                    {
                        Log.WriteLine(Log.ELevel.Info, "");
                    }
                }
                else if (Args.HasValue(Config.ESupportedArgument.RunNamespace))
                {
                    Args.Remove(Config.ESupportedArgument.ForceTests);
                    string runNamespace = Args[Config.ESupportedArgument.RunNamespace];
                    Dictionary<string, Type> days = GetDaysInNamespace(runNamespace);
                    if (days.Count == 0)
                    {
                        Log.WriteLine(Log.ELevel.Error, $"Unable to find any solutions for namespace {runNamespace}");
                    }
                    else
                    {
                        Log.WriteLine(Log.ELevel.Info, $"Running all {runNamespace} Advent of Code solutions\n");
                        Util.Timer timer = new();
                        timer.Start();
                        foreach (var pair in days)
                        {
                            Day day = RunDay(runNamespace, pair.Key, true /*forceSkipWarmup*/);
                            Log.WriteLine(Log.ELevel.Info, "...");
                            Log.WriteLine(Log.ELevel.Info, "..");
                            Log.WriteLine(Log.ELevel.Info, ".");
                        }
                        timer.Stop();
                        Log.WriteLine(Log.ELevel.Info, $"Ran all {runNamespace} Advent of Code solutions in {timer.GetElapsedMs()} (ms)\n");
                    }
                }
                else if (Args.Has(Config.ESupportedArgument.RunAll))
                {
                    List<Dictionary<string, Type>> allDays = GetAllDays();
                    if (allDays.Count == 0)
                    {
                        Log.WriteLine(Log.ELevel.Error, $"Unable to find any solutions");
                    }
                    else
                    {
                        Log.WriteLine(Log.ELevel.Info, $"Running all Advent of Code solutions\n");
                        Util.Timer timer = new();
                        timer.Start();
                        foreach (var dict in allDays)
                        {
                            Log.WriteLine(Log.ELevel.Info, $"Running all {dict.First().Value.Namespace} Advent of Code solutions\n");
                            foreach (var pair in dict)
                            {
                                Day day = RunDay(pair.Value.Namespace, pair.Key, true /*forceSkipWarmup*/);
                                Log.WriteLine(Log.ELevel.Info, "...");
                                Log.WriteLine(Log.ELevel.Info, "..");
                                Log.WriteLine(Log.ELevel.Info, ".");
                            }
                        }
                        timer.Stop();
                        Log.WriteLine(Log.ELevel.Info, $"Ran all Advent of Code solutions in {timer.GetElapsedMs()} (ms)\n");
                    }
                }

                // show performance
                if (Args.Has(Config.ESupportedArgument.ShowPerf))
                {
                    ShowPerformance(baseNamespace);
                }
                // run performance tests
                else if (Args.Has(Config.ESupportedArgument.RunPerf))
                {
                    RunPerformance(baseNamespace);
                }
            }
            catch (Exception e)
            {
                Log.WriteException(e);
            }
        }

        /// <summary>
        /// Parse all config options
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private static Config ParseConfig(string[] args)
        {
            Config config = [];
            config.Init(args);
            return config;
        }

        /// <summary>
        /// Get the latest year for the namespace
        /// </summary>
        /// <returns></returns>
        private string GetLatestNamespace()
        {
            string regex = string.Concat(nameof(AoC), @"\._(\d{4})");
            return Assembly.GetExecutingAssembly().GetTypes()
                        .Select(t => Regex.Match(t.FullName, regex))
                        .Where(m => m.Success).Select(m => m.Value).Max();
        }

        /// <summary>
        /// Get all days in the given namespace.
        /// </summary>
        /// <param name="baseNamespace"></param>
        /// <returns></returns>
        private static Dictionary<string, Type> GetDaysInNamespace(string baseNamespace)
        {
            return Assembly.GetExecutingAssembly().GetTypes()
                                .Where(t => t.BaseType == typeof(Day) && t.Namespace.Contains(baseNamespace))
                                .ToDictionary(t => t.Name, t => t);
        }

        /// <summary>
        /// Get all days.
        /// </summary>
        /// <returns></returns>
        private static List<Dictionary<string, Type>> GetAllDays()
        {
            List<Dictionary<string, Type>> allDays = [];
            IEnumerable<Type> days = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.BaseType == typeof(Day));
            IEnumerable<string> namespaces = days.Select(d => d.Namespace).OrderBy(_ => _).Distinct();
            foreach (string n in namespaces)
            {
                allDays.Add(days.Where(d => d.Namespace == n).ToDictionary(d => d.Name, d => d));
            }
            return allDays;
        }

        /// <summary>
        /// Run the latest day in the given namespace.
        /// </summary>
        /// <param name="baseNamespace"></param>
        /// <returns></returns>
        private Day RunLatestDay(string baseNamespace)
        {
            Dictionary<string, Type> days = GetDaysInNamespace(baseNamespace);
            if (days.Count > 0)
            {
                string latestDay = days.Keys.Select(k => k[3..]).Select(int.Parse).Max().ToString();
                return RunDay(baseNamespace, latestDay);
            }
            return null;
        }

        /// <summary>
        /// Run the specified day in the given namespace.
        /// </summary>
        /// <param name="baseNamespace"></param>
        /// <param name="dayName"></param>
        /// <returns></returns>
        private Day RunDay(string baseNamespace, string dayName, bool forceSkipWarmup = false)
        {
            if (!forceSkipWarmup && Args.Has(Config.ESupportedArgument.RunWarmup))
            {
                Log.WriteLine(Log.ELevel.Info, "...Warming up\n");
                RunWarmup();
            }

            string paddedDayName = dayName.Length > 1 ? dayName : $"0{dayName}";
            Log.WriteLine(Log.ELevel.Info, $"Running {baseNamespace}.{paddedDayName} Advent of Code\n");

            Dictionary<string, Type> days = GetDaysInNamespace(baseNamespace);
            if (days.Count > 0)
            {
                ObjectHandle handle = Activator.CreateInstance(Assembly.GetExecutingAssembly().FullName, days[days.Keys.Where(k => k.Contains(dayName, StringComparison.CurrentCultureIgnoreCase)).First()].FullName);
                if (handle != null)
                {
                    Day day = (Day)handle.Unwrap();
                    day.Run(Args.Has(Config.ESupportedArgument.ForceTests));
                    return day;
                }
            }
            return null;
        }

        /// <summary>
        /// Force the process to be considered the highest priority
        /// </summary>
        private void CycleHighPriorityCore()
        {
            if (OperatingSystem.IsWindows())
            {
                // use a single core
                Process.GetCurrentProcess().ProcessorAffinity = new IntPtr((int)Math.Pow(2, m_curProcessor));
            }

            // prevent process interuptions
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;

            // prevent thread interuptions
            Thread.CurrentThread.Priority = ThreadPriority.Highest;

            // cycle through the different processors
            m_curProcessor = (m_curProcessor + 1) % Environment.ProcessorCount;
        }

        /// <summary>
        /// Run a warmup sequence so that the cpu is ready to run tests
        /// </summary>
        private void RunWarmup()
        {
            CycleHighPriorityCore();

            static long Warmup(long seed, int count)
            {
                long result = seed;
                for (int i = 0; i < count; ++i)
                {
                    result ^= i ^ seed;
                }
                return result;
            }

            const int count = 100000000;
            long seed = Environment.TickCount;

            Util.Timer timer = new();
            timer.Start();
            while (timer.GetElapsedMs() < 1500)
            {
                _ = Warmup(seed, count);
            }
        }

        /// <summary>
        /// Run performance for the specified day
        /// </summary>
        /// <param name="dayType"></param>
        /// <param name="existingRecords"></param>
        /// <param name="runData"></param>
        /// <returns></returns>
        private bool RunPerformance(Type dayType, Part part, long existingRecords, ref PerfData runData)
        {
            Log.WriteLine(Log.ELevel.Info, $"Running {dayType.Namespace}.{dayType.Name}.Part{part} Performance [Requires {RecordCount - existingRecords} Runs]");
            Log.WriteLine(Log.ELevel.Info, "...Warming up");
            RunWarmup();

            DateTime timeout = DateTime.Now.AddMilliseconds(MaxPerfTimeMs);
            DateTime cycleCore = DateTime.Now.AddMilliseconds(MaxPerfTimeoutPerCoreMs);
            bool cycleCores = false;

            // run two warm up days first
            long i = -2;
            long maxI = RecordCount - existingRecords;
            for (; i < maxI; ++i)
            {
                ObjectHandle handle = Activator.CreateInstance(Assembly.GetExecutingAssembly().FullName, dayType.FullName);
                if (handle == null)
                {
                    break;
                }

                Day day = (Day)handle.Unwrap();
                day.Run(part);
                if (i < 0)
                {
                    continue;
                }
                else
                {
                    runData.AddData(day);
                }

                if (Debugger.IsAttached)
                {
                    Log.WriteLine(Log.ELevel.Spam, $"{day.TimeResults[part]}");
                    if (i > 0 && i % (RecordCount / 20) == 0)
                    {
                        Log.WriteLine(Log.ELevel.Info, $"...{i} runs completed");
                    }
                }
                else if (i >= 0)
                {
                    if (cycleCores)
                    {
                        Log.WriteSameLine(Log.ELevel.Info, string.Format("...{0:000.0}% [core swap in {1}][timeout in {2}]", (double)i / (double)(maxI) * 100.0f, (cycleCore - DateTime.Now).ToString(@"mm\:ss\.fff"), (timeout - DateTime.Now).ToString(@"hh\:mm\:ss\.fff")));
                    }
                    else
                    {
                        Log.WriteSameLine(Log.ELevel.Info, string.Format("...{0:000.0}% [timeout in {1}]", (double)i / (double)(maxI) * 100.0f, (timeout - DateTime.Now).ToString(@"hh\:mm\:ss\.fff")));
                    }
                }

                if (DateTime.Now > timeout)
                {
                    break;
                }

                if (cycleCores && DateTime.Now > cycleCore)
                {
                    CycleHighPriorityCore();
                    cycleCore = DateTime.Now.AddMilliseconds(MaxPerfTimeoutPerCoreMs);
                }
            }

            if (Debugger.IsAttached)
            {
                Log.WriteLine(Log.ELevel.Info, $"...{maxI} runs completed");
            }
            else
            {
                if (DateTime.Now > timeout)
                {
                    Log.WriteLine(Log.ELevel.Info, string.Format("...{0:000.0}% [timed out]{1}\n\r", i / (double)maxI * 100.0f, new string(' ', 50)));
                }
                else
                {
                    Log.WriteSameLine(Log.ELevel.Info, string.Format("...{0:000.0}%{1}\n\r", i / (double)maxI * 100.0f, new string(' ', 60)));
                }
            }

            return i == maxI;
        }

        /// <summary>
        /// Run performance for a specific namespace
        /// </summary>
        /// <param name="baseNamespace"></param>
        private void RunPerformance(string baseNamespace)
        {
            Day.UseLogs = false;

            Log.WriteLine(Log.ELevel.Info, $"Running {baseNamespace} Performance\n");

            LoadPerfData(out string runDataFileName, out PerfData perfData);
            Dictionary<string, Type> days = GetDaysInNamespace(baseNamespace);
            foreach (string key in days.Keys)
            {
                ObjectHandle handle = Activator.CreateInstance(Assembly.GetExecutingAssembly().FullName, days[key].FullName);
                if (handle != null)
                {
                    Day day = (Day)handle.Unwrap();
                    if (day != null)
                    {
                        for (Part part = Part.One; part <= Part.Two; ++part)
                        {
                            if (day.GetSolutionVersion(part).Equals(Day.BaseVersion))
                            {
                                continue;
                            }

                            PerfStat stats = perfData.Get(day.Year, day.DayName, part, day.GetSolutionVersion(part));
                            if (stats == null)
                            {
                                RunPerformance(day.GetType(), part, 0, ref perfData);
                            }
                            else if (stats.Count < RecordCount)
                            {
                                RunPerformance(day.GetType(), part, stats.Count, ref perfData);
                            }
                        }
                    }
                }
            }

            SaveRunData(runDataFileName, perfData);
            PrintPerf(baseNamespace, perfData);
            Day.UseLogs = true;
        }

        /// <summary>
        /// Print performance for a specific namespace
        /// </summary>
        /// <param name="baseNamespace"></param>
        private void ShowPerformance(string baseNamespace)
        {
            Day.UseLogs = false;

            Log.WriteLine(Log.ELevel.Info, $"Showing {baseNamespace} Performance\n");
            LoadPerfData(out _, out PerfData perfData);
            PrintPerf(baseNamespace, perfData);
            Day.UseLogs = true;
        }

        /// <summary>
        /// Load the previous perf data
        /// </summary>
        /// <param name="perfDataFileName"></param>
        /// <param name="perfData"></param>
        private static void LoadPerfData(out string perfDataFileName, out PerfData perfData)
        {
            perfDataFileName = Path.Combine(WorkingDirectory.Get, "Data", "perfdata.json");
            if (File.Exists(perfDataFileName))
            {
                Log.WriteLine(Log.ELevel.Info, $"Loading {perfDataFileName}\n");

                string rawJson = File.ReadAllText(perfDataFileName);
                perfData = JsonConvert.DeserializeObject<PerfData>(rawJson);
            }
            else
            {
                perfData = new PerfData();
            }
        }

        /// <summary>
        /// Save the current perf data to a specific file
        /// </summary>
        /// <param name="perfDataFileName"></param>
        /// <param name="perfData"></param>
        private static void SaveRunData(string perfDataFileName, PerfData perfData)
        {
            Log.WriteLine(Log.ELevel.Info, $"Saving {perfDataFileName}\n");

            string rawJson = JsonConvert.SerializeObject(perfData, Formatting.Indented);
            using StreamWriter sWriter = new(perfDataFileName);
            sWriter.Write(rawJson);
        }

        internal class TimeMagnitude(double d, string s) : Base.Pair<double, string>(d, s) { }
        internal class ColorRange(float min, float max) : Base.RangeF(min, max) { }

        /// <summary>
        /// Print out all the performance information from run data
        /// </summary>
        /// <param name="baseNamespace"></param>
        /// <param name="perfData"></param>
        private void PrintPerf(string baseNamespace, PerfData perfData)
        {
            Log.WriteLine(Log.ELevel.Info, $"{baseNamespace} Performance Metrics\n");
            bool compactPerf = Args.Has(Config.ESupportedArgument.CompactPerf);

            Dictionary<string, Type> days = GetDaysInNamespace(baseNamespace);
            int maxStringLength = 0;
            Dictionary<Part, List<string>> logs = [];
            Dictionary<Part, List<double>> mins = [];
            Dictionary<Part, List<double>> avgs = [];
            Dictionary<Part, List<double>> maxs = [];
            Dictionary<Part, List<Color>> minColors = [];
            Dictionary<Part, List<Color>> avgColors = [];
            Dictionary<Part, List<Color>> maxColors = [];
            for (Part part = Part.One; part <= Part.Two; ++part)
            {
                logs[part] = [];
                mins[part] = [];
                avgs[part] = [];
                maxs[part] = [];

                minColors[part] = [];
                avgColors[part] = [];
                maxColors[part] = [];
            }

            List<TimeMagnitude> timeMagnitudes =
            [
                new TimeMagnitude(60000.0, " m"),
                new TimeMagnitude(1000.0, " s"),
                new TimeMagnitude(1.0, "ms"),
                new TimeMagnitude(.001, "µs"),
            ];
            TimeMagnitude getTimeMagnitude(double val)
            {
                foreach (var pair in timeMagnitudes)
                {
                    if (val >= pair.First)
                    {
                        return new TimeMagnitude(val / pair.First, pair.Last);
                    }
                }
                return timeMagnitudes.Last();
            }

            Dictionary<string, Color> timeUnitColor = new()
            {
                [" m"] = Color.Violet,
                [" s"] = Log.Negative,
                ["ms"] = Log.Neutral,
                ["µs"] = Log.Positive
            };

            // min and max only take into account the avg
            // there needs to be a min and max for the Min and the Max
            // use Range<double>
            double min = double.MaxValue, max = double.MinValue;
            string minStr = "", maxStr = "";
            foreach (string key in days.Keys)
            {
                ObjectHandle handle = Activator.CreateInstance(Assembly.GetExecutingAssembly().FullName, days[key].FullName);
                if (handle != null)
                {
                    Day day = (Day)handle.Unwrap();
                    if (day != null)
                    {
                        for (Part part = Part.One; part <= Part.Two; ++part)
                        {
                            string solutionVersion = day.GetSolutionVersion(part);
                            PerfStat stats = perfData.Get(day.Year, day.DayName, part, solutionVersion);

                            string logLine = $"[{day.Year}|{day.DayName}|part{(int)part}|{solutionVersion}]";
                            if (stats == null)
                            {
                                logLine = string.Format("{0} {1}<No stats found>{1}", logLine, Log.ColorMarker);
                                mins[part].Add(double.NaN);
                                avgs[part].Add(double.NaN);
                                maxs[part].Add(double.NaN);
                                avgColors[part].Add(Log.Neutral);
                                minColors[part].Add(Log.Neutral);
                                maxColors[part].Add(Log.Neutral);
                            }
                            else
                            {
                                if (min > stats.Avg)
                                {
                                    min = stats.Avg;
                                    minStr = logLine;
                                }

                                if (max < stats.Avg)
                                {
                                    max = stats.Avg;
                                    maxStr = logLine;
                                }

                                mins[part].Add(stats.Min);
                                avgs[part].Add(stats.Avg);
                                maxs[part].Add(stats.Max);

                                if (compactPerf)
                                {
                                    TimeMagnitude tm = getTimeMagnitude(stats.Avg);
                                    avgColors[part].Add(timeUnitColor[tm.Last]);
                                    logLine += string.Format(" Avg={0}{1:000.000}{0} ({0}{2}{0})", Log.ColorMarker, tm.First, tm.Last);
                                }
                                else
                                {
                                    TimeMagnitude avgTm = getTimeMagnitude(stats.Avg);
                                    TimeMagnitude minTm = getTimeMagnitude(stats.Min);
                                    TimeMagnitude maxTm = getTimeMagnitude(stats.Max);
                                    avgColors[part].Add(timeUnitColor[avgTm.Last]);
                                    minColors[part].Add(timeUnitColor[minTm.Last]);
                                    maxColors[part].Add(timeUnitColor[maxTm.Last]);
                                    logLine += string.Format(" Avg={0}{1:000.000}{0} ({0}{2}{0}) [{3} Records, Min={0}{4:000.000}{0} ({0}{5}{0}), Max={0}{6:000.000}{0} ({0}{7}{0})]", Log.ColorMarker, avgTm.First, avgTm.Last, stats.Count, minTm.First, minTm.Last, maxTm.First, maxTm.Last);
                                }
                            }
                            logs[part].Add(logLine);
                            maxStringLength = Math.Max(maxStringLength, logLine.Length);
                        }
                    }
                }
            }

            double getAvg(double val)
            {
                if (val.Equals(double.NaN))
                {
                    return double.NaN;
                }
                return (val - min) / (max - min);
            }

            ColorRange loR = new(Log.Neutral.R, Log.Negative.R);
            ColorRange loG = new(Log.Neutral.G, Log.Negative.G);
            ColorRange loB = new(Log.Neutral.B, Log.Negative.B);
            ColorRange hiR = new(Log.Positive.R, Log.Neutral.R);
            ColorRange hiG = new(Log.Positive.G, Log.Neutral.G);
            ColorRange hiB = new(Log.Positive.B, Log.Neutral.B);
            Color getColor(double avg)
            {
                if (avg.Equals(double.NaN))
                {
                    return Log.Neutral;
                }

                int getRangedColor(double avg, ColorRange colorRange)
                {
                    return Math.Max(Math.Min((int)(((colorRange.Max - colorRange.Min) * avg) + colorRange.Min), 255), 0);
                }

                int r = 0, g = 0, b = 0;
                if (avg >= 0.5f)
                {
                    r = getRangedColor(avg, loR);
                    g = getRangedColor(avg, loG);
                    b = getRangedColor(avg, loB);
                }
                else
                {
                    r = getRangedColor(avg, hiR);
                    g = getRangedColor(avg, hiG);
                    b = getRangedColor(avg, hiB);
                }

                return Color.FromArgb(r, g, b);
            }

            if (compactPerf)
            {
                int maxLength = logs.SelectMany(l => l.Value).Select(lv => lv.Replace($"{Log.ColorMarker}", "")).Max(lv => lv.Length) + 2;
                string separator = new('#', maxLength * 2 + 10);
                for (int i = 0; i < logs[Part.Two].Count; ++i)
                {
                    if (i % 5 == 0)
                    {
                        Log.WriteLine(Log.ELevel.Info, separator);
                    }
                    Log.Write(Log.ELevel.Info, "##  ");
                    Log.WriteAppend(Log.ELevel.Info, logs[Part.One][i], [getColor(getAvg(avgs[Part.One][i])), avgColors[Part.One][i]]);
                    Log.WriteAppend(Log.ELevel.Info, new string(' ', maxLength - logs[Part.One][i].Replace($"{Log.ColorMarker}", "").Length));
                    Log.WriteAppend(Log.ELevel.Info, "##  ");
                    Log.WriteAppend(Log.ELevel.Info, logs[Part.Two][i], [getColor(getAvg(avgs[Part.Two][i])), avgColors[Part.Two][i]]);
                    Log.WriteAppend(Log.ELevel.Info, new string(' ', maxLength - logs[Part.Two][i].Replace($"{Log.ColorMarker}", "").Length));
                    Log.WriteAppend(Log.ELevel.Info, "##");
                    Log.WriteAppendEnd(Log.ELevel.Info);
                }
                Log.WriteLine(Log.ELevel.Info, separator);
                maxStringLength = separator.Length;
            }
            else
            {
                string separator = new('#', maxStringLength);
                Log.WriteLine(Log.ELevel.Info, separator);
                for (int i = 0; i < logs[Part.One].Count; ++i)
                {
                    for (Part part = Part.One; part <= Part.Two; ++part)
                    {
                        double minColor = getAvg(mins[part][i]);
                        double avgColor = getAvg(avgs[part][i]);
                        double maxColor = getAvg(maxs[part][i]);
                        List<Color> colors = [getColor(minColor), getColor(avgColor), getColor(maxColor), avgColors[part][i], minColors[part][i], maxColors[part][i]];
                        Log.WriteLine(Log.ELevel.Info, logs[part][i], colors);
                    }
                    Log.WriteLine(Log.ELevel.Info, separator);
                }
            }

            double p1Total = avgs[Part.One].Where(a => !a.Equals(double.NaN)).Sum();
            double p2Total = avgs[Part.Two].Where(a => !a.Equals(double.NaN)).Sum();
            double totals = p1Total + p2Total;

            string getLogHeader(string day, string part, string version)
            {
                return string.Format("[{0}|{1}|{2}|{3}]", baseNamespace[^4..], day, part, version);
            }

            void logTotal(double time, string logHeader, string valueType, string logEnder, Color color)
            {
                string noColor = string.Format("{0}{1} {2}=", compactPerf ? "##  " : "", logHeader, valueType);
                Log.Write(Log.ELevel.Info, noColor);

                TimeMagnitude tm = getTimeMagnitude(time);
                string yesColor = string.Format("{0:000.000}", tm.First);
                Log.WriteAppend(Log.ELevel.Info, string.Format("{0}{1}{0}", Log.ColorMarker, yesColor), [color]);

                string remainingLog = string.Format(" ({0}{1}{0}){2}", Log.ColorMarker, tm.Last, logEnder);
                Log.WriteAppend(Log.ELevel.Info, remainingLog, [timeUnitColor[tm.Last]]);

                if (compactPerf)
                {
                    int curLen = noColor.Length + yesColor.Length + remainingLog.Length - 2;
                    string end = new(' ', maxStringLength - curLen - 2);
                    Log.WriteAppend(Log.ELevel.Info, $"{end}##");
                }

                Log.WriteAppendEnd(Log.ELevel.Info);
            }

            logTotal(p1Total, getLogHeader("total", "part1", "--"), "Sum", string.Empty, Log.Neutral);
            logTotal(p2Total, getLogHeader("total", "part2", "--"), "Sum", string.Empty, Log.Neutral);
            logTotal(totals, getLogHeader("total", "-all-", "--"), "Sum", string.Empty, Log.Neutral);
            Log.WriteLine(Log.ELevel.Info, new string('#', maxStringLength));

            if (totals > 0)
            {
                logTotal(min, minStr, "Min", string.Format(" [{0:00.00}%]", min / totals * 100.0f), Log.Positive);
                logTotal(max, maxStr, "Max", string.Format(" [{0:00.00}%]", max / totals * 100.0f), Log.Negative);
                Log.WriteLine(Log.ELevel.Info, new string('#', maxStringLength));
                Log.WriteLine(Log.ELevel.Info, new string('#', maxStringLength));
            }
        }
    }
}