using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour
{
    public SimpleGraphScene simpleGraph;

    public void ClickBtn()
    {
        //Debug.Log("Click!!!");
        simpleGraph.RollDice();
    }
}
