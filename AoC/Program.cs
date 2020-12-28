using System.Threading;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;

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
        private long RecordCount { get { return 1000; } }
        private long MaxPerfTimeMs { get { return 3600000; } } // TODO: convert to command line arg

        public Program(string[] args)
        {
            try
            {
                Day.UseLogs = true;

                // parse command args
                CommandLineArgs commandLineArgs = ParseCommandLineArgs(args);

                if (commandLineArgs.HasArg(CommandLineArgs.SupportedArgument.Help))
                {
                    commandLineArgs.PrintHelp(LogLine);
                    return;
                }

                // get the namespace to use
                string baseNamespace = nameof(AoC._2020);
                if (commandLineArgs.HasArgValue(CommandLineArgs.SupportedArgument.Namespace))
                {
                    baseNamespace = commandLineArgs.Args[CommandLineArgs.SupportedArgument.Namespace];
                }

                // run the day specified or the latest day
                if (commandLineArgs.HasArg(CommandLineArgs.SupportedArgument.Day))
                {
                    Day day = RunDay(baseNamespace, commandLineArgs.Args[CommandLineArgs.SupportedArgument.Day]);
                    if (day == null)
                    {
                        LogLine($"Unable to find {baseNamespace}.{commandLineArgs.Args[CommandLineArgs.SupportedArgument.Day]}");
                    }
                    else
                    {
                        LogLine("");
                    }
                }
                else if (!commandLineArgs.HasArg(CommandLineArgs.SupportedArgument.SkipLatest))
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

                // run performance tests
                if (commandLineArgs.HasArg(CommandLineArgs.SupportedArgument.RunPerf))
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
            // Uses the second Core or Processor for the Test
            Process.GetCurrentProcess().ProcessorAffinity = new IntPtr(2);

            // Prevents "Normal" processes from interrupting Threads
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;

            // Prevents "Normal" Threads from interrupting this thread
            Thread.CurrentThread.Priority = ThreadPriority.Highest;
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
        private bool RunPerformance(Type dayType, TestPart testPart, long existingRecords, ref RunData runData)
        {
            LogLine($"Warming up...");
            RunWarmup();

            Timer timer = new Timer();
            timer.Start();
            LogLine($"Running {dayType.Namespace}.{dayType.Name} Performance [Requires {RecordCount - existingRecords} Runs]");
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
                    LogSameLine(string.Format("...{0:000.0}% [timeout in {1:000.000} (s)]", (double)i / (double)(maxI) * 100.0f, (MaxPerfTimeMs - timer.GetElapsedMs()) / 1000));
                }

                ObjectHandle handle = Activator.CreateInstance(Assembly.GetExecutingAssembly().FullName, dayType.FullName);
                if (handle == null)
                {
                    break;
                }

                Day day = (Day)handle.Unwrap();
                day.RunProblem(testPart);
                runData.AddData(day);

                if (timer.GetElapsedMs() > MaxPerfTimeMs)
                {
                    break;
                }
            }
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                LogSameLine(string.Format("...{0:000.0}%                                   \n\r", (double)i / (double)(maxI) * 100.0f));
            }
            else
            {
                LogLine($"...{maxI} runs completed");
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

            RunData runData;
            string runDataFileName;
            LoadRunData(out runDataFileName, out runData);
            Dictionary<string, Type> days = GetDaysInNamespace(baseNamespace);
            foreach (string key in days.Keys)
            {
                ObjectHandle handle = Activator.CreateInstance(Assembly.GetExecutingAssembly().FullName, days[key].FullName);
                if (handle != null)
                {
                    Day day = (Day)handle.Unwrap();
                    if (day != null)
                    {
                        for (TestPart testPart = TestPart.One; testPart <= TestPart.Two; ++testPart)
                        {
                            if (day.GetSolutionVersion(testPart) == "v0")
                            {
                                continue;
                            }

                            Stats stats = runData.Get(day.Year, day.DayName, day.GetSolutionVersion(testPart), testPart);
                            if (stats == null)
                            {
                                RunPerformance(day.GetType(), testPart, 0, ref runData);
                            }
                            else if (stats.Count < RecordCount)
                            {
                                RunPerformance(day.GetType(), testPart, stats.Count, ref runData);
                            }
                        }
                    }
                }
            }

            SaveRunData(runDataFileName, runData);
            PrintMetrics(baseNamespace, runData);
            Day.UseLogs = true;
        }

        /// <summary>
        /// Load the save data from previous runs
        /// </summary>
        /// <param name="runDataFileName"></param>
        /// <param name="runData"></param>
        private void LoadRunData(out string runDataFileName, out RunData runData)
        {
            string workingDir = Util.WorkingDirectory;
            if (System.Diagnostics.Debugger.IsAttached)
            {
                runDataFileName = Path.Combine(workingDir, "rundata_debugger.json");
            }
            else
            {
                runDataFileName = Path.Combine(workingDir, "rundata_cmd.json");
            }
            if (File.Exists(runDataFileName))
            {
                LogLine($"Loading {runDataFileName}");
                LogLine("");
                string rawJson = File.ReadAllText(runDataFileName);
                runData = JsonConvert.DeserializeObject<RunData>(rawJson);
            }
            else
            {
                runData = new RunData();
            }
        }

        /// <summary>
        /// Save the current run data to a specific file
        /// </summary>
        /// <param name="runDataFileName"></param>
        /// <param name="runData"></param>
        private void SaveRunData(string runDataFileName, RunData runData)
        {
            LogLine($"Saving {runDataFileName}");
            LogLine("");
            string rawJson = JsonConvert.SerializeObject(runData, Formatting.Indented);
            using (StreamWriter sWriter = new StreamWriter(runDataFileName))
            {
                sWriter.Write(rawJson);
            }
        }

        /// <summary>
        /// Print out all the metrics from run data
        /// </summary>
        /// <param name="baseNamespace"></param>
        /// <param name="runData"></param>
        private void PrintMetrics(string baseNamespace, RunData runData)
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
                        for (TestPart testPart = TestPart.One; testPart <= TestPart.Two; ++testPart)
                        {
                            string solutionVersion = day.GetSolutionVersion(testPart);
                            Stats stats = runData.Get(day.Year, day.DayName, solutionVersion, testPart);
                            string logLine = $"[{day.Year}|{day.DayName}|part{(int)testPart}|{solutionVersion}]";
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

                                if (testPart == TestPart.One)
                                {
                                    p1Total += stats.Avg;
                                }

                                if (testPart == TestPart.Two)
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
