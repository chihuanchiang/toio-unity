using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using toio;
using toio.Navigation;

public class Character<TVertexType>
{
    public Character(CubeNavigator nav, Cube cube) {
        this.nav = nav;
        this.cube = cube;
    }

    public CubeNavigator nav { get; set; }
    public Cube cube { get; set; }
    public Vertex<TVertexType> spot { get; set; }
    public Movement mv { get; set; }
}
