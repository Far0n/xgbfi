using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XgbFeatureInteractions.Properties;

namespace XgbFeatureInteractions
{
    public static class GlobalSettings
    {
        public static string XgbModelFile { get; set; }
        public static string OutputXlsxFile { get; set; }
        public static int MaxInteractionDepth { get; set; }
        public static int TopK { get; set; }
        public static int MaxTrees { get; set; }
        public static int MaxDeepening { get; set; }
        public static string SortBy { get; set; }

        static GlobalSettings() {

             XgbModelFile = Settings.Default.XgbModelFile.Replace("\"", "");
             OutputXlsxFile = Settings.Default.OutputXlsxFile.Replace("\"", "");
             MaxInteractionDepth = Settings.Default.MaxInteractionDepth;
             TopK = Settings.Default.TopK;
             MaxTrees = Settings.Default.MaxTrees;
             MaxDeepening = Settings.Default.MaxDeepening;
             SortBy = Settings.Default.SortBy;
        }

    }
}
