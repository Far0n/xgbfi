using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using XgbFeatureInteractions.Properties;

namespace XgbFeatureInteractions
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("********************************");
            Console.WriteLine("* XGBOOST Feature Interactions *");
            Console.WriteLine("********************************");
            Console.ResetColor();

            ParseArgs(args);
            FIScoreComparer.SetComparer(GlobalSettings.SortBy);

            Console.WriteLine("");

            Console.WriteLine("Settings:");
            Console.WriteLine("=========");
            Console.WriteLine(String.Format("XgbModelFile: {0}", GlobalSettings.XgbModelFile));
            Console.WriteLine(String.Format("OutputXlsxFile: {0}", GlobalSettings.OutputXlsxFile));
            Console.WriteLine(String.Format("MaxInteractionDepth: {0}", GlobalSettings.MaxInteractionDepth));
            Console.WriteLine(String.Format("MaxDeepening: {0}", GlobalSettings.MaxDeepening));
            Console.WriteLine(String.Format("MaxTrees: {0}", GlobalSettings.MaxTrees));
            Console.WriteLine(String.Format("TopK: {0}", GlobalSettings.TopK));
            Console.WriteLine(String.Format("SortBy: {0}", FIScoreComparer.SortBy));
            Console.WriteLine();

            if(args.Length == 0)
            {
                Thread t1 = new Thread(() =>
                {
                    for (int c = 3; c >= 0; c--)
                    {
                        Console.Write("\rStarting in {0:00} seconds with the settings above. Press any key to abort ...", c);
                        if (c == 0) break;
                        for (int i = 0; i < 20; i++)
                        {
                            if (Console.KeyAvailable)
                            {
                                Console.ReadKey(true);
                                Console.WriteLine();
                                Environment.Exit(0);
                            }
                            Thread.Sleep(50);
                        }
                    }
                    
                    return;
                });

                t1.Start();
                t1.Join();
                Console.WriteLine("\n");
            }

            var start = DateTime.Now;
            XgbModel xgbModel = XgbModelParser.GetXgbModelFromFile(GlobalSettings.XgbModelFile, GlobalSettings.MaxTrees);

            if(xgbModel == null)
            {
                Environment.Exit(-1);
            }

            var featureInteractions = xgbModel.GetFeatureInteractions(GlobalSettings.MaxInteractionDepth, GlobalSettings.MaxDeepening);

            var end = DateTime.Now;
            GlobalStats.ElapsedTime = (end - start);
            GlobalStats.ParsedTrees = xgbModel.NumTrees;
            GlobalStats.CollectedFeatureInteractions = featureInteractions.Count;

            featureInteractions.WriteToXlsx(GlobalSettings.OutputXlsxFile, GlobalSettings.TopK);

            end = DateTime.Now;
            Console.WriteLine(String.Format("Elapsed Time: {0}", (end - start)));

        }

        static void ParseArgs(string[] args)
        {
            if (args.Length == 0)
            {
                PrintHelp();
                return;
            }

            string cmds = string.Join(" ", args);
            Match m = null;

            if(cmds.Contains("-help"))
            {
                PrintHelp();
                Environment.Exit(0);
            }

            m = Regex.Match(cmds, @"-m\s([^\s]*)");
            if (m.Success)
            {
                var model_file = m.Groups[1].Value;
                GlobalSettings.XgbModelFile = model_file;
            }
            m = Regex.Match(cmds, @"-o\s([^\s]*)");
            if(m.Success) {
                var output_file = m.Groups[1].Value;
                if(!output_file.EndsWith(".xslx"))
                {
                    output_file = output_file + ".xlsx";
                }
                GlobalSettings.OutputXlsxFile = output_file;
            } 

            m = Regex.Match(cmds, @"-d\s([^\s]*)");
            if (m.Success)
            {
                var max_depth = m.Groups[1].Value;
                var tmp = 0;
                int.TryParse(max_depth, out tmp);
                GlobalSettings.MaxInteractionDepth = tmp;
            }
            m = Regex.Match(cmds, @"-g\s([^\s]*)");
            if (m.Success)
            {
                var max_deepening = m.Groups[1].Value;
                var tmp = 0;
                int.TryParse(max_deepening, out tmp);
                GlobalSettings.MaxDeepening = tmp;
            }
            m = Regex.Match(cmds, @"-t\s([^\s]*)");
            if (m.Success)
            {
                var ntrees = m.Groups[1].Value;
                var tmp = 0;
                int.TryParse(ntrees, out tmp);
                GlobalSettings.MaxTrees = tmp;
            }
            m = Regex.Match(cmds, @"-k\s([^\s]*)");
            if (m.Success)
            {
                var k = m.Groups[1].Value;
                var tmp = 0;
                int.TryParse(k, out tmp);
                GlobalSettings.TopK = tmp;
            }
            m = Regex.Match(cmds, @"-s\s([^\s]*)");
            if (m.Success)
            {
                GlobalSettings.SortBy = m.Groups[1].Value;
            }

            return;
        }

        static void PrintHelp()
        {
            Console.ResetColor();
            Console.WriteLine("Usage: XgbFeatureInteractions.exe [Options]");
            Console.WriteLine("\nOptions:");
            Console.WriteLine("\t-m  Xgboost model dump (dumped w/ 'with_stats=True')");
            Console.WriteLine("\t-d  Upper bound for extracted feature interactions depth");
            Console.WriteLine("\t-g  Upper bound for interaction start deepening (zero deepening => interactions starting @root only)");
            Console.WriteLine("\t-t  Upper bound for trees to be parsed");
            Console.WriteLine("\t-k  Upper bound for exportet feature interactions per depth level");
            Console.WriteLine("\t-s  Score metric to sort by (Gain, FScore, wFScore, wFScoreAvg, GainAvg, GainExp)");
            Console.WriteLine("\t-o  Xlsx file to be written");
        }
    }
}
