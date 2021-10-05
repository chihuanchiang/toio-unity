using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public SimpleGraphScene simpleGraph;
    public Text text;
    public Button btn;

    // Start is called before the first frame update
    void Start()
    {
        text.text = "Welcome to Tonopoly Game!";
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OpenBtn()
    {
        btn.gameObject.SetActive(true);
    }

    //roll a dice
    public void ClickBtn() 
    {
        simpleGraph.RollDice();
    }

    //show which player's turn
    public void ShowPlayerOrder(int playernum) 
    {
        text.text = "Now it's Player " + (playernum+1);
    }
    
}
