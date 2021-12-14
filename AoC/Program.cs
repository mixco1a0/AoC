using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using System.Threading;
using System.Runtime.InteropServices;

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

    static class Win32
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetConsoleMode(IntPtr stdHandle, out int mode);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetConsoleMode(IntPtr stdHandle, int mode);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetStdHandle(int stdHande);
    }

    record Color(int R, int G, int B) { }

    class ConsoleExtension
    {
        private static IntPtr s_stdHandle;

        private const string m_formatStringStart = "\x1b[{0};2;";
        private const string m_formatStringColor = "{1};{2};{3}m";
        //private const string           m_formatStringContent = "{4}";
        //private const string m_formatStringEnd = "\x1b[0m";
        //private static readonly string m_formatStringFull    = $"{m_formatStringStart}{m_formatStringColor}{m_formatStringContent}{m_formatStringEnd}";
        private static readonly string m_formatStringFull = $"{m_formatStringStart}{m_formatStringColor}";

        public enum Ground : byte
        {
            Fore = 38,
            Back = 48
        }

        static ConsoleExtension()
        {
            s_stdHandle = Win32.GetStdHandle(-11);
            int mode;
            Win32.GetConsoleMode(s_stdHandle, out mode);
            Win32.SetConsoleMode(s_stdHandle, mode | 0x4);
        }

        public static string GetColorFormat(Ground ground, AoC.Color color)
        {
            return GetColorFormat(ground, color.R, color.G, color.B);
        }

        public static string GetColorFormat(Ground ground, int r, int g, int b)
        {
            return string.Format(m_formatStringFull, (byte)ground, r, g, b);
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
        private Color White = new Color(255, 255, 255);

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

                // TODO: programatically find the latest year to use
                // get the namespace to use
                string baseNamespace = nameof(AoC._2021);
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
            ObjectHandle warmupHandle = Activator.CreateInstance(Assembly.GetExecutingAssembly().FullName, dayType.FullName);
            Day warmupDay = (Day)warmupHandle.Unwrap();
            warmupDay.RunProblem(part);

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

            string minStr = "";
            string maxStr = "";
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

            double min = avgs.SelectMany(p => p.Value).Min(v => v);
            double max = avgs.SelectMany(p => p.Value).Max(v => v);
            Func<double, double> getAvg = (double val) =>
            {
                if (val == double.NaN)
                {
                    return max;
                }
                return (val - min) / (max - min);
            };
            Func<double, AoC.Color> getColor = (double avg) =>
            {
                int r = (int)(avg * 255.0f);
                int g = (int)(255.0f - avg * 255.0f);
                return new Color(r, g, 0);
            };

            string separator = new string('#', maxStringLength);
            for (int i = 0; i < logs[Part.One].Count; ++i)
            {
                for (Part part = Part.One; part <= Part.Two; ++part)
                {
                    double minColor = getAvg(mins[part][i]);
                    double avgColor = getAvg(avgs[part][i]);
                    double maxColor = getAvg(maxs[part][i]);
                    List<AoC.Color> colors = new List<Color>() { getColor(minColor), getColor(avgColor), getColor(maxColor) };
                    LogLineColored(logs[part][i], colors);
                }
                LogLine(separator);
            }

            double p1Total = avgs[Part.One].Sum();
            double p2Total = avgs[Part.Two].Sum();
            double totals = p1Total + p2Total;
            LogLine($"[{baseNamespace[^4..]}|total|part1|--] Sum={TimeSpan.FromMilliseconds(p1Total).ToString(@"ss\.ffffff")} (s)");
            LogLine($"[{baseNamespace[^4..]}|total|part2|--] Sum={TimeSpan.FromMilliseconds(p2Total).ToString(@"ss\.ffffff")} (s)");
            LogLine($"[{baseNamespace[^4..]}|total|-all-|--] Sum={TimeSpan.FromMilliseconds(totals).ToString(@"ss\.ffffff")} (s)");
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
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="colors"></param>
        private void LogLineColored(string message, List<AoC.Color> colors)
        {
            string[] split = Regex.Split(message, @"(\^[^\^]*\^)");
            int colorIndex = 0;
            Console.Write($"{Util.GetLogTimeStamp()} ");
            for (int i = 0; i < split.Length; ++i)
            {
                if (split[i][0] == '^')
                {
                    string format = ConsoleExtension.GetColorFormat(ConsoleExtension.Ground.Fore, colors[colorIndex++]);
                    Console.Write(string.Format("{0}{1}", format, split[i].Substring(1, split[1].Length - 2)));
                }
                else
                {
                    Console.Write(string.Format("{0}{1}", ConsoleExtension.GetColorFormat(ConsoleExtension.Ground.Fore, White), split[i]));
                }
            }
            Console.WriteLine();
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
