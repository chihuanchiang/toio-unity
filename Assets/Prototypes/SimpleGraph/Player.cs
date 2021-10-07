using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public Player(Character first, Character second) {
        First = first;
        Second = second;
        stat = new Stat();
        ResetStat();
    }

    public class Stat {
        public int hp { get; set; }
        public int atk { get; set; }
        public int dex { get; set; }
    }
    
    public Character First { get; set; }
    public Character Second { get; set; }
    public Stat stat { get; set; }

    public void ResetStat() {
        stat.hp = 3;
        stat.atk = 0;
        stat.dex = 0;
    }

    public void IslandAction() {
        switch (First.curr.Value.type) {
            case Island.Type.PowerUpHp:
                AddHp();
                break;
            case Island.Type.PowerUpAtk:
                AddAtk();
                break;
            case Island.Type.PowerUpDex:
                AddDex();
                break;
            case Island.Type.Prison:
                GoToPrison();
                break;
            default:
                break;
        }
    }

    public void AddHp() {
        stat.hp++;
        First.cube.TurnLedOn(0, 255, 0, 500);
        First.cube.PlayPresetSound(0);
    }

    public void AddAtk() {
        stat.atk++;
        First.cube.TurnLedOn(255, 0, 0, 500);
        First.cube.PlayPresetSound(0);
    }

    public void AddDex() {
        stat.dex++;
        First.cube.TurnLedOn(255, 140, 40, 500);
        First.cube.PlayPresetSound(0);
    }

    public void GoToPrison() {
        First.cube.PlayPresetSound(1);
    }
}