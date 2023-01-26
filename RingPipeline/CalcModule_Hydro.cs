using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


/// <summary>
/// ГИДРАВЛИЧЕСКИЙ РАСЧЕТ КОЛЬЦЕВОЙ ВОДОПРОВОДНОЙ СЕТИ
/// </summary>
namespace CalcModule_RingPipeLine
{
    /// <summary>
    /// Узел
    /// </summary>
    public struct RPNode
    {        
        public int Num { get; set; }
        public string G_ID { get; set; }
        public bool Visited { get; set; }

        public List<RPEdge> IncomingEdges;
        public List<RPEdge> OutgoingEdges;
        //public Position Position;
        //public Feature Feature;

        public RPNode(int _Num = 0)
        {
            Num = _Num;
            G_ID = "";
            Visited = false;

            IncomingEdges = new List<RPEdge>();
            OutgoingEdges = new List<RPEdge>();
            //Position = null;
            //Feature = null;
        }
    }

    /// <summary>
    /// Участок сети
    /// </summary>
    public struct RPEdge
    {
        // Номер участка
        public int Num { get; set; }

        // начало участка            
        public RPNode FromNode { get; set; }
        // конец участка
        public RPNode ToNode { get; set; }

        // начало участка            
        public int FromPoint { get; set; }
        // конец участка
        public int ToPoint { get; set; }
        // направление
        public int DireсtSign { get; set; }

        // начало участка            
        public string G_FromPoint { get; set; }
        // конец участка
        public string G_ToPoint { get; set; }

        public string G_ID { get; set; }

        public int FeatureNum { get; set; }

        public bool Visited { get; set; }

        public double Length {get; set;}

        public RPEdge(int _FromPoint = 0, int _ToPoint = 0)
        {
            Num = 0;
            FromPoint = _FromPoint;
            ToPoint = _ToPoint;
            DireсtSign = +1;
            G_FromPoint = "";
            G_ToPoint = "";
            Visited = false;
            FromNode = new RPNode() { Num = _FromPoint };
            ToNode = new RPNode() { Num = _ToPoint };
            G_ID = "";
            Length = 0;
            FeatureNum = 0;
            //Feature = null;
        }

        public RPEdge(RPNode _FromNode, RPNode _ToNode)
        {
            Num = 0;
            FromNode = _FromNode;
            ToNode = _ToNode;
            DireсtSign = +1;
            G_FromPoint = _FromNode.G_ID;
            G_ToPoint = _ToNode.G_ID;
            Visited = false;
            FromPoint = FromNode.Num;
            ToPoint = ToNode.Num;
            G_ID = "";
            Length = 0;
            FeatureNum = 0;
            //Feature = null;
        }
        
        /// <summary>
        /// Направление
        /// </summary>
        public string Direction
        {
            get { return String.Format("({0}->{1})", FromPoint, ToPoint); }
        }
    }

    /// <summary>
    /// колодец
    /// </summary>
    public struct RP_Well
    {
        public int Num;
        public string G_ID;
        // подача/отбор в/из узла
        public double WaterOutgo;
        // подача воды в систему л/с
        public double WaterIn;
        // высотные отметки
        public double Geo_Z;
    }

    /// <summary>
    /// труба
    /// </summary>
    public struct RP_Pipeline
    {
        public int Num;
        public string G_ID;
        public string G_FirstPointId;
        public string G_LastPointId;
        // длина участка
        public double Length;
        // диаметр
        public double Diameter;
    }
            
    /// <summary>
    /// Расчет кольцевых трубопроводов
    /// 
    /// В ходе расчета должны быть определены:
    /// -расходы
    /// -потери напора
    /// 
    /// </summary>
    public partial class CalcModule_Hydro
    {
        public FeatureCollection FeatureCollection;

        // Узлы
        public List<RPNode> Nodes { get; set; }

