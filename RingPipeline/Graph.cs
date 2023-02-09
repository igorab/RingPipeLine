using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

namespace RingPipeLine
{

    public class Lib
    {
        public static Graph graph;

        public static CalcModule_QGraph qGraph()
        {
            return new CalcModule_QGraph();
        }
    }

    /// <summary>
    /// igorab тип элементов
    /// </summary>
    public enum Typ_Graph
    {
        Fill_Ellipse = 0,
        Fill_Rectangle = 1,
    }


    // ============================= TEdge ============================
    public struct TEdge
    {
        public int A;            //
        public int numNode;      //
        public int x1c, x2c, yc; //
        public Color color;
        public bool select;
    }

    // ============================= TNode ============================
    public class TNode
    {
        public string name;  // 4+4*Name.Length
        public TEdge[] Edge; // 4+L2*(5*4) - ребра
        public bool visit;
        public int x, y;   // 4+4
        public int numVisit;
        public Color color;
        public int dist, oldDist;
    }

    /// <summary>
    /// Граф
    /// </summary>
    public class Graph
    {
        public string FileName = "";
        public TNode[] Nodes = new TNode[0];// узлы
        public Bitmap bitmap;
        public byte typ_graph = 0;
        public bool visibleA = false;
        public TNode SelectNode = null;
        public TNode SelectNodeBeg = null;
        int hx = 50;
        int hy = 10;
        public int x1;
        public int x2;
        public int y1;
        public int y2;

        int ofs;
        byte[] byData;

