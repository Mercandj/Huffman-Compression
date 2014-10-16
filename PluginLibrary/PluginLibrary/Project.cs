using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Huffman;
using System.Collections;
using System.Threading;

namespace PluginLibrary {

    public class Project {
        public static HuffmanData huffmanData;

        public static List<Node> treeList;
        public static int[] frequency;
        public static int frequencyCount;
        public static BitArray[] dictionary_compress;
        public static int sizeOfUncompressedData;

        private static int tmpInt;
        private static int tmpByte;
        private static int i, j, v, tmpSizeCompressedBit;
        private static int tmp_index_1;
        private static int tmp_index_2;
        private static bool[] tmpBools;
        private static int tmpInputbitarraySize;
        private static BitArray tmpInputbitarray;
        
        public static bool Compress(ref HuffmanData phuffmanData) {
            huffmanData = phuffmanData;
            treeList = new List<Node>();
            frequency = new int[256];
            sizeOfUncompressedData = huffmanData.uncompressedData.Count();
            huffmanData.sizeOfUncompressedData = sizeOfUncompressedData;
            if(!UpdateFrequency()) {
                if(TestConfig.printAll) System.Console.WriteLine("Class Data : Compress, error UpdateFrequency return false\n");
                return false;
            }
            if(!CreateTree()) {
                if(TestConfig.printAll) System.Console.WriteLine("Class Data : Compress, error CreateTree return false\n");
                return false;
            }
            dictionary_compress = new BitArray[256]; Node node = treeList.ElementAt(0); tmpSizeCompressedBit = 0;
            if(!CreateDico(ref node, new BitArray(0), 0)) {
                if(TestConfig.printAll) System.Console.WriteLine("Class Data : Compress, error CreateDico return false\n");
                return false;
            }
            if(!TranslateCompress()) {
                System.Console.WriteLine("Class Data : Compress, error TranslateWithDico return false\n");
                return false;
            }
            return true;
        }

        public static bool Decompress(ref HuffmanData phuffmanData) {
            huffmanData = phuffmanData;
            treeList = new List<Node>();
            frequencyCount = huffmanData.frequency.Count;
            if(!CreateTree()) {
                if(TestConfig.printAll) System.Console.WriteLine("Class Data : Decompress, error CreateTree return false\n");
                return false;
            }
            if(!TranslateUncompress(treeList.ElementAt(0), ref huffmanData.compressedData, ref huffmanData.uncompressedData)) {
                if(TestConfig.printAll) System.Console.WriteLine("Class Data : Decompress, error TranslateUncompress return false\n");
                return false;
            }
            return true;
        }

        public static bool UpdateFrequency() {
            huffmanData.frequency = new List<KeyValuePair<byte, int>>();
            for(i = 0; i < huffmanData.sizeOfUncompressedData; i++)
                frequency[huffmanData.uncompressedData[i]]++;
            frequencyCount = 0;
            for(i = 0; i < 256; i++)
                if(frequency[i] != 0) {
                    huffmanData.frequency.Add(new KeyValuePair<byte, int>((byte) i, frequency[i]));
                    frequencyCount++;
                }
            return true;
        }

        public static bool CreateTree() {
            for(i = 0; i < frequencyCount; i++)
                treeList.Add(new Node(huffmanData.frequency.ElementAt(i)));

            if(frequencyCount == 1)
                treeList[0] = new Node(treeList.ElementAt(0));

            Point minIndexPoint;
            Node tmp_child1, tmp_child2, tmp_parent;
            while(frequencyCount != 1) {
                minIndexPoint = GetMinsIndex();
                if(minIndexPoint.x != -1 && minIndexPoint.y != -1) {
                    tmp_child1 = treeList.ElementAt(minIndexPoint.x);
                    tmp_child2 = treeList.ElementAt(minIndexPoint.y);
                    tmp_parent = new Node(ref tmp_child1, ref tmp_child2);
                    treeList[minIndexPoint.x] = tmp_parent;
                    treeList.RemoveAt(minIndexPoint.y);
                    frequencyCount--;
                }
            }
            return true;
        }

        private static bool TranslateCompress() {
            int outputSize = 0;
            v = 0;
            BitArray b = null;

            tmpInt = tmpSizeCompressedBit / 8;
            if(tmpSizeCompressedBit % 8 != 0)
                tmpInt++;
            huffmanData.compressedData = new byte[tmpInt];

            byte[] tmp = new byte[] { 1, 2, 4, 8, 16, 32, 64, 128 };            

            for(i = 0; i < sizeOfUncompressedData; i++) {
                tmpByte = huffmanData.uncompressedData[i];
                b = dictionary_compress[tmpByte];                
                for(j = 0; j < b.Count; j++) {
                    if(b[j])
                        huffmanData.compressedData[outputSize] += tmp[v];
                    if(v == 7) {                        
                        outputSize++;
                        if(outputSize > tmpInt - 1)
                            return true;
                        v = 0;
                    }
                    else
                        v++;
                }
            }
            return true;
        }

        public static Point GetMinsIndex() {
            tmp_index_1 = -1;
            tmp_index_2 = -1;
            for(i = 0; i < frequencyCount; i++) {
                tmpInt = treeList.ElementAt(i).GetValue();
                if(tmp_index_1 == -1)
                    tmp_index_1 = i;
                else if(tmp_index_2 == -1)
                    tmp_index_2 = i;
                else if(tmpInt < treeList.ElementAt(tmp_index_1).GetValue())
                    tmp_index_1 = i;
                else if(tmpInt < treeList.ElementAt(tmp_index_2).GetValue())
                    tmp_index_2 = i;
            }
            return new Point(tmp_index_1, tmp_index_2);
        }

        public static bool CreateDico(ref Node top, BitArray recurrence, int indexRecurrence) {
            /* Stop recurrence conditions */
            if(top == null)
                return false;
            if(top.size == 0) {
                dictionary_compress[top.key] = recurrence;
                tmpSizeCompressedBit += indexRecurrence * top.value;
            }

            if(top.child1 != null) {
                tmpBools = new bool[1 + indexRecurrence];
                tmpBools[recurrence.Count] = false;
                recurrence.CopyTo(tmpBools, 0);
                CreateDico(ref top.child1, new BitArray(tmpBools), indexRecurrence + 1);
            }
            if(top.child2 != null) {
                recurrence.Length++;
                recurrence[indexRecurrence] = true;
                CreateDico(ref top.child2, recurrence, indexRecurrence + 1);
            }
            return true;
        }

        public static byte ReadTree(ref Node top, int indexRecurrence) {
            if(top.size == 0) {
                i += indexRecurrence;
                return top.key;
            }
            else if(top.size == 1)
                return ReadTree(ref top.child1, indexRecurrence + 1);            
            if(tmpInputbitarray[indexRecurrence + i])
                return ReadTree(ref top.child2, indexRecurrence + 1);
            else
                return ReadTree(ref top.child1, indexRecurrence + 1);            
        }

        private static bool TranslateUncompress(Node top, ref byte[] input, ref byte[] output) {
            // TODO System.Console.WriteLine("huffmanData.sizeOfUncompressedData=" + huffmanData.sizeOfUncompressedData);
            output = new byte[huffmanData.sizeOfUncompressedData];

            tmpInputbitarray = new BitArray(input);
            tmpInputbitarraySize = tmpInputbitarray.Count;
            i = 0; j = 0;

            while(j < huffmanData.sizeOfUncompressedData) {
                tmpByte = ReadTree(ref top, 0);
                output[j] = (byte) tmpByte;
                j++;
            }
            return true;
        }
    }
}
