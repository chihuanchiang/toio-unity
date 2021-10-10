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
        Miss,
        Retreat,
        Defeat,
        Face,
    }

    private const float _chargeTime = 5.0f;
    private const float _defeatTime = 2.5f;

    private List<Player> _player;
    private Phase _phase;
    private int _turn;
    private int _invTurn { get { return (_turn == 1)?0:1; }}
    private float _elapsedTime = 0.0f;

    public Battle(List<Player> player) {
        _player = player;
        _player[0].BattleX = Island.OriginX - 50;
        _player[0].BattleY = Island.OriginY;
        _player[1].BattleX = Island.OriginX + 50;
        _player[1].BattleY = Island.OriginY;
        ResetBattle();
    }

    public bool Play() {
        _elapsedTime += Time.deltaTime;

        foreach (var p in _player) {
            p.First.Handle.Update();
            p.First.Navigator.Update();
        }

        Movement mv, mv1, mv2;

        GameObject.Find("scene").GetComponent<UI>().ShowBattleStats(_player[0].Stat.Hp, _player[0].Stat.Energy, _player[0].Stat.Luck, _player[1].Stat.Hp, _player[1].Stat.Energy, _player[1].Stat.Luck);

        switch (_phase) {

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
                _player[_turn].Stat.Energy += 0.5f * _player[_turn].Second.Cube.shakeLevel * (1.0f + 0.2f * _player[_turn].Stat.Str) * (float)Time.deltaTime;
                int en = (int)_player[_turn].Stat.Energy;
                _player[_turn].First.Handle.Move(0, 10 + en * 20, 200);
                if (_elapsedTime > _chargeTime) {
                    _phase = (Random.Range(0, 10) < _player[_invTurn].Stat.Luck)?Phase.Miss:Phase.Attack;
                }
                break;

            case Phase.Miss:
                mv = _player[_turn].First.Handle.Move2Target(_player[_invTurn].First.Handle.x, _player[_invTurn].First.Handle.y - 60).Exec();
                if (mv.reached) {
                    _player[_turn].Stat.Energy = 0;
                    _phase = Phase.Retreat;
                }
                break;

            case Phase.Attack:
                mv = _player[_turn].First.Handle.Move2Target(_player[_invTurn].First.Handle.x, _player[_invTurn].First.Handle.y, tolerance:40).Exec();
                if (mv.reached) {
                    _player[_invTurn].First.Cube.PlayPresetSound(1);
                    _player[_invTurn].First.Cube.TurnLedOn(255, 0, 0, 500);
                    _player[_invTurn].Stat.Hp -= (int)(_player[_turn].Stat.Energy * 0.1f); 
                    _player[_turn].Stat.Energy = 0;
                    if (_player[_invTurn].Stat.Hp > 0) {
                        _phase = Phase.Retreat;
                    } else {
                        _player[_invTurn].First.Cube.PlayPresetSound(3);
                        _player[_turn].Point++;
                        _elapsedTime = 0;
                        _phase = Phase.Defeat;
                    }
                }
                break;

            case Phase.Retreat:
                mv = _player[_turn].First.Handle.Move2Target(_player[_turn].BattleX, _player[_turn].BattleY).Exec();
                if (mv.reached) _phase = Phase.Face;
                break;
            
            case Phase.Defeat:
                _player[_invTurn].First.Handle.Move(0, 50, 200);
                if (_elapsedTime > _defeatTime) {
                    ResetBattle();
                    foreach (var p in _player) {
                        p.ResetStat();
                    }
                    return false;
                }
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

    private void ResetBattle() {
        _phase = Phase.Enter;
        _turn = 0;
    }
}   