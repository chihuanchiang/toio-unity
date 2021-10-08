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
    public int phase = 0, last_phase = 0;

    public bool Play() {
        P1.First.handle.Update();
        P2.First.handle.Update();
        Movement mv, mv1, mv2;
        switch(phase) {
            case 0:
                // Turn to enemies
                mv1 = P1.First.handle.Rotate2Target(P2.First.handle.x, P2.First.handle.y).Exec();
                mv2 = P2.First.handle.Rotate2Target(P1.First.handle.x, P1.First.handle.y).Exec();
                if (mv1.reached && mv2.reached) RenewPhase(last_phase + 1);
                break;
            case 1:
                // P1 charge
                mv = P1.First.handle.Move2Target(P2.First.handle.x, P2.First.handle.y, tolerance:40).Exec();
                if (mv.reached) RenewPhase(2);
                break;
            case 2:
                // P1 retreat
                mv = P1.First.handle.Move2Target(Island.originX - 50, Island.originY).Exec();
                if (mv.reached) RenewPhase(0);
                break;
            case 3:
                // P2 charge
                mv = P2.First.handle.Move2Target(P1.First.handle.x, P1.First.handle.y, tolerance:40).Exec();
                if (mv.reached) RenewPhase(4);
                break;
            case 4:
                // P2 retreat
                mv = P2.First.handle.Move2Target(Island.originX + 50, Island.originY).Exec();
                if (mv.reached) {
                    RenewPhase(0);
                    last_phase = 0;
                }
                break;
            default:
                Debug.LogError("Invalid battle phase");
                break;
        }
        return true;
    }

    public void RenewPhase(int next) {
        last_phase = phase;
        phase = next;
    }
}