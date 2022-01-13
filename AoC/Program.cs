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
                if (Args.Has(CommandLine.ESupportedArgument.Day))
                {
                    Day day = RunDay(baseNamespace, Args[CommandLine.ESupportedArgument.Day]);
                    if (day == null)
                    {
                        Logger.WriteLine(Logger.ELogLevel.Error, $"Unable to find {baseNamespace}.{Args[CommandLine.ESupportedArgument.Day]}");
                    }
                    else
                    {
                        Logger.WriteLine(Logger.ELogLevel.Info, "");
                    }
                }
                else if (!Args.Has(CommandLine.ESupportedArgument.SkipLatest))
                {
                    Day latestDay = RunLatestDay(baseNamespace);
                    if (latestDay == null)
                    {
                        Logger.WriteLine(Logger.ELogLevel.Error, $"Unable to find any solutions for namespace {baseNamespace}");
                    }
                    else
                    {
                        Logger.WriteLine(Logger.ELogLevel.Info, "");
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
                Logger.WriteLine(Logger.ELogLevel.Fatal, e.Message);
                Logger.WriteLine(Logger.ELogLevel.Fatal, e.StackTrace);
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
            string regex = string.Concat(nameof(AoC),@"\..*(\d{4})");
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
                                .Where(t => t.BaseType == typeof(AoC.Day) && t.Namespace.Contains(baseNamespace))
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
                Logger.WriteLine(Logger.ELogLevel.Info, "...Warming up\n");
                RunWarmup();
            }

            Logger.WriteLine(Logger.ELogLevel.Info, $"Running {baseNamespace}.{dayName} Advent of Code\n");

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

            Timer timer = new Timer();
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
            Logger.WriteLine(Logger.ELogLevel.Info, $"Running {dayType.Namespace}.{dayType.Name}.Part{part} Performance [Requires {RecordCount - existingRecords} Runs]");
            Logger.WriteLine(Logger.ELogLevel.Info, "...Warming up");
            RunWarmup();
            ObjectHandle warmupHandle = Activator.CreateInstance(Assembly.GetExecutingAssembly().FullName, dayType.FullName);
            if (warmupHandle == null)
            {
                return false;
            }

            Day warmupDay = (Day)warmupHandle.Unwrap();
            warmupDay.RunProblem(part);

            DateTime timeout = DateTime.Now.AddMilliseconds(MaxPerfTimeMs);
            DateTime cycleCore = DateTime.Now.AddMilliseconds(MaxPerfTimeoutPerCoreMs);

            long i = 0;
            long maxI = RecordCount - existingRecords;
            for (; i < maxI; ++i)
            {
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    if (i > 0 && i % (RecordCount / 20) == 0)
                    {
                        Logger.WriteLine(Logger.ELogLevel.Info, $"...{i} runs completed");
                    }
                }
                else
                {
                    Logger.WriteSameLine(Logger.ELogLevel.Info, string.Format("...{0:000.0}% [core swap in {1}][timeout in {2}]", (double)i / (double)(maxI) * 100.0f, (cycleCore - DateTime.Now).ToString(@"mm\:ss\.fff"), (timeout - DateTime.Now).ToString(@"hh\:mm\:ss\.fff")));
                }

                ObjectHandle handle = Activator.CreateInstance(Assembly.GetExecutingAssembly().FullName, dayType.FullName);
                if (handle == null)
                {
                    break;
                }

                Day day = (Day)handle.Unwrap();
                day.RunProblem(part);
                runData.AddData(day);

                if (DateTime.Now > timeout)
                {
                    break;
                }

                if (DateTime.Now > cycleCore)
                {
                    CycleHighPriorityCore();
                    cycleCore = DateTime.Now.AddMilliseconds(MaxPerfTimeoutPerCoreMs);
                }
            }
            if (System.Diagnostics.Debugger.IsAttached)
            {
                Logger.WriteLine(Logger.ELogLevel.Info, $"...{maxI} runs completed");
            }
            else
            {
                if (DateTime.Now > timeout)
                {
                    Logger.WriteLine(Logger.ELogLevel.Info, string.Format("...{0:000.0}% [timed out]{1}\n\r", (double)i / (double)(maxI) * 100.0f, new string(' ', 50)));
                }
                else
                {
                    Logger.WriteSameLine(Logger.ELogLevel.Info, string.Format("...{0:000.0}%{1}\n\r", (double)i / (double)(maxI) * 100.0f, new string(' ', 60)));
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

            Logger.WriteLine(Logger.ELogLevel.Info, $"Running {baseNamespace} Performance\n");

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
            PrintMetrics(baseNamespace, perfData);
            Day.UseLogs = true;
        }

        /// <summary>
        /// Print performance for a specific namespace
        /// </summary>
        /// <param name="baseNamespace"></param>
        private void ShowPerformance(string baseNamespace)
        {
            Day.UseLogs = false;

            Logger.WriteLine(Logger.ELogLevel.Info, $"Showing {baseNamespace} Performance\n");

            PerfData perfData;
            string runDataFileName;
            LoadPerfData(out runDataFileName, out perfData);
            PrintMetrics(baseNamespace, perfData);
            Day.UseLogs = true;
        }

        /// <summary>
        /// Load the previous perf data
        /// </summary>
        /// <param name="perfDataFileName"></param>
        /// <param name="perfData"></param>
        private void LoadPerfData(out string perfDataFileName, out PerfData perfData)
        {
            string workingDir = Util.WorkingDirectory;
            if (System.Diagnostics.Debugger.IsAttached)
            {
                perfDataFileName = Path.Combine(workingDir, "perfdata_cmd.json");
            }
            else
            {
                perfDataFileName = Path.Combine(workingDir, "perfdata_cmd.json");
            }
            if (File.Exists(perfDataFileName))
            {
                Logger.WriteLine(Logger.ELogLevel.Info, $"Loading {perfDataFileName}\n");

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
            Logger.WriteLine(Logger.ELogLevel.Info, $"Saving {perfDataFileName}\n");

            string rawJson = JsonConvert.SerializeObject(perfData, Formatting.Indented);
            using (StreamWriter sWriter = new StreamWriter(perfDataFileName))
            {
                sWriter.Write(rawJson);
            }
        }

        /// <summary>
        /// Print out all the metrics from run data
        /// </summary>
        /// <param name="baseNamespace"></param>
        /// <param name="perfData"></param>
        private void PrintMetrics(string baseNamespace, PerfData perfData)
        {
            Logger.WriteLine(Logger.ELogLevel.Info, $"{baseNamespace} Performance Metrics\n");

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
                                logLine = $"{logLine} No stats found";
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
                                logLine += string.Format(" Avg=^{0:0.000}^ (ms) [{1} Records, Min=^{2:0.000}^ (ms), Max=^{3:0.000}^ (ms)]", stats.Avg, stats.Count, stats.Min, stats.Max);
                            }
                            logs[part].Add(logLine);
                            maxStringLength = Math.Max(maxStringLength, logLine.Length);
                        }
                    }
                }
            }

            // double min = avgs.SelectMany(p => p.Value).Min(v => v);
            // double max = avgs.SelectMany(p => p.Value).Max(v => v);
            Func<double, double> getAvg = (double val) =>
            {
                if (val == double.NaN)
                {
                    return max;
                }
                return (val - min) / (max - min);
            };
            Func<double, Color> getColor = (double avg) =>
            {
                int r = Math.Max(Math.Min((int)(avg * 255.0f), 255), 0);
                int g = Math.Max(Math.Min((int)(255.0f - avg * 255.0f), 255), 0);
                return Color.FromArgb(r, g, 0);
            };

            string separator = new string('#', maxStringLength);
            for (int i = 0; i < logs[Part.One].Count; ++i)
            {
                for (Part part = Part.One; part <= Part.Two; ++part)
                {
                    double minColor = getAvg(mins[part][i]);
                    double avgColor = getAvg(avgs[part][i]);
                    double maxColor = getAvg(maxs[part][i]);
                    List<Color> colors = new List<Color>() { getColor(minColor), getColor(avgColor), getColor(maxColor) };
                    Logger.WriteLine(Logger.ELogLevel.Info, logs[part][i], colors);
                }
                Logger.WriteLine(Logger.ELogLevel.Info, separator);
            }

            double p1Total = avgs[Part.One].Sum();
            double p2Total = avgs[Part.Two].Sum();
            double totals = p1Total + p2Total;
            Logger.WriteLine(Logger.ELogLevel.Info, $"[{baseNamespace[^4..]}|total|part1|--] Sum={TimeSpan.FromMilliseconds(p1Total).ToString(@"ss\.ffffff")} (s)");
            Logger.WriteLine(Logger.ELogLevel.Info, $"[{baseNamespace[^4..]}|total|part2|--] Sum={TimeSpan.FromMilliseconds(p2Total).ToString(@"ss\.ffffff")} (s)");
            Logger.WriteLine(Logger.ELogLevel.Info, $"[{baseNamespace[^4..]}|total|-all-|--] Sum={TimeSpan.FromMilliseconds(totals).ToString(@"ss\.ffffff")} (s)");
            Logger.WriteLine(Logger.ELogLevel.Info, new string('#', maxStringLength));

            if (totals > 0)
            {
                Logger.WriteLine(Logger.ELogLevel.Info, $"{minStr} Min={TimeSpan.FromMilliseconds(min).ToString(@"ss\.ffffff")} (s) [{string.Format("{0:00.00}%", min / (p1Total + p2Total) * 100.0f)}]");
                Logger.WriteLine(Logger.ELogLevel.Info, $"{maxStr} Max={TimeSpan.FromMilliseconds(max).ToString(@"ss\.ffffff")} (s) [{string.Format("{0:00.00}%", max / (p1Total + p2Total) * 100.0f)}]");
                Logger.WriteLine(Logger.ELogLevel.Info, new string('#', maxStringLength));
                Logger.WriteLine(Logger.ELogLevel.Info, new string('#', maxStringLength + 2));
            }
        }
    }
}
