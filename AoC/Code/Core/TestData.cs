using System.Collections.Generic;

namespace AoC.Core
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
                m_input ??= Day.ConvertDayFileToList(RawInput);
                return m_input;
            }
        }
        public string Output { get; set; }
        public Dictionary<string, string> Variables { get; set; }
    }
}