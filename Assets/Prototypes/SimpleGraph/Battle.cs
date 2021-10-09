using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using toio;
using toio.Navigation;

public class Battle {
    public enum Phase {
        Enter,
        Start,
        Charge,
        Retreat,
        Face,
    }

    private Player _p1;
    private Player _p2;
    private float _x1 = Island.originX - 50;
    private float _x2 = Island.originX + 50;
    private float _y1 = Island.originY;
    private float _y2 = Island.originY;
    private Phase _phase = Phase.Enter;
    private bool _turn = false; // false: _p1's turn, true: _p2's turn

    public Battle(Player p1, Player p2) {
        _p1 = p1;
        _p2 = p2;
    }

    public bool Play() {
        _p1.First.handle.Update();
        _p2.First.handle.Update();
        Movement mv, mv1, mv2;
        switch(_phase) {
            case Phase.Enter:
                // Both cubes move to their starting points
                mv1 = _p1.First.nav.Navi2Target(_x1, _y1).Exec();
                mv2 = _p2.First.nav.Navi2Target(_x2, _y2).Exec();
                if (mv1.reached && mv2.reached) _phase = Phase.Start;
                break;
            case Phase.Start:
                // Turn to enemies
                mv1 = _p1.First.handle.Rotate2Target(_x2, _y2).Exec();
                mv2 = _p2.First.handle.Rotate2Target(_x1, _y1).Exec();
                if (mv1.reached && mv2.reached) _phase = Phase.Charge;
                break;
            case Phase.Charge:
                if (_turn) {
                    mv = _p1.First.handle.Move2Target(_p2.First.handle.x, _p2.First.handle.y, tolerance:40).Exec();
                } else {
                    mv = _p2.First.handle.Move2Target(_p1.First.handle.x, _p1.First.handle.y, tolerance:40).Exec();
                }
                if (mv.reached) _phase = Phase.Retreat;
                break;
            case Phase.Retreat:
                if (_turn) {
                    mv = _p1.First.handle.Move2Target(_x1, _y1).Exec();
                } else {
                    mv = _p2.First.handle.Move2Target(_x2, _y2).Exec();
                }
                if (mv.reached) _phase = Phase.Face;
                break;
            case Phase.Face:
                if (_turn) {
                    mv = _p1.First.handle.Rotate2Target(_x2, _y2).Exec();
                } else {
                    mv = _p2.First.handle.Rotate2Target(_x1, _y1).Exec();
                }
                if (mv.reached) {
                    _turn = !_turn;
                    _phase = Phase.Charge;
                }
                break;
            default:
                Debug.LogError("Battle: Invalid Phase");
                break;
        }
        return true;
    }
}   