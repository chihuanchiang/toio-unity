using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using toio;
using toio.Navigation;

public class Prison {
    public enum Phase {
        Start,
        Serve,
        Release,
    }

    private const int _maxTime = 1;
    private const float _actionTime = 3.0f;

    private List<Player> _player;
    private int _time;
    private Phase _phase;
    private float _elapsedTime = 0.0f;

    public Prison(List<Player> player) {
        _player = player;
        Reset();
    }

    public bool Stay(int turn) {
        _elapsedTime += Time.deltaTime;

        switch(_phase) {
            case Prison.Phase.Start:
                _elapsedTime = 0;
                _phase = Prison.Phase.Serve;
                break;

            case Prison.Phase.Serve:
                _player[turn].First.Handle.Move(0, 100, 200);
                if (_elapsedTime > _actionTime) {
                    if (_time > 0) {
                        _time--;
                    } else {
                        _phase = Prison.Phase.Release;
                    }
                }
                break;

            case Prison.Phase.Release:
                Reset();
                return false;
                break;
                
            default:
                Debug.LogError("Prison: Invalid phase");
                break;
        }
        return true;
    }

    public void Reset() {
        _time =  _maxTime;
        _phase = Prison.Phase.Start;
    }
}