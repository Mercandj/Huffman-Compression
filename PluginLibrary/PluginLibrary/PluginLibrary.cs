using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Huffman;

namespace PluginLibrary {
    class PluginLibrary : MarshalByRefObject, IPlugin {

        public bool Compress(ref HuffmanData huffmanData) {
            if(huffmanData.uncompressedData == null)
                return false;
            bool result = Project.Compress(ref huffmanData);
            huffmanData = Project.huffmanData;
            return result;
        }

        public bool Decompress(ref HuffmanData huffmanData) {
            if(huffmanData.compressedData == null || huffmanData.frequency == null)
                return false;
            bool result = Project.Decompress(ref huffmanData);
            huffmanData = Project.huffmanData;
            return result;
        }

        public string PluginName {
            get { return "PluginLibrary.sln"; }
        }
    }
}