        /// <summary>
        /// For test purpose
        /// </summary>
        /// <param name="_RPCalc">object</param>
        public static void InitTestData(ref CalcModule_Hydro _RPCalc, ref ArrayList _RPLParams)
        {
           List<double> WaterIn = (List<double>)_RPLParams[0];
           List<double> WaterOut = (List<double>)_RPLParams[1];
           List<double> EdgeLength = (List<double>)_RPLParams[4];

            //_RPCalc.WaterOutgo = new List<double>() { -320, 25, 100, 150, 75, -30 };
            _RPCalc.WaterOutgo = new List<double>() { WaterOut[2] - WaterIn[1], WaterOut[0], WaterOut[4], WaterOut[1], WaterOut[3], -WaterIn[0] };

            _RPCalc.CountNodes = _RPCalc.WaterOutgo.Count;
            _RPCalc.Geo_Z = new List<double>() { 150.5, 153.2, 155.1, 149, 151.6, 156.3 }; // node height, m

            _RPCalc.Geo_L = new List<double>(/*EdgeLength*/) { 550, 430, 210, 220, 570, 510, 300 }; //length, m

            _RPCalc.PipeMaterialNum = new List<int>() { 3, 3, 3, 3, 3, 3, 3 };
            
            _RPCalc.PipeDiameterNum = new List<int>() { 12, 11, 12, 10, 12, 11, 11 };
            _RPCalc.H_min = 22; // напор, м 
            _RPCalc.D_min = 100; //d, mm
            _RPCalc.OpimizationType = 0;
            _RPCalc.K_day = 1.2;
            _RPCalc.K_hour = 1.44;
            _RPCalc.E_Cost = 0.02; // руб/квтч
            _RPCalc.PaybackPeriod = 7; //лет

            _RPCalc.Nodes = new List<RPNode>();
            for (int nodeIdx = 1; nodeIdx <= 6; nodeIdx ++)
                _RPCalc.Nodes.Add(new RPNode(nodeIdx));

            _RPCalc.RPEdges = new List<RPEdge>() ;

            _RPCalc.RPEdges.Add(new RPEdge(1, 2) { Num = 1, DireсtSign = +1, G_ID = "D0DA026C-8863-4DE1-8543-B6E78B574917", Length = _RPCalc.Geo_L[0], FeatureNum = 22 }) ; 
            _RPCalc.RPEdges.Add(new RPEdge(2, 3) { Num = 2, DireсtSign = +1, G_ID = "1D499B30-63D6-41A0-ABE6-812E93342A69", Length = _RPCalc.Geo_L[1], FeatureNum = 2 });
            _RPCalc.RPEdges.Add(new RPEdge(1, 4) { Num = 3, DireсtSign = -1, G_ID = "4B21DCAC-C51A-471D-B0CE-BCB417150A90", Length = _RPCalc.Geo_L[2], FeatureNum = 8 });
            _RPCalc.RPEdges.Add(new RPEdge(2, 5) { Num = 4, DireсtSign = +1, G_ID = "AEB26C74-1050-4379-9AA0-613B3CD30C9E", Length = _RPCalc.Geo_L[3], FeatureNum = 17 });
            _RPCalc.RPEdges.Add(new RPEdge(5, 3) { Num = 5, DireсtSign = -1, G_ID = "040518C1-2417-4C14-8F0B-A6B63F34E66C", Length = _RPCalc.Geo_L[4], FeatureNum = 0 });
            _RPCalc.RPEdges.Add(new RPEdge(4, 5) { Num = 6, DireсtSign = -1, G_ID = "4C1AAA1D-AE75-4919-95C5-27BC0A958212", Length = _RPCalc.Geo_L[5], FeatureNum = 9 });
            _RPCalc.RPEdges.Add(new RPEdge(6, 3) { Num = 7, DireсtSign = +1, G_ID = "2949FDB6-D3C0-4BBE-A41F-2CCC16AC602F", Length = _RPCalc.Geo_L[6], FeatureNum = 14 });

            _RPCalc.CountAreas = _RPCalc.RPEdges.Count;

            _RPCalc.RPRings = new List<List<RPEdge>>();
            // 1 кольцо
            List<RPEdge> Ring = new List<RPEdge>() { _RPCalc.RPEdges[0], _RPCalc.RPEdges[3], _RPCalc.RPEdges[5], _RPCalc.RPEdges[2] }; //1, 4, -6, -3
            _RPCalc.RPRings.Add(Ring);

            // 2 кольцо
            Ring = new List<RPEdge>() { _RPCalc.RPEdges[1], _RPCalc.RPEdges[4] }; //2, -5, -4, 0

            RPEdge CurEdge = _RPCalc.RPEdges[3]; // поменялось направление
            CurEdge.DireсtSign = -1;
            Ring.Add(CurEdge);
            Ring.Add(new RPEdge() { Num = 0, G_ID = ""});
            _RPCalc.RPRings.Add(Ring);

            _RPCalc.CountRings = _RPCalc.RPRings.Count; // число колец
            _RPCalc.MaxCountAreas = Ring.Count; // максимальное число элементов в кольце (потребуется корректно определить)       
                
        }