        /// <summary>
        /// Отрисовка графа
        /// </summary>
        public void Draw()
        {
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                Color cl = Color.FromArgb(255, 255, 255);
                g.Clear(cl);

                SolidBrush MyBrush = (SolidBrush)Brushes.White;
                Font MyFont = new Font("Courier New", 10, FontStyle.Bold);
                Pen MyPen = Pens.Black;

                string s;
                int N = Nodes.Length;

                for (int i = 0; i < N; i++)
                {
                    if (Nodes[i].Edge != null)
                    {
                        int L = Nodes[i].Edge.Length;
                        MyBrush.Color = Color.White;
                        
                        for (int j = 0; j < L; j++)
                        {
                            switch (typ_graph)
                            {
                                case (byte)Typ_Graph.Fill_Ellipse:

                                    if (Nodes[i].Edge[j].select)
                                        MyPen = Pens.Red;
                                    else
                                        MyPen = new Pen(Nodes[i].Edge[j].color); // Pens.Black;

                                    int a1 = Nodes[i].x;
                                    int b1 = Nodes[i].y;
                                    int a2 = Nodes[Nodes[i].Edge[j].numNode].x;
                                    int b2 = Nodes[Nodes[i].Edge[j].numNode].y;
                                    g.DrawLine(MyPen, new Point(a1, b1), new Point(a2, b2));

                                    s = Convert.ToString(Nodes[i].Edge[j].A);
                                    SizeF size = g.MeasureString(s, MyFont);
                                    
                                    if (visibleA)
                                    {
                                        g.FillRectangle(Brushes.White, (a1 + a2) / 2 - size.Width / 2, (b1 + b2) / 2 - size.Height / 2, size.Width, size.Height);

                                        g.DrawString(s, MyFont, Brushes.Black,
                                            (a1 + a2) / 2 - size.Width / 2,
                                            (b1 + b2) / 2 - size.Height / 2);
                                    }
                                    break;

                                case (byte)Typ_Graph.Fill_Rectangle:

                                    a1 = Nodes[i].x + Nodes[i].Edge[j].x1c;
                                    b1 = Nodes[i].y;
                                    a2 = Nodes[Nodes[i].Edge[j].numNode].x + Nodes[i].Edge[j].x2c;
                                    b2 = Nodes[Nodes[i].Edge[j].numNode].y;

                                    if (Nodes[i].Edge[j].select)
                                        MyPen = Pens.Red;
                                    else
                                        MyPen = new Pen(Nodes[i].Edge[j].color); // Pens.Black;

                                    g.DrawLine(MyPen, new Point(a1, b1 + hy), new Point(a1, (b1 + b2) / 2));
                                    g.DrawLine(MyPen, new Point(a1, (b1 + b2) / 2), new Point(a2, (b1 + b2) / 2));
                                    g.DrawLine(MyPen, new Point(a2, (b1 + b2) / 2), new Point(a2, b2 - hy));

                                    s = Convert.ToString(Nodes[i].Edge[j].A);
                                    size = g.MeasureString(s, MyFont);

                                    if (visibleA)
                                    {
                                        g.FillRectangle(Brushes.White, (a1 + a2) / 2 - size.Width / 2, (b1 + b2) / 2 - size.Height / 2, size.Width, size.Height);

                                        g.DrawString(s, MyFont, Brushes.Black,
                                            (a1 + a2) / 2 - size.Width / 2,
                                            (b1 + b2) / 2 - size.Height / 2);
                                    }

                                    if (b1 < b2)
                                        g.FillEllipse(Brushes.Black, a2 - 3, b2 - hy - 3 - 3, 6, 6);
                                    else
                                        g.FillEllipse(Brushes.Black, a2 - 3, b2 + hy - 3 + 3, 6, 6);

                                    break;
                            }
                        }
                    }
                }

                for (int i = 0; i < N; i++)
                {
                    if (Nodes[i] == SelectNode)
                        MyPen = Pens.Red;
                    else
                        MyPen = Pens.Silver;

                    if (Nodes[i].visit)
                    {
                        MyBrush.Color = Color.Silver;
                    }
                    else
                    {
                        if (Nodes[i] == SelectNode)
                            MyBrush.Color = Color.Yellow;
                        else
                            MyBrush.Color = Color.LightYellow;
                    }

                    switch (typ_graph)
                    {
                        case (byte)Typ_Graph.Fill_Ellipse:

                            MyBrush.Color = Nodes[i].color;
                            g.FillEllipse(MyBrush, Nodes[i].x - hy, Nodes[i].y - hy, 2 * hy, 2 * hy);
                            
                            g.DrawEllipse(Pens.Black, Nodes[i].x - hy, Nodes[i].y - hy, 2 * hy, 2 * hy);

                            s = Convert.ToString(i);

                            SizeF size = g.MeasureString(s, MyFont);

                            g.DrawString(s, MyFont, Brushes.Black,
                                Nodes[i].x - size.Width / 2,
                                Nodes[i].y - size.Height / 2);
                            break;

                        case (byte)Typ_Graph.Fill_Rectangle:

                            g.FillRectangle(MyBrush, Nodes[i].x - hx, Nodes[i].y - hy, 2 * hx, 2 * hy);

                            g.DrawRectangle(MyPen, Nodes[i].x - hx, Nodes[i].y - hy, 2 * hx, 2 * hy);

                            s = Convert.ToString(Nodes[i].name) + "(" + Convert.ToString(Nodes[i].numVisit) + ")";

                            size = g.MeasureString(s, MyFont);

                            g.DrawString(s, MyFont, Brushes.Black,
                                Nodes[i].x - size.Width / 2,
                                Nodes[i].y - size.Height / 2);
                            break;
                    }
                }
            }
        }

        protected int DataInInt()
        {
            int result = BitConverter.ToInt32(byData, ofs);
            ofs += 4;
            return result;
        }

        protected string DataInStr()
        {
            byte[] byByte;

            int L = DataInInt(); 

            byByte = new byte[4 * L];

            for (int j = 0; j <= 4 * L - 1; j++)
                byByte[j] = byData[j + ofs];

            char[] charData = new char[L];

            Decoder d = Encoding.UTF32.GetDecoder();

            d.GetChars(byByte, 0, byByte.Length, charData, 0);

            string s = "";
            for (int j = 0; j < charData.Length; j++)
                s += charData[j];

            ofs += 4 * L;

            return s;
        }

