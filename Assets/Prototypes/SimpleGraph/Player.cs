using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player {
    public class Stats {
        public int HP;
        public int ATK;
        public int DEX;
    }
    
    public Character First;
    public Character Second;
    public Stats Stat;

    public Player(Character first, Character second) {
        First = first;
        Second = second;
        Stat = new Stats();
        ResetStat();
    }

    public void ResetStat() {
        Stat.HP = 3;
        Stat.ATK = 0;
        Stat.DEX = 0;
    }

    public void IslandAction() {
        switch (First.Curr.Value.Type) {
            case Island.Types.PowerUpHp:
                AddHp();
                break;
            case Island.Types.PowerUpAtk:
                AddAtk();
                break;
            case Island.Types.PowerUpDex:
                AddDex();
                break;
            case Island.Types.Prison:
                GoToPrison();
                break;
            case Island.Types.Normal:
                break;
            default:
                break;
        }
    }

    public void AddHp() {
        Stat.HP++;
        First.Cube.TurnLedOn(0, 255, 0, 500);
        First.Cube.PlayPresetSound(0);
    }

    public void AddAtk() {
        Stat.ATK++;
        First.Cube.TurnLedOn(255, 0, 0, 500);
        First.Cube.PlayPresetSound(0);
    }

    public void AddDex() {
        Stat.DEX++;
        First.Cube.TurnLedOn(255, 140, 40, 500);
        First.Cube.PlayPresetSound(0);
    }

    public void GoToPrison() {
        First.Cube.PlayPresetSound(1);
    }
}