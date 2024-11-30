using System.Diagnostics;

namespace AoC.Util
{
    public class Timer
    {
        private Stopwatch m_stopwatch;

        public void Start()
        {
            m_stopwatch = new Stopwatch();
            m_stopwatch.Start();
        }

        public void Stop()
        {
            m_stopwatch?.Stop();
        }

        public double GetElapsedMs()
        {
            if (m_stopwatch != null)
            {
                return m_stopwatch.Elapsed.TotalMilliseconds;
            }
            
            return 0;
        }

        public string Print()
        {
            if (m_stopwatch != null)
            {
                return $"Elapsed: {m_stopwatch.Elapsed.TotalMilliseconds} (ms)";
            }

            return "[Error] Not Running.";
        }
    }
}