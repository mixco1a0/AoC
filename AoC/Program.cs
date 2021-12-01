using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using System.Threading;

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
        int m_curProcessor = 0;

        const long DefaultRecordCount = 1000;
        long m_recordCount = DefaultRecordCount;
        private long RecordCount { get { return m_recordCount; } }

        const long DefaultMaxPerfTimeoutMs = 3600000;
        long m_maxPerfTimeoutMs = DefaultMaxPerfTimeoutMs;
        private long MaxPerfTimeMs { get { return m_maxPerfTimeoutMs; } }

        private CommandLineArgs Args { get; set; }

        public Program(string[] args)
        {
            try
            {
                Day.UseLogs = true;

                // parse command args
                Args = ParseCommandLineArgs(args);

                if (Args.HasArg(CommandLineArgs.SupportedArgument.Help))
                {
                    Args.PrintHelp(LogLine);
                    return;
                }

                // get the number of records to keep for perf tests
                if (Args.HasArgValue(CommandLineArgs.SupportedArgument.PerfRecordCount))
                {
                    m_recordCount = long.Parse(Args.Args[CommandLineArgs.SupportedArgument.PerfRecordCount]);
                }

                // get the timeout runs should adhere to
                if (Args.HasArgValue(CommandLineArgs.SupportedArgument.PerfTimeout))
                {
                    m_maxPerfTimeoutMs = long.Parse(Args.Args[CommandLineArgs.SupportedArgument.PerfTimeout]);
                }

                // get the namespace to use
                string baseNamespace = nameof(AoC._2020);
                if (Args.HasArgValue(CommandLineArgs.SupportedArgument.Namespace))
                {
                    baseNamespace = Args.Args[CommandLineArgs.SupportedArgument.Namespace];
                }

                // run the day specified or the latest day
                if (Args.HasArg(CommandLineArgs.SupportedArgument.Day))
                {
                    Day day = RunDay(baseNamespace, Args.Args[CommandLineArgs.SupportedArgument.Day]);
                    if (day == null)
                    {
                        LogLine($"Unable to find {baseNamespace}.{Args.Args[CommandLineArgs.SupportedArgument.Day]}");
                    }
                    else
                    {
                        LogLine("");
                    }
                }
                else if (!Args.HasArg(CommandLineArgs.SupportedArgument.SkipLatest))
                {
                    Day latestDay = RunLatestDay(baseNamespace);
                    if (latestDay == null)
                    {
                        LogLine($"Unable to find any solutions for namespace {baseNamespace}");
                    }
                    else
                    {
                        LogLine("");
                    }
                }

                // show performance
                if (Args.HasArg(CommandLineArgs.SupportedArgument.ShowPerf))
                {
                    ShowPerformance(baseNamespace);
                }
                // run performance tests
                else if (Args.HasArg(CommandLineArgs.SupportedArgument.RunPerf))
                {
                    RunPerformance(baseNamespace);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }

        /// <summary>
        /// Parse the command line arguments
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private CommandLineArgs ParseCommandLineArgs(string[] args)
        {
            CommandLineArgs commandLineArgs = new CommandLineArgs(args);
            commandLineArgs.Print(LogLine);
            return commandLineArgs;
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
            if (Args.HasArg(CommandLineArgs.SupportedArgument.RunWarmup))
            {
                LogLine("...Warming up");
                LogLine("");
                RunWarmup();
            }

            LogLine($"Running {baseNamespace}.{dayName} Advent of Code");
            LogLine("");

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
        private void SetHighPriority()
        {
            // use a single core
            Process.GetCurrentProcess().ProcessorAffinity = new IntPtr((int)Math.Pow(2, m_curProcessor));

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
            SetHighPriority();

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
            LogLine($"Running {dayType.Namespace}.{dayType.Name}.Part{part} Performance [Requires {RecordCount - existingRecords} Runs]");
            LogLine("...Warming up");
            RunWarmup();

            DateTime timeout = DateTime.Now.AddMilliseconds(MaxPerfTimeMs);

            long i = 0;
            long maxI = RecordCount - existingRecords;
            for (; i < maxI; ++i)
            {
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    if (i > 0 && i % (RecordCount / 20) == 0)
                    {
                        LogLine($"...{i} runs completed");
                    }
                }
                else
                {
                    LogSameLine(string.Format("...{0:000.0}% [timeout in {1}]", (double)i / (double)(maxI) * 100.0f, (timeout - DateTime.Now).ToString(@"hh\:mm\:ss\.fff")));
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
            }
            if (System.Diagnostics.Debugger.IsAttached)
            {
                LogLine($"...{maxI} runs completed");
            }
            else
            {
                if (DateTime.Now > timeout)
                {
                    LogSameLine(string.Format("...{0:000.0}% [timed out]\n\r", (double)i / (double)(maxI) * 100.0f));
                }
                else
                {
                    LogSameLine(string.Format("...{0:000.0}%{1}\n\r", (double)i / (double)(maxI) * 100.0f, new string(' ', 35)));
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

            LogLine($"Running {baseNamespace} Performance");
            LogLine("");

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

            LogLine($"Showing {baseNamespace} Performance");
            LogLine("");

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
                perfDataFileName = Path.Combine(workingDir, "perfdata_debugger.json");
            }
            else
            {
                perfDataFileName = Path.Combine(workingDir, "perfdata_cmd.json");
            }
            if (File.Exists(perfDataFileName))
            {
                LogLine($"Loading {perfDataFileName}");
                LogLine("");
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
            LogLine($"Saving {perfDataFileName}");
            LogLine("");
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
            LogLine($"{baseNamespace} Performance Metrics");
            LogLine("");

            double p1Total = 0.0f;
            double p2Total = 0.0f;
            double min = double.MaxValue;
            string minStr = "";
            double max = double.MinValue;
            string maxStr = "";
            Dictionary<string, Type> days = GetDaysInNamespace(baseNamespace);
            int maxStringLength = 0;
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
                            }
                            else
                            {
                                // todo: make this smarter
                                min = Math.Min(min, stats.Avg);
                                if (min == stats.Avg)
                                {
                                    minStr = logLine;
                                }

                                max = Math.Max(max, stats.Avg);
                                if (max == stats.Avg)
                                {
                                    maxStr = logLine;
                                }

                                if (part == Part.One)
                                {
                                    p1Total += stats.Avg;
                                }

                                if (part == Part.Two)
                                {
                                    p2Total += stats.Avg;
                                }

                                logLine += string.Format(" Avg={0:0.000} (ms) [{1} Records, Min={2:0.000} (ms), Max={3:0.000} (ms)]", stats.Avg, stats.Count, stats.Min, stats.Max);
                            }
                            LogLine(logLine);
                            maxStringLength = Math.Max(maxStringLength, logLine.Length);
                        }
                        LogLine(new string('#', maxStringLength));
                    }
                }
            }

            double totals = p1Total + p2Total;
            LogLine($"[{baseNamespace[^4..]}|total|part1|--] Avg={TimeSpan.FromMilliseconds(p1Total).ToString(@"ss\.ffffff")} (s)");
            LogLine($"[{baseNamespace[^4..]}|total|part2|--] Avg={TimeSpan.FromMilliseconds(p2Total).ToString(@"ss\.ffffff")} (s)");
            LogLine($"[{baseNamespace[^4..]}|total|-all-|--] Avg={TimeSpan.FromMilliseconds(totals).ToString(@"ss\.ffffff")} (s)");
            LogLine(new string('#', maxStringLength));

            if (totals > 0)
            {
                LogLine($"{minStr} Min={TimeSpan.FromMilliseconds(min).ToString(@"ss\.ffffff")} (s) [{string.Format("{0:00.00}%", min / (p1Total + p2Total) * 100.0f)}]");
                LogLine($"{maxStr} Max={TimeSpan.FromMilliseconds(max).ToString(@"ss\.ffffff")} (s) [{string.Format("{0:00.00}%", max / (p1Total + p2Total) * 100.0f)}]");
                LogLine(new string('#', maxStringLength));
                LogLine(new string('#', maxStringLength));
            }
        }

        /// <summary>
        /// Console.Write with a timestamp
        /// </summary>
        /// <param name="message"></param>
        private void Log(string message)
        {
            Console.Write($"{Util.GetLogTimeStamp()} {message}");
        }

        /// <summary>
        /// Console.Write without a timestamp
        /// </summary>
        /// <param name="message"></param>
        private void LogAppend(string message)
        {
            Console.Write($"{message}");
        }

        /// <summary>
        /// Console.WriteLine with a timestamp
        /// </summary>
        /// <param name="message"></param>
        private void LogLine(string message)
        {
            Console.WriteLine($"{Util.GetLogTimeStamp()} {message}");
        }

        /// <summary>
        /// Console.Write into the previous console location
        /// </summary>
        /// <param name="message"></param>
        private void LogSameLine(string message)
        {
            Console.Write($"\r{Util.GetLogTimeStamp()} {message}");
        }
    }
}
