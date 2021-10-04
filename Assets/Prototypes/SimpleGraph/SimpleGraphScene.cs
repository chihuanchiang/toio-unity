using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using toio;
using toio.Navigation;
using toio.MathUtils;
using static System.Math;
using toio.Multimat;


public class SimpleGraphScene : MonoBehaviour
{
    Graph graph;
    CubeManager cm;
    List<Player> player = new List<Player>();
    bool started = false;

    public string MapFile;
    public UI ui;

    async void Start()
    {
        // Build graph
        graph = new Graph();
        ReadMap("Assets/Prototypes/SimpleGraph/maps/" + MapFile);
        graph.V[4].Value.Color = new Color(1, 0, 0, 0.3f);
        graph.V[8].Value.Color = new Color(0, 0, 0, 0.3f);
        graph.V[9].Value.Color = new Color(0, 0, 0, 0.3f);

        // Connect to cubes
        cm = new CubeManager();
        await cm.MultiConnect(4);

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

        // Assign 2 characters to each player
        for (int i = 0; i < 2; i++) {
            var ch1 = new Character(cm.navigators[2 * i], cm.cubes[2 * i]);
            var ch2 = new Character(cm.navigators[2 * i + 1], cm.cubes[2 * i + 1]);
            player.Add(new Player(ch1, ch2));
        }

        // Set the initial spot of each player's first character
        player[0].First.spot = graph.V[0];
        player[1].First.spot = graph.V[2];
        // Set the initial spot of each player's second character
        player[0].Second.spot = graph.V[8];
        player[1].Second.spot = graph.V[9];

        started = true;
    }

    int phase = 0;
    int turn = 0;
    int toio_status = 0; //0: toio stop, 1:toio moves
    int count = 0;
    void Update()
    {
        if (!started) return;
        switch (phase) {
            case 0:
                // Move characters to their starting spot
                if (cm.synced) {
                    bool all_reached = true;
                    foreach (var p in player) {
                        p.First.Navi2Spot();
                        p.Second.Navi2Spot();
                        all_reached &= p.First.mv.reached && p.Second.mv.reached;
                    }
                    if (all_reached)
                    {
                        phase = 1;
                        ui.OpenBtn();
                    }
                }
                break;
            case 1:
                // Players take turns moving to a neighboring spot
                if (cm.synced) {
                    //Debug.Log(toio_status);
                    //Debug.Log(count);
                    if(toio_status == 0 && count == 0)
                    {
                        Debug.Log("First");
                        ui.ShowPlayerOrder(turn);
                        count++;
                    }
                    else if(toio_status == 1)
                    {
                        Debug.Log("Second");
                        var p = player[turn];
                        p.First.Navi2Spot();
                        if (p.First.mv.reached)
                        {
                            Debug.Log("reach");
                            p.First.RenewSpot();
                            turn++;
                            if (turn >= player.Count) turn = 0;
                            toio_status = 0;
                            count = 0;
                        }
                    }
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
            
            graph.V.Add(new Vertex(new Island(new Vector(x, y), islandColor, r)));
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
        toio_status = 1;
    }

}
