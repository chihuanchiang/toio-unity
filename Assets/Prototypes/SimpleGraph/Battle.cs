using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using toio;
using toio.Navigation;

public class Battle {
    public Battle(Player p1, Player p2) {
        P1 = p1;
        P2 = p2;
    }

    public Player P1;
    public Player P2;
    public int phase = 0;
    private const float intervalTime = 0.5f;
    private float elapsedTime = 0.0f;

    public bool Play() {
        P1.First.handle.Update();
        P2.First.handle.Update();
        Movement mv, mv1, mv2;
        switch(phase) {
            case 0:
                mv1 = P1.First.handle.Rotate2Target(P2.First.handle.x, P2.First.handle.y).Exec();
                mv2 = P2.First.handle.Rotate2Target(P1.First.handle.x, P1.First.handle.y).Exec();
                if (mv1.reached && mv2.reached) phase = 1;
                break;
            case 1:
                mv = P1.First.handle.Move2Target(P2.First.handle.x, P2.First.handle.y, tolerance:40).Exec();
                if (mv.reached) phase = 2;
                break;
            case 2:
                mv = P1.First.handle.Move2Target(Island.originX - 50, Island.originY).Exec();
                if (mv.reached) phase = 3;
                break;
            case 3:
                mv = P2.First.handle.Move2Target(P1.First.handle.x, P1.First.handle.y, tolerance:40).Exec();
                if (mv.reached) phase = 4;
                break;
            case 4:
                mv = P2.First.handle.Move2Target(Island.originX + 50, Island.originY).Exec();
                if (mv.reached) phase = 0;
                break;
            default:
                Debug.LogError("Invalid battle phase");
                break;
        }
        return true;
    }
}