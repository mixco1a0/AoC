using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AoC.Core
{
    public interface IGlyph
    {
        public static char On { get => '#'; }
        public static char Off { get => '.'; }

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
        public enum Size
        {
            _5x6,
        }

        private static readonly Dictionary<Size, IGlyph> Converters = new Dictionary<Size, IGlyph>()
        {
            {Size._5x6, new Glyph5x6()},
        };

        public static string Process(string[] glyphs, Size size)
        {
            return Process(glyphs, size, IGlyph.On, IGlyph.Off);
        }

        public static string Process(string[] glyphs, Size size, char on, char off)
        {
            if (glyphs == null || glyphs[0] == null)
            {
                return "glyph error";
            }

            IGlyph glyphConverter = Converters[size];
            Dictionary<char, string[]> alphabet = glyphConverter.Alphabet;

            // make sure the correct characters are used
            if (on != IGlyph.On)
            {
                for (int i = 0; i < glyphs.Length; ++i)
                {
                    glyphs[i] = glyphs[i].Replace(on, IGlyph.On);
                }
            }
            if (off != IGlyph.Off)
            {
                for (int i = 0; i < glyphs.Length; ++i)
                {
                    glyphs[i] = glyphs[i].Replace(off, IGlyph.Off);
                }
            }

            // make sure the glyph adheres to the correct size
            int length = glyphs[0].Length % glyphConverter.Width;
            if (length != 0)
            {
                string empty = new string(IGlyph.Off, 5 - length);
                for (int i = 0; i < glyphs.Length; ++i)
                {
                    glyphs[i] += empty;
                }
            }

            // convert glyph to letter
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < glyphs[0].Length; i += glyphConverter.Width)
            {
                char letter = '?';
                IEnumerable<string> glyph = glyphs.Select(s => s.Substring(i, glyphConverter.Width));
                foreach (KeyValuePair<char, string[]> pair in alphabet)
                {
                    if (pair.Value != null && pair.Value.SequenceEqual(glyph))
                    {
                        letter = pair.Key;
                        break;
                    }
                }
                sb.Append(letter);
            }
            return sb.ToString();
        }
    }
}