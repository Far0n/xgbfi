using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XgbFeatureInteractions
{
    public class SplitValueHistogram : SortedDictionary<double,double>
    {
        public SplitValueHistogram() : base()
        {

        }

        public void AddValue(double splitValue, double count=1)
        {
            if(!this.ContainsKey(splitValue)){
                this.Add(splitValue, 0);
            }
            this[splitValue] += count;
        }

        public void Merge(SortedDictionary<double, double> histogram)
        {
            foreach(var kvp in histogram)
            {
                this.AddValue(kvp.Key, kvp.Value);
            }
        }
    }
}
