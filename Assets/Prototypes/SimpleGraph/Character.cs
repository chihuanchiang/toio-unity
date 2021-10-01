using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using toio;
using toio.Navigation;

public class Character<TVertexType>
{
    public Character(CubeNavigator nav) {
        cubeNav = nav;
    }

    public CubeNavigator cubeNav { get; set; }
    public Vertex<TVertexType> spot { get; set; }
    public Movement mv { get; set; }
}
