using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Round.WPF.MusicPlayer.Module.Cs.LinkBilibiliServer
{
    internal class GetVideoAID
    {
        private static readonly string table = "fZodR9XQDSUm21yCkr6zBqiveYah8bt4xsWpHnJE7jL5VG3guMTKNPAwcF";
        private static readonly Dictionary<char, int> tr = new Dictionary<char, int>();
        private static readonly int[] s = { 11, 10, 3, 8, 4, 6 };
        private const int xor = 177451812;
        private const long add = 8728348608;
        public static long Decode(string x)
        {
            long r = 0;
            for (int i = 0; i <= 5; i++)
            {
                r += tr[x[s[i]]] * (long)Math.Pow(58, i);
            }
            return (r - add) ^ xor;
        }

        public static string Encode(long x)
        {
            x = (x ^ xor) + add;
            char[] r = "BV1  4 1 7  ".ToCharArray();
            for (int i = 0; i < 6; i++)
            {
                r[s[i]] = table[(int)(x / (long)Math.Pow(58, i) % 58)];
            }
            return new string(r);
        }
    }
}
