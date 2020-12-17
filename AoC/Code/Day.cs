using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AoC
{
    abstract public class Day
    {
        public static bool UseLogs { get; set; }

        private enum RunType
        {
            Testing,
            Problem
        }

        #region Required Overrides
        protected abstract List<TestDatum> GetTestData();
        protected abstract string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables);
        protected abstract string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables);
        public virtual string GetSolutionVersion(TestPart testPart)
        {
            return "v0";
        }
        #endregion

        private string DefaultLogID { get { return new string('.', 24); } }

        public string Year { get; private set; }
        public string DayName { get; private set; }
        public Dictionary<TestPart, double> TimeResults { get; private set; }

        private string m_logID;
        private Dictionary<string, List<TestDatum>> m_testData;
        private Dictionary<TestPart, Func<List<string>, Dictionary<string, string>, string>> m_partSpecificFunctions;

        protected Day()
        {
            try
            {
                TimeResults = new Dictionary<TestPart, double>();

                Year = this.GetType().Namespace.ToString()[^4..];
                DayName = this.GetType().ToString()[^5..].ToLower();
                m_logID = DefaultLogID;
                m_testData = new Dictionary<string, List<TestDatum>>();
                m_testData[DayName] = GetTestData();
                m_partSpecificFunctions = new Dictionary<TestPart, Func<List<string>, Dictionary<string, string>, string>>
                {
                    {TestPart.One, RunPart1Solution},
                    {TestPart.Two, RunPart2Solution}
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }

        public void Run()
        {
            // file input
            string fileName = string.Format("{0}.txt", DayName);
            string inputFile = Path.Combine(Util.WorkingDirectory, "Data", Year, fileName);
            IEnumerable<string> rawFileRead = Util.ConvertInputToList(File.ReadAllText(inputFile));

            // run part 1
            RunAll(TestPart.One, rawFileRead);

            // run part 2
            RunAll(TestPart.Two, rawFileRead);

            // reset logging
            m_logID = DefaultLogID;
        }

        private void RunAll(TestPart testPart, IEnumerable<string> problemInput)
        {
            // get test data if there is any
            IEnumerable<TestDatum> partSpecificTestData = m_testData[DayName].Where(datum => datum.TestPart == testPart);

            // run tests if there any
            foreach (TestDatum datum in partSpecificTestData)
            {
                RunPart(RunType.Testing, testPart, datum.Input.ToList(), datum.Output, datum.Variables);
                LogFiller();
            }

            // run problem
            Timer timer = new Timer();
            timer.Start();
            RunPart(RunType.Problem, testPart, problemInput.ToList(), "", null);
            timer.Stop();
            TimeResults[testPart] = timer.GetElapsedMs();

            // print time
            Log($"{timer.Print()}");
            LogFiller();
        }

        private void RunPart(RunType runType, TestPart testPart, List<string> inputs, string expectedOuput, Dictionary<string, string> variables)
        {
            m_logID = string.Format("{0}|{1}|{2}|part{3}", Year, DayName, runType == RunType.Problem ? "problem" : "testing", testPart == TestPart.One ? "1" : "2");
            try
            {
                string actualOutput = m_partSpecificFunctions[testPart](inputs, variables);
                if (runType == RunType.Testing && actualOutput != expectedOuput)
                {
                    LogAnswer($"[ERROR] Expected: {expectedOuput} - Actual: {actualOutput} [ERROR]");
                }
                else
                {
                    LogAnswer(actualOutput);
                }
            }
            catch (Exception e)
            {
                Log($"{e.Message}");
                Log($"{e.StackTrace.Split('\r').FirstOrDefault()}");
            }
        }

        private void Log(string log)
        {
            if (UseLogs)
                Console.WriteLine($"{Util.GetLogTimeStamp()} [{m_logID}] {log}");
        }

        private void LogFiller()
        {
            if (UseLogs)
                Console.WriteLine($"{Util.GetLogTimeStamp()} [{DefaultLogID}]");
        }

        private void LogAnswer(string answer)
        {
            if (UseLogs)
            {
                string buffer = new string('*', answer.Length);
                Console.WriteLine($"{Util.GetLogTimeStamp()} [{m_logID}] *****{buffer}*****");
                Console.WriteLine($"{Util.GetLogTimeStamp()} [{m_logID}] ***  {answer}  ***");
                Console.WriteLine($"{Util.GetLogTimeStamp()} [{m_logID}] *****{buffer}*****");
            }
        }

        protected void Debug(string log)
        {
            if (UseLogs)
                Console.WriteLine($"{Util.GetLogTimeStamp()} [{m_logID}] \t{log}");
        }

        protected void DebugWrite(string log)
        {
            if (UseLogs)
                Console.Write($"{Util.GetLogTimeStamp()} [{m_logID}] \t{log}");
        }
    }
}