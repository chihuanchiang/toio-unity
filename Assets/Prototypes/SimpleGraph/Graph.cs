using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph<TVertexType> {
    public Graph() {
        V = new List<Vertex<TVertexType>>();
        E = new List<Edge<TVertexType>>();
    }

    public List<Vertex<TVertexType>> V { get; set; }
    public List<Edge<TVertexType>> E { get; set; }

    public void AddEdge(int n1, int n2) {
        V[n1].Adj.Add(V[n2]);
        V[n2].Adj.Add(V[n1]);
        E.Add(new Edge<TVertexType>(V[n1], V[n2]));
    }
}

public class Vertex<TVertexType> {
    public Vertex(TVertexType value) {
        Adj = new List<Vertex<TVertexType>>();
        Value = value;
    }

    public List<Vertex<TVertexType>> Adj { get; set; }
    public int Degree { get { return Adj.Count; } }
    public TVertexType Value { get; set; }
}

public class Edge<TVertexType> {
    public Edge(Vertex<TVertexType> src, Vertex<TVertexType> dst) {
        Src = src;
        Dst = dst;
    }

    public Vertex<TVertexType> Src { get; set; }
    public Vertex<TVertexType> Dst { get; set; }
}