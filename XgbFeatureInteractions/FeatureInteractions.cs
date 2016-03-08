using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;
using XgbFeatureInteractions.Properties;

namespace XgbFeatureInteractions
{
    public class FeatureInteractions : Dictionary<string,FeatureInteraction>
    {
        public int MaxDepth
        {
            get
            {
                return this.Max(x => x.Value.Depth);
            }
        }

        public double TotalGain
        {
            get
            {
                return this.Sum(x => x.Value.Gain);
            }
        }

        public double TotalCover
        {
            get
            {
                return this.Sum(x => x.Value.Cover);
            }
        }

        public double TotalFScore
        {
            get
            {
                return this.Sum(x => x.Value.FScore);
            }
        }

        public FeatureInteractions() : base()
        {

        }

        public void Merge(FeatureInteractions interactions)
        {
            foreach(KeyValuePair<string, FeatureInteraction> fi in interactions)
            {
                if(!ContainsKey(fi.Key))
                {
                    Add(fi.Key, fi.Value);
                } else
                {
                    this[fi.Key].Gain += fi.Value.Gain;
                    this[fi.Key].Cover += fi.Value.Cover;
                    this[fi.Key].FScore += fi.Value.FScore;
                    this[fi.Key].FScoreWeighted += fi.Value.FScoreWeighted;
                    this[fi.Key].AverageFScoreWeighted = this[fi.Key].FScoreWeighted / this[fi.Key].FScore;
                    this[fi.Key].AverageGain = this[fi.Key].Gain / this[fi.Key].FScore;
                    this[fi.Key].ExpectedGain += fi.Value.ExpectedGain;
                    this[fi.Key].SumLeafCoversLeft += fi.Value.SumLeafCoversLeft;
                    this[fi.Key].SumLeafCoversRight += fi.Value.SumLeafCoversRight;
                    this[fi.Key].SumLeafValuesLeft += fi.Value.SumLeafValuesLeft;
                    this[fi.Key].SumLeafValuesRight += fi.Value.SumLeafValuesRight;
                    this[fi.Key].TreeIndex += fi.Value.TreeIndex;
                    this[fi.Key].AverageTreeIndex = this[fi.Key].TreeIndex / this[fi.Key].FScore;
                }
            }
        }
        public FeatureInteractions(IEnumerable<KeyValuePair<string, FeatureInteraction>> featureInteractions) : base()
        {
            foreach (var fi in featureInteractions)
            {
                Add(fi.Key, fi.Value);
            }
        }

        public FeatureInteractions GetFeatureInteractionsOfDepth(int depth)
        {
            return new FeatureInteractions(new FeatureInteractions(this.Where(x => x.Value.Depth == depth)));
        }

        public FeatureInteractions GetFeatureInteractionsWithLeafStatistics()
        {
            return new FeatureInteractions(new FeatureInteractions(this.Where(x => x.Value.HasLeafStatistics == true)));
        }

