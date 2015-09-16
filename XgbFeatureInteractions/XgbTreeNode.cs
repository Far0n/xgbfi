using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XgbFeatureInteractions
{
    public class XgbTreeNode
    {
        public int Number { get; set; } 
        public string Feature { get; set; }
        public double Gain { get; set; }
        public double Cover { get; set; }
        public int? LeftChild { get; set; }
        public int? RightChild { get; set; }
        public bool IsLeaf { get; set; }
        public double SplitValue { get; set; }
        public double LeafValue { get; set; }


        public XgbTreeNode()
        {
            Feature = String.Empty;
            Gain = 0;
            Cover = 0;
            Number = -1;
            LeftChild = null;
            RightChild = null;
            LeafValue = 0;
            SplitValue = 0;
            IsLeaf = false;
        }
    }
}
