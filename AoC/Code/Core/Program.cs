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
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace AoC.Core
{
    class Program
    {
        private const int DefaultRecordCount = 100;
        private int m_recordCount = DefaultRecordCount;
        private int RecordCount { get { return m_recordCount; } }

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
                    m_recordCount = int.Parse(Args[CommandLine.ESupportedArgument.PerfRecordCount]);
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
        private void SetThreadPriority()
        {
            // prevent process interuptions
            if (Debugger.IsAttached)
            {
                Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
            }
            else
            {
                // Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
                Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.RealTime;
            }
        }

        /// <summary>
        /// Run a warmup sequence so that the cpu is ready to run tests
        /// </summary>
        private void RunWarmup()
        {
            Action<long> Work = (long seed) =>
            {
                const int count = 100000000;
                long result = seed;
                for (int i = 0; i < count; ++i)
                {
                    result ^= i ^ seed;
                }
            };

            int[] indices = Enumerable.Range(0, Environment.ProcessorCount * 2).ToArray();
            ParallelOptions options = new()
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount
            };
            Parallel.ForEach(indices, options, (index) =>
            {
                Util.Timer timer = new Util.Timer();
                timer.Start();
                while (timer.GetElapsedMs() < 500)
                {
                    Work(Environment.TickCount);
                }
            });
        }

        /// <summary>
        /// Run performance for the specified day
        /// </summary>
        /// <param name="dayType"></param>
        /// <param name="existingRecords"></param>
        /// <param name="runData"></param>
        /// <returns></returns>
        private bool RunPerformance(Day day, Part part, int existingRecords, ref PerfData runData)
        {
            SetThreadPriority();
            Log.WriteLine(Log.ELevel.Info, $"Running {day.GetType().Namespace}.{day.GetType().Name}.Part{part} Performance [Requires {RecordCount - existingRecords} Runs]");
            Log.WriteLine(Log.ELevel.Info, "...Warming up");
            RunWarmup();
            day.RunProblem(part);

            Log.WriteLine(Log.ELevel.Info, "Performance starting...");

            // setup
            int completedDays = 0;
            int maxDays = RecordCount - existingRecords;
            Day[] allDays = new Day[maxDays];

            // keep track of progress
            CancellationTokenSource cancelToken = new CancellationTokenSource();
            DateTime timeout = DateTime.Now.AddMilliseconds(MaxPerfTimeMs);
            Task monitorTask = Task.Factory.StartNew(() =>
            {
                DateTime printTime = Debugger.IsAttached ? DateTime.Now.AddSeconds(5) : DateTime.Now.AddMilliseconds(250);
                while (!cancelToken.IsCancellationRequested)
                {
                    // print 
                    if (DateTime.Now > printTime)
                    {
                        IEnumerable<double> completedDays = allDays.Where(d => d != null && d.TimeResults.ContainsKey(Part.Two)).Select(d => d.TimeResults[part]);
                        if (completedDays.Any())
                        {
                            string timeResultsString = string.Format("[avg={0:00000.000}][min={1:00000.000}][max={2:00000.000}]", completedDays.Average(), completedDays.Min(), completedDays.Max());

                            if (Debugger.IsAttached)
                            {
                                Log.WriteLine(Log.ELevel.Info, $"...{completedDays} runs completed");
                            }
                            else
                            {
                                double percentComplete = (double)completedDays.Count() / (double)(maxDays) * 100.0f;
                                string timeoutString = (timeout - DateTime.Now).ToString(@"hh\:mm\:ss\.fff");
                                Log.WriteSameLine(Log.ELevel.Info, string.Format("...{0:000.0}% {1}[timeout in {2}]", percentComplete, timeResultsString, timeoutString));
                            }
                        }
                    }

                    // timeout if needed
                    if (DateTime.Now > timeout)
                    {
                        cancelToken.Cancel();
                    }

                    // wait again
                    cancelToken.Token.WaitHandle.WaitOne(25);
                }

                // completion logs
                int trueCompletionCount = allDays.Where(d => d != null).Select(d => d.TimeResults[part]).Count();
                if (Debugger.IsAttached)
                {
                    Log.WriteLine(Log.ELevel.Info, $"...{trueCompletionCount} runs completed");
                }
                else
                {
                    double percentComplete = (double)trueCompletionCount / (double)(maxDays) * 100.0f;
                    if (DateTime.Now > timeout)
                    {
                        Log.WriteLine(Log.ELevel.Info, string.Format("...{0:000.0}% [timed out]{1}\n\r", percentComplete, new string(' ', 65)));
                    }
                    else
                    {
                        Log.WriteSameLine(Log.ELevel.Info, string.Format("...{0:000.0}%{1}\n\r", percentComplete, new string(' ', 75)));
                    }
                    Log.WriteLine(Log.ELevel.Info, "");
                }
            });

            // run all of the days
            int[] indices = Enumerable.Range(0, maxDays).ToArray();
            ParallelOptions options = new()
            {
                CancellationToken = cancelToken.Token,
                MaxDegreeOfParallelism = Environment.ProcessorCount / 4
            };
            Parallel.ForEach(indices, options, (index) =>
            {
                ObjectHandle handle = Activator.CreateInstance(Assembly.GetExecutingAssembly().FullName, day.GetType().FullName);
                if (handle == null)
                {
                    return;
                }

                allDays[index] = (Day)handle.Unwrap();
                allDays[index].Input = day.Input;
                allDays[index].Output = day.Output;
                allDays[index].RunProblem(part);
                Interlocked.Increment(ref completedDays);
            });

            // cleanup outputs
            cancelToken.Cancel();
            monitorTask.Wait();

            // add up times
            double timeResultsTotal = 0.0;
            foreach (Day curDay in allDays)
            {
                if (curDay == null)
                {
                    continue;
                }

                runData.AddData(curDay);
                timeResultsTotal += day.TimeResults[part];
            }

            return completedDays == maxDays;
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
            LoadPerfRawData(baseNamespace, ref perfData);
            Dictionary<string, Type> days = GetDaysInNamespace(baseNamespace);
            foreach (string key in days.Keys)
            {
                ObjectHandle handle = Activator.CreateInstance(Assembly.GetExecutingAssembly().FullName, days[key].FullName);
                if (handle != null)
                {
                    Day day = (Day)handle.Unwrap();
                    if (day != null)
                    {
                        // day.ParseInputFile();
                        // day.ParseOutputFile();
                        for (Part part = Part.One; part <= Part.Two; ++part)
                        {
                            if (day.GetSolutionVersion(part) == "v0")
                            {
                                continue;
                            }

                            PerfStat stats = perfData.Get(day.Year, day.DayName, part, day.GetSolutionVersion(part));
                            if (stats == null)
                            {
                                RunPerformance(day, part, 0, ref perfData);
                            }
                            else if (stats.Count < RecordCount)
                            {
                                RunPerformance(day, part, stats.Count, ref perfData);
                            }
                        }
                    }
                }
            }

            SavePerfData(runDataFileName, perfData);
            SavePerfRawData(baseNamespace, perfData);
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
            LoadPerfRawData(baseNamespace, ref perfData);
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
            if (Debugger.IsAttached)
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

        private void LoadPerfRawData(string baseNamespace, ref PerfData perfData)
        {
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
                            string solutionVersion = day.GetSolutionVersion(part);
                            PerfStat stats = perfData.Get(day.Year, day.DayName, part, solutionVersion);
                            if (stats == null)
                            {
                                continue;
                            }

                            string fileName = string.Format("{0}.{1}.{2}.txt", day.DayName, part.ToString(), solutionVersion);
                            string inputFile = Path.Combine(Core.WorkingDirectory.Get, "Data", day.Year, "Perf", fileName);
                            if (File.Exists(inputFile))
                            {
                                stats.Raw = File.ReadAllLines(inputFile).Select(double.Parse).ToList();
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Save the current perf data to a specific file
        /// </summary>
        /// <param name="perfDataFileName"></param>
        /// <param name="perfData"></param>
        private void SavePerfData(string perfDataFileName, PerfData perfData)
        {
            Log.WriteLine(Log.ELevel.Info, $"Saving {perfDataFileName}\n");

            string rawJson = JsonConvert.SerializeObject(perfData, Formatting.Indented);
            using (StreamWriter sWriter = new StreamWriter(perfDataFileName))
            {
                sWriter.Write(rawJson);
            }
        }

        private void SavePerfRawData(string baseNamespace, PerfData perfData)
        {
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
                            string solutionVersion = day.GetSolutionVersion(part);
                            PerfStat stats = perfData.Get(day.Year, day.DayName, part, solutionVersion);
                            if (stats == null || stats.Raw.Count == 0)
                            {
                                continue;
                            }

                            stats.Raw.Sort();
                            string fileName = string.Format("{0}.{1}.{2}.txt", day.DayName, part.ToString().ToLower(), solutionVersion);
                            string outputFile = Path.Combine(Core.WorkingDirectory.Get, "Data", day.Year, "Perf", fileName);
                            using (StreamWriter sWriter = new StreamWriter(outputFile, false))
                            {
                                foreach (double rawTime in stats.Raw)
                                {
                                    sWriter.WriteLine(rawTime);
                                }
                            }
                        }
                    }
                }
            }
        }

        internal class TimeMagnitude : Base.Pair<double, string> { public TimeMagnitude(double d, string s) : base(d, s) { } }
        internal class ColorRange : Base.RangeF { public ColorRange(float min, float max) : base(min, max) { } }

        /// <summary>
        /// Print out all the performance information from run data
        /// </summary>
        /// <param name="baseNamespace"></param>
        /// <param name="perfData"></param>
        private void PrintPerf(string baseNamespace, PerfData perfData)
        {
            Log.WriteLine(Log.ELevel.Info, $"{baseNamespace} Performance Metrics\n");
            bool compactPerf = Args.Has(CommandLine.ESupportedArgument.CompactPerf);

            Dictionary<string, Type> days = GetDaysInNamespace(baseNamespace);
            int maxStringLength = 0;
            Dictionary<Part, List<string>> logs = new Dictionary<Part, List<string>>();
            Dictionary<Part, List<double>> mins = new Dictionary<Part, List<double>>();
            Dictionary<Part, List<double>> avgs = new Dictionary<Part, List<double>>();
            Dictionary<Part, List<double>> maxs = new Dictionary<Part, List<double>>();
            Dictionary<Part, List<Color>> minColors = new Dictionary<Part, List<Color>>();
            Dictionary<Part, List<Color>> avgColors = new Dictionary<Part, List<Color>>();
            Dictionary<Part, List<Color>> maxColors = new Dictionary<Part, List<Color>>();
            for (Part part = Part.One; part <= Part.Two; ++part)
            {
                logs[part] = new List<string>();
                mins[part] = new List<double>();
                avgs[part] = new List<double>();
                maxs[part] = new List<double>();

                minColors[part] = new List<Color>();
                avgColors[part] = new List<Color>();
                maxColors[part] = new List<Color>();
            }

            List<TimeMagnitude> timeMagnitudes = new List<TimeMagnitude>();
            timeMagnitudes.Add(new TimeMagnitude(60000.0, " m"));
            timeMagnitudes.Add(new TimeMagnitude(1000.0, " s"));
            timeMagnitudes.Add(new TimeMagnitude(1.0, "ms"));
            timeMagnitudes.Add(new TimeMagnitude(.001, "µs"));
            Func<double, TimeMagnitude> getTimeMagnitude = (double val) =>
            {
                foreach (var pair in timeMagnitudes)
                {
                    if (val >= pair.First)
                    {
                        return new TimeMagnitude(val / pair.First, pair.Last);
                    }
                }
                return timeMagnitudes.Last();
            };

            Dictionary<string, Color> timeUnitColor = new Dictionary<string, Color>();
            timeUnitColor[" m"] = Color.Violet;
            timeUnitColor[" s"] = Log.Negative;
            timeUnitColor["ms"] = Log.Neutral;
            timeUnitColor["µs"] = Log.Positive;

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
                                double curAvg = stats.GetAvg();
                                if (min > curAvg)
                                {
                                    min = curAvg;
                                    minStr = logLine;
                                }

                                if (max < curAvg)
                                {
                                    max = curAvg;
                                    maxStr = logLine;
                                }

                                double curMin = stats.GetMin();
                                double curMax = stats.GetMax();
                                mins[part].Add(curMin);
                                avgs[part].Add(curAvg);
                                maxs[part].Add(curMax);

                                if (compactPerf)
                                {
                                    TimeMagnitude tm = getTimeMagnitude(curAvg);
                                    avgColors[part].Add(timeUnitColor[tm.Last]);
                                    logLine += string.Format(" Avg={0}{1:000.000}{0} ({0}{2}{0})", Log.ColorMarker, tm.First, tm.Last);
                                }
                                else
                                {
                                    TimeMagnitude avgTm = getTimeMagnitude(curAvg);
                                    TimeMagnitude minTm = getTimeMagnitude(curMin);
                                    TimeMagnitude maxTm = getTimeMagnitude(curMax);
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

            Func<double, double> getAvg = (double val) =>
            {
                if (val.Equals(double.NaN))
                {
                    return double.NaN;
                }
                return (val - min) / (max - min);
            };

            ColorRange loR = new ColorRange(Log.Neutral.R, Log.Negative.R);
            ColorRange loG = new ColorRange(Log.Neutral.G, Log.Negative.G);
            ColorRange loB = new ColorRange(Log.Neutral.B, Log.Negative.B);
            ColorRange hiR = new ColorRange(Log.Positive.R, Log.Neutral.R);
            ColorRange hiG = new ColorRange(Log.Positive.G, Log.Neutral.G);
            ColorRange hiB = new ColorRange(Log.Positive.B, Log.Neutral.B);
            Func<double, Color> getColor = (double avg) =>
            {
                if (avg.Equals(double.NaN))
                {
                    return Log.Neutral;
                }

                Func<double, ColorRange, int> getRangedColor = (double avg, ColorRange colorRange) =>
                {
                    return Math.Max(Math.Min((int)((int)(((colorRange.Max - colorRange.Min) * avg) + colorRange.Min)), 255), 0);
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

            if (compactPerf)
            {
                int maxLength = logs.SelectMany(l => l.Value).Select(lv => lv.Replace($"{Log.ColorMarker}", "")).Max(lv => lv.Length) + 2;
                string separator = new string('#', maxLength * 2 + 10);
                for (int i = 0; i < logs[Part.Two].Count; ++i)
                {
                    if (i % 5 == 0)
                    {
                        Log.WriteLine(Log.ELevel.Info, separator);
                    }
                    Log.Write(Log.ELevel.Info, "##  ");
                    Log.WriteAppend(Log.ELevel.Info, logs[Part.One][i], new List<Color>() { getColor(getAvg(avgs[Part.One][i])), avgColors[Part.One][i] });
                    Log.WriteAppend(Log.ELevel.Info, new string(' ', maxLength - logs[Part.One][i].Replace($"{Log.ColorMarker}", "").Length));
                    Log.WriteAppend(Log.ELevel.Info, "##  ");
                    Log.WriteAppend(Log.ELevel.Info, logs[Part.Two][i], new List<Color>() { getColor(getAvg(avgs[Part.Two][i])), avgColors[Part.Two][i] });
                    Log.WriteAppend(Log.ELevel.Info, new string(' ', maxLength - logs[Part.Two][i].Replace($"{Log.ColorMarker}", "").Length));
                    Log.WriteAppend(Log.ELevel.Info, "##");
                    Log.WriteAppendEnd(Log.ELevel.Info);
                }
                Log.WriteLine(Log.ELevel.Info, separator);
                maxStringLength = separator.Length;
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
                        List<Color> colors = new List<Color>() { getColor(minColor), getColor(avgColor), getColor(maxColor), avgColors[part][i], minColors[part][i], maxColors[part][i] };
                        Log.WriteLine(Log.ELevel.Info, logs[part][i], colors);
                    }
                    Log.WriteLine(Log.ELevel.Info, separator);
                }
            }

            double p1Total = avgs[Part.One].Where(a => !a.Equals(double.NaN)).Sum();
            double p2Total = avgs[Part.Two].Where(a => !a.Equals(double.NaN)).Sum();
            double totals = p1Total + p2Total;

            Func<string, string, string, string> getLogHeader = (string day, string part, string version) =>
            {
                return string.Format("[{0}|{1}|{2}|{3}]", baseNamespace[^4..], day, part, version);
            };

            Action<double, string, string, string, Color> logTotal = (double time, string logHeader, string valueType, string logEnder, Color color) =>
            {
                string noColor = string.Format("{0}{1} {2}=", compactPerf ? "##  " : "", logHeader, valueType);
                Log.Write(Log.ELevel.Info, noColor);

                TimeMagnitude tm = getTimeMagnitude(time);
                string yesColor = string.Format("{0:000.000}", tm.First);
                Log.WriteAppend(Log.ELevel.Info, string.Format("{0}{1}{0}", Log.ColorMarker, yesColor), new List<Color>() { color });

                string remainingLog = string.Format(" ({0}{1}{0}){2}", Log.ColorMarker, tm.Last, logEnder);
                Log.WriteAppend(Log.ELevel.Info, remainingLog, new List<Color>() { timeUnitColor[tm.Last] });

                if (compactPerf)
                {
                    int curLen = noColor.Length + yesColor.Length + remainingLog.Length - 2;
                    string end = new string(' ', maxStringLength - curLen - 2);
                    Log.WriteAppend(Log.ELevel.Info, $"{end}##");
                }

                Log.WriteAppendEnd(Log.ELevel.Info);
            };

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