        protected void IntInData(int k)
        {
            byte[] byByte;
            byByte = BitConverter.GetBytes(k);
            byByte.CopyTo(byData, ofs); 
            ofs += 4;
        }

        protected void StrInData(string s)
        {
            byte[] byByte;
            int L = s.Length; 

            IntInData(L);

            char[] charData = s.ToCharArray();

            byByte = new byte[4 * charData.Length];

            Encoder e = Encoding.UTF32.GetEncoder();

            e.GetBytes(charData, 0, charData.Length, byByte, 0, true);

            byByte.CopyTo(byData, ofs); 
            ofs += 4 * L;
        }

        /// <summary>
        /// вычислить размер файла
        /// </summary>
        /// <returns></returns>
        protected int LengthFile()  
        {
            int n = 4;
            int L1 = Nodes.Length;

            for (int i = 0; i <= L1 - 1; i++)
            {
                n += 16 + 4 * Nodes[i].name.Length;

                int L2 = 0;

                if (Nodes[i].Edge != null)
                {
                    L2 = Nodes[i].Edge.Length;
                }

                n += L2 * 20;
            }

            return n;
        }

        /// <summary>
        /// Прочитать файл
        /// </summary>
        public void Read()
        {
            ofs = 0;
            FileStream aFile = new FileStream(FileName, FileMode.Open);
            int N = (int)aFile.Length;
            byData = new byte[N];

            aFile.Read(byData, 0, N);

            int L1 = DataInInt();

            Nodes = new TNode[L1];

            for (int i = 0; i <= L1 - 1; i++)
            {
                Nodes[i] = new TNode();
                Nodes[i].x = DataInInt();
                Nodes[i].y = DataInInt();
                Nodes[i].name = DataInStr();

                int L2 = DataInInt();
                Nodes[i].Edge = new TEdge[L2];

                if (L2 != 0)
                {
                    for (int j = 0; j <= L2 - 1; j++)
                    {
                        Nodes[i].Edge[j].A = DataInInt();
                        Nodes[i].Edge[j].x1c = DataInInt();
                        Nodes[i].Edge[j].x2c = DataInInt();
                        Nodes[i].Edge[j].yc = DataInInt();
                        Nodes[i].Edge[j].numNode = DataInInt();
                        Nodes[i].Edge[j].color = Color.Silver;
                    }
                }
            }

            aFile.Close();
        }

        public void SaveOld()
        {
            ofs = 0;

            FileStream aFile = new FileStream(FileName, FileMode.Create);
            int N = LengthFile();
            byData = new byte[N];

            int L1 = Nodes.Length;
            IntInData(L1);

            for (int i = 0; i <= L1 - 1; i++)
            {
                IntInData(Nodes[i].x);
                IntInData(Nodes[i].y);
                StrInData(Nodes[i].name);

                int L2 = 0;
                if (Nodes[i].Edge != null)
                    L2 = Nodes[i].Edge.Length;
                IntInData(L2);
                for (int j = 0; j <= L2 - 1; j++)
                {
                    IntInData(Nodes[i].Edge[j].A);
                    IntInData(Nodes[i].Edge[j].x1c);
                    IntInData(Nodes[i].Edge[j].x2c);
                    IntInData(Nodes[i].Edge[j].yc);
                    IntInData(Nodes[i].Edge[j].numNode);
                }
            }
            aFile.Write(byData, 0, N);
            aFile.Close();
        }


