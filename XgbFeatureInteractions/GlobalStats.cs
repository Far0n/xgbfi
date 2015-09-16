using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XgbFeatureInteractions
{
    public static class GlobalStats
    {
        public static int ParsedTrees { get; set; }
        public static long CollectedFeatureInteractions { get; set; }
        public static TimeSpan ElapsedTime { get; set; }
        public static long ModelFileSize { get; set; }
    }
}