        public bool WriteToXlsx(string fileName, int topK)
        {
            try
            {
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }

                FileInfo newFile = new FileInfo(fileName);
                ExcelPackage pck = new ExcelPackage(newFile);

                Console.ResetColor();
                Console.WriteLine("Writing {0}", fileName);

                List<FeatureInteraction> interactions = null;

                for (int depth=0; depth <= MaxDepth; depth++)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine(String.Format("Writing feature interactions with depth {0} ", depth));
                    Console.ResetColor();

                    interactions = GetFeatureInteractionsOfDepth(depth).Values.ToList();
                    interactions.Sort();

                    double KTotalGain = interactions.Sum(x => x.Gain);
                    double TotalCover = interactions.Sum(x => x.Cover);
                    double TotalFScore = interactions.Sum(x => x.FScore);
                    double TotalFScoreWeighted = interactions.Sum(x => x.FScoreWeighted);
                    double TotalFScoreWeightedAverage = interactions.Sum(x => x.AverageFScoreWeighted);

                    if (topK > 0)
                    {
                        interactions = interactions.Take(topK).ToList();
                    }

   
                    if (interactions.Count == 0)
                    {
                        break;
                    }

                    var ws = pck.Workbook.Worksheets.Add(String.Format("Interaction Depth {0}",depth));

                    ws.Row(1).Height = 20;
                    ws.Row(1).Style.Font.Bold = true;
                    ws.Row(1).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    ws.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    ws.Column(1).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    ws.Column(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    ws.Column(1).Width = interactions.Max(x => x.Name.Length) + 10;
                    ws.Column(2).Width = 17;
                    ws.Column(3).Width = 17;
                    ws.Column(4).Width = 17;
                    ws.Column(5).Width = 17;
                    ws.Column(6).Width = 17;
                    ws.Column(7).Width = 17;
                    ws.Column(8).Width = 17;
                    ws.Column(9).Width = 17;
                    ws.Column(10).Width = 17;
                    ws.Column(11).Width = 18;
                    ws.Column(12).Width = 18;
                    ws.Column(13).Width = 19;
                    ws.Column(14).Width = 17;
                    ws.Column(15).Width = 19;

                    ws.Cells[1, 1].Value = "Interaction";
                    ws.Cells[1, 2].Value = "Gain";
                    ws.Cells[1, 3].Value = "FScore";
                    ws.Cells[1, 4].Value = "wFScore";
                    ws.Cells[1, 5].Value = "Average wFScore";
                    ws.Cells[1, 6].Value = "Average Gain";
                    ws.Cells[1, 7].Value = "Expected Gain";
                    ws.Cells[1, 8].Value = "Gain Rank";
                    ws.Cells[1, 9].Value = "FScore Rank";
                    ws.Cells[1, 10].Value = "wFScore Rank";
                    ws.Cells[1, 11].Value = "Avg wFScore Rank";
                    ws.Cells[1, 12].Value = "Avg Gain Rank";
                    ws.Cells[1, 13].Value = "Expected Gain Rank";
                    ws.Cells[1, 14].Value = "Average Rank";
                    ws.Cells[1, 15].Value = "Average Tree Index";

                    var gainSorted = interactions.OrderBy(x => -x.Gain).ToList();
                    var fScoreSorted = interactions.OrderBy(x => -x.FScore).ToList();
                    var fScoreWeightedSorted = interactions.OrderBy(x => -x.FScoreWeighted).ToList();
                    var averagefScoreWeightedSorted = interactions.OrderBy(x => -x.AverageFScoreWeighted).ToList();
                    var averageGainSorted = interactions.OrderBy(x => -x.AverageGain).ToList();
                    var expectedGainSorted = interactions.OrderBy(x => -x.ExpectedGain).ToList();

                    List<object[]> excelData = new List<object[]>();

                    Func<double, double> _formatNumber = (x) =>
                         {
                             return Double.Parse(String.Format("{0:0.00}", x));
                         };

                    foreach (FeatureInteraction fi in interactions)
                    {
                        List<object> rowValues = new List<object>();
                  
                        rowValues.Add(fi.Name); //1
                        rowValues.Add(fi.Gain); //2
                        rowValues.Add(fi.FScore); //3
                        rowValues.Add(fi.FScoreWeighted); //4
                        rowValues.Add(fi.AverageFScoreWeighted); //5
                        rowValues.Add(fi.AverageGain); //6
                        rowValues.Add(fi.ExpectedGain); //7
                        rowValues.Add(gainSorted.FindIndex(x => x.Name == fi.Name) + 1); //8
                        rowValues.Add(fScoreSorted.FindIndex(x => x.Name == fi.Name) + 1); //9
                        rowValues.Add(fScoreWeightedSorted.FindIndex(x => x.Name == fi.Name) + 1); //10
                        rowValues.Add(averagefScoreWeightedSorted.FindIndex(x => x.Name == fi.Name) + 1); //11
                        rowValues.Add(averageGainSorted.FindIndex(x => x.Name == fi.Name) + 1); //12
                        rowValues.Add(expectedGainSorted.FindIndex(x => x.Name == fi.Name) + 1); //13
                        rowValues.Add(_formatNumber(rowValues.Skip(7).Average(x => Double.Parse(x.ToString())))); //14
                        rowValues.Add(fi.AverageTreeIndex); //15

                        excelData.Add(rowValues.ToArray());

                    }
                    ws.Cells["A2"].LoadFromArrays(excelData);
                }

                interactions = GetFeatureInteractionsWithLeafStatistics().Values.ToList();
                if(interactions.Count > 0)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine(String.Format("Writing leaf statistics"));
                    Console.ResetColor();

                    interactions.Sort();

                    var ws = pck.Workbook.Worksheets.Add(String.Format("Leaf Statistics"));

                    ws.Row(1).Height = 20;
                    ws.Row(1).Style.Font.Bold = true;
                    ws.Row(1).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    ws.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    ws.Column(1).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    ws.Column(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    ws.Column(1).Width = interactions.Max(x => x.Name.Length) + 10;
                    ws.Column(2).Width = 20;
                    ws.Column(3).Width = 20;
                    ws.Column(4).Width = 20;
                    ws.Column(5).Width = 20;

                    ws.Cells[1, 1].Value = "Interaction";
                    ws.Cells[1, 2].Value = "Sum Leaf Values Left";
                    ws.Cells[1, 3].Value = "Sum Leaf Values Right";
                    ws.Cells[1, 4].Value = "Sum Leaf Covers Left";
                    ws.Cells[1, 5].Value = "Sum Leaf Covers Right";

                    List<object[]> excelData = new List<object[]>();

                    foreach (FeatureInteraction fi in interactions)
                    {
                        List<object> rowValues = new List<object>();

                        rowValues.Add(fi.Name); //1
                        rowValues.Add(fi.SumLeafValuesLeft); //2
                        rowValues.Add(fi.SumLeafValuesRight); //3
                        rowValues.Add(fi.SumLeafCoversLeft); //4
                        rowValues.Add(fi.SumLeafCoversRight); //5

                        excelData.Add(rowValues.ToArray());

                    }
                    ws.Cells["A2"].LoadFromArrays(excelData);

                }

                pck.Save();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("{0} has been written.", fileName);
                Console.ResetColor();
                return true;
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("ERROR: {0}", e.Message);
                Console.ResetColor();
                return false;
            }

        }
    }
}
