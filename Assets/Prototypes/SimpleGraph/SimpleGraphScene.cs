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

    public UI ui;

    async void Start()
    {
        // Build graph
        graph = new Graph();
#if (UNITY_EDITOR || UNITY_STANDALONE)
        ReadMap("map2");
#elif (UNITY_IOS || UNITY_ANDROID || UNITY_WEBGL)
        ReadMap("map2_devmat");
#endif
        // Set special island types
        graph.V[1].Value.SetType(Island.Type.PowerUpAtk);
        graph.V[4].Value.SetType(Island.Type.Prison);
        graph.V[6].Value.SetType(Island.Type.PowerUpDex);
        graph.V[7].Value.SetType(Island.Type.PowerUpHp);
        graph.V[8].Value.SetType(Island.Type.Dummy);
        graph.V[9].Value.SetType(Island.Type.Dummy);

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
            // navi.usePred = true;
            // navi.mode = Navigator.Mode.BOIDS_AVOID;
            navi.ClearOther();
            cm.navigators.Add(navi);

            navi.ClearWall();
#if (UNITY_EDITOR || UNITY_STANDALONE)
            handle.borderRect = new RectInt(0, 0, 910, 500);
            navi.AddBorder(30, x1:0, x2:910, y1:0, y2:500);
#elif (UNITY_IOS || UNITY_ANDROID || UNITY_WEBGL)
            handle.borderRect = new RectInt(58, 102, 688, 512);
            navi.AddBorder(30, x1:58, x2:746, y1:102, y2:614);
#endif
        }

        // Assign 2 characters to each player
        for (int i = 0; i < 2; i++) {
            var ch1 = new Character(cm.navigators[2 * i], cm.handles[2 * i], cm.cubes[2 * i]);
            var ch2 = new Character(cm.navigators[2 * i + 1], cm.handles[2 * i + 1], cm.cubes[2 * i + 1]);
            player.Add(new Player(ch1, ch2));
        }

        // Set the initial spot of each player's first character
        player[0].First.next = graph.V[0];
        player[1].First.next = graph.V[2];
        // Set the initial spot of each player's second character
        player[0].Second.next = graph.V[8];
        player[1].Second.next = graph.V[9];

        started = true;
    }

    int phase = 0;
    int turn = 0;
    int toio_status = 0; //0: toio stop (wait for ui input), 1:toio moves (wait for cubes to reach their spots)
    bool flag_ui = false;
    void Update()
    {
        if (!started) return;
        switch (phase) {
            case 0:
                // Move characters to their starting spot
                if (cm.synced) {
                    bool all_reached = true;
                    foreach (var p in player) {
                        p.First.Move2Next();
                        p.Second.Move2Next();
                        all_reached &= p.First.mv.reached && p.Second.mv.reached;
                    }
                    if (all_reached)
                    {
                        foreach (var p in player) {
                            p.First.RenewNext();
                        }
                        phase = 1;
                        ui.OpenBtn();
                    }
                }
                break;
            case 1:
                // Players take turns moving to a neighboring spot
                if (cm.synced) {
                    //Debug.Log(toio_status);
                    if(toio_status == 0 && !flag_ui)
                    {
                        // Debug.Log("Waiting for ui input");
                        toio_status = 1; // for testing
                        ui.ShowPlayerOrder(turn);
                        flag_ui = true;
                    }
                    else if(toio_status == 1)
                    {
                        // Debug.Log("Waiting for cubes to reach their spots");
                        var p = player[turn];
                        p.First.Move2Next();
                        if (p.First.mv.reached)
                        {
                            Debug.Log("reach");
                            p.IslandAction();
                            if (player[0].First.curr == player[1].First.curr) {
                                phase = 2;
                            }
                            p.First.RenewNext();
                            turn++;
                            if (turn >= player.Count) turn = 0;
                            toio_status = 0;
                            flag_ui = false;
                        }
                    }
                }
                break;
            case 2:
                // Battle: move characters to the battle field
                if (cm.synced) {
                    Movement mv1 = player[0].First.nav.Navi2Target(Island.originX - 50, Island.originY).Exec();
                    Movement mv2 = player[1].First.nav.Navi2Target(Island.originX + 50, Island.originY).Exec();
                    if (mv1.reached && mv2.reached) {
                        player[0].First.next = graph.V[0];
                        player[1].First.next = graph.V[2];
                        phase = 3;
                    }
                }
                break;
            case 3:
                // Battle
                if (cm.synced) {
                    foreach (var p in player) {
                        p.First.cube.PlayPresetSound(0);
                    }
                    phase = 0;
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
        var f = Resources.Load<TextAsset>(file);
        string[] lines = f.text.Split('\n');
        int ln = 0;
        int N;
        
        // Parse vertices
        int.TryParse(lines[ln++], out N);
        for (int i = 0; i < N; i++) {
            string[] line = lines[ln++].Split(' ');
            int x, y, r;
            int.TryParse(line[0], out x);
            int.TryParse(line[1], out y);
            int.TryParse(line[2], out r);
            graph.V.Add(new Vertex(new Island(new Vector(x, y), r)));
            Debug.Log(string.Format("Add vertex with x:{0} y:{1} r:{2}", x, y, r));
        }

        // Parse edges
        int.TryParse(lines[ln++], out N);
        for (int i = 0; i < N; i++) {
            string[] line = lines[ln++].Split(' ');
            int n1, n2;
            int.TryParse(line[0], out n1);
            int.TryParse(line[1], out n2);
            Debug.Log(string.Format("Add edge with n1:{0} n2:{1}", n1, n2));
            graph.AddEdge(n1, n2);
        }
    }

    // Users click the button to move toio cube (like roll a dice) 
    public void RollDice()
    {
        Debug.Log("Roll A DICE");
        toio_status = 1;
    }

}
