using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;


namespace CalcModule_RingPipeLine
{
    public class FeatureCollection
    {
        public List<string> Features;
        public Dictionary<string, string> Properties;
    }

    class CalcModule_RingPipeLine_Calc
    {
    }
    
    /// <summary>
    /// Начальное потокораспределение
    /// </summary>
    public class RPInitialFlowDistribution
    {
        public double[] Q_Flow { get; set; }

        public CalcModule_Hydro RingPipeline
        {
            get { return RPL; }
            set
            {
                RPL = value;
                CountAreas = value.CountAreas; CountNodes = value.CountNodes; CountRings = value.CountRings;
            }
        }

        private CalcModule_Hydro RPL;

        int CountAreas { get; set; }
        /// <summary>
        /// Количество узлов. Алгебраическая сумма расходов в каждом узле равна 0.
        /// </summary>
        public int CountNodes { get; set; }

        /// <summary>
        /// Количество колец 
        /// </summary>
        public int CountRings { get; set; }

        public RPInitialFlowDistribution()
        {
            RPL = new CalcModule_Hydro();
            CountAreas = 0;
            CountNodes = 0;
            CountRings = 0;

        }

        /// <summary>
        /// Kirchhoff's first law
        /// </summary>
        public void Circuite()
        {
            double consumption = 0;
            double[] Q = new double[CountNodes];

            for (int i = 0; i < CountNodes; i++)
            {
                consumption += Q[i];
            }
        }

        /// <summary>
        /// Kirchhoff's second law
        /// </summary>
        public void WaterSupply()
        {
            double[] S = new double[CountRings]; //гидравлическое сопротивление i-го участка
            double[] Q = new double[CountRings]; //расходы участков, входящих в j-е кольцо
            int beta = 2;
            double WaterLossSum = 0;

            for (int i = 0; i < CountRings; i++)
            {
                WaterLossSum += S[i] * Math.Pow(Q[i], beta);
            }
        }

        /// <summary>
        /// формирование системы линейных алгебраических уравнений
        /// </summary>
        public void SolutionLESystem()
        {
            double[,] A = new double[RPL.CountAreas, RPL.CountAreas];

            double[] Q = new double[RPL.CountAreas];

            double[] B = new double[RPL.CountAreas];

            int[,] Edges = new int[RPL.CountAreas, 2];

            int idxN = 0;
            foreach (RPEdge rpEdge in RPL.RPEdges)
            {
                Edges[idxN, 0] = rpEdge.FromPoint;
                Edges[idxN, 1] = rpEdge.ToPoint;
                idxN++;
            }

            // составляем матрицу А
            int iRow, jCol;
            //матрица уравнения баланса расходов в узлах
            for (int iN = 0; iN < CountNodes - 1; iN++) // идем сверху вниз по строкам
            {
                iRow = iN + 1;
                for (int jArea = 0; jArea < CountAreas; jArea++) // цикл по столбцам
                {
                    int FromNode = Edges[jArea, 0];
                    int ToNode = Edges[jArea, 1];

                    jCol = jArea + 1;

                    if (iRow == FromNode)
                        A[iN, jArea] = -1;
                    else if (iRow == ToNode)
                        A[iN, jArea] = 1;
                    else
                        A[iN, jArea] = 0;
                }
            }

            //матрица уравнения баланса потерь напора в кольцах
            int kR = CountNodes - 1;
            foreach (List<RPEdge> Ring in RPL.RPRings)
            {
                foreach (RPEdge rpEdge in Ring)
                {
                    int col = Math.Abs(rpEdge.Num) - 1;

                    if (rpEdge.DireсtSign > 0)
                    {
                        A[kR, col] = 1;
                    }
                    else if (rpEdge.DireсtSign < 0)
                    {
                        A[kR, col] = -1;
                    }
                }

                kR++;
            }

            //заполняем вектор правой части
            double[] wOutgo = RPL.WaterOutgo.ToArray<double>(); //расход
            //уравнение баланса потерь напора в кольцах
            for (int k = 0; k < CountNodes - 1; k++)
                B[k] = wOutgo[k];

            for (int k = CountNodes - 1; k < CountAreas; k++)
                B[k] = 0;

            int size = RPL.CountAreas;
            LinearEquationsSystem LS = new LinearEquationsSystem(size);
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                    LS.SetMatrixCell(i, j, A[j, i]);
            }

