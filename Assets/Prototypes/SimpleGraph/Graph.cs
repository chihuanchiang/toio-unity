using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph {
    public Graph() {
        V = new List<Vertex>();
        E = new List<Edge>();
    }

    public List<Vertex> V { get; set; }
    public List<Edge> E { get; set; }

    public void AddEdge(int n1, int n2) {
        V[n1].Adj.Add(V[n2]);
        V[n2].Adj.Add(V[n1]);
        E.Add(new Edge(V[n1], V[n2]));
    }
}

public class Vertex {
    public Vertex(Island value) {
        Adj = new List<Vertex>();
        Value = value;
    }

    public List<Vertex> Adj { get; set; }
    public int Degree { get { return Adj.Count; } }
    public Island Value { get; set; }
}

public class Edge {
    public Edge(Vertex src, Vertex dst) {
        Src = src;
        Dst = dst;
    }

    public Vertex Src { get; set; }
    public Vertex Dst { get; set; }
}