        /// <summary>
        /// Циклы в графе
        /// </summary>
        public void GraphCycles()
        {
            Lib.qGraph().CreateGraph();

            ofs = 0;

            int N = LengthFile();

            byData = new byte[N];

            int L1 = Nodes.Length;

            IntInData(L1);

            for (int i = 0; i <= L1 - 1; i++)
            {
                IntInData(Nodes[i].x);
                IntInData(Nodes[i].y);
                StrInData(Nodes[i].name);

                int L2 = 0;
                if (Nodes[i].Edge != null)
                {
                    L2 = Nodes[i].Edge.Length;
                }

                IntInData(L2);

                for (int j = 0; j <= L2 - 1; j++)
                {
                    IntInData(Nodes[i].Edge[j].A);
                    IntInData(Nodes[i].Edge[j].x1c);
                    IntInData(Nodes[i].Edge[j].x2c);
                    IntInData(Nodes[i].Edge[j].yc);
                    IntInData(Nodes[i].Edge[j].numNode);
                }
            }
        }


        /// <summary>
        /// найти узел
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>Узел либо Null</returns>
        public TNode FindNode(int x, int y) 
        {
            int N = Nodes.Length;
            int i = -1;
            bool Ok = false;

            while ((i < N - 1) && !Ok)
            {
                i++;
                Ok = (Nodes[i].x - hx <= x) && (x <= Nodes[i].x + hx) &&
                     (Nodes[i].y - hy <= y) && (y <= Nodes[i].y + hy);
            }

            if (Ok) 
                return Nodes[i]; 
            else 
                return null;
        }

        /// <summary>
        /// Удалить ребро
        /// </summary>
        public void DeSelectEdge()
        {
            int N = Nodes.Length;

            for (int i = 0; i < N; i++)
            {
                if (Nodes[i].Edge != null)
                {
                    int L = Nodes[i].Edge.Length;

                    for (int j = 0; j < L; j++)
                        Nodes[i].Edge[j].select = false;
                }
            }
        }

        /// <summary>
        /// // добавить узел 
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        public void AddNode(int x, int y) 
        {
            int N = Nodes.Length;
            Array.Resize<TNode>(ref Nodes, ++N);

            Nodes[N - 1] = new TNode();
            Nodes[N - 1].name = "Node " + Convert.ToString(N - 1);
            Nodes[N - 1].x = x;
            Nodes[N - 1].y = y;
            Nodes[N - 1].color = Color.White;
        }

        /// <summary>
        /// добавить ребро
        /// </summary>
        public void AddEdge()
        {
            int n = -1; 
            bool ok = false;
            int Ln = Nodes.Length;

            while ((n < Ln - 1) && !ok)
            {
                ok = Nodes[++n] == SelectNode;
            }

            int L = 0;

            if (SelectNodeBeg.Edge != null)
            {
                L = SelectNodeBeg.Edge.Length;
            }

            Array.Resize<TEdge>(ref SelectNodeBeg.Edge, ++L);
            SelectNodeBeg.Edge[L - 1].numNode = n;
            SelectNodeBeg.Edge[L - 1].A = 0;
            SelectNodeBeg.Edge[L - 1].x1c = x1 - SelectNodeBeg.x;
            SelectNodeBeg.Edge[L - 1].x2c = x2 - SelectNode.x;
            SelectNodeBeg.Edge[L - 1].yc = (SelectNode.y + SelectNodeBeg.y) / 2;
            SelectNodeBeg.Edge[L - 1].color = Color.Silver;
        }

        /// <summary>
        /// расстояние до линии
        /// </summary>
        /// <returns></returns>
        public int DistLine(int u, int v, int x1, int y1, int x2, int y2)  
        {

            int A = y2 - y1;
            int B = -x2 + x1;
            int C = -x1 * A - y1 * B;
            int D = A * A + B * B;

            if (D != 0)
                return (int)(Math.Abs(A * u + B * v + C) / Math.Sqrt(D));
            else
                return 0;
        }

