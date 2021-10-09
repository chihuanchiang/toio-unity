using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph {
    public List<Vertex> V;
    public List<Edge> E;

    public Graph() {
        V = new List<Vertex>();
        E = new List<Edge>();
    }

    public void AddEdge(int n1, int n2) {
        V[n1].Adj.Add(V[n2]);
        V[n2].Adj.Add(V[n1]);
        E.Add(new Edge(V[n1], V[n2]));
    }
}

public class Vertex {
    public List<Vertex> Adj;
    public int Degree { get { return Adj.Count; } }
    public Island Value;

    public Vertex(Island value) {
        Adj = new List<Vertex>();
        Value = value;
    }
}

public class Edge {
    public Vertex Src;
    public Vertex Dst;

    public Edge(Vertex src, Vertex dst) {
        Src = src;
        Dst = dst;
    }
}