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

using AoC.Core;

using Newtonsoft.Json;

namespace AoC
{
    class EntryPoint
    {
        static void Main(string[] args)
        {
            new Program(args);
        }
    }

    class Program
    {
        private int m_curProcessor = 0;

        private const long DefaultRecordCount = 1000;
        private long m_recordCount = DefaultRecordCount;
        private long RecordCount { get { return m_recordCount; } }

        private const long DefaultMaxPerfTimeoutMs = 3600000;
        private const long MaxPerfTimeoutPerCoreMs = 150000;
        private long m_maxPerfTimeoutMs = DefaultMaxPerfTimeoutMs;
        private long MaxPerfTimeMs { get { return m_maxPerfTimeoutMs; } }

        private CommandLine Args { get; set; }

        public Program(string[] args)
        {
            try
            {
                Day.UseLogs = true;

                // parse command args
                Args = ParseCommandLineArgs(args);

                if (Args.Has(CommandLine.ESupportedArgument.Help))
                {
                    Args.PrintHelp();
                    return;
                }

                // get the number of records to keep for perf tests
                if (Args.HasValue(CommandLine.ESupportedArgument.PerfRecordCount))
                {
                    m_recordCount = long.Parse(Args[CommandLine.ESupportedArgument.PerfRecordCount]);
                }

                // get the timeout runs should adhere to
                if (Args.HasValue(CommandLine.ESupportedArgument.PerfTimeout))
                {
                    m_maxPerfTimeoutMs = long.Parse(Args[CommandLine.ESupportedArgument.PerfTimeout]);
                }

                // get the namespace to use
                string baseNamespace = GetLatestNamespace();
                if (Args.HasValue(CommandLine.ESupportedArgument.Namespace))
                {
                    baseNamespace = Args[CommandLine.ESupportedArgument.Namespace];
                }

                // run the day specified or the latest day
                if (Args.HasValue(CommandLine.ESupportedArgument.Day))
                {
                    Day day = RunDay(baseNamespace, Args[CommandLine.ESupportedArgument.Day]);
                    if (day == null)
                    {
                        Log.WriteLine(Log.ELevel.Error, $"Unable to find {baseNamespace}.{Args[CommandLine.ESupportedArgument.Day]}");
                    }
                    else
                    {
                        Log.WriteLine(Log.ELevel.Info, "");
                    }
                }
                else if (!Args.Has(CommandLine.ESupportedArgument.SkipLatest))
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

                // show performance
                if (Args.Has(CommandLine.ESupportedArgument.ShowPerf))
                {
                    ShowPerformance(baseNamespace);
                }
                // run performance tests
                else if (Args.Has(CommandLine.ESupportedArgument.RunPerf))
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
        /// Parse the command line arguments
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private CommandLine ParseCommandLineArgs(string[] args)
        {
            CommandLine commandLine = new CommandLine(args);
            commandLine.Print();
            return commandLine;
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
        private Dictionary<string, Type> GetDaysInNamespace(string baseNamespace)
        {
            return Assembly.GetExecutingAssembly().GetTypes()
                                .Where(t => t.BaseType == typeof(Day) && t.Namespace.Contains(baseNamespace))
                                .ToDictionary(t => t.Name, t => t);
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
        private Day RunDay(string baseNamespace, string dayName)
        {
            if (Args.Has(CommandLine.ESupportedArgument.RunWarmup))
            {
                Log.WriteLine(Log.ELevel.Info, "...Warming up\n");
                RunWarmup();
            }

            Log.WriteLine(Log.ELevel.Info, $"Running {baseNamespace}.{dayName} Advent of Code\n");

            Dictionary<string, Type> days = GetDaysInNamespace(baseNamespace);
            if (days.Count > 0)
            {
                ObjectHandle handle = Activator.CreateInstance(Assembly.GetExecutingAssembly().FullName, days[days.Keys.Where(k => k.ToLower().Contains(dayName.ToLower())).First()].FullName);
                if (handle != null)
                {
                    Day day = (Day)handle.Unwrap();
                    day.Run();
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

            Func<long, int, long> Warmup = (long seed, int count) =>
            {
                long result = seed;
                for (int i = 0; i < count; ++i)
                {
                    result ^= i ^ seed;
                }
                return result;
            };

            long result = 0;
            const int count = 100000000;
            long seed = Environment.TickCount;

            Util.Timer timer = new Util.Timer();
            timer.Start();
            while (timer.GetElapsedMs() < 1500)
            {
                result = Warmup(seed, count);
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
                day.RunProblem(part);
                if (i < 0)
                {
                    continue;
                }
                else
                {
                    runData.AddData(day);
                }

                if (System.Diagnostics.Debugger.IsAttached)
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

            if (System.Diagnostics.Debugger.IsAttached)
            {
                Log.WriteLine(Log.ELevel.Info, $"...{maxI} runs completed");
            }
            else
            {
                if (DateTime.Now > timeout)
                {
                    Log.WriteLine(Log.ELevel.Info, string.Format("...{0:000.0}% [timed out]{1}\n\r", (double)i / (double)(maxI) * 100.0f, new string(' ', 50)));
                }
                else
                {
                    Log.WriteSameLine(Log.ELevel.Info, string.Format("...{0:000.0}%{1}\n\r", (double)i / (double)(maxI) * 100.0f, new string(' ', 60)));
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

            PerfData perfData;
            string runDataFileName;
            LoadPerfData(out runDataFileName, out perfData);
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
                            if (day.GetSolutionVersion(part) == "v0")
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

            PerfData perfData;
            string runDataFileName;
            LoadPerfData(out runDataFileName, out perfData);
            PrintPerf(baseNamespace, perfData);
            Day.UseLogs = true;
        }

        /// <summary>
        /// Load the previous perf data
        /// </summary>
        /// <param name="perfDataFileName"></param>
        /// <param name="perfData"></param>
        private void LoadPerfData(out string perfDataFileName, out PerfData perfData)
        {
            string workingDir = Core.WorkingDirectory.Get;
            if (System.Diagnostics.Debugger.IsAttached)
            {
                perfDataFileName = Path.Combine(workingDir, "perfdata_debugger.json");
            }
            else
            {
                perfDataFileName = Path.Combine(workingDir, "perfdata_cmd.json");
            }
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
        private void SaveRunData(string perfDataFileName, PerfData perfData)
        {
            Log.WriteLine(Log.ELevel.Info, $"Saving {perfDataFileName}\n");

            string rawJson = JsonConvert.SerializeObject(perfData, Formatting.Indented);
            using (StreamWriter sWriter = new StreamWriter(perfDataFileName))
            {
                sWriter.Write(rawJson);
            }
        }

        /// <summary>
        /// Print out all the performance information from run data
        /// </summary>
        /// <param name="baseNamespace"></param>
        /// <param name="perfData"></param>
        private void PrintPerf(string baseNamespace, PerfData perfData)
        {
            Log.WriteLine(Log.ELevel.Info, $"{baseNamespace} Performance Metrics\n");

            Dictionary<string, Type> days = GetDaysInNamespace(baseNamespace);
            int maxStringLength = 0;
            Dictionary<Part, List<string>> logs = new Dictionary<Part, List<string>>();
            Dictionary<Part, List<double>> mins = new Dictionary<Part, List<double>>();
            Dictionary<Part, List<double>> avgs = new Dictionary<Part, List<double>>();
            Dictionary<Part, List<double>> maxs = new Dictionary<Part, List<double>>();
            for (Part part = Part.One; part <= Part.Two; ++part)
            {
                logs[part] = new List<string>();
                mins[part] = new List<double>();
                avgs[part] = new List<double>();
                maxs[part] = new List<double>();
            }

            List<Base.Pair<double, string>> TimeMagnitude = new List<Base.Pair<double, string>>();
            TimeMagnitude.Add(new Base.Pair<double, string>(3600000.0, "h"));
            TimeMagnitude.Add(new Base.Pair<double, string>(60000.0, "m"));
            TimeMagnitude.Add(new Base.Pair<double, string>(1000.0, "s"));
            TimeMagnitude.Add(new Base.Pair<double, string>(1.0, "ms"));
            TimeMagnitude.Add(new Base.Pair<double, string>(.001, "µs"));
            Func<double, Base.Pair<double, string>> getTimeMagnitude = (double val) =>
            {
                foreach (var pair in TimeMagnitude)
                {
                    if (val >= pair.First)
                    {
                        return new Base.Pair<double, string>(val / pair.First, pair.Last);
                    }
                }
                return TimeMagnitude.Last();
            };

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

                                if (Args.Has(CommandLine.ESupportedArgument.CompactPerf))
                                {
                                    var tm = getTimeMagnitude(stats.Avg);
                                    logLine += string.Format(" Avg={0}{1:000.000}{0} ({2})", Log.ColorMarker, tm.First, tm.Last);
                                }
                                else
                                {
                                    var avgTm = getTimeMagnitude(stats.Avg);
                                    var minTm = getTimeMagnitude(stats.Min);
                                    var maxTm = getTimeMagnitude(stats.Max);
                                    logLine += string.Format(" Avg={0}{1:000.000}{0} ({2}) [{3} Records, Min={0}{4:000.000}{0} ({5}), Max={0}{6:000.000}{0} ({7})]", Log.ColorMarker, avgTm.First, avgTm.Last, stats.Count, minTm.First, minTm.Last, maxTm.First, maxTm.Last);
                                }
                            }
                            logs[part].Add(logLine);
                            maxStringLength = Math.Max(maxStringLength, logLine.Length);
                        }
                    }
                }
            }

            Func<double, double> getAvg = (double val) =>
            {
                if (val.Equals(double.NaN))
                {
                    return double.NaN;
                }
                return (val - min) / (max - min);
            };

            Base.Pair<float, float> loR = new Base.Pair<float, float>(Log.Neutral.R, Log.Negative.R);
            Base.Pair<float, float> loG = new Base.Pair<float, float>(Log.Neutral.G, Log.Negative.G);
            Base.Pair<float, float> loB = new Base.Pair<float, float>(Log.Neutral.B, Log.Negative.B);
            Base.Pair<float, float> hiR = new Base.Pair<float, float>(Log.Positive.R, Log.Neutral.R);
            Base.Pair<float, float> hiG = new Base.Pair<float, float>(Log.Positive.G, Log.Neutral.G);
            Base.Pair<float, float> hiB = new Base.Pair<float, float>(Log.Positive.B, Log.Neutral.B);
            Func<double, Color> getColor = (double avg) =>
            {
                if (avg.Equals(double.NaN))
                {
                    return Core.Log.Neutral;
                }

                Func<double, Base.Pair<float, float>, int> getRangedColor = (double avg, Base.Pair<float, float> color) =>
                {
                    return Math.Max(Math.Min((int)((int)(((color.Last - color.First) * avg) + color.First)), 255), 0);
                };

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
            };

            if (Args.Has(CommandLine.ESupportedArgument.CompactPerf))
            {
                int maxLength = logs.SelectMany(l => l.Value).Max(lv => lv.Length) + 2;
                string separator = new string('#', maxLength * 2 + 6);
                for (int i = 0; i < logs[Part.Two].Count; ++i)
                {
                    if (i % 5 == 0)
                    {
                        Log.WriteLine(Log.ELevel.Info, separator);
                    }
                    Log.Write(Log.ELevel.Info, "## ");
                    Log.WriteAppend(Log.ELevel.Info, logs[Part.One][i], new List<Color>() { getColor(getAvg(avgs[Part.One][i])) });
                    Log.WriteAppend(Log.ELevel.Info, new string(' ', maxLength - logs[Part.One][i].Length));
                    Log.WriteAppend(Log.ELevel.Info, " ## ");
                    Log.WriteAppend(Log.ELevel.Info, logs[Part.Two][i], new List<Color>() { getColor(getAvg(avgs[Part.Two][i])) });
                    Log.WriteAppend(Log.ELevel.Info, new string(' ', maxLength - logs[Part.Two][i].Length));
                    Log.WriteAppend(Log.ELevel.Info, " ##");
                    Log.WriteAppendEnd(Log.ELevel.Info);
                }
                Log.WriteLine(Log.ELevel.Info, separator);
            }
            else
            {
                string separator = new string('#', maxStringLength);
                Log.WriteLine(Log.ELevel.Info, separator);
                for (int i = 0; i < logs[Part.One].Count; ++i)
                {
                    for (Part part = Part.One; part <= Part.Two; ++part)
                    {
                        double minColor = getAvg(mins[part][i]);
                        double avgColor = getAvg(avgs[part][i]);
                        double maxColor = getAvg(maxs[part][i]);
                        List<Color> colors = new List<Color>() { getColor(minColor), getColor(avgColor), getColor(maxColor) };
                        Log.WriteLine(Log.ELevel.Info, logs[part][i], colors);
                    }
                    Log.WriteLine(Log.ELevel.Info, separator);
                }
            }

            double p1Total = avgs[Part.One].Where(a => !a.Equals(double.NaN)).Sum();
            double p2Total = avgs[Part.Two].Where(a => !a.Equals(double.NaN)).Sum();
            double totals = p1Total + p2Total;
            // TODO: smart time metric (show largest time form, m, s, ms, etc)
            Log.WriteLine(Log.ELevel.Info, $"[{baseNamespace[^4..]}|total|part1|--] Sum={TimeSpan.FromMilliseconds(p1Total).ToString(@"mm\.ss\.ffffff")} (m)");
            Log.WriteLine(Log.ELevel.Info, $"[{baseNamespace[^4..]}|total|part2|--] Sum={TimeSpan.FromMilliseconds(p2Total).ToString(@"mm\.ss\.ffffff")} (m)");
            Log.WriteLine(Log.ELevel.Info, $"[{baseNamespace[^4..]}|total|-all-|--] Sum={TimeSpan.FromMilliseconds(totals).ToString(@"mm\.ss\.ffffff")} (m)");
            Log.WriteLine(Log.ELevel.Info, new string('#', maxStringLength));

            if (totals > 0)
            {
                Log.WriteLine(Log.ELevel.Info, $"{minStr} Min={TimeSpan.FromMilliseconds(min).ToString(@"ss\.ffffff")} (s) [{string.Format("{0:00.00}%", min / (p1Total + p2Total) * 100.0f)}]");
                Log.WriteLine(Log.ELevel.Info, $"{maxStr} Max={TimeSpan.FromMilliseconds(max).ToString(@"ss\.ffffff")} (s) [{string.Format("{0:00.00}%", max / (p1Total + p2Total) * 100.0f)}]");
                Log.WriteLine(Log.ELevel.Info, new string('#', maxStringLength));
                Log.WriteLine(Log.ELevel.Info, new string('#', maxStringLength));
            }
        }
    }
}
