using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XgbFeatureInteractions.Properties;

namespace XgbFeatureInteractions
{
    public class XgbModel
    {
        public List<XgbTree> XgbTrees { get; set; }
        private FeatureInteractions _treeFeatureInteractions { get; set; }
        private int _maxInteractionDepth { get; set; }
        private HashSet<string> _pathMemo { get; set; }
        private int _maxDeepening { get; set; }
        public int NumTrees
        {
            get { return XgbTrees.Count; }
        }
        public XgbModel()
        {
            XgbTrees = new List<XgbTree>();
        }
        public FeatureInteractions GetFeatureInteractions(int maxInteractionDepth = -1, int maxDeepening = -1)
        {
            FeatureInteractions xgbFeatureInteractions = new FeatureInteractions();
            _maxInteractionDepth = maxInteractionDepth;
            _maxDeepening = maxDeepening;

            Console.ResetColor();
            if(_maxInteractionDepth == -1)
                Console.WriteLine(String.Format("Collectiong feature interactions"));
            else
                Console.WriteLine(String.Format("Collectiong feature interactions up to depth {0}", _maxInteractionDepth));

            for (int i = 0; i < NumTrees; i++)
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write(String.Format("Collectiong feature interactions within tree #{0} ", i+1));

                _treeFeatureInteractions = new FeatureInteractions();
                _pathMemo = new HashSet<string>();
                CollectFeatureInteractions(XgbTrees[i], new HashSet<XgbTreeNode>() , currentGain: 0, currentCover: 0, pathProbability: 1, depth: 0, deepening: 0);

                //double treeGain = _treeFeatureInteractions.GetFeatureInteractionsOfDepth(0).Sum(x => x.Value.Gain);
                //foreach (KeyValuePair<string, FeatureInteraction> fi in _treeFeatureInteractions)
                //{
                //    fi.Value.Gain /= treeGain;
                //}

                Console.WriteLine(String.Format("=> number of interactions: {0}", _treeFeatureInteractions.Count));
                Console.ResetColor();
                xgbFeatureInteractions.Merge(_treeFeatureInteractions);
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(String.Format("{0} feature interactions has been collected.\n", xgbFeatureInteractions.Count));
            Console.ResetColor();

            return xgbFeatureInteractions;
        }
        private void CollectFeatureInteractions(XgbTree tree, HashSet<XgbTreeNode> currentInteraction, double currentGain, double currentCover, double pathProbability, int depth, int deepening)
        {
            if (tree.IsLeafNode)
            {
                return;
            }

            currentInteraction.Add(tree.Data);
            currentGain += tree.Data.Gain;
            currentCover += tree.Data.Cover;

            var pathProbabilityLeft = pathProbability * (((XgbTree)tree.Left).Data.Cover / tree.Data.Cover);
            var pathProbabilityRight = pathProbability * (((XgbTree)tree.Right).Data.Cover / tree.Data.Cover);

            var fi = new FeatureInteraction(currentInteraction, currentGain, currentCover, pathProbability, 1);

            if (depth < _maxDeepening || _maxDeepening < 0)
            {
                var newInteractionLeft = new HashSet<XgbTreeNode>() { };
                var newInteractionRight = new HashSet<XgbTreeNode>() { };

                CollectFeatureInteractions((XgbTree)tree.Left, newInteractionLeft, 0, 0, pathProbabilityLeft, depth + 1, deepening + 1);
                CollectFeatureInteractions((XgbTree)tree.Right, newInteractionRight, 0, 0, pathProbabilityRight, depth + 1, deepening + 1);
            }

            var path = string.Join("-", currentInteraction.Select(x => x.Number));

            if (!_treeFeatureInteractions.ContainsKey(fi.Name))
            {
                _treeFeatureInteractions.Add(fi.Name, fi);
                _pathMemo.Add(path);
            }
            else
            {    
                // reoccurrence?
                if(_pathMemo.Contains(path))
                {
                    return;
                }

                _pathMemo.Add(path);
                var tfi = _treeFeatureInteractions[fi.Name];
                tfi.Gain += currentGain;
                tfi.Cover += currentCover;
                tfi.FScore += 1;
                tfi.FScoreWeighted += pathProbability;
                tfi.AverageFScoreWeighted = tfi.FScoreWeighted / tfi.FScore;
                tfi.AverageGain = tfi.Gain / tfi.FScore;
                tfi.ExpectedGain += currentGain * pathProbability;

            }

            if (currentInteraction.Count - 1 == _maxInteractionDepth)
                return;


            var currentInteractionLeft = new HashSet<XgbTreeNode>(currentInteraction);
            var currentInteractionRight = new HashSet<XgbTreeNode>(currentInteraction);

            var leftTree = (XgbTree)(tree.Left);
            var rightTree = (XgbTree)(tree.Right);

            if (leftTree.IsLeafNode && deepening == 0)
            {
                var tfi = _treeFeatureInteractions[fi.Name];
                tfi.SumLeafValuesLeft += leftTree.Data.LeafValue;
                tfi.SumLeafCoversLeft += leftTree.Data.Cover;
                tfi.HasLeafStatistics = true;
            }

            if (rightTree.IsLeafNode && deepening == 0)
            {
                var tfi = _treeFeatureInteractions[fi.Name];
                tfi.SumLeafValuesRight += rightTree.Data.LeafValue;
                tfi.SumLeafCoversRight += rightTree.Data.Cover;
                tfi.HasLeafStatistics = true;
            }

            CollectFeatureInteractions((XgbTree)tree.Left, currentInteractionLeft, currentGain, currentCover, pathProbabilityLeft, depth + 1, deepening);
            CollectFeatureInteractions((XgbTree)tree.Right, currentInteractionRight, currentGain, currentCover, pathProbabilityRight, depth + 1, deepening);

        }

    }
}
