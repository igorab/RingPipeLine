using System;
using System.Collections.Generic;
using System.Linq;
using Application = Microsoft.Office.Interop.Excel.Application;
using Microsoft.Office.Interop.Excel;
using System.IO;

namespace CalcModule_RingPipeLine
{
    public partial class CalcModule_Hydro
    {
        public const string cPipeLine = "pipeline_w";
        public const string cConsumer = "consumer_w";
        public const string cWS = "supply_w";
        public const string cWell = "inspection_well_w";

        //диаметр условный (трубы стальные электросварные)
        public List<double> D_Nom = new List<double>() { 0, 50, 65, 70, 80, 100, 125, 150, 175, 200, 250, 300, 350, 400, 450, 500, 600, 700, 800, 900, 1000, 1200, 1400, 1500, 1600 };

        public List<string> Mat_Nom = new List<string>() { "", "", "", "" };

        public int FindMaterialNum(string _mat)
        {
            int num = 0;
            foreach (string material in Mat_Nom)
            {
                num++;
                if (material == _mat)
                    break;
            }
            return num;
        }

        public int FindDiameterNum(double diameter = 0)
        {
            int num = 0;

            for (int idx = 0; idx < D_Nom[idx]; idx++)
            {
                if (diameter >= D_Nom[idx])
                    num = idx;
            }

            return num;
        }


        public RP_Well DataWellFind_First(string _firstPointId, ref List<RP_Well> _ListWell)
        {
            RP_Well rp_Well = new RP_Well { Num = 0, G_ID = "", Geo_Z = 0, WaterIn = 0, WaterOutgo = 0 };
            IEnumerable<RP_Well> query;

            if (_ListWell != null)
            {
                query = _ListWell.Where(wline => wline.G_ID == _firstPointId);

                if (query.Count() > 0)
                {
                    rp_Well = query.First();
                }
            }

            return rp_Well;
        }

        public RP_Well DataWellFind_Last(string _lastPointId, ref List<RP_Well> _ListWell)
        {
            RP_Well rp_Well = new RP_Well { Num = 0, G_ID = "", Geo_Z = 0, WaterIn = 0, WaterOutgo = 0 };
            IEnumerable<RP_Well> query;

            if (_ListWell != null)
            {
                query = _ListWell.Where(wline => wline.G_ID == _lastPointId);

                if (query.Count() > 0)
                {
                    rp_Well = query.First();
                }
            }

            return rp_Well;
        }

        //Найти ребро
        public RPEdge DataEdgeFind_First(int _firstPointNum, ref List<RPEdge> _RPEdges)
        {
            RPEdge rpEdge = new RPEdge() { Num = 0, FromPoint = 0, ToPoint = 0, G_FromPoint = "", G_ToPoint = "" };
            IEnumerable<RPEdge> query;

            if (_RPEdges != null)
            {
                query = _RPEdges.Where(edge => edge.FromPoint == _firstPointNum);

                if (query.Count<RPEdge>() > 0)
                {
                    rpEdge = query.First<RPEdge>();
                }
            }

            return rpEdge;
        }


        public RP_Pipeline DataPipeLineFind_First(string _firstPointId, ref List<RP_Pipeline> _ListPipeLine)
        {
            RP_Pipeline rp_Pipeline = new RP_Pipeline { Num = 0, G_ID = "", G_FirstPointId = "", G_LastPointId = "", Diameter = 0, Length = 0 };
            IEnumerable<RP_Pipeline> queryPipeLine;

            if (_ListPipeLine != null)
            {
                queryPipeLine = _ListPipeLine.Where(line => line.G_FirstPointId == _firstPointId);

                if (queryPipeLine.Count<RP_Pipeline>() > 0)
                {
                    rp_Pipeline = queryPipeLine.First<RP_Pipeline>();
                }
            }

            return rp_Pipeline;
        }