        #region variables
        /// <summary>
        /// Общее число участков сети
        /// </summary>
        public int CountAreas { get; set; }

        /// <summary>
        /// Общее число узлов в сети
        /// </summary>
        public int CountNodes { get; set; }

        /// <summary>
        /// Общее число независимых колец (контуров) в сети
        /// </summary>
        public int CountRings { get; set; }

        /// <summary>
        /// Наибольшее число участков в одном кольце
        /// </summary>
        public int MaxCountAreas { get; set; }

        /// <summary>
        /// массив конфигурации сети
        /// </summary>
        public List<RPEdge> RPEdges { get; set; }

        /// <summary>
        /// конфигурация кольца
        /// </summary>
        public struct RPRing
        {
            public List<RPEdge> Edges { get; set; } 

            public RPRing(List<RPEdge> _Edges)
            {
                Edges = _Edges;
            }
        }

        /// <summary>
        /// массив конфигурации колец
        /// </summary>
        public List<List<RPEdge>> RPRings { get; set; }

        /// <summary>
        /// узловые расходы
        /// </summary>
        public List<double> WaterOutgo { get; set; }

        /// <summary>
        /// геодезические отметки узлов
        /// </summary>
        public List<double> Geo_Z { get; set; }

        /// <summary>
        /// длины участков
        /// </summary>        
        public List<double> Geo_L { get; set; }

        /// <summary>
        /// коды материалов труб
        /// </summary>
        public List<int> PipeMaterialNum { get; set; }

        /// <summary>
        /// номер диаметра труб - по сортаменту
        /// </summary>
        public List<int> PipeDiameterNum { get; set; }

        /// <summary>
        /// необходимый свободный напор в узле
        /// </summary>
        public double H_min { get; set; }

        /// <summary>
        /// минимальный диаметр при оптимизации
        /// </summary>
        public double D_min { get; set; }

        /// <summary>
        /// вид оптимизации
        /// </summary>
        public int OpimizationType;

        /// <summary>
        /// коэффициент суточной неравномерности потребления
        /// </summary>
        public double K_day;

        /// <summary>
        /// коэффициент часовой неравномерности потребления
        /// </summary>
        public double K_hour;

        /// <summary>
        /// стоимость электроэнергии
        /// </summary>
        public double E_Cost;

        /// <summary>
        /// Срок окупаемости
        /// </summary>
        public double PaybackPeriod { get; set; }
        #endregion

        public bool PrintExcel = false;

