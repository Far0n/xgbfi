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
        AverageFScoreWeighted,
        AverageGain,
        ExpectedGain
    }

    static class FIScoreComparer
    {
        private static Func<FeatureInteraction,FeatureInteraction,int> _comparer { set; get; }
        public static SortingMetric SortBy { private set; get; }

        public static void SetComparer(string sortBy)
        {
            sortBy = sortBy.ToLower().Replace(" ", "");
            if(sortBy == "gain")
            {
                SetComparer(SortingMetric.Gain);
                return;
            }

            if (sortBy == "fscore")
            {
                SetComparer(SortingMetric.FScore);
                return;
            }

            if (sortBy == "wfscore")
            {
                SetComparer(SortingMetric.FScoreWeighted);
                return;
            }

            if (sortBy == "wfscoreavg")
            {
                SetComparer(SortingMetric.AverageFScoreWeighted);
                return;
            }

            if (sortBy == "gainavg")
            {
                SetComparer(SortingMetric.AverageGain);
                return;
            }

            if (sortBy == "gainexp")
            {
                SetComparer(SortingMetric.ExpectedGain);
                return;
            }
        }

        public static void SetComparer(SortingMetric sortingMetric)
        {
            switch(sortingMetric)
            {
                
                case SortingMetric.Gain:
                default:
                    SortBy = SortingMetric.Gain;
                    _comparer = (a, b) =>
                    {
                        return -a.Gain.CompareTo(b.Gain);
                    };
                    break;
                case SortingMetric.FScore:
                    SortBy = SortingMetric.FScore;
                    _comparer = (a, b) =>
                    {
                        return -a.FScore.CompareTo(b.FScore);
                    };
                    break;
                case SortingMetric.FScoreWeighted:
                    SortBy = SortingMetric.FScoreWeighted;
                    _comparer = (a, b) =>
                    {
                        return -a.FScoreWeighted.CompareTo(b.FScoreWeighted);
                    };
                    break;
                case SortingMetric.AverageFScoreWeighted:
                    SortBy = SortingMetric.AverageFScoreWeighted;
                    _comparer = (a, b) =>
                    {
                        return -a.AverageFScoreWeighted.CompareTo(b.AverageFScoreWeighted);
                    };
                    break;
                case SortingMetric.AverageGain:
                    SortBy = SortingMetric.AverageGain;
                    _comparer = (a, b) =>
                    {
                        return -a.AverageGain.CompareTo(b.AverageGain);
                    };
                    break;
                case SortingMetric.ExpectedGain:
                    SortBy = SortingMetric.ExpectedGain;
                    _comparer = (a, b) =>
                    {
                        return -a.ExpectedGain.CompareTo(b.ExpectedGain);
                    };
                    break;

            }
        }

        static FIScoreComparer()
        {
            SetComparer(SortingMetric.Gain);
        }

        public static int Compare(FeatureInteraction a, FeatureInteraction b)
        {
            return _comparer(a, b);
        }


    }
}