        public RP_Pipeline DataPipeLineFind_Last(string _lastPointId, ref List<RP_Pipeline> _ListPipeLine)
        {
            RP_Pipeline rp_Pipeline = new RP_Pipeline { Num = 0, G_ID = "", G_FirstPointId = "", G_LastPointId = "", Diameter = 0, Length = 0 };
            IEnumerable<RP_Pipeline> queryPipeLine;

            if (_ListPipeLine != null)
            {
                queryPipeLine = _ListPipeLine.Where(line => line.G_LastPointId == _lastPointId);

                if (queryPipeLine.Count<RP_Pipeline>() > 0)
                {
                    rp_Pipeline = queryPipeLine.First<RP_Pipeline>();
                }
            }

            return rp_Pipeline;
        }

        // поиск циклов
        private void FindRingsInPileline_W(ref List<RPEdge> _RPEdges)
        {
            RPRings = new List<List<RPEdge>>();
            List<RPEdge> Ring = new List<RPEdge>();
            List<RP_Pipeline> rp_Pipelines = new List<RP_Pipeline>();
            List<RP_Well> rp_Wells = new List<RP_Well>();

            foreach (var well in rp_Wells)
            {
                int wellNum = well.Num;
                int nodeFrom, 
                    nodeTo;

                RP_Pipeline RP_PipelineFirst = DataPipeLineFind_First(well.G_ID, ref rp_Pipelines);
                nodeFrom = RP_PipelineFirst.Num;

                RP_Pipeline RP_PipelineLast = DataPipeLineFind_Last(well.G_ID, ref rp_Pipelines);                
                nodeTo = RP_PipelineLast.Num;

                var edge = new RPEdge(nodeFrom, nodeTo);

                RPEdges.Add(edge);
            }
            
            RPRings.Add(Ring);
        }

        /// <summary>
        /// определить циклы в схеме трубопровода
        /// </summary>
        private void FindCyclesInPipeline()
        {
            List<int> vertices = new List<int> { 1, 2, 3 };
            ;
            List<int[]> edges = new List<int[]>() { new int[] { 1, 2 }, new int[] { 2, 3 }, new int[] { 3, 1 } };

            foreach (RPNode[] V in Edges)
            {
                RPNode pFrom = V[0];
                RPNode pTo = V[1];
            }

            List<PointXY> distPointsNet = PointsNet.Distinct().ToList();
            IEnumerable<PointXY> Points = PointsNet.Distinct();

            int idx = 0;
            Dictionary<int, PointXY> Vertecies = new Dictionary<int, PointXY>();
            foreach (PointXY Vertex in Points)
            {
                PointXY PointNum = new PointXY() { Idx = ++idx, X = Vertex.X, Y = Vertex.Y };

                Vertecies.Add(idx, PointNum);
            }

            //RPGraph<int> graph = PathGraph.CreateGraph();
            //graph.CreateGraph(vertices, edges);
        }

    }


    public class PointXY : IEquatable<PointXY>
    {
        public int Idx { get; set; }
        public double X { get; set; }
        public double Y { get; set; }

        public bool Equals(PointXY other)
        {
            Boolean equal = false;

            //Check whether the compared object is null.
            if (Object.ReferenceEquals(other, null)) return false;

            //Check whether the compared object references the same data.
            if (Object.ReferenceEquals(this, other)) return true;

            //equal = ((X == other.X) && (Y == other.Y));

            return equal;
        }

        public int GetHashCode(PointXY product)
        {
            //Check whether the object is null
            if (Object.ReferenceEquals(product, null)) return 0;

            //Get hash code for the Name field if it is not null.
            int hashProductName = product.X == 0 ? 0 : product.X.GetHashCode();

            //Get hash code for the Code field.
            int hashProductCode = product.Y.GetHashCode();

            //Calculate the hash code for the product.
            return hashProductName ^ hashProductCode;
        }
    }


    /// <summary>
    /// Структура данных для вывода в excel
    /// </summary>
    public struct RPReportFields
    {
        public int SectionId;
        public string Direction;
        public double WaterСonsumption;
        public double WaterVelocity;
        public double PressureLoss;
        public double Diameter;
        public int Iter; 
        public double dH;
        public double dQ;
    }

