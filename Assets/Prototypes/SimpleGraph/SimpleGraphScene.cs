using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using toio;
using toio.MathUtils;
using static System.Math;


// [ExecuteInEditMode]
public class SimpleGraphScene : MonoBehaviour
{
    Graph<Island> graph;
    CubeManager cm;
    Character<Island> p1;
    bool started = false;

    async void Start()
    {
        // Build graph
        graph = new Graph<Island>();

        for (int i = 0; i < 5; i++) {
            graph.V.Add(new Vertex<Island>());
        }

        Color islandColor = new Color(0, 0, 1, 0.3f);
        graph.V[0].Value = new Island(new Vector(100, 100), islandColor, 40);
        graph.V[1].Value = new Island(new Vector(400, 100), islandColor, 40);
        graph.V[2].Value = new Island(new Vector(400, 400), islandColor, 40);
        graph.V[3].Value = new Island(new Vector(100, 400), islandColor, 40);
        graph.V[4].Value = new Island(new Vector(250, 250), new Color(1, 0, 0, 0.3f), 80);

        graph.AddEdge(0, 1);
        graph.AddEdge(1, 2);
        graph.AddEdge(2, 3);
        graph.AddEdge(3, 0);
        graph.AddEdge(0, 4);
        graph.AddEdge(1, 4);
        graph.AddEdge(2, 4);
        graph.AddEdge(3, 4);

        // Connect to cubes
        cm = new CubeManager();
        await cm.SingleConnect();

        // Assign cube to characters
        p1 = new Character<Island>(cm.navigators[0]);

        // Set the first spot of each character
        p1.spot = graph.V[0];

        started = true;
    }

    int phase = 0;
    void Update()
    {
        switch (phase) {
            case 0:
                // Move toio Core cube to starting position
                if (cm.synced) {
                    p1.mv = p1.cubeNav.Navi2Target(p1.spot.Value.Pos.x, p1.spot.Value.Pos.y).Exec();
                    if (p1.mv.reached) phase = 1;
                }
                break;
            case 1:
                // Move toio Core cube to a random selected neighbor
                if (cm.synced) {
                    if (p1.mv.reached) {
                        p1.spot = p1.spot.Adj[Random.Range(0, p1.spot.Degree)];
                    }
                    p1.mv = p1.cubeNav.Navi2Target(p1.spot.Value.Pos.x, p1.spot.Value.Pos.y).Exec();
                }
                break;
            default:
                Debug.LogError("Invalid phase");
                break;
        }
    }

    void OnDrawGizmos() {
        if (!started) return;

        foreach (var v in graph.V) {
            Gizmos.color = v.Value.Color;
            Gizmos.DrawSphere(v.Value.Pos3, v.Value.Radius3);
        }

        foreach (var e in graph.E) {
            Gizmos.color = new Color(0,0,0,1.0f);
            Gizmos.DrawLine(e.Src.Value.Pos3, e.Dst.Value.Pos3);
        }
    }

}
