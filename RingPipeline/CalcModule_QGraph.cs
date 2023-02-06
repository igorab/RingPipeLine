﻿using System;
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
    class CalcModule_QGraph
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
        public void InitGraph()
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

        //Dijkstra extension methods
        public void AlgorithmExtensions<TVertex, TEdge>() where TEdge : QuickGraph.IEdge<TVertex>
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


    }
}