        /// <summary>
        ///  найти ребро
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="NumLine"></param>
        /// <returns></returns>
        public int FindLine(int x, int y, out int NumLine)  
        {
            int L = Nodes.Length;
            bool ok = false; 
            int i = -1; 
            NumLine = -1; 
            int j = -1;
            while ((i < L - 1) && !ok)
            {
                i++;
                if (Nodes[i].Edge != null)
                {
                    int L1 = Nodes[i].Edge.Length; j = -1;
                    while ((j < L1 - 1) && !ok)
                    {
                        j++;
                        int a1 = Nodes[i].x;
                        int b1 = Nodes[i].y;
                        int a2 = Nodes[Nodes[i].Edge[j].numNode].x;
                        int b2 = Nodes[Nodes[i].Edge[j].numNode].y;
                        int u1 = Math.Min(a1, a2);
                        int u2 = Math.Max(a1, a2);
                        int v1 = Math.Min(b1, b2);
                        int v2 = Math.Max(b1, b2);
                        int Eps = 4;
                        ok = (u1 - Eps <= x) && (x <= u2 + Eps) && (v1 - Eps <= y) && (y <= v2 + Eps);
                        ok = (DistLine(x, y, a1, b1, a2, b2) <= Eps) && ok;
                    }
                }
            }
            if (ok)
            {
                NumLine = j;
                return i;
            }
            else
                return -1;
        }

        /// <summary>
        /// удалить ребро
        /// </summary>
        /// <param name="NumNode"></param>
        /// <param name="NumEdge"></param>
        public void DelEdge(int NumNode, int NumEdge)  
        {
            int L = Nodes[NumNode].Edge.Length;

            for (int i = NumEdge; i < L - 2; i++)
            {
                Nodes[NumNode].Edge[i] = Nodes[NumNode].Edge[i + 1];
            }

            Array.Resize<TEdge>(ref Nodes[NumNode].Edge, L - 1);
        }

        #region New
        public void ReadN()
        {
            FileStream aFile = new FileStream(FileName, FileMode.Open);
            BinaryReader f = new BinaryReader(aFile, Encoding.UTF7);

            int L1 = f.ReadInt32();
            Nodes = new TNode[L1];

            for (int i = 0; i < L1; i++)
            {
                Nodes[i] = new TNode();
                Nodes[i].x = f.ReadInt32();
                Nodes[i].y = f.ReadInt32();
                Nodes[i].name = f.ReadString();

                int L2 = f.ReadInt32();

                Nodes[i].Edge = new TEdge[L2];

                for (int j = 0; j < L2; j++)
                {
                    Nodes[i].Edge[j] = new TEdge();
                    Nodes[i].Edge[j].x1c = f.ReadInt32();
                    Nodes[i].Edge[j].x2c = f.ReadInt32();
                    Nodes[i].Edge[j].yc = f.ReadInt32();
                    Nodes[i].Edge[j].numNode = f.ReadInt32();
                }
            }

            f.Close();
            aFile.Close();
        }


        /// <summary>
        /// Записать в файл
        /// </summary>
        public void Write()
        {
            FileStream aFile = new FileStream(FileName, FileMode.Create);

            BinaryWriter f = new BinaryWriter(aFile, Encoding.UTF7);

            int L1 = Nodes.Length;

            f.Write(L1);

            for (int i = 0; i < L1; i++)
            {
                f.Write(Nodes[i].x);
                f.Write(Nodes[i].y);
                f.Write(Nodes[i].name);

                int L2 = Nodes[i].Edge.Length;
                f.Write(L2);

                for (int j = 0; j < L2; j++)
                {
                    f.Write(Nodes[i].Edge[j].A);
                    f.Write(Nodes[i].Edge[j].x1c);
                    f.Write(Nodes[i].Edge[j].x2c);
                    f.Write(Nodes[i].Edge[j].yc);
                    f.Write(Nodes[i].Edge[j].numNode);
                }
            }

            f.Close();
            aFile.Close();
        }
        #endregion
    }
}
