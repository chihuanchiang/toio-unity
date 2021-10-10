using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using toio;
using toio.Navigation;

public class Character {
    public CubeNavigator Navigator;
    public CubeHandle Handle;
    public Cube Cube;
    public Vertex Home;
    public Vertex Next;
    public Vertex Curr;
    public Movement mv;

    public Character(CubeNavigator navigator, CubeHandle handle, Cube cube) {
        Navigator = navigator;
        Handle = handle;
        Cube = cube;
    }

    public void Move2Next() {
        Curr.Value.Occupied = false;
        if (Next.Value.Occupied) {
            mv = Handle.Move2Target(Next.Value.Pos.x, Next.Value.Pos.y, tolerance:80).Exec();
        } else {
            mv = Handle.Move2Target(Next.Value.Pos.x, Next.Value.Pos.y).Exec();
        }
    }

    public void UpdateNext() {
        Next = Curr.Adj[Random.Range(0, Curr.Degree)];
    }

    public void Point2Home() {
        Next = Home;
        Curr = Home;
    }
}