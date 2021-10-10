using System.Collections;
using System.Collections.Generic;
using static System.Math;
using UnityEngine;
using toio;
using toio.Navigation;
using toio.MathUtils;
using toio.Multimat;


public class SimpleGraphScene : MonoBehaviour
{
    public UI ui;
    public bool BypassUi = false;
    
    private Graph _graph;
    private CubeManager _cm;
    private List<Player> _player = new List<Player>();
    private Battle _battle;
    private bool _started = false;
    private int _phase = 0;
    private int _turn = 0;
    private int _inputStatus = 0; //0: toio stop (wait for ui input), 1:toio moves (wait for cubes to reach their spots)
    private bool _flagUi = false;

    async void Start()
    {
        // Build graph
        _graph = new Graph();
#if (UNITY_EDITOR || UNITY_STANDALONE)
        ReadMap("map2");
#elif (UNITY_IOS || UNITY_ANDROID || UNITY_WEBGL)
        ReadMap("map2_devmat");
#endif
        // Set special island types
        _graph.V[1].Value.SetType(Island.Types.PowerUpStr);
        _graph.V[4].Value.SetType(Island.Types.Prison);
        _graph.V[6].Value.SetType(Island.Types.PowerUpLuck);
        _graph.V[7].Value.SetType(Island.Types.PowerUpHp);
        _graph.V[8].Value.SetType(Island.Types.Dummy);
        _graph.V[9].Value.SetType(Island.Types.Dummy);

        // Connect to cubes
        _cm = new CubeManager();
        await _cm.MultiConnect(4);

        // Setup multimat
        _cm.handles.Clear();
        _cm.navigators.Clear();
        foreach (var cube in _cm.cubes)
        {
            var handle = new HandleMats(cube);
            _cm.handles.Add(handle);
            var navigator = new CubeNavigator(handle);
            navigator.usePred = true;
            navigator.mode = Navigator.Mode.BOIDS_AVOID;
            navigator.ClearOther();
            _cm.navigators.Add(navigator);

            navigator.ClearWall();
#if (UNITY_EDITOR || UNITY_STANDALONE)
            handle.borderRect = new RectInt(0, 0, 910, 500);
            navigator.AddBorder(30, x1:0, x2:910, y1:0, y2:500);
#elif (UNITY_IOS || UNITY_ANDROID || UNITY_WEBGL)
            handle.borderRect = new RectInt(58, 102, 688, 512);
            navigator.AddBorder(30, x1:58, x2:746, y1:102, y2:614);
#endif
        }

        // Assign 2 characters to each player
        for (int i = 0; i < 2; i++) {
            var ch1 = new Character(_cm.navigators[2 * i], _cm.handles[2 * i], _cm.cubes[2 * i]);
            var ch2 = new Character(_cm.navigators[2 * i + 1], _cm.handles[2 * i + 1], _cm.cubes[2 * i + 1]);
            _player.Add(new Player(ch1, ch2));
        }

        // Set up battle
        _battle = new Battle(_player);

        // Set the home spot of each character
        _player[0].First.Home = _graph.V[0];
        _player[1].First.Home = _graph.V[2];
        _player[0].Second.Home = _graph.V[8];
        _player[1].Second.Home = _graph.V[9];
        // Set the first spot of each character to their home
        foreach (var p in _player) {
            p.First.Point2Home();
            p.Second.Point2Home();
        }

        _started = true;
    }

    void Update()
    {
        if (!_started) return;
        switch (_phase) {
            case 0:
                // Move characters to their starting spot
                if (_cm.synced) {
                    bool allReached = true;
                    foreach (var p in _player) {
                        p.First.Move2Next();
                        p.Second.Move2Next();
                        allReached &= p.First.mv.reached && p.Second.mv.reached;
                    }
                    if (allReached) {
                        foreach (var p in _player) {
                            p.First.UpdateNext();
                        }
                        _phase = 1;
                        ui.TurnOnMoveBtn();
                    }
                }
                break;
            case 1:
                // Players take turns moving to a neighboring spot
                //Debug.Log(_inputStatus);
                if(_inputStatus == 0 && !_flagUi) {
                    // Bypass ui input for testing
                    if (BypassUi) _inputStatus = 1;

                    ui.ShowPlayerOrder(_turn);
                    _flagUi = true;
                }
                else if(_inputStatus == 1)
                {
                    var p = _player[_turn];
                    if (_cm.synced) {
                        p.First.Move2Next(_player[0].First.Curr == _player[1].First.Curr);
                        if (p.First.mv.reached)
                        {
                            Debug.Log("reach");
                            p.IslandAction();
                            if (_player[0].First.Curr == _player[1].First.Curr) {
                                _phase = 2;

                                ui.TurnOffMoveText();
                                ui.TurnOffMoveBtn();
                                ui.TurnOnBattleText();
                            }
                            p.First.UpdateNext();
                            _turn++;
                            if (_turn >= _player.Count) _turn = 0;
                            _inputStatus = 0;
                            _flagUi = false;
                        }
                    }
                }
                break;
            case 2:
                // Battle
                if (!_battle.Play()) {
                    foreach (var p in _player) {
                        p.First.Point2Home();
                    }
                    _phase = 0;

                    ui.TurnOffBattleText();
                    ui.TurnOnMoveText();
                }
                break;
            default:
                Debug.LogError("SimpleGraphScene: Invalid phase");
                break;
        }
    }

    void OnDrawGizmos() {
        if (!_started) return;

        foreach (var v in _graph.V) {
            Gizmos.color = v.Value.Color;
            Gizmos.DrawSphere(v.Value.Pos3, v.Value.Radius3);
        }

        foreach (var e in _graph.E) {
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
            _graph.V.Add(new Vertex(new Island(new Vector(x, y), r)));
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
            _graph.AddEdge(n1, n2);
        }
    }

    // Users click the button to move toio cube (like roll a dice) 
    public void RollDice()
    {
        Debug.Log("Roll A DICE");
        _inputStatus = 1;
    }
}