        // Ребра
        private List<RPNode[]> Edges { get; set; }
        private List<PointXY> PointsNet { get; set; } // ??
        private List<double[]> PointsCS { get; set; } // ??

        
        /// <summary>
        /// новый объект
        /// </summary>
        /// <returns></returns>
        public static CalcModule_Hydro construct()
        {
            CalcModule_Hydro RPCalc = new CalcModule_Hydro();
            return RPCalc;
        }

        
        /// <summary>
        /// распарсить geo json        
        /// </summary>
        protected void InitDataPipeline(ref List<RP_Well> rp_Source, ref List<RP_Well> rp_Consumer, ref List<RP_Well> rp_Wells, ref List<RP_Pipeline> rp_Pipelines )
        {
            Edges = new List<RPNode[]>();
            int numPipe = 0;
            int numCons = 0, numSource = 0, numWell = 0;                                
            SortedSet<string> setPoints = new SortedSet<string> { cConsumer, cWS, cWell };

            int iFrom = 1, 
                iTo = 2;
                        
            PointsNet = new List<PointXY>();
            PointsCS = new List<double[]>();

            IDictionary<String, Object> prop;
            KeyValuePair<string, object> keyValue;

            #region InitData
            /*
            foreach (Feature feature in FeatureCollection.Features)
            {
                prop = feature.Properties;
                keyValue = prop.First();

                if (keyValue.Value.ToString() == cPipeLine || setPoints.Contains(keyValue.Value.ToString()))
                {
                    switch (feature.Geometry.Type)
                    {
                        case GeoJSON.Net.GeoJSONObjectType.LineString:

                            LineString ls = feature.Geometry as LineString;
                            double dLength = GeoAlgo.GetLength(ls.Coordinates);

                            Points _points = ls.Coordinates;

                            int nPoints = _points.Count;
                            if (nPoints <= 1)
                                break;

                            for (int nPoint = 0; nPoint + 1 < nPoints; ++nPoint)
                            {
                                IPosition p0 = _points[nPoint];
                                IPosition p1 = _points[nPoint + 1];
                                double dx = p1.Longitude - p0.Longitude;
                                double dy = p1.Latitude - p0.Latitude;

                                Edges.Add(new RPNode[] { new RPNode() { Num = iFrom }, new RPNode() {Num = iTo } });

                                iFrom = iTo + 1;
                                iTo = iFrom + 1;

                                PointXY pXY = new PointXY() { X = Math.Round(p0.Latitude, 2), Y = Math.Round(p0.Longitude, 2) };
                                PointsNet.Add(pXY);
                                pXY = new PointXY() { X = Math.Round(p1.Latitude, 2), Y = Math.Round(p1.Longitude, 2) };
                                PointsNet.Add(pXY);

                            }

                            if (keyValue.Value.ToString() == cPipeLine)
                            {
                                RP_Pipeline pipeline = new RP_Pipeline();

                                pipeline.Num = ++numPipe;
                                if (prop.ContainsKey("ID"))
                                    pipeline.G_ID = Convert.ToString(prop["ID"]);
                                if (prop.ContainsKey("FirstPointID"))
                                    pipeline.G_FirstPointId = Convert.ToString(prop["FirstPointID"]);
                                if (prop.ContainsKey("LastPointID"))
                                    pipeline.G_LastPointId = Convert.ToString(prop["LastPointID"]);

                                pipeline.Length = dLength;
                                if (prop.ContainsKey("Diameter"))
                                    pipeline.Diameter = Convert.ToDouble(prop["Diameter"]);

                                rp_Pipelines.Add(pipeline);
                            }

                            break;

                        case GeoJSON.Net.GeoJSONObjectType.Point:

                            Point ps = feature.Geometry as Point;
                            Position pos = ps.Coordinates;

                            PointsCS.Add(new double[] { Math.Round(pos.Latitude, 2), Math.Round(pos.Longitude, 2) });

                            string kV = keyValue.Value.ToString();

                            if (kV == cWell) // колодцы
                            {
                                RP_Well well = new RP_Well();
                                well.Num = ++numWell;
                                if (prop.ContainsKey("ID"))
                                    well.G_ID = Convert.ToString(prop["ID"]);

                                well.WaterOutgo = 0;

                                if (prop.ContainsKey("ProjectElevation"))
                                    well.Geo_Z = Convert.ToDouble(prop["ProjectElevation"]);                                 

                                rp_Wells.Add(well);
                            }
                            else if (kV == cWS)
                            {
                                RP_Well ws = new RP_Well();
                                ws.Num = ++numSource;
                                if (prop.ContainsKey("ID"))
                                    ws.G_ID = Convert.ToString(prop["ID"]);

                                ws.WaterOutgo = 0;
                                if (prop.ContainsKey("WaterFlowNodal"))
                                    ws.WaterIn = Convert.ToDouble(prop["WaterFlowNodal"]);

                                if (prop.ContainsKey("ProjectElevation"))
                                    ws.Geo_Z = Convert.ToDouble(prop["ProjectElevation"]);

                                rp_Source.Add(ws);

                            }
                            else if (kV == cConsumer)
                            {
                                RP_Well cons = new RP_Well();
                                cons.Num = ++numCons;
                                if (prop.ContainsKey("ID"))
                                    cons.G_ID = Convert.ToString(prop["ID"]);
                                if (prop.ContainsKey("WaterFlowConsumer"))
                                    cons.WaterOutgo = Convert.ToDouble(prop["WaterFlowConsumer"]);
                                if (prop.ContainsKey("ProjectElevation"))
                                    cons.Geo_Z = Convert.ToDouble(prop["ProjectElevation"]);
                                cons.WaterIn = 0;
                                rp_Consumer.Add(cons);
                            }
                            break;
                    }
                }               
            }
            */



            #endregion
        }


