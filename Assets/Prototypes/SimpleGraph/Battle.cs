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
        Attack,
        Retreat,
        Face,
    }

    private const float _chargeTime = 5.0f;

    private List<Player> _player;
    private Phase _phase = Phase.Enter;
    private int _turn = 0;
    private int _invTurn { get { return (_turn == 1)?0:1; }}
    private float _elapsedTime = 0.0f;

    public Battle(List<Player> player) {
        _player = player;
        _player[0].BattleX = Island.OriginX - 50;
        _player[0].BattleY = Island.OriginY;
        _player[1].BattleX = Island.OriginX + 50;
        _player[1].BattleY = Island.OriginY;
    }

    public bool Play() {
        _elapsedTime += Time.deltaTime;

        foreach (var p in _player) {
            p.First.Handle.Update();
            p.First.Navigator.Update();
        }

        Movement mv, mv1, mv2;
        switch(_phase) {
            case Phase.Enter:
                mv1 = _player[0].First.Navigator.Navi2Target(_player[0].BattleX, _player[0].BattleY).Exec();
                mv2 = _player[1].First.Navigator.Navi2Target(_player[1].BattleX, _player[1].BattleY).Exec();
                if (mv1.reached && mv2.reached) _phase = Phase.Start;
                break;
            case Phase.Start:
                mv1 = _player[0].First.Handle.Rotate2Target(_player[1].BattleX, _player[1].BattleY).Exec();
                mv2 = _player[1].First.Handle.Rotate2Target(_player[0].BattleX, _player[0].BattleY).Exec();
                if (mv1.reached && mv2.reached) {
                    _elapsedTime = 0.0f;
                    _phase = Phase.Charge;
                }
                break;
            case Phase.Charge:
                _player[_turn].First.Handle.Move(0, _player[_turn].Stat.Energy * 20, 200);
                if (_elapsedTime > _chargeTime) _phase = Phase.Attack;
                break;
            case Phase.Attack:
                mv = _player[_turn].First.Handle.Move2Target(_player[_invTurn].First.Handle.x, _player[_invTurn].First.Handle.y, tolerance:40).Exec();
                if (mv.reached) _phase = Phase.Retreat;
                break;
            case Phase.Retreat:
                mv = _player[_turn].First.Handle.Move2Target(_player[_turn].BattleX, _player[_turn].BattleY).Exec();
                if (mv.reached) _phase = Phase.Face;
                break;
            case Phase.Face:
                mv = _player[_turn].First.Handle.Rotate2Target(_player[_invTurn].BattleX, _player[_invTurn].BattleY).Exec();
                if (mv.reached) {
                    _turn = _invTurn;
                    _elapsedTime = 0.0f;
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