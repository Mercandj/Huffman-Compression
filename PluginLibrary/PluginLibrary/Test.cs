using Huffman;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace PluginLibrary {
    public class Test {

        public int testNb = 0;
        long timeMsCompress = 0;
        long timeMsDecompress = 0;
        Stopwatch sw;

        int sizeUncompress, sizeCompress;

        public Test() {
            sw = new Stopwatch();
        }

        public bool Execute(String input) {
            return Execute(Lib.GetBytes(input));
        }

        public bool Execute(byte[] input) {

            this.testNb++;

            if(TestConfig.printAll) System.Console.WriteLine("");
            if(TestConfig.printAll) System.Console.WriteLine("-------------------   TEST " + testNb + "   -------------------");
            if(TestConfig.printAll) System.Console.WriteLine("");

            /* COMPRESS */
            if(TestConfig.printAll) System.Console.WriteLine("(" + testNb + ") Compress : \"" + Lib.GetString(input) + "\"");
            if(TestConfig.printAll) System.Console.WriteLine("(" + testNb + ") Compress : \"" + ToStringBitTab(input) + "\"");

            HuffmanData huffmanData = new HuffmanData();
            huffmanData.uncompressedData = input;

            sw.Start();
            Project.Compress(ref huffmanData);
            sw.Stop();
            this.timeMsCompress = sw.ElapsedMilliseconds;
            sw.Reset();

            sizeUncompress = Project.sizeOfUncompressedData;
            sizeCompress = Project.huffmanData.compressedData.Count();

            if(TestConfig.printAll) System.Console.WriteLine("(" + testNb + ") Compress : Hz : " + ToStringFrequency(Project.huffmanData.frequency));
            if(TestConfig.printAll) System.Console.WriteLine("(" + testNb + ") Compress : Tree Links : ");
            if(TestConfig.printAll) PrintTree(Project.treeList.ElementAt(0));
            if(TestConfig.printAll) System.Console.WriteLine("(" + testNb + ") Compress : dico : " + ToStringDico());
            if(TestConfig.printAll) System.Console.WriteLine("(" + testNb + ") Compress : inputSize = {0} : outputSize = {1}", Project.huffmanData.uncompressedData.Count(), Project.huffmanData.compressedData.Count());
            if(TestConfig.printAll) System.Console.WriteLine("(" + testNb + ") Compress : output : \"" + ToStringBitTab(Project.huffmanData.compressedData) + "\"");
            if(TestConfig.printAll) System.Console.WriteLine("");

            /* DECOMPRESS */
            HuffmanData huffmanData2 = new HuffmanData();
            huffmanData2.frequency = new List<KeyValuePair<byte, int>>(Project.huffmanData.frequency);
            huffmanData2.compressedData = new byte[Project.huffmanData.compressedData.Count()];
            huffmanData2.sizeOfUncompressedData = Project.huffmanData.sizeOfUncompressedData;
            Array.Copy(Project.huffmanData.compressedData, huffmanData2.compressedData, Project.huffmanData.compressedData.Count());

            sw.Start();
            Project.Decompress(ref huffmanData2);
            huffmanData2.uncompressedData = Project.huffmanData.uncompressedData;
            sw.Stop();
            this.timeMsDecompress = sw.ElapsedMilliseconds;
            sw.Reset();

            if(TestConfig.printAll) System.Console.WriteLine("(" + testNb + ") Uncompress :  Hz : " + ToStringFrequency(Project.huffmanData.frequency));
            if(TestConfig.printAll) System.Console.WriteLine("(" + testNb + ") Uncompress : Tree Links : ");
            if(TestConfig.printAll) PrintTree(Project.treeList.ElementAt(0));
            //if(Config.printAll) System.Console.WriteLine("(" + testNb + ") Uncompress : dico : " + data2.dictionary);
            if(TestConfig.printAll) System.Console.WriteLine("(" + testNb + ") Uncompress : inputSize = {0} : outputSize = {1}", Project.huffmanData.compressedData.Count(), Project.huffmanData.uncompressedData.Count());
            if(TestConfig.printAll) System.Console.WriteLine("(" + testNb + ") Uncompress : output  : \"" + ToStringBitTab(Project.huffmanData.uncompressedData) + "\"");
            if(TestConfig.printAll) System.Console.WriteLine("(" + testNb + ") Uncompress : output  : \"" + Lib.GetString(Project.huffmanData.uncompressedData) + "\"");
            if(TestConfig.printAll) System.Console.WriteLine("");

            bool result = CompareByteArray(ref huffmanData.uncompressedData, ref huffmanData2.uncompressedData);
            if(TestConfig.printAll) System.Console.WriteLine("(" + testNb + ") RESULT input " + (result ? "==" : "!=") + " output !");
            if(TestConfig.printAll) System.Console.WriteLine("");
            if(TestConfig.printAll) System.Console.WriteLine("-------------------  END TEST " + testNb + "  -------------------");
            if(TestConfig.printAll) System.Console.WriteLine("");
            if(TestConfig.printAll) System.Console.WriteLine("");

            if(!TestConfig.printAll) System.Console.WriteLine("(" + testNb + ")\t[" + (result ? "SUCCESS" : "ECHEC") + "] ! : Rate Gain  = " + (100 - (int) (((sizeCompress * 1.0) / (sizeUncompress * 1.0)) * 100.0)) + "% \t: Time = " + (timeMsCompress + timeMsDecompress) + "\tms  \t(" + timeMsCompress + ",\t" + timeMsDecompress + ")");
            
            return result;
        }

        static bool CompareByteArray(ref byte[] b1, ref byte[] b2) {
            if(b1.Count() != b2.Count())
                return false;
            for(int i = 0; i < b1.Count(); i++)
                if(b1[i] != b2[i])
                    return false;
            return true;
        }

        static void PrintTree(Node top) {
            switch(top.size) {
                case 2:
                System.Console.WriteLine("[ " + top + "\t-- link --\t" + top.child1 + " ]");
                System.Console.WriteLine("[ " + top + "\t-- link --\t" + top.child2 + " ]");
                PrintTree(top.child1);
                PrintTree(top.child2);
                break;
                case 1:
                System.Console.WriteLine("[ " + top + "\t-- link --\t" + top.child1 + " ]");
                PrintTree(top.child1);
                break;
            }
        }

        static string ToStringDico() {
            String result = "";
            for(int i = 0; i < 256; i++)
                if(Project.dictionary_compress[i] != null) {
                    result += i + "[";
                    foreach(bool b in Project.dictionary_compress[i])
                        result += (b ? "1" : "0");
                    result += "] ";
                }
            return result;
        }

        static String ToStringBitTab(byte[] input) {
            String result = "";
            if(input != null) {
                foreach(byte var in input)
                    result += var + " ";
            }
            return result;
        }

        static String ToStringFrequency(List<KeyValuePair<byte, int>> frequency) {
            String result = "";
            if(frequency != null) {
                foreach(KeyValuePair<byte, int> var in frequency)
                    result += var.Key + "[" + var.Value + "] ";
            }
            return result;
        }

        static String ContainsChar(List<KeyValuePair<byte, int>> frequency, char caract, int occ, int numTest) {
            if(frequency == null)
                return "Erreur ContainsChar : frequency == null\n";
            String result = "";
            for(int i = 0; i < frequency.Count(); i++) {
                if(frequency.ElementAt(i).Key == Convert.ToByte(caract)) {
                    if(frequency.ElementAt(i).Value == occ)
                        result += "Test " + numTest + " : OK : nombre de \"" + caract + "\" = " + frequency.ElementAt(i).Value;
                    else
                        result += "Test " + numTest + " : Erreur : nombre \"" + caract + "\"\n";
                }
            }
            return result;
        }
    }
}
