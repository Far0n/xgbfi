using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NGenerics.DataStructures.Trees;

namespace XgbFeatureInteractions
{
    public class XgbTree : BinaryTree<XgbTreeNode>
    {
        public XgbTree(XgbTreeNode root) : base(root)
        {

        }

    }
}
