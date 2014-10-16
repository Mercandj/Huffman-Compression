using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginLibrary {
    class Lib {        
        internal static String ByteToStringBin(byte b) {
            return Convert.ToString(b, 2).PadLeft(8, '0');
        }

        internal static byte[] GetBytes(string str) {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        internal static string GetString(byte[] bytes) {
            return System.Text.Encoding.UTF8.GetString(bytes);
        }
    }
}
