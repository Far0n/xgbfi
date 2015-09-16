using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NGenerics.DataStructures.Trees;
using XgbFeatureInteractions.Properties;

namespace XgbFeatureInteractions
{
    public static class XgbModelParser
    {
        private static Dictionary<int, XgbTreeNode> xgbNodeList = new Dictionary<int, XgbTreeNode>();
        private static Regex nodeRegex = new Regex(@"(\d+):\[(.*)<(.+)\]\syes=(.*),no=(.*),missing=.*,gain=(.*),cover=(.*)", RegexOptions.Compiled);
        private static Regex leafRegex = new Regex(@"(\d+):leaf=(.*),cover=(.*)", RegexOptions.Compiled);

        public static XgbModel GetXgbModelFromFile(string fileName, int maxTrees)
        {
            XgbModel xgbModel = new XgbModel();

            if (!File.Exists(fileName))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(String.Format("Error: File {0} does not exist.",fileName));
                Console.ResetColor();
                return null;
            }


            Console.ResetColor();
            Console.WriteLine(String.Format("Parsing {0}", fileName));

            int numTree = 0;

            var fileInfo = new FileInfo(fileName);
            GlobalStats.ModelFileSize = fileInfo.Length;

            using (StreamReader sr = new StreamReader(fileName))
            {
                while(!sr.EndOfStream) {
                    

                    var line = sr.ReadLine().Trim();
                    if (line.StartsWith("booster") || line == String.Empty)
                    {
                        if (xgbNodeList.Count > 0)
                        {
                            numTree++;
                            Console.ForegroundColor = ConsoleColor.DarkGreen;
                            Console.Write(String.Format("Constructing tree #{0} ", numTree));


                            XgbTree tree = new XgbTree(xgbNodeList[0]);
                            ConstructXgbTree(tree);

                            Console.WriteLine(String.Format("=> depth: {0} ({1} nodes)", tree.Height, xgbNodeList.Count));
                            Console.ResetColor();

                            xgbModel.XgbTrees.Add(tree);
                            xgbNodeList.Clear();
                            if (numTree == maxTrees) break;
                        }
                    }
                    else
                    {
                        var node = ParseXgbTreeNode(line);
                        if (node == null)
                        {
                            return null;
                        }
                        xgbNodeList.Add(node.Number, node);                      
                    }
                }
            }
            if (xgbNodeList.Count > 0 && (maxTrees < 0 || numTree < maxTrees))
            {
                numTree++;
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write(String.Format("Constructing tree #{0} ", numTree));


                XgbTree tree = new XgbTree(xgbNodeList[0]);
                ConstructXgbTree(tree);

                Console.WriteLine(String.Format("=> depth: {0} ({1} nodes)", tree.Height, xgbNodeList.Count));
                Console.ResetColor();

                xgbModel.XgbTrees.Add(tree);
                xgbNodeList.Clear();
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(String.Format("{0} trees has been constructed.\n", xgbModel.NumTrees));
            Console.ResetColor();
            return xgbModel;
        }

        private static XgbTreeNode ParseXgbTreeNode(string line)
        {
            var node = new XgbTreeNode();
            try {
                if (line.Contains("leaf"))
                {
                    Match m = leafRegex.Match(line);
                    node.Number = Int32.Parse(m.Groups[1].Value);
                    node.LeafValue = Double.Parse(m.Groups[2].Value, CultureInfo.InvariantCulture);
                    node.Cover = Double.Parse(m.Groups[3].Value, CultureInfo.InvariantCulture);
                    node.IsLeaf = true;
                }
                else
                {
                    Match m = nodeRegex.Match(line);
                    node.Number = Int32.Parse(m.Groups[1].Value);
                    node.Feature = m.Groups[2].Value;
                    node.SplitValue = Double.Parse(m.Groups[3].Value, CultureInfo.InvariantCulture);
                    node.LeftChild = Int32.Parse(m.Groups[4].Value);
                    node.RightChild = Int32.Parse(m.Groups[5].Value);
                    node.Gain = Double.Parse(m.Groups[6].Value, CultureInfo.InvariantCulture);
                    node.Cover = Double.Parse(m.Groups[7].Value, CultureInfo.InvariantCulture);
                    node.IsLeaf = false;
                }
            } catch(Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(String.Format("Error: Invalid model file. Did you dump the model w/ with_stats=True?"));
                Console.WriteLine(String.Format("Unable to parse line '{0}'", line));
                Console.WriteLine(e.Message);
                Console.ResetColor();
                return null;
            }

            return node;
        }

        private static void ConstructXgbTree(XgbTree tree)
        {
            if (tree.Data.LeftChild != null)
            {
                tree.Add(new XgbTree(xgbNodeList[(int)tree.Data.LeftChild]));
                ConstructXgbTree((XgbTree)tree.Left);
            }


            if (tree.Data.RightChild != null)
            {
                tree.Add(new XgbTree(xgbNodeList[(int)tree.Data.RightChild]));
                ConstructXgbTree((XgbTree)tree.Right);
            }

        }


    }
}
