using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player {
    // Hp: the amount of damage the character can take
    // Str: Helps with the accumulation of energy
    // Luck: the likelihood that the character can dodge an attack
    // Energy: The potential to deal damage to opponents
    public class Stats {
        public int Hp;
        public int Str;
        public int Luck;
        public float Energy;
    }
    
    public Character First;
    public Character Second;
    public Stats Stat;
    public int Point;
    public float BattleX;
    public float BattleY;

    public Player(Character first, Character second) {
        First = first;
        Second = second;
        Stat = new Stats();
        Point = 0;
        ResetStat();
    }

    public void ResetStat() {
        Stat.Hp = 3;
        Stat.Str = 0;
        Stat.Luck = 0;
        Stat.Energy = 0;
    }

    public void IslandAction() {
        switch (First.Curr.Value.Type) {
            case Island.Types.PowerUpHp:
                AddHp();
                break;
            case Island.Types.PowerUpAtk:
                AddStr();
                break;
            case Island.Types.PowerUpDex:
                AddLuck();
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

    private void AddHp() {
        Stat.Hp++;
        First.Cube.TurnLedOn(0, 255, 0, 500);
        First.Cube.PlayPresetSound(0);
    }

    private void AddStr() {
        Stat.Str++;
        First.Cube.TurnLedOn(255, 0, 0, 500);
        First.Cube.PlayPresetSound(0);
    }

    private void AddLuck() {
        Stat.Luck++;
        First.Cube.TurnLedOn(255, 140, 40, 500);
        First.Cube.PlayPresetSound(0);
    }

    private void GoToPrison() {
        First.Cube.PlayPresetSound(1);
    }
}