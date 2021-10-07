using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using toio;
using toio.Navigation;

public class Character
{
    public Character(CubeNavigator nav, Cube cube) {
        this.nav = nav;
        this.cube = cube;
    }

    public CubeNavigator nav { get; set; }
    public Cube cube { get; set; }
    public Vertex next { get; set; }
    public Vertex curr { get; set; }
    public Movement mv { get; set; }

    public void Navi2Next() {
        mv = nav.Navi2Target(next.Value.Pos.x, next.Value.Pos.y, tolerance:120).Exec();
        curr = next;
    }

    public void RenewNext() {
        next = next.Adj[Random.Range(0, next.Degree)];
    }
}