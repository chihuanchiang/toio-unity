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
    public Vertex spot { get; set; }
    public Movement mv { get; set; }

    public void Navi2Spot() {
        mv = nav.Navi2Target(spot.Value.Pos.x, spot.Value.Pos.y).Exec();
    }

    public void RenewSpot() {
        spot = spot.Adj[Random.Range(0, spot.Degree)];
    }
}