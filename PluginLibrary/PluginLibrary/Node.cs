using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Huffman;

namespace PluginLibrary {
    public class Node {
        public Node child1, child2;
        public int value;
        public byte key;
        public int size;
        
        public Node(KeyValuePair<byte, int> keyValue) {
            child1 = null;
            child2 = null;
            this.key = keyValue.Key;
            this.value = keyValue.Value;
            this.size = 0;
        }

        public Node(Node child1) {
            this.child1 = child1;
            child2 = null;
            this.size = 1;
        }
        
        public Node(ref Node child1, ref Node child2) {
            this.child1 = child1;
            this.child2 = child2;
            this.size = 2;
        }
        
        public int GetValue() {
            switch(this.size) {
                case 2:
                return this.child1.GetValue() + this.child2.GetValue();
                case 1:
                return this.child1.GetValue();
            }
            return this.value;
        }

        public override string ToString() {
            if(this.size == 0)
                return String.Format("(byte:{0}, val:{1}, size:{2})", this.key, GetValue(), size);
            return String.Format("(val:{0}, size:{1})", GetValue(), size);
        }
    }
}
