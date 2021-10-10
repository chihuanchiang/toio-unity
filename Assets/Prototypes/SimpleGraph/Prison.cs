using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using toio;
using toio.MathUtils;
using toio.Navigation;

public class Prison {
    public enum Phase {
        Escape,
        Chase,
        Caught,
        Serve,
    }

    private const float _actionTime = 3.0f;

    private Vertex _v;
    private Vector _midPoint;
    private List<Player> _player;
    private Phase _phase;
    private float _elapsedTime = 0.0f;

    public Prison(List<Player> player, Vertex v) {
        _player = player;
        _v = v;
        var neighbor = _v.Adj[0];
        _midPoint = (_v.Value.Pos + neighbor.Value.Pos) / 2;
        Reset();
    }

    public bool Stay(int turn) {
        _elapsedTime += Time.deltaTime;

        foreach (var p in _player) {
            p.First.Handle.Update();
            p.First.Navigator.Update();
            p.Second.Handle.Update();
            p.Second.Navigator.Update();
        }

        Movement mv, mv1, mv2;
        switch(_phase) {
            case Prison.Phase.Escape:
                mv = _player[turn].First.Navigator.Navi2Target(_midPoint, 20).Exec();
                if (mv.reached) {
                    foreach (var p in _player) {
                        p.Second.Cube.PlayPresetSound(1);
                    }
                    _phase = Prison.Phase.Chase;
                }
                break;
            
            case Prison.Phase.Chase:
                _player[turn].First.Navigator.NaviAwayTarget(_player[0].Second.Handle.x, _player[0].Second.Handle.y, 20).Exec();
                mv1 = _player[0].Second.Handle.Move2Target(_player[turn].First.Handle.x, _player[turn].First.Handle.y, 80, tolerance:60).Exec();
                mv2 = _player[1].Second.Handle.Move2Target(_player[turn].First.Handle.x, _player[turn].First.Handle.y, 80, tolerance:60).Exec();
                if (mv1.reached || mv2.reached) {
                    _phase = Prison.Phase.Caught;
                }
                break;

            case Prison.Phase.Caught:
                mv = _player[turn].First.Navigator.Navi2Target(_v.Value.Pos, 30).Exec();
                _player[0].Second.Handle.Rotate2Target(_player[turn].First.Handle.x, _player[turn].First.Handle.y).Exec();
                _player[1].Second.Handle.Rotate2Target(_player[turn].First.Handle.x, _player[turn].First.Handle.y).Exec();
                if (mv.reached) {
                    _elapsedTime = 0;
                    _phase = Prison.Phase.Serve;
                }
                break;
                
            case Prison.Phase.Serve:
                _player[turn].First.Handle.Move(0, 100, 200);
                mv1 = _player[0].Second.Navigator.Navi2Target(_player[0].Second.Home.Value.Pos).Exec();
                mv2 = _player[1].Second.Navigator.Navi2Target(_player[1].Second.Home.Value.Pos).Exec();
                if ((_elapsedTime > _actionTime) && mv1.reached && mv2.reached) {
                    Reset();
                    return false;
                }
                break;

            default:
                Debug.LogError("Prison: Invalid phase");
                break;
        }
        return true;
    }

    public void Reset() {
        _phase = Prison.Phase.Escape;
    }
}