using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XgbFeatureInteractions
{

    public enum SortingMetric
    {
        Gain,
        FScore,
        FScoreWeighted,
        FScoreWeightedAverage,
        AverageGain,
        ExpectedGain
    }

    static class FeatureScoreComparer
    {
        private static Func<FeatureInteraction,FeatureInteraction,int> _comparer { set; get; }

        public static void SetComparer(string sortingMetric)
        {

        }

        public static void SetComparer(SortingMetric sortingMetric)
        {
            switch(sortingMetric)
            {
                
                case SortingMetric.Gain:
                default:
                    _comparer = (a, b) =>
                    {
                        return -a.Gain.CompareTo(b.Gain);
                    };
                    break;
                case SortingMetric.FScore:
                    _comparer = (a, b) =>
                    {
                        return -a.FScore.CompareTo(b.FScore);
                    };
                    break;
                case SortingMetric.FScoreWeighted:
                    _comparer = (a, b) =>
                    {
                        return -a.FScoreWeighted.CompareTo(b.FScoreWeighted);
                    };
                    break;
                case SortingMetric.FScoreWeightedAverage:
                    _comparer = (a, b) =>
                    {
                        return -a.FScoreWeightedAverage.CompareTo(b.FScoreWeightedAverage);
                    };
                    break;
                case SortingMetric.AverageGain:
                    _comparer = (a, b) =>
                    {
                        return -a.AverageGain.CompareTo(b.AverageGain);
                    };
                    break;
                case SortingMetric.ExpectedGain:
                    _comparer = (a, b) =>
                    {
                        return -a.ExpectedGain.CompareTo(b.ExpectedGain);
                    };
                    break;

            }
        }

        static FeatureScoreComparer()
        {
            SetComparer(SortingMetric.Gain);
        }

        public static int Compare(FeatureInteraction a, FeatureInteraction b)
        {
            return _comparer(a, b);
        }


    }
}
