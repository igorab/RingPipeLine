using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using QuickGraph;
using QuikGraph.Algorithms;
using QuikGraph.Algorithms.ShortestPath;

using QuikGraph.Collections;
using QuikGraph.Constants;
using QuikGraph.Predicates;
using QuikGraph.Utils;

namespace RingPipeLine
{
    public class CalcModule_QGraph
    {
        /// https://github.com/YaccConstructor/QuickGraph/wiki/graph-theory-reminder
        public interface IEdge<TVertex>
        {
            TVertex Source { get; }
            TVertex Target { get; }
        }

        
        public CalcModule_QGraph()
        {
            InitGraph();
        }


        void AdjGraph()
        {
            var graph = new AdjacencyGraph<int, Edge<int>>();

            foreach (var vertex in graph.Vertices)
            {
                foreach (var edge in graph.OutEdges(vertex))
                    Console.WriteLine(edge);
            }
        }


        /// <summary>
        /// https://github.com/YaccConstructor/QuickGraph/wiki/Creating-Graphs
        /// </summary>
        protected void InitGraph()
        {

            var edges = new SEdge<int>[] { new SEdge<int>(1, 2), new SEdge<int>(0, 1) };

            var graph = edges.ToAdjacencyGraph<int, SEdge<int>>();

            var g = new AdjacencyGraph<int, TaggedEdge<int, string>>();

            int v1 = 1;
            int v2 = 2;

            g.AddVertex(v1);
            g.AddVertex(v2);

            var e1 = new TaggedEdge<int, string>(v1, v2, "hello");

            g.AddEdge(e1);

            int v3 = 3;
            int v4 = 4;

            // v3, v4 are not added to the graph yet
            var e2 = new TaggedEdge<int, string>(v3, v4, "hello");

            g.AddVerticesAndEdge(e2);

            // iterate Verices
            foreach (var v in g.Vertices)
                Console.WriteLine(v);

            // Use the Edges property get an enumerable collection of edges:
            foreach (var e in g.Edges)
                Console.WriteLine(e);

            //Iterate out edges
            foreach (var v in g.Vertices)
            {
                foreach (var e in g.OutEdges(v))
                    Console.WriteLine(e);
            }

        }

        /// <summary>
        /// igorab CreateGraph
        /// </summary>
        /// <param name="_graphNodes"></param>
        public void CreateGraph(TNode[] _graphNodes)
        {
            var edges = new SEdge<int>[] { new SEdge<int>(1, 2), new SEdge<int>(0, 1) };

            foreach (var node in _graphNodes)
            {
                var sEdge = new SEdge<int>(1, 2);
                edges.Append(sEdge);
            }

            var graph = edges.ToAdjacencyGraph<int, SEdge<int>>();

            var g = new AdjacencyGraph<int, TaggedEdge<int, string>>();

            int v1 = 1;
            int v2 = 2;

            g.AddVertex(v1);
            g.AddVertex(v2);

            var e1 = new TaggedEdge<int, string>(v1, v2, "hello");

            g.AddEdge(e1);

            int v3 = 3;
            int v4 = 4;

            // v3, v4 are not added to the graph yet
            var e2 = new TaggedEdge<int, string>(v3, v4, "hello");

            g.AddVerticesAndEdge(e2);

            // iterate Verices
            foreach (var v in g.Vertices)
                Console.WriteLine(v);

            // Use the Edges property get an enumerable collection of edges:
            foreach (var e in g.Edges)
                Console.WriteLine(e);

            //Iterate out edges
            foreach (var v in g.Vertices)
            {
                foreach (var e in g.OutEdges(v))
                    Console.WriteLine(e);
            }

        }


        /// <summary>
        /// Incremental Connected Components
        /// </summary>
        /// <typeparam name="TVertex"></typeparam>
        public void IncrementalConnectedComponents<TVertex>()
        {
            var g = new AdjacencyGraph<int, SEdge<int>>();

            int[] vert = new int[] { 0, 1, 2, 3 };
            g.AddVertexRange(vert );

            /*
            var components = AlgorithmExtensions.IncrementalConnectedComponents(g);

            var current = components();
            Assert.AreEqual(4, current.Key);

            g.AddEdge(new SEdge<int>(0, 1));
            current = components(); // query algorithm again
            Assert.AreEqual(3, current.Key);
            */

        }

        //Dijkstra extension methods
        public void AlgorithmExt<TVertex, TEdge>() where TEdge : QuickGraph.IEdge<TVertex>
        {
            /*
            IVertexAndEdgeListGraph<TVertex, TEdge> graph;
            Func<TEdge, double> edgeCost = e => 1; // constant cost
            TVertex root ;

            // compute shortest paths
            TryFunc<TVertex, TEdge> tryGetPaths = graph.ShortestPathsDijkstra(edgeCost, root);

            // query path for given vertices
            TVertex target;// = ...;
            IEnumerable<TEdge> path;
            if (tryGetPaths(target, out path))
                foreach (var edge in path)
                    Console.WriteLine(edge);
            

            Func<TEdge, double> edgeCost = e => 1; // constant cost
            // We want to use Dijkstra on this graph
            var dijkstra = new DijkstraShortestPathAlgorithm<TEdge, TEdge>(graph, edgeCost);

            // Attach a Vertex Predecessor Recorder Observer to give us the paths
            var predecessors = new QuickGraph.Algorithms.Observers.VertexPredecessorRecorderObserver<TVertex, TEdge>();

            using (predecessors.Attach(dijkstra))
                // Run the algorithm with A set to be the source
                dijkstra.Compute("A");
            */
        }

        internal void CalculateGraph(TNode[] Nodes)
        {
            List<TEdge> edges = new List<TEdge>() ;

            int L1 = Nodes.Length;

            for (int i = 0; i <= L1 - 1; i++)
            {
                int iX = Nodes[i].x ;
                int iY = Nodes[i].y;
                string sName = Nodes[i].name;

                int L2 = 0;
                if (Nodes[i].Edge != null)
                {
                    L2 = Nodes[i].Edge.Length;
                }

                for (int j = 0; j <= L2 - 1; j++)
                {
                    TEdge edge = Nodes[i].Edge[j];

                    int InDataA = edge.A;
                    int InData_x1c = edge.x1c;
                    int InData_x2c = edge.x2c;
                    int InData_yc = edge.yc;
                    int InData_numNode = edge.numNode;

                    edges.Add(edge);
                }
            }
        }
    }
}
