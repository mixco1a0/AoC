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

        private string m_runDataFile;
        private RunData m_runData;

        public Program(string[] args)
        {
            try
            {
                Day.UseLogs = true;

                // run the latest day in the given namespace
                string baseNamespace = nameof(AoC._2020);

                Day latestDay = RunLatestDay(baseNamespace);
                if (latestDay == null)
                    LogLine($"Unable to find any solutions for namespace {baseNamespace}");

                // if the latest solution includes valid versions, run performance
                else if (latestDay.GetSolutionVersion(TestPart.One) != "v0" && latestDay.GetSolutionVersion(TestPart.Two) != "v0")
                    RunPerformance(baseNamespace);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
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
            LogLine("");
            LogLine($"Running Latest {baseNamespace} Advent of Code");
            LogLine("");

            Dictionary<string, Type> days = GetDaysInNamespace(baseNamespace);
            if (days.Count > 0)
            {
                string latestDay = days.Keys.Select(k => k[3..]).Select(int.Parse).Max().ToString();
                ObjectHandle handle = Activator.CreateInstance(Assembly.GetExecutingAssembly().FullName, days[days.Keys.Where(k => k.Contains(latestDay)).First()].FullName);
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
        /// Run performance for the specific day
        /// </summary>
        /// <param name="dayType"></param>
        /// <param name="existingRecords"></param>
        private void RunPerformance(Type dayType, long existingRecords)
        {
            LogLine($"Running {dayType.Namespace}.{dayType.Name} Performance [Requires {RecordCount - existingRecords} Runs]");
            for (int i = 0; i < (RecordCount - existingRecords); ++i)
            {
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    if (i > 0 && i % (RecordCount / 20) == 0)
                        LogLine($"...{i} runs completed");
                }
                else
                    LogSameLine(string.Format("{0:000.00}%", (double)i / (double)(RecordCount - existingRecords) * 100.0f));


                ObjectHandle handle = Activator.CreateInstance(Assembly.GetExecutingAssembly().FullName, dayType.FullName);
                if (handle == null)
                    break;

                Day day = (Day)handle.Unwrap();
                day.Run();
                m_runData.AddData(day);
            }
            if (!System.Diagnostics.Debugger.IsAttached)
                LogLine("100.00%");
            else
                LogLine($"...{RecordCount - existingRecords} runs completed");
        }

        private void RunPerformance(string baseNamespace)
        {
            Day.UseLogs = false;
            LogLine("");
            LoadRunData();
            LogLine($"Running {baseNamespace} Performance");


            Dictionary<string, Type> days = GetDaysInNamespace(baseNamespace);
            foreach (string key in days.Keys)
            {
                ObjectHandle handle = Activator.CreateInstance(Assembly.GetExecutingAssembly().FullName, days[key].FullName);
                if (handle != null)
                {
                    Day day = (Day)handle.Unwrap();
                    if (day != null && day.GetSolutionVersion(TestPart.One) != "v0" && day.GetSolutionVersion(TestPart.Two) != "v0")
                    {
                        for (TestPart testPart = TestPart.One; testPart <= TestPart.Two; ++testPart)
                        {
                            Stats stats = m_runData.Get(day.Year, day.DayName, day.GetSolutionVersion(testPart), testPart);
                            if (stats == null)
                                RunPerformance(day.GetType(), 0);
                            else if (stats.Count < RecordCount)
                                RunPerformance(day.GetType(), stats.Count);
                        }
                    }
                }
            }

            SaveRunData();
            PrintMetrics(baseNamespace);
            LogLine("");
            Day.UseLogs = true;
        }

        private void LoadRunData()
        {
            string workingDir = Util.WorkingDirectory;
            if (System.Diagnostics.Debugger.IsAttached)
                m_runDataFile = Path.Combine(workingDir, "rundata_debugger.json");
            else
                m_runDataFile = Path.Combine(workingDir, "rundata_cmd.json");
            if (File.Exists(m_runDataFile))
            {
                LogLine($"Loading {m_runDataFile}");
                string rawJson = File.ReadAllText(m_runDataFile);
                m_runData = JsonConvert.DeserializeObject<RunData>(rawJson);
            }
            else
            {
                m_runData = new RunData();
            }
        }


        private void SaveRunData()
        {
            LogLine($"Saving {m_runDataFile}");
            string rawJson = JsonConvert.SerializeObject(m_runData, Formatting.Indented);
            using (StreamWriter sWriter = new StreamWriter(m_runDataFile))
            {
                sWriter.Write(rawJson);
            }
        }

        private void PrintMetrics(string baseNamespace)
        {
            LogLine("");
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
                            Stats stats = m_runData.Get(day.Year, day.DayName, solutionVersion, testPart);
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
                                    minStr = logLine;

                                max = Math.Max(max, stats.Avg);
                                if (max == stats.Avg)
                                    maxStr = logLine;

                                if (testPart == TestPart.One)
                                    p1Total += stats.Avg;
                                if (testPart == TestPart.Two)
                                    p2Total += stats.Avg;

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

        private void Log(string message)
        {
            Console.Write($"|{DateTime.Now.ToString("hh:mm:ss.fff")}| {message}");
        }

        private void LogLine(string message)
        {
            Console.WriteLine($"|{DateTime.Now.ToString("hh:mm:ss.fff")}| {message}");
        }

        private void LogSameLine(string message)
        {
            Console.Write($"\r|{DateTime.Now.ToString("hh:mm:ss.fff")}| {message}");
        }
    }
}