    /// <summary>
    /// Вывести результаты расчета в Excel
    /// </summary>
    public class RPPrintExcel
    {
        public List<RPReportFields> ReportOutput { get; set; }

        protected Application application;
        protected Workbook workBook;
        protected Worksheet worksheet;

        public virtual string WorkingFolder()
        {
            return ""; //SpecialFolders.WorkingFolder;
        }

        public void InitReport()
        {
            // Файл шаблона
            string template = TemplateName();

            application = new Application
            {
                DisplayAlerts = false
            };

            // Открываем книгу
            workBook = application.Workbooks.Open(Path.Combine(WorkingFolder(), template));
            //workBook = application.Workbooks.Open(Path.Combine(Environment.CurrentDirectory, template));

            // Получаем активную таблицу
            worksheet = workBook.ActiveSheet as Worksheet;

        }

        /// <summary>
        /// Вставить новую строку в Excel
        /// </summary>
        /// <param name="rowNumber">Номер строки</param>
        protected void InsertNewRow(int rowNumber)
        {
            Range line = (Range)worksheet.Rows[rowNumber + 1];
            line.Insert();
        }

        protected void PrintReportRow(int _row, RPReportFields _fields)
        {
            string range;
            string[] columns = { "A", "B", "C", "D", "E", "F", "G", "H", "I" };

            range = String.Format("{0}{1:d}", columns[0], _row);
            worksheet.Range[range].Value = _fields.SectionId;

            worksheet.Range[range].HorizontalAlignment = XlHAlign.xlHAlignLeft;
            worksheet.Range[range].Font.Bold = true;

            range = String.Format("{0}{1:d}", columns[1], _row);
            worksheet.Range[range].Value = _fields.Direction;

            range = String.Format("{0}{1:d}", columns[2], _row);
            worksheet.Range[range].Value = _fields.WaterСonsumption;

            range = String.Format("{0}{1:d}", columns[3], _row);
            worksheet.Range[range].Value = _fields.WaterVelocity;

            range = String.Format("{0}{1:d}", columns[4], _row);
            worksheet.Range[range].Value = _fields.PressureLoss;

            range = String.Format("{0}{1:d}", columns[5], _row);
            worksheet.Range[range].Value = _fields.Diameter;

            range = String.Format("{0}{1:d}", columns[6], _row);
            worksheet.Range[range].Value = _fields.Iter;

            range = String.Format("{0}{1:d}", columns[7], _row);
            worksheet.Range[range].Value = _fields.dQ;

            range = String.Format("{0}{1:d}", columns[8], _row);
            worksheet.Range[range].Value = _fields.dH;

            worksheet.Range[range].EntireRow.Hidden = false;

            InsertNewRow(_row);
        }


        private string TemplateName()
        {
            return @"MVKReport\ВОР-водопровод.xltx";
        }

        public void ShowData()
        {
            int currentRow = 4;

            try
            {
                InitReport();

                foreach (RPReportFields ReportFields in ReportOutput)
                {
                    PrintReportRow(++currentRow, ReportFields);
                }

                application.Visible = true;

            }
            catch
            {
                if (application != null)
                    application.Quit();

                //err_txt = "Ошибка при формировании отчета!";
            }
        }
        
    }


    /// <summary>
    /// Увязка сети 
    /// </summary>
    public class RPLinkage
    {
        public double CaseDiameter(int _materialNum, int _diameterNum)
        {
            return 0;
        }

        /// <summary>
        /// удельное гидравлическое сопротивление труб
        /// </summary>
        /// <param name="_materialNum"></param>
        /// <param name="_diameterNum"></param>
        /// <param name="_waterflowRate"></param>
        /// <returns></returns>
        public double CalcHydraulicResistance(int _materialNum, int _diameterNum, double _waterflowRate)
        {
            return 0;
        }

        /// <summary>
        /// падение напора на участке
        /// </summary>
        /// <returns></returns>
        public double PressureLoss()
        {
            return 0;
        }
    }

}
