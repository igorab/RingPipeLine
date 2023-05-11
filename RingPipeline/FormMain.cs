using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RingPipeLine
{
    public partial class FormMain : Form
    {
        Graphics g;
        public static byte flTools;
        bool drawing = false;

        /// <summary>
        /// Tools options
        /// </summary>
        enum FlTool
        {
            SelectNode = 0,
            AddNode = 1,
            AddEdge = 2,
            DeSelectEdge = 3,
            DelNode = 4
        }

        public FormMain()
        {
            InitializeComponent();

            g = CreateGraphics();

            saveFileDialog1.Filter = "Dat (*.dat)|*.dat|All Files (*.*)|*.*";
            openFileDialog1.Filter = "Dat (*.dat)|*.dat|All Files (*.*)|*.*";

            Lib.graph = new Graph();
            Lib.graph.bitmap = new Bitmap(ClientRectangle.Width, ClientRectangle.Height);
        }

        public void MyDraw()
        {
            Lib.graph.Draw();

            g.DrawImage(Lib.graph.bitmap, ClientRectangle);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //graph = new Graph();
                Lib.graph.FileName = openFileDialog1.FileName;
                Lib.graph.Read();

                Text = "Graph: [" + Lib.graph.FileName + "]";
                MyDraw();
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Lib.graph.FileName = saveFileDialog1.FileName;
                Lib.graph.Write();
            }

            saveFileDialog1.FileName = Lib.graph.FileName;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Lib.graph.FileName = saveFileDialog1.FileName;
                Lib.graph.SaveOld();
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Lib.graph.FileName = openFileDialog1.FileName;
                Lib.graph.ReadN();

                Text = "Graph: [" + Lib.graph.FileName + "]";

                MyDraw();
            }
        }

        private void toolStripMenuItem1_Click_1(object sender, EventArgs e)
        {
            if (Lib.graph.FileName != "")
            {
                Lib.graph.SaveOld();
            }
            else
            {
                saveFileDialog1.FileName = Lib.graph.FileName;

                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    Lib.graph.FileName = saveFileDialog1.FileName;
                    Lib.graph.SaveOld();
                }
            }
        }

        /// <summary>
        /// igorab Tools
        /// </summary>
        private void FormMain_MouseDown(object sender, MouseEventArgs e)
        {
            FlTool flTool = (FlTool)flTools;

            switch (flTool)
            {
                case FlTool.SelectNode:

                    Lib.graph.SelectNode = Lib.graph.FindNode(e.X, e.Y);
                    drawing = Lib.graph.SelectNode != null;
                    break;

                case FlTool.AddNode:
                    Lib.graph.AddNode(e.X, e.Y);
                    MyDraw();
                    break;

                case FlTool.AddEdge:
                    Lib.graph.SelectNodeBeg = Lib.graph.FindNode(e.X, e.Y);
                    drawing = Lib.graph.SelectNodeBeg != null;
                    Lib.graph.x1 = e.X; 
                    Lib.graph.y1 = e.Y;
                    Lib.graph.x2 = e.X; 
                    Lib.graph.y2 = e.Y;
                    break;

                case FlTool.DeSelectEdge:

                    Lib.graph.DeSelectEdge();
                    int NumLine = -1;
                    int NumNode = Lib.graph.FindLine(e.X, e.Y, out NumLine);
                    if (NumNode != -1)
                    {
                        Lib.graph.DelEdge(NumNode, NumLine);
                        MyDraw();
                    }
                    break;

                case FlTool.DelNode:

                    int nx = Lib.graph.Nodes[0].x;
                    break;
            }
        }

        private void FormMain_MouseMove(object sender, MouseEventArgs e)
        {
            if (drawing)
            {
                switch (flTools)
                {
                    case 0:
                        Lib.graph.SelectNode.x = e.X;
                        Lib.graph.SelectNode.y = e.Y;
                        MyDraw();
                        break;
                    case 2:
                        Lib.graph.x2 = e.X; Lib.graph.y2 = e.Y;
                        MyDraw();
                        break;
                }
            }
            else
            {
                switch (flTools)
                {
                    case 0:
                    case 2:
                        Lib.graph.SelectNode = Lib.graph.FindNode(e.X, e.Y);
                        MyDraw();
                        break;

                    case 3:
                        Lib.graph.DeSelectEdge();
                        int NumLine = -1;
                        int NumNode = Lib.graph.FindLine(e.X, e.Y, out NumLine);
                        if (NumNode != -1)
                        {
                            Lib.graph.Nodes[NumNode].Edge[NumLine].select = true;
                            MyDraw();
                        }
                        break;
                }
            }
        }

        private void FormMain_MouseUp(object sender, MouseEventArgs e)
        {
            drawing = false;
            switch (flTools)
            {
                case 2:

                    Lib.graph.SelectNode = Lib.graph.FindNode(e.X, e.Y);

                    if (Lib.graph.SelectNode != null)
                    {
                        Lib.graph.AddEdge();
                        MyDraw();
                    }
                    break;
            }
        }

        /// <summary>
        /// igorab
        /// </summary>
        private void toolsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormTools formTools;

            foreach (Form frm in Application.OpenForms)
            {
                if (frm is FormTools)
                {
                    frm.Activate();
                    return;
                }
            }

            formTools = new FormTools();
            formTools.Show();
        }

        /// <summary>
        /// igorab Циклы в графе
        /// </summary>
        private void cyclesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Lib.graph.GraphCycles();
        }
    }
}
