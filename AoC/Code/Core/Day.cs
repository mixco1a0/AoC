using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace AoC.Core
{
    public enum Part
    {
        One = 1,
        Two = 2
    }

    abstract public class Day
    {
        public static bool UseLogs { get; set; }
        private const char LogCorrect = '#';
        private const char LogUnknown = '?';
        private const char LogIncorrect = '!';
        private static readonly string DefaultLogID = new string('.', 27);

        private enum RunType
        {
            Testing,
            Problem
        }

        #region Required Overrides
        public virtual string GetSolutionVersion(Part part) => "v0";
        public virtual bool SkipTestData => false;
        protected abstract List<TestDatum> GetTestData();
        public virtual void RunWarmup() { }
        protected abstract string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables);
        protected abstract string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables);
        #endregion


        public string Year { get; private set; }
        public string DayName { get; private set; }
        public Dictionary<Part, double> TimeResults { get; private set; }
        public Dictionary<Part, double> TimeWasted { get; private set; }
        private double TimeWaste { get; set; }
        private bool m_forceTests;
        private string m_debugFile;

        private string LogID { get; set; }
        private Dictionary<string, List<TestDatum>> TestData { get; set; }
        private Dictionary<Part, Func<List<string>, Dictionary<string, string>, string>> PartSpecificFunctions { get; set; }

        protected Day()
        {
            try
            {
                TimeResults = new Dictionary<Part, double>();
                TimeWasted = new Dictionary<Part, double>();
                TimeWaste = 0.0f;

                Year = this.GetType().Namespace.ToString()[^4..];
                DayName = this.GetType().ToString()[^5..].ToLower();
                LogID = DefaultLogID;
                TestData = new Dictionary<string, List<TestDatum>>();
                TestData[DayName] = GetTestData();
                PartSpecificFunctions = new Dictionary<Part, Func<List<string>, Dictionary<string, string>, string>>
                {
                    {Part.One, RunPart1Solution},
                    {Part.Two, RunPart2Solution}
                };

                m_debugFile = Path.Combine(Core.WorkingDirectory.Get, "Data", Year, "Debug", $"{DayName}", $"{Path.GetRandomFileName().Replace(".", "")}.log");
                if (File.Exists(m_debugFile))
                {
                    File.Delete(m_debugFile);
                }
            }
            catch (Exception e)
            {
                Core.Log.WriteException(e);
            }
        }

        public void Run(bool forceTests = false)
        {
            m_forceTests = forceTests;

            // file input
            IEnumerable<string> input = GetInputFile();

            // file output
            IEnumerable<string> output = GetOutputFile();

            // run part 1
            RunInternal(Part.One, input, output);

            // run part 2
            RunInternal(Part.Two, input, output);

            // reset logging
            LogID = DefaultLogID;
        }

        public void Run(Part part)
        {
            m_forceTests = false;

            // run part
            RunInternal(part, GetInputFile(), GetOutputFile());

            // reset logging
            LogID = DefaultLogID;
        }

        private IEnumerable<string> GetInputFile()
        {
            string fileName = string.Format("{0}.txt", DayName);
            string inputFile = Path.Combine(Core.WorkingDirectory.Get, "Data", Year, "In", fileName);
            if (!File.Exists(inputFile))
            {
                using (File.Create(inputFile)) { }
            }
            return ConvertDayFileToList(File.ReadAllText(inputFile));
        }

        private IEnumerable<string> GetOutputFile()
        {
            string fileName = string.Format("{0}.txt", DayName);
            string outputFile = Path.Combine(Core.WorkingDirectory.Get, "Data", Year, "Out", fileName);
            List<string> output;
            if (!File.Exists(outputFile))
            {
                using (File.Create(outputFile)) { }
                output = new List<string>();
            }
            else
            {
                output = ConvertDayFileToList(File.ReadAllText(outputFile)).ToList();
            }
            output.AddRange(new string[2] { string.Empty, string.Empty });
            return output;
        }

        private void RunInternal(Part part, IEnumerable<string> problemInput, IEnumerable<string> problemOutput)
        {
            // get test data if there is any
            IEnumerable<TestDatum> partSpecificTestData = TestData[DayName].Where(datum => datum.TestPart == part);

            if (!SkipTestData || m_forceTests)
            {
                // run tests if there are any
                foreach (TestDatum datum in partSpecificTestData)
                {
                    // make sure the test has an output, and isn't just the default empty test set
                    if (!string.IsNullOrWhiteSpace(datum.Output))
                    {
                        RunTimedInternal(RunType.Testing, part, datum.Input.ToList(), datum.Output, datum.Variables);
                    }
                }
            }

            TimeResults[part] = RunTimedInternal(RunType.Problem, part, problemInput.ToList(), problemOutput.ElementAt(((int)part) - 1), null);
            TimeWasted[part] = TimeWaste;
        }

        private double RunTimedInternal(RunType runType, Part part, List<string> inputs, string expectedOuput, Dictionary<string, string> variables)
        {
            TempLog.WriteLine = (s) => Log(s);
            TempLog.WriteFile = (s) => LogFile(s);
            LogFiller();

            TimeWaste = 0.0f;
            RunTimedPartInternal(runType, part, inputs, expectedOuput, variables, out Util.Timer timer);

            TempLog.Reset();
            Log(Core.Log.ELevel.Info, $"{timer.Print()}");
            return timer.GetElapsedMs();
        }

        private void RunTimedPartInternal(RunType runType, Part part, List<string> inputs, string expectedOuput, Dictionary<string, string> variables, out Util.Timer timer)
        {
            timer = new Util.Timer();
            LogID = string.Format("{0}|{1}|{2}|part{3}|{4}", Year, DayName, runType.ToString().ToLower(), part == Part.One ? "1" : "2", GetSolutionVersion(part));
            try
            {
                var partSpecificFunction = PartSpecificFunctions[part];
                timer.Start();
                string actualOutput = partSpecificFunction(inputs, variables);
                timer.Stop();
                if (runType == RunType.Testing)
                {
                    if (actualOutput != expectedOuput)
                    {
                        LogAnswer($"[ERROR] Expected: {expectedOuput} - Actual: {actualOutput} [ERROR]", LogIncorrect, Core.Log.Negative);
                    }
                    else
                    {
                        LogAnswer(actualOutput, LogCorrect, Core.Log.Positive);
                    }
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(expectedOuput))
                    {
                        LogAnswer(actualOutput, LogUnknown, Core.Log.Neutral);
                    }
                    else if (actualOutput != expectedOuput)
                    {
                        LogAnswer($"[ERROR] Expected: {expectedOuput} - Actual: {actualOutput} [ERROR]", LogIncorrect, Core.Log.Negative);
                    }
                    else
                    {
                        LogAnswer(actualOutput, LogCorrect, Core.Log.Positive);
                    }
                }
            }
            catch (Exception e)
            {
                Core.Log.WriteException(e);
            }
        }

        protected void WasteTime(Action action)
        {
            Util.Timer timer = new Util.Timer();
            timer.Start();
            action();
            timer.Stop();
            TimeWaste += timer.GetElapsedMs();
        }

        protected void Log(Core.Log.ELevel logLevel, string log)
        {
            if (UseLogs)
            {
                Core.Log.WriteLine(logLevel, $"[{LogID}] {log}");
            }
        }

        protected void Log(string log)
        {
            if (UseLogs)
            {
                Core.Log.WriteLine(Core.Log.ELevel.Info, $"[{LogID}] \t{log}");
            }
        }

        protected void LogFile(string log)
        {
            if (UseLogs)
            {
                if (!File.Exists(m_debugFile))
                {
                    string path = Path.GetDirectoryName(m_debugFile);
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    using (FileStream fs = File.Create(m_debugFile)) { }
                }
                using (StreamWriter sw = File.AppendText(m_debugFile))
                {
                    sw.WriteLine($"[{LogID}] \t{log}");
                }
            }
        }

        private void LogFiller()
        {
            if (UseLogs)
            {
                Core.Log.WriteLine(Core.Log.ELevel.Info, $"[{DefaultLogID}]");
            }
        }

        private void LogAnswer(string answer, char filler, Color color)
        {
            if (UseLogs && !string.IsNullOrWhiteSpace(answer))
            {
                string buffer = new string(filler, answer.Length);
                string empty = new string(' ', answer.Length);
                string bigFiller = new string(filler, 5);
                string smallFiller = new string(filler, 3);
                Core.Log.WriteLine(Core.Log.ELevel.Info, $"[{LogID}] {bigFiller}{buffer}{bigFiller}");
                Core.Log.WriteLine(Core.Log.ELevel.Info, $"[{LogID}] {smallFiller}  {empty}  {smallFiller}");
                Core.Log.WriteLine(Core.Log.ELevel.Info, $"[{LogID}] {smallFiller}  {Core.Log.ColorMarker}{answer}{Core.Log.ColorMarker}  {smallFiller}", new List<Color>() { color });
                Core.Log.WriteLine(Core.Log.ELevel.Info, $"[{LogID}] {smallFiller}  {empty}  {smallFiller}");
                Core.Log.WriteLine(Core.Log.ELevel.Info, $"[{LogID}] {bigFiller}{buffer}{bigFiller}");
            }
        }

        public static IEnumerable<string> ConvertDayFileToList(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return new List<string>();
            }
            return input.Split('\n').Select(str => str.Trim('\r'));
        }

        public static void GetVariable(string variableName, int defaultValue, Dictionary<string, string> variables, out int returnValue)
        {
            returnValue = defaultValue;
            if (variables != null && variables.TryGetValue(variableName, out string value))
            {
                returnValue = int.Parse(value);
            }
        }

        public static void GetVariable(string variableName, long defaultValue, Dictionary<string, string> variables, out long returnValue)
        {
            returnValue = defaultValue;
            if (variables != null && variables.TryGetValue(variableName, out string value))
            {
                returnValue = long.Parse(value);
            }
        }

        public static void GetVariable(string variableName, char defaultValue, Dictionary<string, string> variables, out char returnValue)
        {
            returnValue = defaultValue;
            if (variables != null && variables.TryGetValue(variableName, out string value))
            {
                returnValue = value[0];
            }
        }

        public static void GetVariable(string variableName, string defaultValue, Dictionary<string, string> variables, out string returnValue)
        {
            returnValue = defaultValue;
            if (variables != null && variables.TryGetValue(variableName, out string value))
            {
                returnValue = value;
            }
        }
    }
}