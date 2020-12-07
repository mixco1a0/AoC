using System.Diagnostics;
namespace AoC
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
            if (m_stopwatch != null)
            {
                m_stopwatch.Stop();
            }
        }

        public string Print()
        {
            if (m_stopwatch != null)
            {
                //return $"Elapsed: {m_stopwatch.Elapsed.TotalMilliseconds} (ms) [{m_stopwatch.Elapsed.Ticks} (ticks)]";
                return $"Elapsed: {m_stopwatch.Elapsed.TotalMilliseconds} (ms)";
            }

            return "[Error] Not Running.";
        }
    }
}