        /// <summary>
        /// Go!
        /// </summary>
        public void Run(out ArrayList CalcParams)
        {
            // Источники
            List<RP_Well> rp_Source = new List<RP_Well>();
            // Потребители
            List<RP_Well> rp_Consumer = new List<RP_Well>();
            // Колодцы
            List<RP_Well> rp_Wells = new List<RP_Well>();
            //Трубопроводы
            List<RP_Pipeline>  rp_Pipelines = new List<RP_Pipeline>();
            
            List<RPEdge> rpEdgeSources = new List<RPEdge>();
            List<RPEdge> rpEdgeConsumer = new List<RPEdge>();

            // Сформировать массив из параметров
            CalcParams = new ArrayList();
            // Подача в узлы , л/c
            var WaterIn = new List<double>();
            // Отборы в узлах, л/c
            var WaterOutgo = new List<double>(); 
            // Геодезические отметки узлов
            var Geo_Z = new List<double>(); 
            // Длины участков
            var Geo_L = new List<double>();  
            // Материал труб
            var PipeMaterialsNum = new List<int>(); 
            // Диаметры труб
            var PipeDiametersNum = new List<int>(); 

            // Получить данные из чертежа
            InitDataPipeline(ref rp_Source, ref rp_Consumer, ref rp_Wells, ref rp_Pipelines);

            foreach (RP_Well Cons in rp_Consumer)
            {
                WaterOutgo.Add(Cons.WaterOutgo);
                Geo_Z.Add(Cons.Geo_Z);
            }
            
            foreach (RP_Well Source in rp_Source)
            {
                WaterIn.Add(Source.WaterIn);
                Geo_Z.Add(Source.Geo_Z);
            }
            CalcParams.Add(WaterIn);
            CalcParams.Add(WaterOutgo);
            var CountNodes = WaterOutgo.Count;
            CalcParams.Add(CountNodes);            
            CalcParams.Add(Geo_Z);
            
            foreach (var pipe in rp_Pipelines)
            {
                //Geo_L.Add(pipe.Length);
                PipeMaterialsNum.Add(3); //FindMaterialNum()                                 
                PipeDiametersNum.Add( FindDiameterNum(pipe.Diameter)); 
            }
            
            CalcParams.Add(Geo_L);            
            CalcParams.Add(PipeMaterialsNum);            
            CalcParams.Add(PipeDiametersNum);

            var H_min = 22; // напор, м 
            CalcParams.Add(H_min);
            var D_min = 100; //d, mm
            CalcParams.Add(D_min);            
            
            List<RPEdge> rpEdges = new List<RPEdge>(); // Требуется сформировать грани
            int edgeNum = 0;
            foreach (RP_Pipeline rp_pipeline in rp_Pipelines)
            {
                int pipelineNum = rp_pipeline.Num;
                RPNode nodeFirst,
                       nodeLast;

                RP_Well rp_well_First = DataWellFind_First(rp_pipeline.G_FirstPointId, ref rp_Wells);
                nodeFirst = new RPNode() { Num = rp_well_First.Num, G_ID = rp_well_First.G_ID, Visited = false };

                RP_Well rp_well_Last = DataWellFind_Last(rp_pipeline.G_LastPointId, ref rp_Wells);
                nodeLast = new RPNode() { Num = rp_well_Last.Num, G_ID = rp_well_Last.G_ID, Visited = false };

                if (nodeFirst.Num > 0 && nodeLast.Num > 0)
                {
                    RPEdge rp_edge = new RPEdge(nodeFirst, nodeLast);
                    rp_edge.Num = ++edgeNum;
                    rp_edge.G_ID = rp_pipeline.G_ID;
                    rp_edge.Length = rp_pipeline.Length;

                    Geo_L.Add(rp_pipeline.Length);

                    rpEdges.Add(rp_edge);
                }

                RP_Well rp_Source_First = DataWellFind_First(rp_pipeline.G_FirstPointId, ref rp_Source);
                RPNode nodeSource = new RPNode() { Num = rp_Source_First.Num, G_ID = rp_Source_First.G_ID, Visited = false };

                if (nodeSource.Num != 0)
                {
                    RPEdge rp_edge = new RPEdge(nodeSource, nodeLast);
                    rp_edge.Num = rpEdgeSources.Count+1;
                    rp_edge.G_ID = rp_pipeline.G_ID;

                    rpEdgeSources.Add(rp_edge);
                }

                RP_Well rp_Consumer_First = DataWellFind_First(rp_pipeline.G_FirstPointId, ref rp_Consumer);
                RPNode nodeConsumer = new RPNode() { Num = rp_Consumer_First.Num, G_ID = rp_Consumer_First.G_ID, Visited = false };

                if (nodeConsumer.Num != 0)
                {
                    RPEdge rp_edge = new RPEdge(nodeSource, nodeLast);
                    rp_edge.Num = rpEdgeConsumer.Count + 1;
                    rp_edge.G_ID = rp_pipeline.G_ID;

                    rpEdgeConsumer.Add(rp_edge);
                }                
            }
                       
            CountAreas = rpEdges.Count;

            RPRings = new List<List<RPEdge>>();

            FindRingsInPileline(ref rpEdges);

            //FindCyclesInPipeline();
                        
            CountRings = RPRings.Count; // число колец
            MaxCountAreas = 4;// Ring.Count;                       

            /***** OUT *****/
            CalcParams.Add(rpEdges);
            CalcParams.Add(RPRings);
        }

