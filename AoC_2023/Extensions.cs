using System.Text;

namespace AoC_2023
{
    public static class Extensions
    {
        public static int ToIntFromBin(this string src)
        {
            return Convert.ToInt32(src, 2);
        }

        public static long ToLongFromBin(this string src)
        {
            return Convert.ToInt64(src, 2);
        }

        private static readonly Dictionary<char, string> bin = new Dictionary<char, string>
        {
            {'0', "0000"},
            {'1', "0001"},
            {'2', "0010"},
            {'3', "0011"},
            {'4', "0100"},
            {'5', "0101"},
            {'6', "0110"},
            {'7', "0111"},
            {'8', "1000"},
            {'9', "1001"},
            {'A', "1010"},
            {'B', "1011"},
            {'C', "1100"},
            {'D', "1101"},
            {'E', "1110"},
            {'F', "1111"},
        };

        public static string ToBinFromHex(this string src)
        {
            var sb = new StringBuilder();

            foreach (var c in src)
            {
                sb.Append(bin[c]);
            }

            return sb.ToString();
        }

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> ts)
        {
            return new HashSet<T>(ts);
        }
        
        public static TValue SafeGet<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue val = default)
        {
            return dict.TryGetValue(key, out var v) ? v : val;
        }

        public static string[] SplitEmpty(this string str, params string[] separators)
        {
            return str.Split(separators, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}