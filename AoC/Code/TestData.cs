using System.Collections.Generic;

namespace AoC
{
    public class TestDatum
    {
        public Part TestPart { get; set; }
        public string RawInput { private get; set; }
        private IEnumerable<string> m_input = null;
        public IEnumerable<string> Input
        {
            get
            {
                if (m_input == null)
                {
                    m_input = Day.ConvertInputToList(RawInput);
                }
                return m_input;
            }
        }
        public string Output { get; set; }
        public Dictionary<string, string> Variables { get; set; }
    }
}