            for (int i = 0; i < size; i++)
                LS.SetFreeVector(i, B[i]);

            LS.Solve();

            for (int i = 0; i < size; i++)
            {
                Q[i] = LS.Solution(i);
            }

            Q_Flow = Q;
            //RPOptimize.RunCalc(Q, RPL);
        }

        /// <summary>
        /// GO!
        /// </summary>
        /// <param name="_featureCollection"></param>
        /// <returns>output</returns>
        public static FeatureCollection RunCalc(FeatureCollection _featureCollection, bool _printExcel = false)
        {
            RPInitialFlowDistribution flowDistribution = new RPInitialFlowDistribution();

            CalcModule_Hydro RPL = CalcModule_Hydro.construct();

            RPL.FeatureCollection = _featureCollection;
            RPL.PrintExcel = _printExcel;

            /********** RUN *********/
            ArrayList listRPLParams;
            RPL.Run(out listRPLParams);

            CalcModule_Hydro.InitTestData(ref RPL, ref listRPLParams);

            listRPLParams.Add(RPL.CountAreas);
            listRPLParams.Add(RPL.CountNodes);
            listRPLParams.Add(RPL.CountRings);
            listRPLParams.Add(RPL.RPEdges);
            listRPLParams.Add(RPL.Nodes);

            flowDistribution.RingPipeline = RPL;

            flowDistribution.SolutionLESystem();

            RPOptimize.RunCalc(flowDistribution.Q_Flow, flowDistribution.RingPipeline);

            return flowDistribution.RPL.FeatureCollection;
        }
    }

    internal class LinearEquationsSystem
    {
        private int size;

        public LinearEquationsSystem(int size)
        {
            this.size = size;
        }

        internal void SetFreeVector(int i, double v)
        {
            throw new NotImplementedException();
        }

        internal void SetMatrixCell(int i, int j, double v)
        {
            throw new NotImplementedException();
        }

        internal double Solution(int i)
        {
            throw new NotImplementedException();
        }

        internal void Solve()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// подпрограмма оптимизации
    /// расчет параметров сети
    /// увязка сети методом Лобачева-Кросса
    /// </summary>
    public partial class RPOptimize
    {
        /// <summary>
        /// рассчитанный расход на участках трассы 
        /// Начальное потокораспредление
        /// </summary>
        public double[] Q_Edges { get; set; }

        public double Epsilon = 1.0; // абсолютная погрешность увязки
        private int MaxIter = 25;

        /// <summary>
        /// Расчетная схема
        /// </summary>
        public CalcModule_Hydro RPL { get; set; }
        public List<RPReportFields> ReportOutput { get; set; }

        /*********** Результаты расчета **********/
        public List<double> Q_WaterCons; //расход воды л/сек                                                   
        public List<double> Vel_FlowRate { get; set; } // скорости воды по ребрам м/сек  
        public List<double> DeltaP_WPLosses { get; set; } // разности напоров по ребрам

        RPReportFields reportFields = new RPReportFields();

        public RPOptimize()
        {
            ReportOutput = new List<RPReportFields>();
            Q_WaterCons = new List<double>();

            Vel_FlowRate = new List<double>();
            DeltaP_WPLosses = new List<double>();
        }

        // _H_PrL_Corr потери напора на участке
        // _Vel скорости
        protected void CalcParameters(double[] _Q_Corr, ref double[] _H_PrL_Corr, ref double[] _Vel, ref int _Iter, double[] _dH_Corr, double[] _dQ_Corr)
        {
            // диаметры
            double[] D = new double[_Q_Corr.Length];
                        
            int i = 0;
            foreach (double q in _Q_Corr)
            {
                double q_m = q * 0.001;                

                D[i] = Diameter(i); //диаметр в [мм]

                double d_m = D[i] / 1000;
                _Vel[i] = 4 * q_m / (Math.PI * Math.Pow(d_m, 2));

                double L_edge = RPL.Geo_L[i];

                double i_hydr = 0; // гидравлический уклон
                double K_hydr = Koef("K");
                double p_hydr = Koef("P");
                double n_hydr = 2; //  Koef("n");
                i_hydr = K_hydr * Math.Pow(q_m, n_hydr) / Math.Pow(d_m, p_hydr);

                _H_PrL_Corr[i] = i_hydr * L_edge;
               
                reportFields.Iter = _Iter;
                reportFields.SectionId = i + 1;
                reportFields.Direction = RPL.RPEdges[i].Direction;
                reportFields.Diameter = D[i];
                reportFields.PressureLoss = _H_PrL_Corr[i];
                reportFields.WaterVelocity = _Vel[i];
                reportFields.WaterСonsumption = q;
                reportFields.dH = _dH_Corr[i];
                reportFields.dQ = _dQ_Corr[i];

                ReportOutput.Add(reportFields);

                i++;
            }            
        }
                                
        /// <summary>
        /// Расчет параметров и увязка сети
        /// </summary>
        public void CalcParametersAndLinkage(int _Iter = 0)
        {
            // диаметры
            double[] D = new double[Q_Edges.Length];
            // скорости
            double[] Vel = new double[Q_Edges.Length];
            // потери напора на участке
            double[] H_PrL = new double[Q_Edges.Length];
            
            int i = 0;
            foreach (double q in Q_Edges)
            {
                double q_m = q * 0.001;
                
                D[i] = Diameter(i); //диаметр в [мм]

                double d_m = D[i] / 1000;
                Vel[i] = 4 * q_m / (Math.PI * Math.Pow(d_m, 2));

                double L_edge = RPL.Geo_L[i];

                double i_hydr = 0; // гидравлический уклон
                double K_hydr = Koef("K");
                double p_hydr = Koef("P");
                double n_hydr = 2; //  Koef("n");
                i_hydr = K_hydr * Math.Pow(q_m, n_hydr) / Math.Pow(d_m, p_hydr);

                H_PrL[i] = i_hydr * L_edge;

                reportFields.Iter = _Iter;
                reportFields.SectionId = i + 1;
                reportFields.Direction = RPL.RPEdges[i].Direction;
                reportFields.Diameter = D[i];
                reportFields.PressureLoss = H_PrL[i];
                reportFields.WaterVelocity = Vel[i];
                reportFields.WaterСonsumption = q;
                reportFields.dH = 0;
                reportFields.dQ = 0;
                ReportOutput.Add(reportFields);

                i++;
            }

            //увязка сети
            // повторять пока dh не будет меньше заданной величины
            // коррекция на кольцах
            double[] dH = new double[RPL.RPRings.Count]; 
            double[] dQ = new double[RPL.RPRings.Count];

            // коррекция потоков на ребрах
            double[] Q_E_Corr = Q_Edges; //инициализируем начальными потоками
            double[] dQ_E_Corr = new double[Q_Edges.Length];
            double[] H_PrL_Corr = H_PrL;
            double[] dH_E_Corr = new double[Q_Edges.Length];
            double[] Vel_Corr = Vel; 

            while (_Iter <= MaxIter)
            {
                // вычисляем величну, на которую требуется скорректировать
                CalcDH_Correction(H_PrL, Vel, D, ref dH, ref dQ);
                
                // критерий завершения расчета (достижения требуемой точности)
                if (Math.Abs(dH.Max<double>()) <= Epsilon && Math.Abs(dH.Min<double>()) <= Epsilon)
                    break;

                //double[] sum_dH; // сумма потерь напора по кольцу
                //double[] sum_dQ; // поправочный расход

                //корректируем
                Linkage(dH, dQ, ref Q_E_Corr, ref dQ_E_Corr, ref dH_E_Corr);

                //пересчитываем с учетом коррекции
                _Iter++;  // Номер итерации
                CalcParameters(Q_E_Corr, ref H_PrL_Corr, ref Vel_Corr, ref _Iter, dH_E_Corr, dQ_E_Corr);

                // повторяем цикл
                Q_WaterCons = new List<double>(Q_E_Corr); 
                Vel_FlowRate = new List<double>(Vel_Corr);
                DeltaP_WPLosses = new List<double>(H_PrL_Corr);
            }
        }

        /// <summary>
        /// Коррекция расхода на участке
        /// </summary>
        /// <param name="_dH_Sum_Ring">Невязка порь напора на кольце</param>
        /// <param name="_dQ">Коррекция на кольце</param>
        /// <param name="_Q_Corr">Возвращаем расход исправленный</param>
        public void Linkage( double[] _dH_Sum_Ring, double[] _dQ, ref double[] _Q_Corr, ref double[] _dQ_Corr, ref double[] _dH_Corr)
        {                        
            for (int kR = 0; kR < RPL.RPRings.Count; kR++)
            {               
                double dq = _dQ[kR];
                double dh = _dH_Sum_Ring[kR];

                List<RPEdge> rp_ring = RPL.RPRings[kR];

                foreach (RPEdge rp_edge in rp_ring)
                {
                    if (rp_edge.Num > 0)
                    {
                        _dQ_Corr[rp_edge.Num - 1] = dq;
                        _dH_Corr[rp_edge.Num - 1] = dh;

                        // edge.DireсtSign Учесть!
                        _Q_Corr[rp_edge.Num - 1] -= dq; // коррекция расхода на участке
                    }
                }
            }            
        }

        /// <summary>
        /// Перераспределение балансовых расходов
        /// Вычисляются значение потерь напора на каждом участке
        /// увязка сети методом Лобачева-Кросса
        /// </summary>
        public void CalcDH_Correction(double[] H_PrL, double[] _V, double[] _D, ref double[] dH, ref double[] dQ)
        {            
            double[] dH_loss = new double[RPL.RPRings.Count]; // потери напора на кольце

            int k = 0;
            // обходим каждое кольцо
            foreach (List<RPEdge> rp_ring in RPL.RPRings)
            {
                double Sum_R = 0;
                double S_Q_2 = 0;
                double delta_H_k = 0; // Невязка потерь напора на кольце
                double delta_Q_k = 0;  // Поправка невязки k-го кольца             

                foreach (RPEdge rp_edge in rp_ring)
                {
                    int edgeNum = rp_edge.Num;

                    if (edgeNum == 0)
                        continue;

                    int idx = Math.Abs(edgeNum) - 1;

                    Sum_R += Math.Sign(rp_edge.DireсtSign) * H_PrL[idx];

                    double q_m = Q_Edges[idx] * 0.001;

                    double d_m = _D[idx] * 0.001;

                    // удельное сопротивление на участке
                    double S_j_0 = S0_Steel_N(d_m, _V[idx]);

                    double S_j = Math.Sign(rp_edge.DireсtSign) * S_j_0 * RPL.Geo_L[idx];

                    // Невязка потерь напора на кольце
                    delta_H_k += S_j * Math.Pow(q_m, 2);

                    S_Q_2 += 2 * S_j * q_m;
                }

                dH_loss[k] = Sum_R;

                delta_Q_k = delta_H_k / S_Q_2;

                dH[k] = delta_H_k;
                dQ[k] = delta_Q_k;

                k++;
            }
        }

        public void PrintReport() // перенести в другое место!!
        {
            RPPrintExcel printExcel = new RPPrintExcel();
            printExcel.ReportOutput = ReportOutput;

            printExcel.ShowData();
        }

        /// <summary>
        /// RUN!
        /// </summary>
        /// <param name="_Q_WLoss">Начальное потокораспределение</param>
        /// <param name="_RPL"></param>
        /// <returns></returns>
        public static RPOptimize RunCalc(double[] _Q_WLoss, CalcModule_Hydro _RPL)
        {
            RPOptimize rpOptimize = new RPOptimize();

            rpOptimize.Q_Edges = _Q_WLoss;
            rpOptimize.RPL = _RPL;
            
            rpOptimize.CalcParametersAndLinkage();
            
            foreach (RPEdge rpEdge in _RPL.RPEdges)
            {
                if (rpEdge.FeatureNum < _RPL.FeatureCollection.Features.Count)
                {
                    var feature = _RPL.FeatureCollection.Features[rpEdge.FeatureNum];

                    var properties = _RPL.FeatureCollection.Properties;

                    /*
                    // скорость, м/c
                    if (properties.ContainsKey("FlowRate"))
                    {
                        properties["FlowRate"] = rpOptimize.Vel_FlowRate[rpEdge.Num - 1];
                    }

                    // расход воды, л/с
                    if (properties.ContainsKey("WaterFlow"))
                    {
                        properties["WaterFlow"] = rpOptimize.Q_Edges[rpEdge.Num - 1];
                    }

                    // потери напора по длине, м
                    if (properties.ContainsKey("WaterPressureLosses"))
                    {
                        properties["WaterPressureLosses"] = rpOptimize.DeltaP_WPLosses[rpEdge.Num - 1];
                    }
                    */
                }
            }

            if (_RPL.PrintExcel)
            {
                rpOptimize.PrintReport();
            }

            return rpOptimize;
        }

    }

    public partial class RPOptimize
    {
        /// <summary>
        /// Расчет удельного сопротивления
        /// </summary>
        /// <returns>Трубы стальные новые</returns>                
        public static double S0_Steel_N(double _D, double _V)
        {
            double S0 = 0;

            if (_V != 0)
                S0 = (0.001314 / Math.Pow(_D, 5.226)) * Math.Pow((1 + 0.84 / _V), 0.226);

            return S0;
        }

        /// <summary>
        /// Коэффициент сопротивления трения по длине
        /// </summary>
        /// <param name="_D"></param>
        /// <param name="_V"></param>
        /// <returns></returns>
        public static double Lamda_N(double _D, double _V)
        {
            double Lambda = 0;

            double A1 = 0.001314;
            double C = 0.684;
            double m_1 = 5.226;
            double m_2 = 0.226;

            if (_V != 0)
                Lambda = (A1 / Math.Pow(_D, m_1)) * Math.Pow((1 + C / _V), m_2);

            return Lambda;
        }

        /// <summary>
        /// Коэффициенты для расчета предельных расходов
        /// </summary>
        /// <returns></returns>
        public double Koef(string _koef)
        {
            double Koef_value = 0;

            Dictionary<string, double> Koef_steel = new Dictionary<string, double>()
            {
                {"n", 1.9},
                {"K", 0.001790},
                {"P", 5.1},
                {"alfa", 1.4},
                {"R", 4.6},
                {"b", 53},
            };

            Koef_steel.TryGetValue(_koef, out Koef_value);

            return Koef_value;
        }

        /// <summary>
        /// Диаметр трубы
        /// </summary>
        /// <returns>Выбираем из сортамента</returns>
        public double Diameter(int _DNum)
        {
            double diameter = 0;

            var D_Nom = RPL.D_Nom;

            int idxD = RPL.PipeDiameterNum[_DNum];

            diameter = D_Nom[idxD];

            // диаметр расчетный
            //List<double> D_Calc = new List<double>() { };

            return diameter;
        }
    }


    /// <summary>
    /// расчет режима водопроводной сети 
    /// </summary>
    public class RPOptimize_WS /*: RPOptimize*/
    {
        /// <summary>
        /// расчет режима водопроводной сети
        /// </summary>
        public void CalculationMode()
        {

        }

        /// <summary>
        /// связывает скорость по трубе с объемным расходом воды
        /// </summary>
        /// <returns></returns>
        public double V2Q(double _V, double _D, double _loc)
        {
            double Q_res;

            Q_res = _V * _loc * Math.PI * Math.Pow(_D, 2) / 4;

            return Q_res;
        }

        // Зона шероховатых труб
        public double Lambda_Altschul(double _D, double Re)
        {
            double lambda = 0;
            double delta = 1;

            lambda = 0.11 * Math.Pow(68 / Re + delta / _D, 0.25);

            return lambda;
        }

        //Потери напора по длине для труб постоянного диаметра определяют­ся по формуле Дарси-Вейсбаха
        public double Darcie(double _L, double _D)
        {
            double lambda = 1; // коэффициент потерь на трение по длине (коэффициент Дарси)

            double Re = 3000;
            // считаем как для зона шероховатых труб , есть различные другие варианты (например зона гидравлически гладких труб)
            lambda = Lambda_Altschul(_D, Re);

            double dH, dzeta, V = 1, g = 9.8;

            dzeta = lambda * _L / _D;

            dH = dzeta * V * V / (2 * g);

            return dH;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_D"></param>
        /// <returns></returns>
        public double delta_Q(double _D = 0)
        {
            double alpha = 0.312;
            double beta = 0.226;
            double gamma = 1.9 * Math.Pow(1, -6);
            double nu = 1.3 * Math.Pow(1, -6); //кинематический коэффициент вязкости воды
            double delta = nu;
            double g = 9.81;
            double ro = 1000;

            double deltaQ;

            double dh_dq; // производная dh по dq
            double Q = 0;
            double C1,
                   C2;
            double V = 1.0; // скорость воды

            //lambda - коэффициент сопротивления трения по длине
            double lambda = (alpha / Math.Pow(_D, beta)) * Math.Pow((gamma + nu / V), beta);

            C1 = (8 * alpha) / (g * Math.PI * Math.PI * ro * ro * Math.Pow(_D, 5 + beta));

            C2 = delta * Math.PI * _D * _D * ro / 4;

            double h_i = C1 * Math.Pow(gamma * Math.Pow(Q, 2 / beta) + C2 * Math.Pow(Q, 2 / beta - 1), beta);


            double C3, C4;
            C3 = 2 * gamma / beta;
            C4 = C2 * (2 / beta - 1);
            dh_dq = C1 * (gamma * Math.Pow(Q, 2 / beta) + C2 * Math.Pow(Q, 2 / beta - 1)) * (C3 * Math.Pow(Q, 2 / beta - 1) + C4 * Math.Pow(Q, 2 / beta - 2));

            deltaQ = h_i / dh_dq;

            return deltaQ;
        }
    }

    /// <summary>
    /// Расчет свободных напоров
    /// </summary>
    public class RPCalcH
    {
        const double rho = 1000;

        /// <summary>
        /// key- ребро Value - dP
        /// </summary>
        Dictionary<int, double> TableResult;

        public double Friction_Factor(double _Re, double _eD)
        {
            return _Re * _eD;
        }

        //Вычисление коэффициента сопротивления
        public double K_from_f(double _lambda, double _length, double _diameter)
        {
            return _lambda * _length * _diameter;
        }

        //Вычисление падения давления из K,  𝜌  и скорости
        private double dP_from_K(double k, double rho, double v)
        {
            throw new NotImplementedException();
        }

        //Вычисление падения напора из того же самого, но в метрах воды
        private double head_from_K(double k, double v)
        {
            throw new NotImplementedException();
        }

        private double Reynolds(double _diameter, double _V, double _nju)
        {
            throw new NotImplementedException();
        }

        //Падения давлений по ребрам
        public void SoutionResult(double _length, double _diameter)
        {
            double lambda = Friction_Factor(1, 1);

            double K = K_from_f(lambda, _length, _diameter);


            double dP = dP_from_K(K, rho, 0.638) / 100000;

            dP = head_from_K(K, 0.638); // head_from_K returns dP in m

            double V = 1.1;
            double nju = 1;
            double Re = Reynolds(_diameter, V, nju);

            //Расчет местных сопротивлений. Формула Альшуля
        }
    }
}
