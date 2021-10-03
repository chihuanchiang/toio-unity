using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using toio;
using toio.Navigation;
using toio.MathUtils;
using static System.Math;
using toio.Multimat;


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
        ReadMap("Assets/Prototypes/SimpleGraph/1.map");
        graph.V[4].Value.Color = new Color(1, 0, 0, 0.3f); // Set an island to color red

        // Connect to cubes
        cm = new CubeManager();
        await cm.SingleConnect();

        // Setup multimat
        cm.handles.Clear();
        cm.navigators.Clear();
        foreach (var cube in cm.cubes)
        {
            var handle = new HandleMats(cube);
            cm.handles.Add(handle);
            var navi = new CubeNavigator(handle);
            navi.usePred = true;
            navi.mode = Navigator.Mode.BOIDS_AVOID;
            cm.navigators.Add(navi);

            handle.borderRect = new RectInt(0, 0, 910, 500);
            navi.ClearWall();
            navi.AddBorder(30, x1:0, x2:910, y1:0, y2:500);
        }

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
                        phase = 0;
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

    // Read the game map from a file
    public void ReadMap(string file) {
        Color islandColor = new Color(0, 0, 1, 0.3f);
        // Parse vertices
        int N;
        System.IO.StreamReader f = new System.IO.StreamReader(file);
        int.TryParse(f.ReadLine(), out N);
        for (int i = 0; i < N; i++) {
            string[] line = f.ReadLine().Split(' ');
            int x, y, r;
            int.TryParse(line[0], out x);
            int.TryParse(line[1], out y);
            int.TryParse(line[2], out r);
            Debug.Log(string.Format("Add vertex with x:{0} y:{1} r:{2}", x, y, r));
            
            graph.V.Add(new Vertex<Island>(new Island(new Vector(x, y), islandColor, r)));
        }

        // Parse edges
        int.TryParse(f.ReadLine(), out N);
        for (int i = 0; i < N; i++) {
            string[] line = f.ReadLine().Split(' ');
            int n1, n2;
            int.TryParse(line[0], out n1);
            int.TryParse(line[1], out n2);
            Debug.Log(string.Format("Add edge with n1:{0} n2:{1}", n1, n2));
            graph.AddEdge(n1, n2);
        }
        f.Close();
    }

    // Users click the button to move toio cube (like roll a dice) 
    public void RollDice()
    {
        Debug.Log("Roll A DICE");
        phase = 1;
    }
}
