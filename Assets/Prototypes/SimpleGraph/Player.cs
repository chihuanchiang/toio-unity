using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public Player(Character first, Character second) {
        First = first;
        Second = second;
    }
    
    public Character First { get; set; }
    public Character Second { get; set; }
}