using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AoC.Util
{
    public static class Glyph
    {
        public static char On { get => '#'; }
        public static char Off { get => '.'; }
        public static char UnknownLetter { get => '?'; }
    }

    public interface IGlyph
    {
        public static KeyValuePair<char, string[]> DefaultPair { get => default; }
        public abstract Dictionary<char, string[]> Alphabet { get; }
        public abstract int Width { get; }
        public abstract int Height { get; }

    }

    public class Glyph5x6 : IGlyph
    {
        private static readonly Dictionary<char, string[]> s_alphabet = new Dictionary<char, string[]>()
        {
            {'A', new string[] {
                ".##..",
                "#..#.",
                "#..#.",
                "####.",
                "#..#.",
                "#..#."}
            },
            {'B', new string[] {
                "###..",
                "#..#.",
                "###..",
                "#..#.",
                "#..#.",
                "###.."}
            },
            {'C', new string[] {
                ".##..",
                "#..#.",
                "#....",
                "#....",
                "#..#.",
                ".##.."}
                },
            {'D', null},
            {'E', new string[] {
                "####.",
                "#....",
                "###..",
                "#....",
                "#....",
                "####."}
            },
            {'F', new string[] {
                "####.",
                "#....",
                "###..",
                "#....",
                "#....",
                "#...."}
            },
            {'G', new string[] {
                ".##..",
                "#..#.",
                "#....",
                "#.##.",
                "#..#.",
                ".###."}
            },
            {'H', new string[] {
                "#..#.",
                "#..#.",
                "####.",
                "#..#.",
                "#..#.",
                "#..#."}
            },
            {'I', null},
            {'J', new string[] {
                "..##.",
                "...#.",
                "...#.",
                "...#.",
                "#..#.",
                ".##.."}
            },
            {'K', new string[] {
                "#..#.",
                "#.#..",
                "##...",
                "#.#..",
                "#.#..",
                "#..#."}
            },
            {'L', new string[] {
                "#....",
                "#....",
                "#....",
                "#....",
                "#....",
                "####."}
            },
            {'M', null},
            {'N', null},
            {'O', new string[] {
                ".##..",
                "#..#.",
                "#..#.",
                "#..#.",
                "#..#.",
                ".##.."}
            },
            {'P', new string[] {
                "###..",
                "#..#.",
                "#..#.",
                "###..",
                "#....",
                "#...."}
            },
            {'Q', null},
            {'R', new string[] {
                "###..",
                "#..#.",
                "#..#.",
                "###..",
                "#.#..",
                "#..#."}
            },
            {'S', null},
            {'T', null},
            {'U', new string[] {
                "#..#.",
                "#..#.",
                "#..#.",
                "#..#.",
                "#..#.",
                ".##.."}
            },
            {'V', null},
            {'W', null},
            {'X', null},
            {'Y', new string[] {
                "#...#",
                "#...#",
                ".#.#.",
                "..#..",
                "..#..",
                "..#.."}
            },
            {'Z', new string[] {
                "####.",
                "...#.",
                "..#..",
                ".#...",
                "#....",
                "####."}
            },
        };

        public Dictionary<char, string[]> Alphabet { get => s_alphabet; }
        public int Width { get => 5; }
        public int Height { get => 6; }
    }

    public class GlyphConverter
    {
        public enum EType
        {
            _5x6,
        }

        private static readonly Dictionary<EType, IGlyph> Converters = new Dictionary<EType, IGlyph>()
        {
            {EType._5x6, new Glyph5x6()},
        };

        public static string Process(string[] glyphs, EType type)
        {
            return Process(glyphs, type, Glyph.On, Glyph.Off);
        }

        public static string Process(string[] glyphs, EType type, char on, char off)
        {
            if (glyphs == null || glyphs[0] == null)
            {
                return "glyph error";
            }

            IGlyph glyphConverter = Converters[type];
            Dictionary<char, string[]> alphabet = glyphConverter.Alphabet;

            // make sure the correct characters are used
            if (on != Glyph.On)
            {
                for (int i = 0; i < glyphs.Length; ++i)
                {
                    glyphs[i] = glyphs[i].Replace(on, Glyph.On);
                }
            }
            if (off != Glyph.Off)
            {
                for (int i = 0; i < glyphs.Length; ++i)
                {
                    glyphs[i] = glyphs[i].Replace(off, Glyph.Off);
                }
            }

            // make sure the glyph adheres to the correct size
            int length = glyphs[0].Length % glyphConverter.Width;
            if (length != 0)
            {
                string empty = new string(Glyph.Off, glyphConverter.Width - length);
                for (int i = 0; i < glyphs.Length; ++i)
                {
                    glyphs[i] += empty;
                }
            }

            // convert glyph to letter
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < glyphs[0].Length; i += glyphConverter.Width)
            {
                IEnumerable<string> glyph = glyphs.Select(g => g.Substring(i, glyphConverter.Width));
                KeyValuePair<char, string[]> matchingPair = alphabet.FirstOrDefault(a => a.Value != null && a.Value.SequenceEqual(glyph));
                if (!matchingPair.Equals(IGlyph.DefaultPair))
                {
                    sb.Append(matchingPair.Key);
                }
                else
                {
                    sb.Append(Glyph.UnknownLetter);
                }
            }
            return sb.ToString();
        }
    }
}