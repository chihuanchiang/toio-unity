using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public SimpleGraphScene simpleGraph;
    public Text MoveText;
    public Button MoveBtn;
    public Text BattleText_P1;
    public Text[] P1_StatsText;
    public Text BattleText_P2;
    public Text[] P2_StatsText;


    // Start is called before the first frame update
    void Start()
    {
        MoveText.text = "Welcome to Tonopoly!";
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TurnOnMoveBtn()
    {
        MoveBtn.gameObject.SetActive(true);
    }

    public void TurnOffMoveBtn()
    {
        MoveBtn.gameObject.SetActive(false);
    }

    public void TurnOnMoveText()
    {
        MoveText.gameObject.SetActive(true);
    }

    public void TurnOffMoveText()
    {
        MoveText.gameObject.SetActive(false);
    }

    public void TurnOnBattleText()
    {
        BattleText_P1.gameObject.SetActive(true);
        BattleText_P2.gameObject.SetActive(true);
    }

    public void TurnOffBattleText()
    {
        BattleText_P1.gameObject.SetActive(false);
        BattleText_P2.gameObject.SetActive(false);
    }


    //roll a dice
    public void ClickBtn() 
    {
        simpleGraph.RollDice();
    }


    //show which player's turn
    public void ShowPlayerOrder(int playernum) 
    {
        MoveText.text = "It's Player" + (playernum+1) + "'s turn";
    }
    
    public void ShowBattleStats(int P1_Hp, int P1_Energy, int P1_Luck, int P2_Hp, int P2_Energy, int P2_Luck)
    {
        //BattleText_P1.text = "P1\n <color = green> HP: 3 </color>\n <color = red> ATK:" + P1_Energy+ "</color>\n <color = yellow> Luck:" + P1_Luck + " </color>";
        //Player1
        P1_StatsText[0].text = "HP:" + P1_Hp;
        P1_StatsText[0].color = Color.green;
        P1_StatsText[1].text = "ATK:" + P1_Energy;
        P1_StatsText[1].color = Color.red;
        P1_StatsText[2].text = "Luck:" + P1_Luck;
        P1_StatsText[2].color = Color.yellow;

        //Player2
        P2_StatsText[0].text = "HP:" + P2_Hp;
        P2_StatsText[0].color = Color.green;
        P2_StatsText[1].text = "ATK:" + P2_Energy;
        P2_StatsText[1].color = Color.red;
        P2_StatsText[2].text = "Luck:" + P2_Luck;
        P2_StatsText[2].color = Color.yellow;
    }
}
