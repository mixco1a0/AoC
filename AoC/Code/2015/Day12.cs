using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AoC._2015
{
    class Day12 : Day
    {
        public Day12() { }
        public override string GetSolutionVersion(Part part)
        {
            switch (part)
            {
                // case Part.One:
                //     return "v1";
                // case Part.Two:
                //     return "v1";
                default:
                    return base.GetSolutionVersion(part);
            }
        }
        protected override List<TestDatum> GetTestData()
        {
            List<TestDatum> testData = new List<TestDatum>();
            testData.Add(new TestDatum
            {
                TestPart = Part.One,
                Output = "6",
                RawInput =
@"[1,2,3]"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.One,
                Output = "3",
                RawInput =
@"[[[3]]]"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.One,
                Output = "0",
                RawInput =
@"{}"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.Two,
                Output = "",
                RawInput =
@""
            });
            return testData;
        }

        private int Count(JContainer container)
        {
            int count = 0;
            if (container != null)
            {
                for (JToken token = container.First; token != null; token = token.Next)
                {
                    if (token.HasValues)
                    {
                        count += Count(token as JContainer);
                    }
                    else if (token.Type == JTokenType.Integer)
                    {
                        count += token.Value<int>();
                    }
                }
            }
            return count;
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            dynamic json = JsonConvert.DeserializeObject(inputs.First());
            return Count(json).ToString();
        }

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            return "";
        }
    }
}