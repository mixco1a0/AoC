using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace _2020
{
    abstract class Day
    {
        private string m_logID;

        protected abstract string GetDay();
        protected abstract void RunPart1Solution(List<string> inputs);
        protected abstract void RunPart2Solution(List<string> inputs);

        private string DefaultLogID { get { return new string('.', 19); }}

        protected Day()
        {
            try
            {
                m_logID = DefaultLogID;
                Run();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }

        private void Run()
        {
            SharedRun(false /* run example */);

            // break up the logs a bit
            LogFiller();
            LogFiller();

            SharedRun(true  /* run problem */);
        }

        private void SharedRun(bool isProblem)
        {
            string fileName = string.Format("{0}{1}{2}", GetDay(), isProblem ? "" : "_example", ".txt");
            m_logID = string.Format("{0}|{1}|partX", GetDay(), isProblem ? "problem" : "example");

            string inputFile = Path.Combine(Directory.GetCurrentDirectory(), "Data", fileName);
            string[] rawFileRead = File.ReadAllText(inputFile).Split('\n').Select(str => str.Trim('\r')).ToArray();

            List<string> inputs = rawFileRead.ToList();
            Timer timer = new Timer();
            timer.Start();
            RunPart1(inputs);
            timer.Stop();
            Log($"{timer.Print()}");
            LogFiller();

            inputs = rawFileRead.ToList();
            timer.Start();
            RunPart2(inputs);
            timer.Stop();
            Log($"{timer.Print()}");
            LogFiller();

            m_logID = DefaultLogID;
        }
        
        private void RunPart1(List<string> inputs)
        {
            m_logID = $"{m_logID[0..^1]}1";
            try
            {
                RunPart1Solution(inputs);
            }
            catch (Exception e)
            {
                Log($"{e.Message}");
                Log($"{e.StackTrace.Split('\r').FirstOrDefault()}");
            }
        }
        
        private void RunPart2(List<string>inputs)
        {
            m_logID = $"{m_logID[0..^1]}2";
            try
            {
                RunPart2Solution(inputs);
            }
            catch (Exception e)
            {
                Log($"{e.Message}");
                Log($"{e.StackTrace.Split('\r').FirstOrDefault()}");
            }
        }

        private void Log(string log)
        {
            Console.WriteLine($"[{m_logID}] {log}");
        }

        private void LogFiller()
        {
            Console.WriteLine($"[{DefaultLogID}]");
        }

        protected void Debug(string log)
        {
            Console.WriteLine($"[{m_logID}] \t{log}");
        }

        protected void LogAnswer(string answer)
        {
            string buffer = new string('*', answer.Length);
            Console.WriteLine($"[{m_logID}] *****{buffer}*****");
            Console.WriteLine($"[{m_logID}] ***  {answer}  ***");
            Console.WriteLine($"[{m_logID}] *****{buffer}*****");
        }
    }
}

/*
using System.Collections.Generic;

namespace _2020
{
    class DayXX : Day
    {
        public DayXX() : base() {}
        
        protected override string GetDay() { return nameof(DayXX).ToLower(); }

        protected override void RunPart1Solution(List<string> inputs)
        {
            LogAnswer("NA");
        }

        protected override void RunPart2Solution(List<string>inputs)
        {
            LogAnswer("NA");
        }
    }
}

*/