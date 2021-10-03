using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player<TVertexType>
{
    public Player(Character<TVertexType> first, Character<TVertexType> second) {
        First = first;
        Second = second;
    }
    
    public Character<TVertexType> First { get; set; }
    public Character<TVertexType> Second { get; set; }
}