        // поиск циклов
        private void FindRingsInPileline(ref List<RPEdge> _RPEdges)
        {
            List<int> EdgesVisited = new List<int>() {3 };

            for (int iCR = 1; iCR <= CountRings; iCR++)
            {
                List<RPEdge> Ring = new List<RPEdge>();                
                RPEdge currentEdge = new RPEdge() { Num = 0 };
                if (iCR == 1)
                    currentEdge = _RPEdges.First<RPEdge>();
                else
                {
                    foreach (var e in _RPEdges)
                    {
                        currentEdge = e;
                        if (!EdgesVisited.Contains(currentEdge.Num))
                            break;
                    }
                }

                if (currentEdge.Num == 0)
                    break;

                int startEdgeNum = currentEdge.Num;

                currentEdge.Visited = true;
                EdgesVisited.Add(currentEdge.Num);

                Ring.Add(currentEdge);
                bool ringFound = false;
                int numEdgesInRing = 0;

                while (!ringFound && numEdgesInRing <= CountAreas)
                {
                    numEdgesInRing++;

                    RPEdge nextEdge = DataEdgeFind_First(currentEdge.ToPoint, ref _RPEdges);
                    
                    if (startEdgeNum == nextEdge.Num)
                    {
                        ringFound = true; // обошли цикл
                    }
                    else
                    {
                        currentEdge = nextEdge;

                        if (nextEdge.Num != 0)
                        {
                            EdgesVisited.Add(nextEdge.Num);
                            nextEdge.Visited = true;
                            Ring.Add(nextEdge);
                        }
                    }

                    if (ringFound)
                    {
                        RPRings.Add(Ring);                        
                        break;
                    }
                }                
            }
        }
                
    }

}
