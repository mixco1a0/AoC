using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace _2020
{
    abstract class Day
    {
        private string m_curFile;

        protected abstract string GetDay();
        protected abstract void RunPart1Solution(List<string> inputs);
        protected abstract void RunPart2Solution(List<string> inputs);

        protected Day()
        {
            m_curFile = "";
            Run();
        }

        private void Run()
        {
            SharedRun($"{GetDay()}_example.txt");
            SharedRun($"{GetDay()}.txt");
        }

        private void SharedRun(string fileName)
        {
            m_curFile = fileName;

            string inputFile = Path.Combine(Directory.GetCurrentDirectory(), "Data", fileName);
            string[] rawFileRead = File.ReadAllText(inputFile).Split('\n').Select(str => str.Trim('\r')).ToArray();

            List<string> inputs = rawFileRead.ToList();
            Timer timer = new Timer();
            timer.Start();
            RunPart1(inputs);
            timer.Stop();
            Log($"{timer.Print()}");
            Log("");

            inputs = rawFileRead.ToList();
            timer.Start();
            RunPart2(inputs);
            timer.Stop();
            Log($"{timer.Print()}");
            Log("");

            m_curFile = "";
        }
        
        private void RunPart1(List<string> inputs)
        {
            Log("Part 1");
            RunPart1Solution(inputs);
        }
        
        private void RunPart2(List<string>inputs)
        {
            Log("Part 2");
            RunPart2Solution(inputs);
        }

        private void Log(string log)
        {
            Console.WriteLine($"[{m_curFile}] {log}");
        }

        protected void Debug(string log)
        {
            Console.WriteLine($"[{m_curFile}] \t{log}");
        }

        protected void LogAnswer(string answer)
        {
            string buffer = new string('*', answer.Length);
            Console.WriteLine($"[{m_curFile}] *****{buffer}*****");
            Console.WriteLine($"[{m_curFile}] ***  {answer}  ***");
            Console.WriteLine($"[{m_curFile}] *****{buffer}*****");
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
            LogAnswer("");
        }

        protected override void RunPart2Solution(List<string>inputs)
        {
            LogAnswer("");
        }
    }
}

*/